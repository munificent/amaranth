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
    public class PlayerInputControl : NonvisualControl, IInputHandler
    {
        public PlayerInputControl(Game game)
            : base()
        {
            mGame = game;
        }

        protected override void Init()
        {
            base.Init();
        }
        #region IInputHandler Members

        public IEnumerable<KeyInstruction> KeyInstructions
        {
            get
            {
                yield return new KeyInstruction("Help", new KeyInfo(Key.H, true));
                yield return new KeyInstruction("Exit", new KeyInfo(Key.Escape));
            }
        }

        public bool KeyDown(KeyInfo key)
        {
            bool handled = false;

            Hero hero = mGame.Hero;
            PlayGameScreen screen = (PlayGameScreen)Screen;

            if (key.Control)
            {
                handled = true;

                switch (key.Key)
                {
                    case Key.I: hero.SetNextAction(new FireAction(hero, Direction.NW)); break;
                    case Key.O: hero.SetNextAction(new FireAction(hero, Direction.N)); break;
                    case Key.P: hero.SetNextAction(new FireAction(hero, Direction.NE)); break;
                    case Key.K: hero.SetNextAction(new FireAction(hero, Direction.W)); break;
                    case Key.L: screen.Fire(); break;
                    case Key.Semicolon: hero.SetNextAction(new FireAction(hero, Direction.E)); break;
                    case Key.Comma: hero.SetNextAction(new FireAction(hero, Direction.SW)); break;
                    case Key.Period: hero.SetNextAction(new FireAction(hero, Direction.S)); break;
                    case Key.Slash: hero.SetNextAction(new FireAction(hero, Direction.SE)); break;

                    default: handled = false; break;
                }
            }
            else if (key.Shift)
            {
                handled = true;

                switch (key.Key)
                {
                    case Key.I: hero.SetBehavior(new RunBehavior(hero, Direction.NW)); break;
                    case Key.O: hero.SetBehavior(new RunBehavior(hero, Direction.N)); break;
                    case Key.P: hero.SetBehavior(new RunBehavior(hero, Direction.NE)); break;
                    case Key.K: hero.SetBehavior(new RunBehavior(hero, Direction.W)); break;
                    case Key.L: hero.SetBehavior(new RestBehavior(hero)); break;
                    case Key.Semicolon: hero.SetBehavior(new RunBehavior(hero, Direction.E)); break;
                    case Key.Comma: hero.SetBehavior(new RunBehavior(hero, Direction.SW)); break;
                    case Key.Period: hero.SetBehavior(new RunBehavior(hero, Direction.S)); break;
                    case Key.Slash: hero.SetBehavior(new RunBehavior(hero, Direction.SE)); break;

                    case Key.S: hero.SetNextAction(new TakeStairsAction(hero)); break;

                    case Key.G: hero.SetBehavior(new PickUpAllBehavior(hero)); break;

                    case Key.C: Screen.UI.PushScreen(new HeroInfoScreen(hero)); break;
                    default: handled = false; break;
                }
            }
            else
            {
                handled = true;

                switch (key.Key)
                {
                    case Key.Escape:
                        screen.Quit();
                        break;

                    case Key.I: hero.SetNextAction(new WalkAction(hero, Direction.NW)); break;
                    case Key.O: hero.SetNextAction(new WalkAction(hero, Direction.N)); break;
                    case Key.P: hero.SetNextAction(new WalkAction(hero, Direction.NE)); break;
                    case Key.K: hero.SetNextAction(new WalkAction(hero, Direction.W)); break;
                    case Key.L: hero.SetNextAction(new WalkAction(hero, Direction.None)); break;
                    case Key.Semicolon: hero.SetNextAction(new WalkAction(hero, Direction.E)); break;
                    case Key.Comma: hero.SetNextAction(new WalkAction(hero, Direction.SW)); break;
                    case Key.Period: hero.SetNextAction(new WalkAction(hero, Direction.S)); break;
                    case Key.Slash: hero.SetNextAction(new WalkAction(hero, Direction.SE)); break;

                    case Key.H: Screen.UI.PushScreen(new HelpScreen()); break;

                    case Key.C:
                        screen.CloseDoor();
                        break;

                    case Key.F1:
                        // wizard mode
                        mGame.Hero.Health.AddBonus(BonusType.Wizard, 1000);
                        mGame.Hero.Health.Current += 1000;
                        mGame.Hero.Currency += 1000;
                        mGame.Log.Write(LogType.Special, "Wizard mode ON!");
                        break;

                    case Key.F2:
                        // wizard: go up
                        mGame.Log.Write(LogType.Special, "You ascend closer to the light.");
                        mGame.ChangeFloor(-1);
                        break;

                    case Key.F3:
                        // wizard: go down
                        mGame.Log.Write(LogType.Special, "You descend deeper into the dungeon.");
                        mGame.ChangeFloor(1);
                        break;

                    case Key.F4:
                        // wizard: stat improve
                        mGame.Log.Write(LogType.Special, "Your stats improve.");
                        hero.Stats.Strength.Base++;
                        hero.Stats.Agility.Base++;
                        hero.Stats.Stamina.Base++;
                        hero.Stats.Will.Base++;
                        hero.Stats.Intellect.Base++;
                        hero.Stats.Charisma.Base++;
                        break;

                    case Key.F5:
                        // wizard mode
                        hero.Currency += 1000;
                        mGame.Log.Write(LogType.Special, "Money falls from the sky!");
                        break;

                    case Key.F6:
                        break;

                    case Key.F7:
                        foreach (Store store in mGame.Town.Stores)
                        {
                            store.UpdateInventory();
                        }
                        mGame.Log.Write(LogType.Special, "The stores shuffle their stock.");
                        break;

                    case Key.F8:
                        foreach (Vec pos in mGame.Dungeon.Bounds)
                        {
                            mGame.Dungeon.SetTileExplored(pos);
                            mGame.Dungeon.SetTilePermanentLit(pos, true);
                        }
                        mGame.Log.Write(LogType.Special, "You see what's going on here!");
                        break;

                    case Key.F9:
                        hero.Inventory.Add(new Item(hero.Position, mGame.Content.Items.Find("sling"), 1));
                        hero.Inventory.Add(new Item(hero.Position, mGame.Content.Items.Find("stone"), 99));
                        break;

                    case Key.F11:
                        /*
                        mGame.Log.Write(LogType.Special, "Starting profile");
                        JetBrains.dotTrace.Api.CPUProfiler.Start();
                         */
                        break;

                    case Key.F12:
                        /*
                        JetBrains.dotTrace.Api.CPUProfiler.StopAndSaveSnapShot();
                        mGame.Log.Write(LogType.Special, "Ended profile");
                         */
                        break;

                    case Key.F15: Profiler.Init(); break;
                    case Key.F16: Profiler.Shutdown(); break;

                    default: handled = false; break;
                }
            }

            if ((Screen.UI != null) && !hero.Behavior.NeedsUserInput)
            {
                ((PlayGameScreen)Screen).ProcessGame();
            }

            return handled;
        }

        public bool KeyUp(KeyInfo key)
        {
            return false;
        }

        #endregion

        private readonly Game mGame;
    }
}
