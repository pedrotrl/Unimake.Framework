using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Unimake.ETL
{
    /// <summary>
    /// Objeto que representa uma linha do datareader
    /// </summary>
    public class DataReaderRow: IRow
    {
        private IDataReader Data { get; set; }

        /// <summary>
        /// Inicia um objeto datareader com uma linha (tupla)
        /// </summary>
        /// <param name="dr">datareader que será usado para inicar este objeto</param>
        public DataReaderRow(IDataReader dr)
        {
            this.Data = dr;
        }

        /// <summary>
        /// retorna a linha do datareader
        /// </summary>
        /// <param name="key">chave existente no datareader</param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                return this.Data[key];
            }
        }

        /// <summary>
        /// Retorna os nomes dos campos que compõe esta linha
        /// </summary>
        public IEnumerable<string> Fields
        {
            get
            {
                for(int i = 0; i < this.Data.FieldCount; i++)
                    yield return this.Data.GetName(i);
            }
        }

        /// <summary>
        /// Retorna true se o campo informado existir
        /// </summary>
        /// <param name="field">Nome do campo</param>
        /// <returns></returns>
        public bool ContainsField(string field)
        {
            for(int i = 0; i < this.Data.FieldCount; i++)
                if(this.Data.GetName(i) == field)
                    return true;

            return false;
        }
    }
}
