using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;
using Amaranth.UI;
using Amaranth.Data;
using Amaranth.Engine;

namespace Amaranth.TermApp
{
    public class WelcomeScreen : Screen, IInputHandler
    {
        public WelcomeScreen()
            : base("Welcome")
        {
            FixedMenu menu = new FixedMenu();
            menu.Position = new Vec(5, 10);

            menu.Items.Add(new MenuItem("New Hero", () => NewHero(), "New"));
            menu.Items.Add(new MenuItem("Open Hero", () => OpenHero(), "Open"));

            string lastHero = GameSettings.Instance.LastHero;
            if (!String.IsNullOrEmpty(lastHero) && Game.CanLoad(lastHero))
            {
                menu.Items.Add(new MenuItem("Open \"" + lastHero + "\"", 'L', () => OpenLastHero(), "Open"));
            }

            Controls.Add(menu);

            Controls.Add(new TitleBar());
            Controls.Add(new StatusBar());

            FocusFirst();
        }

        protected override void Init()
        {
            mContent = DataFiles.Load();
        }

        private void NewHero()
        {
            UI.PushScreen(new NewHeroScreen(mContent));
        }
        
        private void OpenHero()
        {
            UI.PushScreen(new LoadHeroScreen(mContent));
        }

        private void OpenLastHero()
        {
            Game game = Game.Load(GameSettings.Instance.LastHero, mContent);

            UI.PushScreen(new PlayGameScreen(game));
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
                UI.Quit();
                return true;
            }

            return false;
        }

        public bool KeyUp(KeyInfo key)
        {
            return false;
        }

        #endregion

        private Content mContent;
    }
}
