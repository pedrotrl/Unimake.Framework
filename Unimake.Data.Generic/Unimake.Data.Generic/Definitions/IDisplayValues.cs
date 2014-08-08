using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unimake.Data.Generic.Model;
using Unimake.Data.Generic.Helper.Paginate;

namespace Unimake.Data.Generic.Definitions
{
    /// <summary>
    /// Interface responsável por exibir os valores 
    /// </summary>
    public interface IDisplayValues
    {
        /// <summary>
        /// Nome das colunas. A primeira coluna é sempre oculta
        /// </summary>
        IList<Parameter> Columns { get; set; }

        /// <summary>
        /// Valores que serão adicionados na grid.
        /// <para>A chave será sempre a coluna passada na propriedade Columns</para>
        /// </summary>
        IList<object[]> Values { get; set; }

        /// <summary>
        /// Modelo para ser usado em caso de uma atualização
        /// </summary>
        IParentModel Model { get; }

        /// <summary>
        /// Condição de filtro utilizada para buscar os valores exibidos.
        /// </summary>
        Where Where { get; set; }

        /// <summary>
        /// Objeto DataReader utilizado para preencher os valores de exibição
        /// </summary>
        DataReader DataReader { get; set; }

        /// <summary>
        /// Define se estes valores serão paginados dinamicamente.
        /// <para>Se informado, será utilizado este método para tratar os valores que serão exibidos ao objeto consumidor</para>
        /// </summary>
        PagingHelperHandler DynamicPaging { get; set; }

        /// <summary>
        /// Retorna true se for uma paginação dinâmica
        /// </summary>
        bool IsDynamicPaging { get; }

        /// <summary>
        /// Retorna true se os valores estão filtrados
        /// </summary>
        bool IsFiltered { get; }

        /// <summary>
        /// Move o registro para a página solicitada.
        /// <para>Por ser uma definição dinâmica, não temos como saber quem é a última página.</para>
        /// </summary>
        /// <returns>true se realmente moveu para a página</returns>
        bool MoveToPage(int page);

        /// <summary>
        /// Filtra os registros de acordo com a condição passada
        /// </summary>
        /// <param name="w">Filtro que deverá ser aplicado</param>
        void ApplyFilters(Where w);

        /// <summary>
        /// Limpa os filtros que foram aplicados ao registro
        /// </summary>
        void ClearFilters();
    }
}
