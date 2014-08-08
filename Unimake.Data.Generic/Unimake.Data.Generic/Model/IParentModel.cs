using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unimake.Data.Generic.Definitions;

namespace Unimake.Data.Generic.Model
{
    /// <summary>
    /// public interface base para todos os tipos de objeto dentro da aplicação que são tabelas do tipo Pai
    /// </summary>
    public interface IParentModel: IBaseModel
    {
        /// <summary>
        /// Exclui o registro e retorna verdadeiro se excluiu com sucesso
        /// </summary>
        /// <returns></returns>
        bool Delete();

        /// <summary>
        /// Popula um registro pelo GUID
        /// </summary>
        /// <param name="pk">identificador do registro</param>
        void Populate<T>(T pk);

        /// <summary>
        /// Prepara os valores e retorna os em um formato de exibição para o usuário
        /// <para>Como padrão retorna os três primeiros campos do select que foi criado</para>
        /// </summary>
        /// <param name="where">Filtro, se necessário. Não é obrigatório e pode ser nulo</param>
        /// <returns>Retorna os valores em um um formato de exibição para o usuário</returns>
        IDisplayValues GetDisplayValues(Where where = null);
    }

    /// <summary>
    /// Esta interface representa as classes pais e filhas ao mesmo tempo.
    /// </summary>
    /// <typeparam name="T">Tipo pai desta classe</typeparam>
    public interface IParentModel<T>: IChildModel<T>, IParentModel
        where T: IParentModel
    {

    }
}
