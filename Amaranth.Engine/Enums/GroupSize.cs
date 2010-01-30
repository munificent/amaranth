using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// Defines the different groups sizes a <see cref="Race"/> of <see cref="Monster">Monsters</see> may
    /// be spawned at.
    /// </summary>
    public enum GroupSize
    {
        Single,
        Group,
        Pack,
        Swarm,
        Horde
    }
}
