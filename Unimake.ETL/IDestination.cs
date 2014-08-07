using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.ETL
{
    /// <summary>
    /// Responsável pelos processo de carga após a transformação
    /// </summary>
    public interface IDestination
    {
        /// <summary>
        /// Indica que está no contexto de transformação do objeto
        /// </summary>
        /// <param name="context">Contexto de transformação</param>
        /// <param name="action"></param>
        void InTransformContext(ITransform context, Action action);

        /// <summary>
        /// Processa o destino
        /// </summary>
        /// <param name="row">Linha que deverá ser processada por este destino</param>
        void Process(IRow row);

        /// <summary>
        /// Executa o processo de inserção no objeto de destino
        /// </summary>
        /// <param name="row">Linha que deverá ser processada por este destino</param>
        void Insert(IRow row);

        /// <summary>
        /// Executa o processo de atualização no objeto de destino
        /// </summary>
        /// <param name="row">Linha que deverá ser processada por este destino</param>
        void Update(IRow row);

        /// <summary>
        /// Executa o processo de exclusão no objeto de destino
        /// </summary>
        /// <param name="row">Linha que deverá ser processada por este destino</param>
        void Delete(IRow row);
    }
}
