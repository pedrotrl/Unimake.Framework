using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.ETL.Source
{
    /// <summary>
    /// Classe de base para os tipos de origem
    /// </summary>
    public abstract class SourceBase: ISource
    {
        #region protected
        /// <summary>
        /// Indica que está no contexto de transformação do objeto
        /// </summary>
        /// <param name="context">Contexto de transformação</param>
        /// <param name="action"></param>

        protected virtual void InTransformContext(ITransform context, Action action)
        {

        }
        #endregion

        #region ISource members
        /// <summary>
        /// Indica que está no contexto de transformação do objeto
        /// </summary>
        /// <param name="context">Contexto de transformação</param>
        /// <param name="action"></param>

        void ISource.InTransformContext(ITransform context, Action action)
        {
            this.InTransformContext(context, action);
        }
        #endregion

        #region abstrações
        /// <summary>
        /// Linhas que vão sofrer a transformação
        /// </summary>
        public abstract IEnumerable<IRow> Rows { get; }
        #endregion
    }
}
