using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;
using Amaranth.Terminals;

namespace Amaranth.UI
{
    public interface IUserInterfaceScreen
    {
        void Attach(NotNull<UserInterface> ui, ITerminal terminal);
        void Detach(NotNull<UserInterface> ui);

        /// <summary>
        /// Called when the Screen has become the current screen after having a screen above it removed.
        /// </summary>
        void Activate();

        /// <summary>
        /// Called when the Screen is no longer the current screen after having a new one added above it.
        /// </summary>
        void Deactivate();
    }
}
