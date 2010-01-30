using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;
using Amaranth.Terminals;
using Amaranth.UI;
using Amaranth.Engine;

namespace Amaranth.TermApp
{
    public class StoreScreen : Screen, IInputHandler
    {
        public StoreScreen(Store store, PlayGameScreen playGameScreen)
            : base("Would you like to buy or sell?")
        {
            mStore = store;

            mStoreInventory = new InventoryControl(playGameScreen.Game, new Rect(15, 0, 50, 21), playGameScreen);
            mStoreInventory.SetStore(store, true);
            Controls.Add(mStoreInventory);

            mHeroInventory = new InventoryControl(playGameScreen.Game, new Rect(66, 0, 54, 21), playGameScreen);
            mHeroInventory.SetStore(store, false);
            Controls.Add(mHeroInventory);

            Controls.Add(new StatusBar());
        }

        protected override void OnPaint(ITerminal terminal)
        {
            terminal[WindowBounds].Clear();
        }

        private Rect WindowBounds { get { return new Rect(15, 0, 50, 30); } }

        #region IInputHandler Members

        public IEnumerable<KeyInstruction> KeyInstructions
        {
            get
            {
                if (FocusControl == null)
                {
                    yield return new KeyInstruction("Buy", new KeyInfo(Key.B, true));
                    yield return new KeyInstruction("Sell", new KeyInfo(Key.S, true));

                    yield return new KeyInstruction("Leave Store", new KeyInfo(Key.Escape));
                }
            }
        }

        public bool KeyDown(KeyInfo key)
        {
            switch (key.Key)
            {
                case Key.Escape:
                    UI.PopScreen();
                    return true;

                case Key.B:
                    mStoreInventory.Buy();
                    return true;

                case Key.S:
                    mHeroInventory.Sell();
                    return true;
            }

            return false;
        }

        public bool KeyUp(KeyInfo key)
        {
            return false;
        }

        #endregion

        private Store mStore;
        private InventoryControl mStoreInventory;
        private InventoryControl mHeroInventory;
    }
}
