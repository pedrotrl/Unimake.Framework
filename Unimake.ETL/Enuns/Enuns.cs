using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.ETL.Enuns
{
    /// <summary>
    /// Operação que será executada pela linha
    /// </summary>
    public enum RowOperation
    {
        /// <summary>
        /// Simplesmente processa a linha no destino
        /// </summary>
        Process,

        /// <summary>
        /// Será executado um insert no destino
        /// </summary>
        Insert,

        /// <summary>
        /// Será executado uma atualização no destino
        /// </summary>
        Update,

        /// <summary>
        /// Será executado uma exclusão no destino
        /// </summary>
        Delete,

        /// <summary>
        /// Ignora a linha no destino
        /// </summary>
        Ignore
    }
}
