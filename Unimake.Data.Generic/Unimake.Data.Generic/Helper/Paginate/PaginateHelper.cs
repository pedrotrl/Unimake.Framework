using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unimake.Data.Generic.Definitions;

namespace Unimake.Data.Generic.Helper.Paginate
{
    /// <summary>
    /// Define o Handler que irá tratar o método de paginação dos resultados
    /// </summary>
    /// <param name="where">Condição where para a paginação. Se não informado o limite será usado 1000 como padrão</param>
    /// <returns></returns>
    public delegate IDisplayValues PagingHelperHandler(Where where);
}
