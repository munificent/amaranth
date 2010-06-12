using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;
using Malison.Core;

using Amaranth.Util;
using Amaranth.UI;
using Amaranth.Engine;

namespace Amaranth.TermApp
{
    public class PlayGameScreen : Screen
    {
        public Game Game { get { return mGame; } }

        public PlayGameScreen(Game game) : base()
        {
            mGame = game;

            Controls.Add(new StatsControl(mGame.Hero, new Rect(0, 0, 14, 31)));
            mOverheadControl = new OverheadControl(mGame, new Rect(15, 1, 50, 30));
            Controls.Add(mOverheadControl);
            Controls.Add(new StatusControl(mGame, new Rect(15, 31, 50, 1)));

            // top single-line log
            Controls.Add(new LogControl(mGame.Log, new Rect(15, 0, 50, 1)));

            // main log
            Controls.Add(new LogControl(mGame.Log, new Rect(0, 32, 65, 17)));
            Controls.Add(new InventoryControl(mGame, new Rect(66, 0, 54, 21), this));
            Controls.Add(new DescriptionControl(mGame, new Rect(66, 32, 54, 17)));
            Controls.Add(new StatusBar());
            Controls.Add(new PlayerInputControl(mGame));

            mYesNoPrompt = new PromptYesNoBar("Save and quit the game?");
            mYesNoPrompt.Visible = false;
            Controls.Add(mYesNoPrompt);

            mDirectionPrompt = new PromptDirectionBar("Close the door in which direction?");
            mDirectionPrompt.Visible = false;
            Controls.Add(mDirectionPrompt);

            mTargetPrompt = new PromptTargetBar(mOverheadControl);
            mTargetPrompt.Visible = false;
            Controls.Add(mTargetPrompt);

            ListenTo(mGame.StoreEntered, Game_StoreEntered);
        }

        public void Quit()
        {
            // get confirmation
            mYesNoPrompt.Visible = true;

            bool quit = mYesNoPrompt.Read(false, false);
            mYesNoPrompt.Visible = false;

            if (quit)
            {
                // auto-save before quitting
                mGame.Save();
                mGame.Log.Message("Saved.");

                UI.PopScreen();
            }
        }

        public void Buy(NotNull<Item> item, NotNull<Store> store)
        {
            int price = store.Value.GetBuyPrice(mGame.Hero, item);

            // ask how many to buy
            int maxAffordable = mGame.Hero.Currency / price;
            int quantity = PromptForItemQuantity(item, 1, Math.Min(item.Value.Quantity, maxAffordable));

            if (quantity != 0)
            {
                // confirm the purchase
                PromptYesNoBar prompt = new PromptYesNoBar("Buy " + item.Value.ToString(quantity, ItemStringOptions.ShowQuantity) + " for " + (price * quantity) + "?");
                Controls.Add(prompt);
                bool buy = prompt.Read(true, false);
                Controls.Remove(prompt);

                if (buy)
                {
                    mGame.Hero.SetNextAction(new BuyAction(mGame.Hero, item, quantity, store));

                    ProcessGame();
                }
            }
        }

        public void Sell(NotNull<Item> item, NotNull<Store> store)
        {
            int quantity = PromptForItemQuantity(item, item.Value.Quantity, item.Value.Quantity);
            int price = store.Value.GetSellPrice(mGame.Hero, item);

            if (quantity != 0)
            {
                // confirm the purchase
                PromptYesNoBar prompt = new PromptYesNoBar("Sell " + item.Value.ToString(quantity, ItemStringOptions.ShowQuantity) + " for " + (price * quantity) + "?");
                Controls.Add(prompt);
                bool sell = prompt.Read(true, false);
                Controls.Remove(prompt);

                if (sell)
                {
                    mGame.Hero.SetNextAction(new SellAction(mGame.Hero, item, quantity, price, store));

                    ProcessGame();
                }
            }
        }

        public void PickUp(NotNull<Item> item)
        {
            int quantity = PromptForItemQuantity(item, item.Value.Quantity, item.Value.Quantity);

            if (quantity != 0)
            {
                mGame.Hero.SetNextAction(new PickUpAction(mGame.Hero, item, quantity));

                ProcessGame();
            }
        }

        public void Use(NotNull<Item> item)
        {
            Vec? target = null;

            // see if it needs a target
            if (item.Value.Type.Target == ItemTarget.Tile)
            {
                target = PromptForTarget();
            }

            // only use the item if a target was given or not needed
            if (target.HasValue || (item.Value.Type.Target != ItemTarget.Tile))
            {
                mGame.Hero.SetNextAction(new UseAction(mGame.Hero, item, target));

                ProcessGame();
            }
        }

        public void Wield(NotNull<Item> item)
        {
            mGame.Hero.SetNextAction(new EquipAction(mGame.Hero, item));

            ProcessGame();
        }

        public void Drop(NotNull<Item> item)
        {
            int quantity = PromptForItemQuantity(item, item.Value.Quantity, item.Value.Quantity);

            mGame.Hero.SetNextAction(new DropAction(mGame.Hero, item, quantity));

            ProcessGame();
        }

        public void Fire()
        {
            Vec? target = PromptForTarget();

            if (target.HasValue)
            {
                mGame.Hero.SetNextAction(new FireAction(mGame.Hero, target.Value));
            }
        }

        public void CloseDoor()
        {
            Direction direction = PromptForDirection();

            if (direction != Direction.None)
            {
                mGame.Hero.SetNextAction(new CloseDoorAction(mGame.Hero, mGame.Hero.Position + direction));

                ProcessGame();
            }
        }

        protected override void Init()
        {
            base.Init();

            ProcessGame();
        }

        public void ProcessGame()
        {
            GameResult result = new GameResult();

            while (!result.NeedsAction && !result.IsGameOver)
            {
                mOverheadControl.StartGameUpdate();

                result = mGame.Process();

                mOverheadControl.EndGameUpdate();

                UI.Update();

                if (result.NeedsPause) UI.Delay();

                if (result.CheckForCancel && UI.CheckForCancel())
                {
                    mGame.CancelAction();
                }
            }

            // handle a game over
            if (result.IsGameOver)
            {
                UI.SetScreen(new DeathScreen());
            }
        }

        protected override void OnPaint(ITerminal terminal)
        {
            terminal.Clear();

            terminal = terminal[TermColor.DarkGray];

            terminal[65, 0, 1, terminal.Size.Y].DrawBox(false, true);
            terminal[0, 31, 65, 1].DrawBox(false, true);
            terminal[66, 31, 54, 1].DrawBox(false, true);
            terminal[65, 31].Write(Glyph.BarUpDownLeftRight);

            terminal[14, 0, 1, 31].DrawBox(false, true);
            terminal[14, 31].Write(Glyph.BarUpLeftRight);
        }

        protected override void OnActivate()
        {
            mOverheadControl.Visible = true;
        }

        private int PromptForItemQuantity(Item item, int initialQuantity, int maxQuantity)
        {
            int quantity = 1;

            // if picking up a stack, get the quantity from the user
            if (item.Quantity > 1)
            {
                PromptNumberBar prompt = new PromptNumberBar();
                Controls.Add(prompt);

                quantity = prompt.Read(1, initialQuantity, maxQuantity);

                Controls.Remove(prompt);
            }

            return quantity;
        }

        private Direction PromptForDirection()
        {
            mDirectionPrompt.Visible = true;

            Direction direction = mDirectionPrompt.Read(Direction.None, Direction.None);

            mDirectionPrompt.Visible = false;

            return direction;
        }

        private Vec? PromptForTarget()
        {
            mTargetPrompt.Visible = true;

            Vec? target = mTargetPrompt.Read();

            mTargetPrompt.Visible = false;

            return target;
        }

        private void Game_StoreEntered(Store store, EventArgs e)
        {
            //### bob: having to hide this is a hack. have to do it because it will
            // paint over the store screen.
            mOverheadControl.Visible = false;

            UI.PushScreen(new StoreScreen(store, this));
        }

        private Game mGame;

        private OverheadControl mOverheadControl;
        private PromptYesNoBar mYesNoPrompt;
        private PromptDirectionBar mDirectionPrompt;
        private PromptTargetBar mTargetPrompt;
    }
}
