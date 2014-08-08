using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unimake.Data.Generic.Model;
using Unimake.Data.Generic.Schema;

namespace Unimake.Data.Generic
{
    /// <summary>
    /// Define o "Meu Tipo" para poder retornar a hierarquia das classes
    /// </summary>
    public struct MyHierarchicalType
    {
        public int Order { get; set; }
        public Type Type { get; set; }
    }
}