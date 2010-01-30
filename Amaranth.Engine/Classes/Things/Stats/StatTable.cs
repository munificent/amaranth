using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Contains lookup tables to convert <see cref="Stat"/> values into various game modifiers
    /// and bonuses.
    /// </summary>
    public static class StatTable
    {
        /// <summary>
        /// Gets a bonus curve that ranges from -50 to +100, centered around 15.
        /// </summary>
        public static int GetNegativeFiftyToHundred(int value)
        {
            if (value <= 15)
            {
                switch (value)
                {
                    case 1: return -50;    // crippled
                    case 2: return -43;
                    case 3: return -35;
                    case 4: return -28;
                    case 5: return -22;
                    case 6: return -17;
                    case 7: return -12;
                    case 8: return -8;
                    case 9: return -5;
                    case 10: return -3;     // weak
                    case 11: return -2;
                    case 12: return -1;
                    case 13: return -1;
                    default: return 0;
                }
            }
            else if (value <= 20) return Math2.Remap(15, value, 20,  0,   5);
            else if (value <= 25) return Math2.Remap(20, value, 25,  5,  15);
            else if (value <= 40) return Math2.Remap(25, value, 40, 15,  60);
            else                  return Math2.Remap(40, value, 50, 60, 100);
        }

        public static float GetNegativeThreeToTen(int value)
        {                                                                           //  1 = -3.0
            if      (value <= 10) return Math2.Remap( 1, value, 10, -3.0f, -1.0f);  // 10 = -1.0
            else if (value <= 15) return Math2.Remap(10, value, 15, -1.0f,  0.0f);  // 15 =  0
            else if (value <= 20) return Math2.Remap(15, value, 20,  0.0f,  1.0f);  // 20 =  1.0
            else if (value <= 30) return Math2.Remap(20, value, 30,  1.0f,  3.0f);  // 30 =  3.0
            else if (value <= 40) return Math2.Remap(30, value, 40,  3.0f,  6.0f);  // 40 =  6.0
            else                  return Math2.Remap(40, value, 50,  6.0f, 10.0f);  // 50 = 10.0
        }

        public static float GetPointFiveToTwo(int value)
        {                                                                          //  1 = 0.5
            if (value <= 10) return Math2.Remap(1, value, 10, 0.5f, 0.8f);         // 10 = 0.8
            else if (value <= 15) return Math2.Remap(10, value, 15, 0.8f, 1.0f);   // 15 = 1.0
            else if (value <= 20) return Math2.Remap(15, value, 20, 1.0f, 1.2f);   // 20 = 1.2
            else if (value <= 30) return Math2.Remap(20, value, 30, 1.2f, 1.4f);   // 30 = 1.4
            else if (value <= 40) return Math2.Remap(30, value, 40, 1.4f, 1.6f);   // 40 = 1.6
            else return Math2.Remap(40, value, 50, 1.6f, 2.0f);                    // 50 = 2.0
        }

        public static float GetPointOneToTen(int value)
        {                                                                          //  1 = 0.1
            if (value <= 10) return Math2.Remap(1, value, 10, 0.1f, 0.5f);         // 10 = 0.5
            else if (value <= 15) return Math2.Remap(10, value, 15, 0.5f, 1.0f);   // 15 = 1.0
            else if (value <= 20) return Math2.Remap(15, value, 20, 1.0f, 2.0f);   // 20 = 2.0
            else if (value <= 30) return Math2.Remap(20, value, 30, 2.0f, 3.0f);   // 30 = 3.0
            else if (value <= 40) return Math2.Remap(30, value, 40, 3.0f, 6.0f);   // 40 = 6.0
            else return Math2.Remap(40, value, 50, 6.0f, 10.0f);                   // 50 = 10.0
        }
    }
}
