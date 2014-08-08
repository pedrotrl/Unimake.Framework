using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Data.Generic.Definitions.Mapping
{
    /// <summary>
    /// Implementa o método responsável por definir o tipo base para ser utilizado dentro da base de dados
    /// </summary>
    public interface IMappingType
    {
        /// <summary>
        /// Retorna o valor mapeado para aser usado na base de dados
        /// </summary>
        object ConvertToDbValue();
    }
}