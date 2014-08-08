using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Unimake.Data.Generic.Schema
{
    public class Databases: List<Database>
    {
        #region Static

        /// <summary>
        /// Lista dos bancos de dados carregados previamente.
        /// </summary>
        public static Databases DatabaseList { get; set; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor.
        /// </summary>
        public Databases()
        {

        }

        #endregion

        #region Índices

        /// <summary>
        /// Retorna uma database através do nome em que essa tem no índice.
        /// </summary>
        /// <param name="databaseName">Nome da database</param>
        /// <returns>Database</returns>
        public Database this[string databaseName]
        {
            get { return this[IndexOf(databaseName)]; }
            set { this[IndexOf(databaseName)] = value; }

        }

        #endregion

        #region Métodos

        /// <summary>
        /// Retorna o índice do banco de dados na coleção.
        /// </summary>
        /// <param name="databaseName">Nome do banco de dados.</param>
        /// <returns>int</returns>
        public int IndexOf(string databaseName)
        {
            for(int i = 0; i <= Count - 1; i++)
            {
                Database item = this[i];
                if(item.Name.Equals(databaseName, StringComparison.InvariantCultureIgnoreCase))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Verifica se o índice contém uma database pelo nome.
        /// </summary>
        /// <param name="databaseName">Nome do banco de dados</param>
        /// <returns>bool</returns>
        public bool Exists(string databaseName)
        {
            foreach(Database database in this)
                if(database.Name.Equals(databaseName, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }

        /// <summary>
        /// Verifica se o índice contém uma database pelo nome.
        /// </summary>
        /// <param name="databaseName">Nome da database</param>
        /// <returns>bool</returns>
        public bool Contains(string databaseName)
        {
            if(IndexOf(databaseName) < 0)
                return false;

            return true;
        }

        /// <summary>
        /// Pega uma database pelo nome no índice e retorna
        /// o objeto da classe correspondente.
        /// </summary>
        /// <param name="databaseName">Nome da database</param>
        /// <returns>Database</returns>
        public Database GetDatabase(string databaseName)
        {
            Database database = null;
            if(Exists(databaseName))
                database = this[databaseName];

            return database;
        }

        #endregion

        #region Refresh

        /// <summary>
        /// Obtém todos os bancos de dados.
        /// </summary>
        /// <param name="connection">Conexão</param>
        /// <returns>Databases - lista de databases</returns>
        public static Databases GetDatabases(Connection connection)
        {
            Databases databases = null;
            bool flagCnn = false;

            try
            {
                if(connection.State == ConnectionState.Closed)
                {
                    flagCnn = true;
                    connection.Open();
                }

                if(connection.DatabaseType == DatabaseType.PostgreSQL)
                    databases = GetDatabasesFromPGSql(connection);

                else if(connection.DatabaseType == DatabaseType.SQLite)
                    databases = new Databases { new Database { Name = connection.Database } };

                return databases;

            }
            finally
            {
                if(flagCnn)
                    connection.Close();
            }
        }

        /// <summary>
        /// Obtém o
        /// </summary>
        /// <param name="connection">Connexão com o banco</param>
        /// <returns>Lista de database</returns>
        private static Databases GetDatabasesFromPGSql(Connection connection)
        {
            Databases databases = new Databases();
            Command cmd = connection.CreateCommand("databases");
            cmd.CommandText = @"SELECT datname FROM pg_database WHERE datistemplate = false;";

            DataReader rs = cmd.ExecuteReader();

            while(rs.Read())
                databases.Add(new Database(connection, rs.GetString(0)));

            return databases;
        }
        #endregion
    }
}
