using System;
using System.Collections.Generic;
using System.Text;

namespace Amaranth.Engine
{
    [Flags]
    public enum GameResultFlags
    {
        Default         = 0x0000,
        NeedsPause      = 0x0001,
        NeedsUserInput  = 0x0002,
        CheckForCancel  = 0x0004,

        GameOver        = 0x0080
    }

    public class GameResult
    {
        public bool NeedsPause { get { return IsFlagSet(GameResultFlags.NeedsPause); } }
        public bool NeedsAction { get { return IsFlagSet(GameResultFlags.NeedsUserInput); } }
        public bool CheckForCancel { get { return IsFlagSet(GameResultFlags.CheckForCancel); } }

        public bool IsGameOver { get { return IsFlagSet(GameResultFlags.GameOver); } }

        public Entity Entity { get { return mEntity; } }

        public GameResult(GameResultFlags flags, Entity entity)
        {
            mFlags = flags;
            mEntity = entity;
        }

        public GameResult(GameResultFlags flags)
            : this(flags, null)
        {
        }

        public GameResult()
            : this(GameResultFlags.Default, null)
        {
        }

        private bool IsFlagSet(GameResultFlags flags)
        {
            return (mFlags & flags) == flags;
        }

        private GameResultFlags mFlags;
        private Entity mEntity;
    }
}
