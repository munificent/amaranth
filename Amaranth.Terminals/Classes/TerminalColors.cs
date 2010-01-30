using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;

namespace Amaranth.Terminals
{
    public static class TerminalColors
    {
        public static Color Black { get { return Color.Black; } }
        public static Color White { get { return Color.White; } }

        public static Color LightGray { get { return Color.FromArgb(192, 192, 192); } }
        public static Color Gray { get { return Color.FromArgb(128, 128, 128); } }
        public static Color DarkGray { get { return Color.FromArgb(48, 48, 48); } }

        public static Color LightRed { get { return Color.FromArgb(255, 160, 160); } }
        public static Color Red { get { return Color.FromArgb(220, 0, 0); } }
        public static Color DarkRed { get { return Color.FromArgb(100, 0, 0); } }

        public static Color LightOrange { get { return Color.FromArgb(255, 200, 170); } }
        public static Color Orange { get { return Color.FromArgb(255, 128, 0); } }
        public static Color DarkOrange { get { return Color.FromArgb(128, 64, 0); } }

        public static Color LightGold { get { return Color.FromArgb(255, 230, 150); } }
        public static Color Gold { get { return Color.FromArgb(255, 192, 0); } }
        public static Color DarkGold { get { return Color.FromArgb(128, 96, 0); } }

        public static Color LightYellow { get { return Color.FromArgb(255, 255, 150); } }
        public static Color Yellow { get { return Color.FromArgb(255, 255, 0); } }
        public static Color DarkYellow { get { return Color.FromArgb(128, 128, 0); } }

        public static Color LightGreen { get { return Color.FromArgb(130, 255, 90); } }
        public static Color Green { get { return Color.FromArgb(0, 200, 0); } }
        public static Color DarkGreen { get { return Color.FromArgb(0, 100, 0); } }

        public static Color LightCyan { get { return Color.FromArgb(200, 255, 255); } }
        public static Color Cyan { get { return Color.FromArgb(0, 255, 255); } }
        public static Color DarkCyan { get { return Color.FromArgb(0, 128, 128); } }

        public static Color LightBlue { get { return Color.FromArgb(128, 160, 255); } }
        public static Color Blue { get { return Color.FromArgb(0, 64, 255); } }
        public static Color DarkBlue { get { return Color.FromArgb(0, 37, 168); } }

        public static Color LightPurple { get { return Color.FromArgb(200, 140, 255); } }
        public static Color Purple { get { return Color.FromArgb(128, 0, 255); } }
        public static Color DarkPurple { get { return Color.FromArgb(64, 0, 128); } }

        public static Color LightBrown { get { return Color.FromArgb(190, 150, 100); } }
        public static Color Brown { get { return Color.FromArgb(160, 110, 60); } }
        public static Color DarkBrown { get { return Color.FromArgb(100, 64, 32); } }

        public static Color Flesh { get { return LightOrange; } }
        public static Color Pink { get { return LightRed; } }

        public static Color FromName(string name)
        {
            // use reflection to go through the properties
            foreach (PropertyInfo property in typeof(TerminalColors).GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                if (property.PropertyType.Equals(typeof(Color)) && property.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    // found it
                    return (Color)property.GetValue(null, new object[0]);
                }
            }

            throw new ArgumentException("Could not find a color named \"" + name + "\" in TerminalColors.");
        }

        public static Color FromEscapeChar(char c)
        {
            switch (c)
            {
                case 'k': return DarkGray;
                case 'K': return Black;

                case 'm': return Gray; // "m"edium

                case 'w': return White;
                case 'W': return LightGray;

                case 'r': return Red;
                case 'R': return DarkRed;

                case 'o': return Orange;
                case 'O': return DarkOrange;

                case 'l': return Gold;
                case 'L': return DarkGold;

                case 'y': return Yellow;
                case 'Y': return DarkYellow;

                case 'g': return Green;
                case 'G': return DarkGreen;

                case 'c': return Cyan;
                case 'C': return DarkCyan;

                case 'b': return Blue;
                case 'B': return DarkBlue;

                case 'p': return Purple;
                case 'P': return DarkPurple;

                case 'f': return Flesh;
                case 'F': return Brown;

                default: return White;
            }
        }
    }
}
