using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unimake.Data.Generic.Definitions;
using Unimake.Data.Generic.Helper.Paginate;
using Unimake.Data.Generic.Model;
using Unimake.Data.Generic.Exceptions;

namespace Unimake.Data.Generic
{
    /// <summary>
    /// Classe responsável por exibir os valores 
    /// </summary>
    public class DisplayValues: IDisplayValues
    {

        #region locais
        IList<object[]> values = null;
        bool init = false;
        Where oldWhere = null;
        #endregion

        #region Propriedades
        /// <summary>
        /// Retorna o tamanho da página de registros.
        /// Padrão é 100 registros por página
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Retorna true se os valores estão filtrados
        /// </summary>
        public bool IsFiltered { get; private set; }
        /// <summary>
        /// Nome das colunas. A primeira coluna é sempre oculta
        /// </summary>
        public IList<Parameter> Columns { get; set; }

        /// <summary>
        /// Valores que serão adicionados na grid.
        /// <para>A chave será sempre a coluna passada na propriedade Columns</para>
        /// <para></para>
        /// </summary>
        public IList<object[]> Values
        {
            get
            {
                if(IsDynamicPaging && !init)
                {
                    MoveToPage(1);
                    init = true;
                }

                return values;
            }
            set { values = value; }
        }

        /// <summary>
        /// Modelo para ser usado em caso de uma atualização
        /// </summary>
        public IParentModel Model { get; private set; }

        /// <summary>
        /// Condição de filtro utilizada para buscar os valores exibidos.
        /// </summary>
        public Where Where { get; set; }

        /// <summary>
        /// Objeto DataReader utilizado para preencher os valores de exibição
        /// </summary>
        public DataReader DataReader { get; set; }

        /// <summary>
        /// Define se estes valores serão paginados dinamicamente.
        /// <para>Se informado, será utilizado este método para tratar os valores que serão exibidos ao objeto consumidor</para>
        /// </summary>
        public PagingHelperHandler DynamicPaging { get; set; }

        /// Retorna true se for uma paginação dinâmica
        /// </summary>
        public bool IsDynamicPaging { get { return DynamicPaging != null; } }
        #endregion

        #region Métodos
        /// <summary>
        /// Move o registro para a página solicitada.
        /// <para>Por ser uma definição dinâmica, não temos como saber quem é a última página.</para>
        /// </summary>
        /// <returns>true se realmente moveu para a página</returns>
        /// <exception cref="DynamicPaging">Exceção lançada quando o DynamicPaging não está definido</exception>
        public bool MoveToPage(int page)
        {
            bool result = false;

            if(IsDynamicPaging)
            {
                //se ficar como zero o offset, algo pode dar errado, então ajustamos para ter sempre o pagesize padrão
                if(Where.Limit.From <= PageSize)
                    Where.Limit = new Limit(PageSize, 0);

                Where.Limit = new Limit(Where.Limit.From * ((page - 1) + 1), Where.Limit.OffSet);
                values.Clear();

                IDisplayValues displayValues = DynamicPaging.Invoke(Where);
                if(displayValues != null)
                {
                    DataReader = displayValues.DataReader;
                    values = displayValues.Values;

                    if(page > 1 && values.Count == 0)
                    {
                        MoveToPage(--page);
                        return false;
                    }
                }

                result = (values != null && values.Count > 0);
            }
            else
                throw new DynamicPaging();

            return result;
        }

        /// <summary>
        /// Filtra os registros de acordo com a condição passada
        /// </summary>
        /// <param name="w">Filtro que deverá ser aplicado</param>
        public void ApplyFilters(Unimake.Data.Generic.Where w)
        {
            #region OldWhere
            if(oldWhere == null)
                oldWhere = Where.Clone();
            #endregion

            #region new where
            Where = new Where();
            foreach(var item in w)
            {
                Where.Add(item);
            }

            Where.Parameters.AddRange(w.Parameters);
            #endregion

            MoveToPage(1);
            IsFiltered = true;
        }

        /// <summary>
        /// Limpa os filtros que foram aplicados ao registro
        /// </summary>
        public void ClearFilters()
        {
            if(IsFiltered)
            {
                Where = oldWhere.Clone();
                MoveToPage(1);
            }
        }
        #endregion

        #region Construtores
        /// <summary>
        /// Inicia esta instância
        /// </summary>
        public DisplayValues()
            : this((IParentModel)null)
        {

        }

        /// <summary>
        /// Inicia esta instância
        /// </summary>
        public DisplayValues(IParentModel model)
        {
            PageSize = 100;
            Columns = new List<Parameter>();
            Values = new List<object[]>();
            Model = model;
            Where = new Where
            {
                Limit = new Limit(PageSize, 0)
            };
        }
        #endregion
    }
}
