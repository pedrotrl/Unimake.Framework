using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Data.Generic.Model
{
    /// <summary>
    /// Interface para tabelas que são do tipo filhas.
    /// </summary>
    public interface IChildModel<T>: IBaseModel
        where T: IParentModel
    {
        /// <summary>
        /// Registro pai deste registro
        /// </summary>
        T Parent { get; set; }
    }
}
