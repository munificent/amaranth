using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Util
{
    //### bob: refactor code to use this
    /// <summary>
    /// <see cref="Exception"/> class for receiving an unknown enum value. Throw this, for example,
    /// from the <c>default</c> case of a <c>switch</c> that expects to cover all enum values.
    /// </summary>
    public class UnknownEnumException : Exception
    {
        public UnknownEnumException(object value)
            : base("The enum value \"" + value.ToString() + "\" is not known.")
        {
        }
    }
}
