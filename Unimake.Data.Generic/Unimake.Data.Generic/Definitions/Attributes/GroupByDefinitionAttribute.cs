using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Data.Generic.Definitions.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class GroupByDefinitionAttribute : Attribute
    {
        public string  GroupBy { get; private set; }
        public GroupByDefinitionAttribute(string groupBy)
        {
            GroupBy = groupBy;
        }
    }
}
