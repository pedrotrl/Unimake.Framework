using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Unimake.ETL
{
    /// <summary>
    /// Dicionário de linhas que serão transformadas
    /// </summary>
    public class DictionaryRow: IRow
    {
        /// <summary>
        /// Valores que estão associados a este dicionário
        /// </summary>
        protected IDictionary<string, object> Values { get; set; }

        /// <summary>
        /// Instancia este objeto
        /// </summary>
        public DictionaryRow()
        {
            this.Values = new Dictionary<string, object>();
        }

        /// <summary>
        /// Instancia este objeto
        /// </summary>
        /// <param name="values">Valores para inicialização do objeto</param>
        public DictionaryRow(IDictionary<string, object> values)
        {
            this.Values = values;
        }

        /// <summary>
        /// Índice para retornar a linha tratada
        /// </summary>
        /// <param name="field">nome do campo para retornar a linha</param>
        /// <returns></returns>
        public object this[string field]
        {
            get
            {
                object val;
                this.Values.TryGetValue(field, out val);
                return val;
            }
            set
            {
                this.Values[field] = value;
            }
        }

        /// <summary>
        /// Campos que compõe este dicionário
        /// </summary>
        public IEnumerable<string> Fields
        {
            get
            {
                return this.Values.Keys;
            }
        }

        /// <summary>
        /// Se true, o campo requerido faz parte deste dicionário
        /// </summary>
        /// <param name="field">Nome do campo que será validado</param>
        /// <returns></returns>
        public bool ContainsField(string field)
        {
            return this.Values.ContainsKey(field);
        }
    }
}
