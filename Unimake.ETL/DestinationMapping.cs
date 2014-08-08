using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.ETL
{
    /// <summary>
    /// Mapeamento para o destino utilizado na transformação
    /// </summary>
    public class DestinationMapping
    {
        /// <summary>
        /// Campo que está sendo mapeado 
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Função que irá tratar o mapeamento e transforma o campo
        /// </summary>
        public Func<object, object> Transform { get; set; }
    }
}
