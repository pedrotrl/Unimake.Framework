using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Data.Generic.Schema
{
    public class Database
    {
        #region Propriedades

        /// <summary>
        /// Conexão referenciada.
        /// </summary>
        public Connection Connection { get; set; }

        /// <summary>
        /// Nome do banco de dados.
        /// </summary>
        public string Name { get; internal set; }

        #endregion

        #region Construtores

        public Database()
        {
        }

        public Database(Connection connection, string name)
        {
            this.Connection = connection;
            this.Name = name;
        }

        #endregion
    }
}
