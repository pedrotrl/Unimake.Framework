using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unimake.ETL.Transform;
using Unimake.ETL.Enuns;

namespace Unimake.ETL
{
    /// <summary>
    /// Transforma a fonte de origem para a fonte esperada pelo destino
    /// </summary>
    public interface ITransform
    {
        /// <summary>
        /// Define o mapeamento que foi utilizado para este transformação. Ex: de-> para
        /// </summary>
        Dictionary<string, string> TransformMap { get; }
        /// <summary>
        /// Objeto de origem que foi usado para esta transformação
        /// </summary>
        ISource Source { get; }

        /// <summary>
        /// Valida a linha do objeto
        /// </summary>
        Action<IRow, RowValidation> DoRowValidation { get; set; }

        /// <summary>
        /// Funções utilizadas para a transformação do valor
        /// </summary>
        Dictionary<string, Func<object, object>> TransformFuncs { get; }

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
        /// Executa o evento de linha inválida
        /// </summary>
        /// <param name="inputRow">linha atual, que causou a invalidação</param>
        /// <param name="rowValidation">Resultado da validação</param>
        void RaiseRowInvalid(IRow inputRow, RowValidation rowValidation);

        /// <summary>
        /// Retorna a operação que deverá ser executada por esta linha
        /// </summary>
        Func<IRow, RowOperation> GetRowOperation { get; set; }

        /// <summary>
        /// Executa ação definida para o registro no objeto de destino
        /// </summary>
        /// <param name="rowOp">Operação que deverás er executada</param>
        /// <param name="transformedRow">Linha que foi transformada</param>
        void ProcessTransformedRow(RowOperation rowOp, DictionaryRow transformedRow);
    }
}