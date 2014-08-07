using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.ETL
{
    /// <summary>
    /// Mapeia os métodos necessários para retornar um coleção de campos
    /// </summary>
    public interface IFieldNames
    {
        /// <summary>
        /// Retorna uma coleção de nomes de campos
        /// </summary>
        /// <returns>Retorna uma coleção de nomes de campos</returns>
        IEnumerable<string> GetFieldNames();
    }
}
