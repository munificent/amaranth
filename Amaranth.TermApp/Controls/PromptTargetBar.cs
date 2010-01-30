using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Engine;
using Amaranth.UI;
using Amaranth.Util;
using Amaranth.Terminals;

namespace Amaranth.TermApp
{
    public enum TargetMode
    {
        Direction,
        Absolute,
        Entity
    }

    public class PromptTargetBar : PromptBar<Vec?>
    {
        public PromptTargetBar(NotNull<OverheadControl> overheadControl)
            : base("Choose a target:")
        {
            mOverheadControl = overheadControl;
        }

        public Vec? Read()
        {
            //### bob: add support for targeting last targeted thing
            mOverheadControl.Target = mOverheadControl.Game.Hero.Position;
            mOverheadControl.TargetMode = TargetMode.Direction;

            Vec? target = base.Read(mOverheadControl.Target, null);

            // clear the target
            mOverheadControl.Target = null;

            return target;
        }

        protected override IEnumerable<KeyInstruction> KeyInstructions
        {
            get
            {
                switch (mOverheadControl.TargetMode)
                {
                    case TargetMode.Direction:
                        yield return new KeyInstruction("Choose Direction",
                            new KeyInfo(Key.I), new KeyInfo(Key.O), new KeyInfo(Key.P),
                            new KeyInfo(Key.Semicolon), new KeyInfo(Key.Slash), new KeyInfo(Key.Period),
                            new KeyInfo(Key.Comma), new KeyInfo(Key.K));
                        break;

                    case TargetMode.Absolute:
                    case TargetMode.Entity:
                        yield return new KeyInstruction("Move Target",
                            new KeyInfo(Key.I), new KeyInfo(Key.O), new KeyInfo(Key.P),
                            new KeyInfo(Key.Semicolon), new KeyInfo(Key.Slash), new KeyInfo(Key.Period),
                            new KeyInfo(Key.Comma), new KeyInfo(Key.K));
                        break;
                }

                yield return new KeyInstruction("Switch Mode", new KeyInfo(Key.Tab));
            }
        }

        protected override bool OnKeyDown(KeyInfo key, ref Vec? value)
        {
            // map a key to a direction
            Direction keyDirection = Direction.None;
            switch (key.Key)
            {
                case Key.I: keyDirection = Direction.NW; break;
                case Key.O: keyDirection = Direction.N; break;
                case Key.P: keyDirection = Direction.NE; break;
                case Key.Semicolon: keyDirection = Direction.E; break;
                case Key.Slash: keyDirection = Direction.SE; break;
                case Key.Period: keyDirection = Direction.S; break;
                case Key.Comma: keyDirection = Direction.SW; break;
                case Key.K: keyDirection = Direction.W; break;
            }

            switch (mOverheadControl.TargetMode)
            {
                case TargetMode.Direction:
                    if (keyDirection != Direction.None)
                    {
                        mOverheadControl.Target = mOverheadControl.Game.Hero.Position + keyDirection;
                        value = mOverheadControl.Target;
                        return true;
                    }
                    else if (key.Key == Key.Tab)
                    {
                        mOverheadControl.TargetMode = TargetMode.Absolute;
                    }
                    break;

                case TargetMode.Absolute:
                    if (keyDirection != Direction.None)
                    {
                        mOverheadControl.Target += keyDirection;
                    }
                    else if (key.Key == Key.Tab)
                    {
                        mOverheadControl.TargetMode = TargetMode.Entity;
                    }
                    break;

                case TargetMode.Entity:
                    if (keyDirection != Direction.None)
                    {
                        //### bob: not implemented yet
                    }
                    else if (key.Key == Key.Tab)
                    {
                        mOverheadControl.TargetMode = TargetMode.Direction;
                        mOverheadControl.Target = mOverheadControl.Game.Hero.Position;
                    }
                    break;
            }

            value = mOverheadControl.Target;
            return false;
        }

        private OverheadControl mOverheadControl;
    }
}
