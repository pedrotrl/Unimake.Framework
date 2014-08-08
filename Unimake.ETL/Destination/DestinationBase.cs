using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.ETL.Destination
{
    /// <summary>
    /// Define a base para os tipos de destino
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DestinationBase<T>: IWithActionDestination<T>
        where T: IDestination
    {
        #region actions
        /// <summary>
        /// Processa o destino
        /// </summary>
        protected virtual Action<IRow> DoProcess { get; set; }

        /// <summary>
        /// Executa o processo de inserção no objeto de destino
        /// </summary>
        protected virtual Action<IRow> DoInsert { get; set; }

        /// <summary>
        /// Executa o processo de atualização no objeto de destino
        /// </summary>
        protected virtual Action<IRow> DoUpdate { get; set; }

        /// <summary>
        /// Executa o processo de exclusão no objeto de destino
        /// </summary>
        protected virtual Action<IRow> DoDelete { get; set; }
        #endregion

        #region protected
        /// <summary>
        /// Indica que está no contexto de transformação do objeto
        /// </summary>
        /// <param name="context">Contexto de transformação</param>
        /// <param name="action"></param>
        protected virtual void InTransformContext(ITransform context, Action action)
        {
            action();
        }
        #endregion

        #region IDestination members
        /// <summary>
        /// Indica que está no contexto de transformação do objeto
        /// </summary>
        /// <param name="context">Contexto de transformação</param>
        /// <param name="action"></param>
        void IDestination.InTransformContext(ITransform context, Action action)
        {
            this.InTransformContext(context, action);
        }

        /// <summary>
        /// Processa o destino
        /// </summary>
        /// <param name="row">Linha que deverá ser processada por este destino</param>
        public virtual void Process( IRow row)
        {
            DoProcess(row);
        }

        /// <summary>
        /// Executa o processo de inserção no objeto de destino
        /// </summary>
        /// <param name="row">Linha que deverá ser processada por este destino</param>
        public virtual void Insert(IRow row)
        {
            DoInsert(row);
        }

        /// <summary>
        /// Executa o processo de atualização no objeto de destino
        /// </summary>
        /// <param name="row">Linha que deverá ser processada por este destino</param>
        public virtual void Update( IRow row)
        {
            DoUpdate(row);
        }

        /// <summary>
        /// Executa o processo de exclusão no objeto de destino
        /// </summary>
        /// <param name="row">Linha que deverá ser processada por este destino</param>
        public virtual void Delete( IRow row)
        {
            DoDelete(row);
        }
        #endregion

        #region IWithActionDestination<ObjectDestination> members
        /// <summary>
        /// Ação que será processada no destino
        /// </summary>
        /// <param name="action">ação que será executada para este método</param>
        public abstract T ProcessWithAction(Action<IRow> action);

        /// <summary>
        /// Executa o processo de inserção no objeto de destino
        /// </summary>
        /// <param name="action">ação que será executada para este método</param>
        public abstract T InsertWithAction(Action<IRow> action);

        /// <summary>
        /// Executa o processo de atualização no objeto de destino
        /// </summary>
        /// <param name="action">ação que será executada para este método</param>
        public abstract T UpdateWithAction(Action<IRow> action);

        /// <summary>
        /// Executa o processo de exclusão no objeto de destino
        /// </summary>
        /// <param name="action">ação que será executada para este método</param>
        public abstract T DeleteWithAction(Action<IRow> action);
        #endregion
    }
}
