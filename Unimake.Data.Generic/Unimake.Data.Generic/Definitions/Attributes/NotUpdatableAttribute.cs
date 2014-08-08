using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Data.Generic.Definitions.Attributes
{
    /// <summary>
    /// identifica que a classe não pode sofrer UPDATE será tratada sempre com INSERT
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class NotUpdatableAttribute: Attribute
    {
    }
}
