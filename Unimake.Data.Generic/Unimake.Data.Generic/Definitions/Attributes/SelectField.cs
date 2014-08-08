using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Data.Generic.Definitions.Attributes
{
    public struct SelectField
    {
        public string FieldName { get; private set; }
        public string FieldAlias { get; private set; }

        public SelectField(string fieldName, string fieldAlias)
            : this()
        {
            FieldName = fieldName;
            FieldAlias = fieldAlias;
        }

        public override string ToString()
        {
            return FieldName + " AS " + FieldAlias;
        }
    }
}
