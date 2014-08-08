using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unimake.Data.Generic.Definitions.Mapping;

namespace Unimake.Data.Generic.Definitions
{
    /// <summary>
    /// Identifica um identificador único para a tabela
    /// </summary>
    public interface IID: IMappingType, ICloneable, IComparable
    {
        /// <summary>
        /// Retorna true se este ID for válido
        /// </summary>
        /// <returns></returns>
        bool IsValid();

        /// <summary>
        /// Cria um novo ID vazio
        /// </summary>
        /// <typeparam name="T">Define o tipo deste ID</typeparam>
        /// <returns></returns>
        T CreateNewID<T>();

        /// <summary>
        /// Cria um novo ID vazio com base no parâmetro passado e valida
        /// </summary>
        /// <param name="id">identificador que deverá ser criado</param>
        /// <typeparam name="T">Define o tipo deste ID</typeparam>
        /// <returns></returns>
        T CreateNewID<T>(T id);
    }
}
