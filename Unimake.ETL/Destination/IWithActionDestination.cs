using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unimake.Data.Generic;

namespace Unimake.ETL.Destination
{
    /// <summary>
    /// Determina que este destino pode ser utilizado ações 
    /// <typeparam name="T">Determina o tipo de objeto que será retornado pelas ações</typeparam>
    /// </summary>
    interface IWithActionDestination<T>: IDestination
        where T: IDestination
    {
        /// <summary>
        /// Processa o destino
        /// </summary>
        /// <param name="action">ação que será executada para este método</param>
        T ProcessWithAction(Action<IRow> action);

        /// <summary>
        /// Executa o processo de inserção no objeto de destino
        /// </summary>
        /// <param name="action">ação que será executada para este método</param>
        T InsertWithAction(Action<IRow> action);

        /// <summary>
        /// Executa o processo de atualização no objeto de destino
        /// </summary>
        /// <param name="action">ação que será executada para este método</param>
        T UpdateWithAction(Action<IRow> action);

        /// <summary>
        /// Executa o processo de exclusão no objeto de destino
        /// </summary>
        /// <param name="action">ação que será executada para este método</param>
        T DeleteWithAction(Action<IRow> action);
    }
}
