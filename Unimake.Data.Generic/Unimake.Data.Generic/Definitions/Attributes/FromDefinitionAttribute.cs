using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Data.Generic.Definitions.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class FromDefinitionAttribute: Attribute
    {
        public IList<string> Joins { get; private set; }
        public string From { get; private set; }

        public FromDefinitionAttribute(string from, params string[] joins)
        {
            Joins = new List<string>();
            From = from;

            foreach(var item in joins)
            {
                Joins.Add(item);
            }
        }
    }
}