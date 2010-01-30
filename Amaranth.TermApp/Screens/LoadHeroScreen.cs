using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;
using Amaranth.UI;
using Amaranth.Engine;

namespace Amaranth.TermApp
{
    public class LoadHeroScreen : Screen, IInputHandler
    {
        public LoadHeroScreen(Content content)
            : base("Load Existing Hero")
        {
            mContent = content;

            mHeroesMenu = new ScrollMenu("Hero");
            mHeroesMenu.Position = new Vec(5, 15);

            foreach (string hero in Game.Heroes)
            {
                mHeroesMenu.Items.Add(new MenuItem(hero));
            }

            Controls.Add(mHeroesMenu);

            Controls.Add(new TitleBar());
            Controls.Add(new StatusBar());

            FocusFirst();
        }

        private void StartGame()
        {
            Game game = Game.Load(mHeroesMenu.SelectedItem.Text, mContent);

            GameSettings.Instance.LastHero = game.Hero.Name;

            UI.SetScreen(new PlayGameScreen(game));
        }

        #region IInputHandler Members

        public IEnumerable<KeyInstruction> KeyInstructions
        {
            get
            {
                yield return new KeyInstruction("Load hero", new KeyInfo(Key.Enter));
                yield return new KeyInstruction("Back", new KeyInfo(Key.Escape));
            }
        }

        public bool KeyDown(KeyInfo key)
        {
            switch (key.Key)
            {
                case Key.Enter:
                    StartGame();
                    return true;

                case Key.Escape:
                    UI.PopScreen();
                    return true;

                default:
                    return false;
            }
        }

        public bool KeyUp(KeyInfo key)
        {
            return false;
        }

        #endregion

        private Content mContent;
        private ScrollMenu mHeroesMenu;
    }
}
