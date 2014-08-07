using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.ETL
{
    /// <summary>
    /// Transforma a fonte de origem para a fonte esperada pelo destino
    /// </summary>
    public interface ITransform
    {
        /// <summary>
        /// Retorna os campos de origem mapeados
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetMappedSourceFields();

        /// <summary>
        /// Retorna os campos de destino mapeados
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetMappedDestinationFields();

        /// <summary>
        /// Executa o processo de transformação do objeto
        /// </summary>
        /// <returns>Retorna o objeto que foi criado por este processo de transformação.
        /// <para>Pode ser o caminho de um arquivo, ou o objeto em memória. Depende de cada
        /// implementação</para></returns>
        object Execute();

        /// <summary>
        /// Executa o processo de transformação do objeto
        /// </summary>
        /// <returns>Retorna o objeto que foi criado por este processo de transformação.</returns>
        T Execute<T>();
    }
}
