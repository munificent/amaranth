using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Amaranth.UI;
using Amaranth.Engine;

namespace Amaranth.TermApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            UserInterface ui = new UserInterface("Amaranth v" + Game.Version);
            ui.Run(new WelcomeScreen());

            Console.WriteLine("saving setting " + GameSettings.Instance.LastHero);
            GameSettings.Instance.Save();
        }
    }
}
