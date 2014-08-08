using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.ETL
{
    /// <summary>
    /// Define o tipo de origem que terá seus dados recuperados
    /// </summary>
    public interface ISource
    {
        /// <summary>
        /// Indica que está no contexto de transformação do objeto
        /// </summary>
        /// <param name="context">Contexto de transformação</param>
        /// <param name="action">Ação que será executada para a transformação</param>
        void InTransformContext(ITransform context, Action action);

        /// <summary>
        /// Linhas que vão sofrer a transformação
        /// </summary>
        IEnumerable<IRow> Rows { get; }
    }
}
