using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Amaranth.Engine;
using Amaranth.Terminals;
using Amaranth.UI;

namespace Amaranth.TermApp
{
    /// <summary>
    /// A screen that shows detailed class-specific information for a Hero.
    /// </summary>
    public class ClassInfoScreen : Screen, IInputHandler
    {
        public ClassInfoScreen(Hero hero)
            : base("Class info for " + hero.Name)
        {
            mHero = hero;

            Controls.Add(new StatusBar());
        }

        protected override void OnPaint(ITerminal terminal)
        {
            terminal.Clear();

            //### bob: hack, just warrior is supported
            Warrior warrior = (Warrior)mHero.Class;

            int y = 5;
            foreach (var lore in warrior.SlayLore)
            {
                terminal[10, y].Write(lore.Group);
                terminal[30, y].Write(lore.Slain.ToString());
                terminal[40, y].Write(lore.Level.ToString());
                y++;
            }
        }

        #region IInputHandler Members

        public IEnumerable<KeyInstruction> KeyInstructions
        {
            get
            {
                yield return new KeyInstruction("Return to Game", new KeyInfo(Key.Escape));
                yield return new KeyInstruction("Show Hero Info", new KeyInfo(Key.Tab));
            }
        }

        public bool KeyDown(KeyInfo key)
        {
            if (key.Key == Key.Escape)
            {
                UI.PopScreen();
                return true;
            }
            else if (key.Key == Key.Tab)
            {
                UI.SetScreen(new HeroInfoScreen(mHero));
                return true;
            }

            return false;
        }

        public bool KeyUp(KeyInfo key)
        {
            return false;
        }

        #endregion

        private Hero mHero;
    }
}
