using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Data.Generic.Definitions.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class SelectDefinitionAttribute: Attribute
    {
        public IList<SelectField> SelectFields { get; set; }

        /// <summary>
        /// inicializa os campos do select
        /// </summary>
        /// <param name="selectFields">passe no formato Tabela.Nome:Alias.
        /// <para>irá causar erro se passado de forma diferente</para></param>
        public SelectDefinitionAttribute(params string[] selectFields)
        {
            string[] pedacinhos = null;

            SelectFields = new List<SelectField>();

            foreach(string item in selectFields)
            {
                pedacinhos = item.Split(new char[] { ':' });

                SelectFields.Add(new SelectField(pedacinhos[0], pedacinhos[1]));
            }
        }
    }
}
