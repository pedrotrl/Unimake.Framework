using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Data.Generic.Definitions.Mapping
{
    public class ForeignKeys: Dictionary<string, object>
    {
        public new void Add(string key, object value)
        {
            //feito desta forma para não dar erros e atualizar sempre
            this[key.ToLower()] = value;
        }

        public new bool ContainsKey(string key)
        {
            return base.ContainsKey(key.ToLower());
        }

        public new object this[string key]
        {
            get { return base[key.ToLower()]; }
            set { base[key.ToLower()] = value; }
        }
    }
}
