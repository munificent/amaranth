using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// Defines basic on/off flags a <see cref="Race"/> can have.
    /// </summary>
    [Flags]
    public enum RaceFlags
    {
        Default     = 0,

        OpensDoors  = 1 << 0,
        Boss        = 1 << 1,
        Unique      = 1 << 2
    }

    public static class RaceFlagExtensions
    {
        public static bool IsSet(this RaceFlags options, RaceFlags flags)
        {
            return (options & flags) == flags;
        }
    }
}
