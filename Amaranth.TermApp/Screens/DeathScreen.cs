using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;
using Amaranth.UI;
using Amaranth.Data;

namespace Amaranth.TermApp
{
    public class DeathScreen : Screen, IInputHandler
    {
        public DeathScreen()
            : base("You Have Died")
        {
            FixedMenu menu = new FixedMenu();
            menu.Position = new Vec(5, 10);

            menu.Items.Add(new MenuItem("Quit", () => UI.Quit()));

            Controls.Add(menu);

            Controls.Add(new TitleBar());
            Controls.Add(new StatusBar());

            FocusFirst();
        }

        #region IInputHandler Members

        public IEnumerable<KeyInstruction> KeyInstructions
        {
            get
            {
                yield return new KeyInstruction("Quit", new KeyInfo(Key.Escape));
            }
        }

        public bool KeyDown(KeyInfo key)
        {
            if (key.Key == Key.Escape)
            {
                UI.PopScreen();
                return true;
            }

            return false;
        }

        public bool KeyUp(KeyInfo key)
        {
            return false;
        }

        #endregion
    }
}
