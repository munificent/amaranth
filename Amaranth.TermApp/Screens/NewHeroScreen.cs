using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;
using Amaranth.UI;
using Amaranth.Engine;

namespace Amaranth.TermApp
{
    public class NewHeroScreen : Screen, IInputHandler
    {
        public NewHeroScreen(Content content)
            : base("Create Hero")
        {
            mContent = content;

            mNameBox = new TextBox("Name", "Someone Jones");
            mNameBox.Position = new Vec(10, 5);
            mNameBox.Width = 20;
            Controls.Add(mNameBox);

            ScrollMenu sexMenu = new ScrollMenu("Sex", new string[] { "Female", "Male" });
            sexMenu.Position = new Vec(5, 11);
            Controls.Add(sexMenu);

            mRaceMenu = new ScrollMenu("Race");
            mRaceMenu.Position = new Vec(sexMenu.Bounds.Right + 5, 11);
            mRaceMenu.ItemSelected += RaceMenu_ItemSelected;

            foreach (HeroRace race in mContent.HeroRaces)
            {
                mRaceMenu.Items.Add(new MenuItem(race.Name));
            }

            Controls.Add(mRaceMenu);

            mClassMenu = new ScrollMenu("Class", new string[] { "Warrior" });
            mClassMenu.Position = new Vec(mRaceMenu.Bounds.Right + 5, 11);
            Controls.Add(mClassMenu);

            mLevelMenu = new ScrollMenu("Level", new string[] { "1", "10" });
            mLevelMenu.Position = new Vec(mClassMenu.Bounds.Right + 5, 11);
            Controls.Add(mLevelMenu);

            mCheatMenu = new ScrollMenu("Cheat Death", new string[] { "Yes", "No" });
            mCheatMenu.Position = new Vec(mLevelMenu.Bounds.Right + 5, 11);
            Controls.Add(mCheatMenu);

            mStatsControl = new NewStatsControl(new Vec(3, 17));
            mStatsControl.Race = mContent.HeroRaces.Find(mRaceMenu.SelectedItem.Text);
            Controls.Add(mStatsControl);

            Controls.Add(new TitleBar());
            Controls.Add(new StatusBar());

            // default to human male
            sexMenu.Selected = 1;
            mRaceMenu.Selected = mContent.HeroRaces.IndexOf(mContent.HeroRaces.Find("Human"));

            FocusFirst();
        }

        private void StartGame()
        {
            Hero hero = null;

            bool cheatDeath = mCheatMenu.SelectedItem.Text == "Yes";
            string race = mRaceMenu.SelectedItem.Text;

            switch (mLevelMenu.SelectedItem.Text)
            {
                case "1": hero = Hero.CreateNew(mContent, mNameBox.Text,
                    race, mStatsControl.Stats, cheatDeath); break;
                case "10": hero = Hero.CreateCanonicalLevel10(mContent, mNameBox.Text,
                    race, mStatsControl.Stats, cheatDeath); break;
            }

            Game game = new Game(hero, mContent);

            GameSettings.Instance.LastHero = hero.Name;

            UI.SetScreen(new PlayGameScreen(game));
        }

        private void RaceMenu_ItemSelected(object sender, EventArgs e)
        {
            mStatsControl.Race = mContent.HeroRaces.Find(mRaceMenu.SelectedItem.Text);
        }

        #region IInputHandler Members

        public IEnumerable<KeyInstruction> KeyInstructions
        {
            get
            {
                yield return new KeyInstruction("Create hero", new KeyInfo(Key.Enter));
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
        private TextBox mNameBox;
        private ScrollMenu mRaceMenu;
        private ScrollMenu mClassMenu;
        private ScrollMenu mLevelMenu;
        private ScrollMenu mCheatMenu;
        NewStatsControl mStatsControl;
    }
}
