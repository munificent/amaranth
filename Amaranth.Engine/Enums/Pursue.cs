using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// Describes how well a <see cref="Monster"/> pursues its target.
    /// </summary>
    public enum Pursue
    {
        Closely,
        Unmoving,
        SlightlyErratically,
        Erratically,
        VeryErratically
    }
}
