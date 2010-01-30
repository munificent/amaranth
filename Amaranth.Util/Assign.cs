using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Util
{
    public static class Assign
    {
        /// <summary>
        /// Initializes a field by assigning it the given value only if the field has not been initialized (i.e. is null)
        /// and the value is not null.
        /// </summary>
        /// <typeparam name="T">Type of field to assign.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value to initialize it to.</param>
        /// <param name="argName">The argument name of the value.</param>
        public static void OnlyOnce<T>(ref T field, T value, string argName)
        {
            if (value == null) throw new ArgumentNullException(argName);
            if (field != null) throw new InvalidOperationException("Cannot assign " + argName + " more than once.");

            field = value;
        }
    }
}
