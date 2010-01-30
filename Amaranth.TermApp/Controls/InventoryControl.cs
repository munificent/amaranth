using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Amaranth.Util;
using Amaranth.Terminals;
using Amaranth.UI;
using Amaranth.Engine;

namespace Amaranth.TermApp
{
    public class InventoryControl : RectControl, IInputHandler, IFocusable
    {
        public InventoryControl(Game game, Rect bounds, NotNull<PlayGameScreen> playGameScreen)
            : base(bounds)
        {
            mGame = game;
            mPlayGameScreen = playGameScreen;

            RepaintOn(game.Hero.Inventory.ItemAdded);
            RepaintOn(game.Hero.Inventory.ItemRemoved);

            ListenTo(game.Hero.Inventory.ItemChanged, Inventory_ItemChanged);
            ListenTo(game.Hero.Equipment.ItemEquipped, Equipment_ItemEquipped);
            ListenTo(game.Hero.Equipment.ItemChanged, Equipment_ItemChanged);

            ListenTo(game.Dungeon.Items.ItemRemoved, Dungeon_ItemAdded);
            ListenTo(game.Dungeon.Items.ItemRemoved, Dungeon_ItemRemoved);
            ListenTo(game.Hero.Moved, Hero_Moved);
        }

        public void SetStore(NotNull<Store> store, bool isStoreInventory)
        {
            mStore = store;
            mIsStoreInventory = isStoreInventory;

            if (mIsStoreInventory)
            {
                SetView(Viewing.Store);
            }
        }

        public void Buy()
        {
            SetView(Choosing.Buying);
            Screen.Focus(this);
        }

        public void Sell()
        {
            SetView(Choosing.Selling);
            Screen.Focus(this);
        }

        protected override void OnPaint(ITerminal terminal)
        {
            terminal.Clear();

            Color titleColor = HasFocus ? TerminalColors.White : TerminalColors.Gray;

            // draw the title
            switch (mViewing)
            {
                case Viewing.Inventory: terminal[titleColor].Write("Inventory"); break;
                case Viewing.Equipment: terminal[titleColor].Write("Equipment"); break;
                case Viewing.Ground:    terminal[titleColor].Write("On Ground"); break;
                case Viewing.Store:     terminal[titleColor].Write(mStore.Name); break;

                default: throw new UnknownEnumException(mViewing);
            }

            // draw the price title
            if (mStore != null)
            {
                terminal[new Vec(-6, 0)][titleColor].Write("Price");
            }

            // draw the items
            int index = 0;
            foreach (Item item in Items)
            {
                // bail if we have too many to show
                if (index >= Bounds.Height) break;

                PaintItem(index, item);

                index++;
            }

            // clear the empty space
            for (; index < Items.Max; index++)
            {
                Vec pos = new Vec(0, index) + 1;

                terminal[pos][TerminalColors.DarkGray].Write(GetKey(index));
            }
        }

        private IItemCollection Items
        {
            get
            {
                switch (mViewing)
                {
                    case Viewing.Inventory: return mGame.Hero.Inventory;
                    case Viewing.Equipment: return mGame.Hero.Equipment;
                    case Viewing.Ground:    return new ItemsOnGroundCollection(mGame.Dungeon, mGame.Hero.Position);
                    case Viewing.Store:     return mStore.Items;
                    default: throw new UnknownEnumException(mViewing);
                }
            }
        }

        private void PaintItem(int index, Item item)
        {
            Vec pos = new Vec(0, index) + 1;

            ITerminal terminal = Terminal;

            if (IsChoosable(item))
            {
                // hilight the item since it is choosable
                terminal[pos][TerminalColors.White].Write(GetKey(index));
                terminal[pos.OffsetX(1)][TerminalColors.Yellow].Write(Glyph.TriangleRight);
            }
            else
            {
                // don't hilight the item since it can't be chosen
                terminal[pos][TerminalColors.DarkGray].Write(GetKey(index));

                if (item != null)
                {
                    terminal[pos.OffsetX(1)][TerminalColors.DarkGray].Write(Glyph.TriangleRight);
                }
            }

            if (item != null)
            {
                // show the item in the slot
                terminal[pos.OffsetX(2)].Write(GameArt.Get(item));

                Color itemColor;
                Color priceColor;

                if (!HasFocus || IsChoosable(item))
                {
                    itemColor = TerminalColors.White;
                }
                else
                {
                    itemColor = TerminalColors.DarkGray;
                }

                if (IsChoosable(item))
                {
                    priceColor = (mViewing == Viewing.Store) ? TerminalColors.Green : TerminalColors.Gold;
                }
                else
                {
                    priceColor = (mViewing == Viewing.Store) ? TerminalColors.DarkGreen : TerminalColors.DarkGold;
                }

                terminal[pos.OffsetX(4)][itemColor].Write(item.ToString());

                // show its price
                if (mStore != null)
                {
                    int price;
                    if (mViewing == Viewing.Store)
                    {
                        price = mStore.GetBuyPrice(mGame.Hero, item);
                    }
                    else
                    {
                        price = mStore.GetSellPrice(mGame.Hero, item);
                    }

                    terminal[new Vec(-9, pos.Y)][priceColor].Write(price.ToString("n0").PadLeft(8, ' '));
                }
            }
            else
            {
                // no item in the slot, so show the category of item that can go there
                terminal[pos.OffsetX(4)][TerminalColors.DarkGray].Write(Items.GetCategory(index));
            }
        }

        private bool IsChoosable(Item item)
        {
            // bail if there is no item
            if (item == null) return false;

            switch (mChoosing)
            {
                case Choosing.PickingUp:
                    // everything on the ground can be picked up
                    return mViewing == Viewing.Ground;

                case Choosing.Wielding:
                    // can't wield the store's stuff!
                    if (mViewing == Viewing.Store) return false;

                    return mGame.Hero.Equipment.CanEquip(item);

                case Choosing.Using:
                    return item.CanUse;

                case Choosing.Dropping:
                    // everything not on the ground can be dropped
                    return mViewing != Viewing.Ground;

                case Choosing.Buying:
                    // everything affordable in the store can be chosen
                    if (mViewing == Viewing.Store)
                    {
                        return mGame.Hero.Currency >= mStore.GetBuyPrice(mGame.Hero, item);
                    }
                    return false;

                case Choosing.Selling:
                    // can't sell the store's stuff!
                    return mViewing != Viewing.Store;

                default:
                    return false;
            }
        }

        private void SwitchView()
        {
            switch (mViewing)
            {
                case Viewing.Inventory:
                    SetView(Viewing.Equipment);
                    break;

                case Viewing.Equipment:
                    if (mStore == null)
                    {
                        SetView(Viewing.Ground);
                    }
                    else
                    {
                        // don't show the ground in the store
                        SetView(Viewing.Inventory);
                    }
                    /*
                    if (mStore == null)
                    {
                        SetView(Viewing.Ground);
                    }
                    else
                    {
                        SetView(Viewing.Store);
                    }*/
                    break;

                case Viewing.Ground:
                    SetView(Viewing.Inventory);
                    break;

                case Viewing.Store:
                    /*
                    SetView(Viewing.Inventory);*/
                    break;

                default: throw new UnknownEnumException(mViewing);
            }
        }

        private void SetView(Choosing choosing, Viewing view)
        {
            bool changed = false;

            if (mChoosing != choosing)
            {
                mChoosing = choosing;

                if (choosing == Choosing.Nothing)
                {
                    Screen.Focus(null);
                }
                else
                {
                    Screen.Focus(this);
                }

                changed = true;
            }

            if (mViewing != view)
            {
                mViewing = view;
                changed = true;
            }

            if (changed)
            {
                Repaint();
            }
        }

        private void SetView(Viewing view)
        {
            SetView(mChoosing, view);
        }

        private void SetView(Choosing choosing)
        {
            SetView(choosing, mViewing);
        }

        /// <summary>
        /// Switches to the first view that has something the user can actually select in it.
        /// </summary>
        /// <returns></returns>
        private Viewing ShowChoosableView()
        {
            Viewing startingView = mViewing;

            bool hasChoosable = false;
            while (!hasChoosable)
            {
                foreach (Item item in Items)
                {
                    if (IsChoosable(item))
                    {
                        hasChoosable = true;
                        break;
                    }
                }

                // try the next view
                if (!hasChoosable)
                {
                    SwitchView();

                    // bail if we wrapped around
                    if (mViewing == startingView) break;
                }
            }

            return mViewing;
        }

        //### bob: temp
        private char GetKey(int index)
        {
            return (char)('a' + index);
        }

        private void ChooseItem(Item item)
        {
            switch (mChoosing)
            {
                case Choosing.Nothing:
                    throw new InvalidOperationException("Should not have chosen an item in this mode.");

                case Choosing.PickingUp:
                    mPlayGameScreen.PickUp(item);
                    break;

                case Choosing.Wielding:
                    mPlayGameScreen.Wield(item);
                    break;

                case Choosing.Using:
                    mPlayGameScreen.Use(item);
                    break;

                case Choosing.Dropping:
                    mPlayGameScreen.Drop(item);
                    break;

                case Choosing.Buying:
                    mPlayGameScreen.Buy(item, mStore);
                    break;

                case Choosing.Selling:
                    mPlayGameScreen.Sell(item, mStore);
                    break;

                default: throw new UnknownEnumException(item);
            }

            // done
            SetView(Choosing.Nothing);
        }

        private void Inventory_ItemChanged(Item item, EventArgs args)
        {
            if (mViewing == Viewing.Inventory)
            {
                PaintItem(Items.IndexOf(item), item);
            }
        }

        private void Equipment_ItemEquipped(Item item, EventArgs args)
        {
            if (mViewing == Viewing.Equipment)
            {
                PaintItem(Items.IndexOf(item), item);
            }
        }

        private void Equipment_ItemChanged(Item item, EventArgs args)
        {
            if (mViewing == Viewing.Equipment)
            {
                PaintItem(Items.IndexOf(item), item);
            }
        }

        private void Hero_Moved(Entity hero, ValueChangeEventArgs<Vec> args)
        {
            if (mViewing == Viewing.Ground)
            {
                Repaint();
            }
        }

        private void Dungeon_ItemAdded(Item item, EventArgs args)
        {
            if ((mViewing == Viewing.Ground) && (item.Position == mGame.Hero.Position))
            {
                Repaint();
            }
        }

        private void Dungeon_ItemRemoved(Item item, EventArgs args)
        {
            if ((mViewing == Viewing.Ground) && (item.Position == mGame.Hero.Position))
            {
                Repaint();
            }
        }

        #region IInputHandler Members

        public IEnumerable<KeyInstruction> KeyInstructions
        {
            get
            {
                if (HasFocus)
                {
                    yield return new KeyInstruction("Choose item", new KeyInfo(Key.A, true), new KeyInfo(Key.Dash), new KeyInfo(Key.T, true));
                }
                else if (mStore != null)
                {
                    /*
                    yield return new KeyInstruction("Buy", new KeyInfo(Key.B, true));
                    yield return new KeyInstruction("Sell", new KeyInfo(Key.S, true));
                     */
                }

                if (mStore == null)
                {
                    yield return new KeyInstruction("Switch inventory", new KeyInfo(Key.Tab));
                }

                if (HasFocus)
                {
                    yield return new KeyInstruction("Cancel", new KeyInfo(Key.Escape));
                }
            }
        }

        public bool KeyDown(KeyInfo key)
        {
            switch (mChoosing)
            {
                case Choosing.PickingUp:
                case Choosing.Dropping:
                case Choosing.Using:
                case Choosing.Wielding:
                case Choosing.Buying:
                case Choosing.Selling:
                    // user is selecting items
                    switch (key.Key)
                    {
                        case Key.Tab:
                            SwitchView();
                            return true;

                        case Key.Escape:
                            SetView(Choosing.Nothing);
                            Screen.Focus(null);
                            return true;

                        default:
                            // see if an item was chosen
                            char? c = key.Character;
                            if (c.HasValue)
                            {
                                int index = 0;
                                foreach (Item item in Items)
                                {
                                    if (IsChoosable(item) && (c.Value == GetKey(index)))
                                    {
                                        ChooseItem(item);
                                        return true;
                                    }

                                    index++;
                                }
                            }

                            // if we got here, an item wasn't chosen
                            return false;
                    }

                case Choosing.Nothing:
                    // user is not selecting items
                    if (!key.Shift)
                    {
                        switch (key.Key)
                        {
                            case Key.Tab:
                                SwitchView();
                                return true;

                            case Key.G:
                                if (mStore == null)
                                {
                                    SetView(Choosing.PickingUp, Viewing.Ground);
                                    return true;
                                }
                                break;

                            case Key.W:
                                SetView(Choosing.Wielding);
                                return true;

                            case Key.U:
                                if (mStore == null)
                                {
                                    SetView(Choosing.Using);
                                    return true;
                                }
                                break;

                            case Key.D:
                                if (mStore == null)
                                {
                                    SetView(Choosing.Dropping);
                                }
                                return true;
                                /*
                            case Key.B:
                                if (mStore != null)
                                {
                                    SetView(Choosing.Buying);
                                    return true;
                                }
                                break;

                            case Key.S:
                                if (mStore != null)
                                {
                                    SetView(Choosing.Selling);
                                    return true;
                                }
                                break;
                                 */
                        }
                    }
                    break;
            }

            return false;
        }

        public bool KeyUp(KeyInfo key)
        {
            return false;
        }

        #endregion

        #region IFocusable Members

        public string Instruction
        {
            get
            {
                switch (mChoosing)
                {
                    case Choosing.Nothing:   return "(nothing)";
                    case Choosing.PickingUp: return "Choose an item to get";
                    case Choosing.Wielding: return "Choose an item to wield";
                    case Choosing.Using: return "Choose an item to use";
                    case Choosing.Dropping: return "Choose an item to drop";
                    case Choosing.Buying: return "Choose an item to buy";
                    case Choosing.Selling: return "Choose an item to sell";
                    default: throw new UnknownEnumException(mChoosing);
                }
            }
        }

        public void GainFocus()
        {
            Repaint();
        }

        public void LoseFocus()
        {
            Repaint();
        }

        #endregion

        private enum Choosing
        {
            Nothing,
            PickingUp,
            Wielding,
            Using,
            Dropping,
            Buying,
            Selling
        }

        private enum Viewing
        {
            Inventory,
            Equipment,
            Ground,
            Store
        }

        private Viewing mViewing;
        private Choosing mChoosing;

        private PlayGameScreen mPlayGameScreen;
        private Game mGame;
        private Store mStore;
        private bool mIsStoreInventory;
    }
}
