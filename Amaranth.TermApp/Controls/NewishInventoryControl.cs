using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Sorrow.Util;
using Sorrow.Terminals;
using Sorrow.UI;
using Sorrow.Engine;

namespace Sorrow.App
{
    public class InventoryControl : ItemListControl, IInputHandler, IFocusable
    {
        public InventoryControl(Rect bounds, NotNull<PlayerInputControl> playerInputControl)
            : base(bounds)
        {
            mPlayerInputControl = playerInputControl;

            RepaintOn(Game.Hero.Inventory.ItemAdded);
            RepaintOn(Game.Hero.Inventory.ItemRemoved);
            RepaintOn(Game.Hero.Equipment.ItemEquipped);

            ListenTo(Game.Hero.Moved, Hero_Moved);
        }

        protected override string ItemsTitle
        {
            get
            {
                switch (mViewing)
                {
                    case Viewing.Inventory: return "Inventory";
                    case Viewing.Equipment: return "Equipment";
                    case Viewing.Ground: return "On Ground";

                    default: throw new UnknownEnumException(mViewing);
                }
            }
        }

        protected override IItemCollection Items
        {
            get
            {
                switch (mViewing)
                {
                    case Viewing.Inventory: return Game.Hero.Inventory;
                    case Viewing.Equipment: return Game.Hero.Equipment;
                    case Viewing.Ground: return new ItemsOnGroundCollection(Game.Hero.Position);
                    default: throw new UnknownEnumException(mViewing);
                }
            }
        }

        protected override bool IsChoosable(Item item)
        {
            // bail if there is no item
            if (item == null) return false;

            switch (mChoosing)
            {
                case Choosing.PickingUp:
                    // everything on the ground can be picked up
                    return mViewing == Viewing.Ground;

                case Choosing.Wielding:
                    return Game.Hero.Equipment.CanEquip(item);

                case Choosing.Using:
                    return item.CanUse;

                case Choosing.Dropping:
                    // everything not on the ground can be dropped
                    return mViewing != Viewing.Ground;

                default:
                    return false;
            }
        }

        protected override void ChooseItem(Item item)
        {
            switch (mChoosing)
            {
                case Choosing.Nothing:
                    throw new InvalidOperationException("Should not have chosen an item in this mode.");

                case Choosing.PickingUp:
                    mPlayerInputControl.PickUp(item);
                    break;

                case Choosing.Wielding:
                    mPlayerInputControl.Wield(item);
                    break;

                case Choosing.Using:
                    mPlayerInputControl.Use(item);
                    break;

                case Choosing.Dropping:
                    mPlayerInputControl.Drop(item);
                    break;

                default: throw new UnknownEnumException(item);
            }

            // done
            SetView(Choosing.Nothing);
        }

        private void SwitchView()
        {
            switch (mViewing)
            {
                case Viewing.Inventory: SetView(Viewing.Equipment); break;
                case Viewing.Equipment: SetView(Viewing.Ground); break;
                case Viewing.Ground: SetView(Viewing.Inventory); break;
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
                    view = ShowChoosableView();
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

        private void Hero_Moved(Entity hero, ValueChangeEventArgs<Vec> args)
        {
            if (mViewing == Viewing.Ground)
            {
                Repaint();
            }
        }

        #region IInputHandler Members

        public override IEnumerable<KeyInstruction> KeyInstructions
        {
            get
            {
                // get the base ones
                foreach (KeyInstruction instruction in base.KeyInstructions)
                {
                    yield return instruction;
                }

                if (HasFocus)
                {
                    yield return new KeyInstruction("Cancel", new KeyInfo(Key.Escape));
                }

                yield return new KeyInstruction("Switch inventory", new KeyInfo(Key.Tab));
            }
        }

        public override bool KeyDown(KeyInfo key)
        {
            if (HasFocus)
            {
                // user is selecting items
                switch (key.Key)
                {
                    case Key.Tab:
                        SwitchView();
                        return true;

                    case Key.Escape:
                        SetView(Choosing.Nothing);
                        return true;

                    default:
                        return base.KeyDown(key);
                }
            }
            else
            {
                if (!key.Shift)
                {
                    // user is not selecting items
                    switch (key.Key)
                    {
                        case Key.Tab:
                            SwitchView();
                            return true;

                        case Key.G:
                            SetView(Choosing.PickingUp, Viewing.Ground);
                            return true;

                        case Key.W:
                            SetView(Choosing.Wielding);
                            return true;

                        case Key.U:
                            SetView(Choosing.Using);
                            return true;

                        case Key.D:
                            SetView(Choosing.Dropping);
                            return true;
                    }
                }
            }
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
                    case Choosing.Nothing: return "(nothing)";
                    case Choosing.PickingUp: return "Choose an item to get";
                    case Choosing.Wielding: return "Choose an item to wield";
                    case Choosing.Using: return "Choose an item to use";
                    case Choosing.Dropping: return "Choose an item to drop";
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
            Dropping
        }

        private enum Viewing
        {
            Inventory,
            Equipment,
            Ground
        }

        private Viewing mViewing;
        private Choosing mChoosing;

        private PlayerInputControl mPlayerInputControl;
    }
}
