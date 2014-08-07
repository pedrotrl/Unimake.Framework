using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.ETL
{
    /// <summary>
    /// Define a linha que será transformada ou selecionada
    /// </summary>
    public interface IRow
    {
        /// <summary>
        /// Índice que retorna o objeto
        /// </summary>
        /// <param name="field">nome do campo</param>
        /// <returns>Objeto deste campo</returns>
        object this[string field] { get; }

        /// <summary>
        /// Campos que compõe esta linha
        /// </summary>
        IEnumerable<string> Fields { get; }

        /// <summary>
        /// Se verdadeiro o campo "field" existe nesta coleção
        /// </summary>
        /// <param name="field">nome do campo</param>
        /// <returns>true, se o objeto existir, senão false</returns>
        bool ContainsField(string field);
    }
}
