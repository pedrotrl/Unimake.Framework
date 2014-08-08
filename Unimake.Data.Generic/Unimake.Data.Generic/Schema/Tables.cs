using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.OleDb;
using System.Data;

namespace Unimake.Data.Generic.Schema
{
    /// <summary>
    /// coleção de tabelas da base de dados
    /// </summary>
    public class Tables: List<Table>
    {
        #region static
        /// <summary>
        /// Lista de tabelas carregadas previamente
        /// </summary>
        public static Tables TableList { get; private set; }
        #endregion

        #region Construtores
        private Tables()
        { }
        #endregion

        #region Métodos
        /// <summary>
        /// verifica se uma tabela existe
        /// </summary>
        /// <param name="tableName">nome da tabela</param>
        /// <returns>retorna true se existir</returns>
        public bool Exists(string tableName)
        {
            foreach(Table t in this)
                if(t.Name.Equals(tableName, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }

        /// <summary>
        /// retorna o índice da tabela na coleção
        /// </summary>
        /// <param name="tableName">nome da tabela</param>
        /// <returns>índice</returns>
        public int IndexOf(string tableName)
        {
            for(int i = 0; i <= Count - 1; i++)
            {
                Table item = this[i];
                if(item.Name.Equals(tableName, StringComparison.InvariantCultureIgnoreCase))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// use este método para recuperar uma tabela que nao sabe 
        /// se existe ou nao na base de dados
        /// </summary>
        /// <param name="tableName">nome da tabela</param>
        /// <returns>retorna null se a tabela nao existir</returns>
        public Table GetTable(string tableName)
        {
            Table ret = null;
            if(Exists(tableName)) ret = this[tableName];
            return ret;
        }

        /// <summary>
        /// Verifica se o índice contém uma tabela através do nome.
        /// </summary>
        /// <param name="tableName">Nome da tabela</param>
        /// <returns>Table</returns>
        public bool Contains(string tableName)
        {
            if(IndexOf(tableName) >= 0)
                return true;
            else
                return false;
        }
        #endregion

        #region Indices
        public Table this[string tableName]
        {
            get { return this[IndexOf(tableName)]; }

            set { this[IndexOf(tableName)] = value; }

        }
        #endregion

        #region Refresh
        public static Tables GetTables(Connection connection)
        {
            Tables result = null;

            bool flagCnn = false;
            try
            {
                if(connection.State == ConnectionState.Closed)
                {
                    flagCnn = true;
                    connection.Open();
                }

                if(connection.DatabaseType == DatabaseType.PostgreSQL)
                    result = GetTablesFromPGSql(connection);
                else if(connection.DatabaseType == DatabaseType.SQLite)
                    result = GetTablesFromSQLite(connection);

            }
            finally
            {
                if(flagCnn)
                    connection.Close();
            }

            TableList = result.MemberwiseClone() as Tables;

            return result;
        }

        private static Tables GetTablesFromPGSql(Connection connection)
        {
            Tables result = new Tables();

            Command cmd = connection.CreateCommand("tables");
            cmd.CommandText = "SELECT table_name FROM information_schema.tables ";
            cmd.CommandText += "WHERE table_type = 'BASE TABLE'";
            cmd.CommandText += " AND table_schema = 'public'";
            DataReader rs = cmd.ExecuteReader();

            while(rs.Read())
                result.Add(new Table(connection, rs.GetString(0)));

            return result;
        }

        private static Tables GetTablesFromSQLite(Connection connection)
        {
            Tables result = new Tables();
            Command cmd = connection.CreateCommand("tables");
            cmd.CommandText = @"SELECT tbl_name FROM sqlite_master 
                                WHERE type = 'table' AND name <> 'sqlite_sequence'";

            DataReader rs = cmd.ExecuteReader();

            while(rs.Read())
                result.Add(new Table(connection, rs.GetString(0)));

            return result;
        }
        #endregion
    }
}
