using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Unimake.Data.Generic;
using Unimake.Data.Generic.Exceptions;

namespace Unimake.ETL.Source
{
    /// <summary>
    /// Abstração para os tipos sql de origem 
    /// </summary>
    public abstract class SqlBase: SourceBase, IFieldNames
    {
        #region propriedades
        /// <summary>
        /// Conexão atual do objeto
        /// </summary>
        protected Connection ConnectionObject { get; set; }

        /// <summary>
        /// String de conexão do objeto
        /// </summary>
        protected string ConnectionString { get; set; }

        /// <summary>
        /// Tipo de comando que será executado
        /// </summary>
        protected CommandType CommandType { get; set; }

        /// <summary>
        /// Texto que será passado ao comando
        /// </summary>
        protected string CommandText { get; set; }
        #endregion

        #region métodos
        /// <summary>
        /// Retorna a lista de nomes deste objeto
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> GetFieldNames()
        {
            IList<string> fieldNames = new List<string>();
            WithConnection(conn =>
            {
                Command cmd = GetSelectCommand(conn);
                DataReader rdr = cmd.ExecuteReader(CommandBehavior.KeyInfo);
                DataTable schemaTable = rdr.GetSchemaTable();
                foreach(DataRow dr in schemaTable.Rows)
                {
                    fieldNames.Add((string)dr["BaseColumnName"]);
                }
            });
            return fieldNames;
        }

        /// <summary>
        /// Cria o comando Select e retorna
        /// </summary>
        /// <param name="conn">conexão utilizada para a criação do objeto command</param>
        /// <returns></returns>
        protected Command GetSelectCommand(Connection conn)
        {
            Command result;
            switch(this.CommandType)
            {
                case CommandType.Text:
                    result = new Command(this.CommandText, conn);
                    break;
                case CommandType.StoredProcedure:
                    result = new Command(this.CommandText, conn);
                    break;
                default:
                    result = new Command("SELECT * FROM " + CommandText, conn);
                    break;
            }
            return result;
        }

        /// <summary>
        /// Abre a conexão e executa a ação associada
        /// </summary>
        /// <param name="action">ação que deverá ser executada</param>
        protected void WithConnection(Action<Connection> action)
        {
            if(this.ConnectionObject != null)
            {
                bool doOpen = this.ConnectionObject.State == ConnectionState.Closed;
                if(doOpen)
                    this.ConnectionObject.Open();
                action(this.ConnectionObject);
                if(doOpen)
                    this.ConnectionObject.Close();
            }
            else if(this.ConnectionString != null)
            {
                using(Connection conn = new Connection(this.ConnectionString))
                {
                    conn.Open();
                    action(conn);
                }
            }
            else
                throw new NoConnectionSet();
        }

        /// <summary>
        /// Abre a conexão e executa a lista de ações associadas
        /// </summary>
        /// <typeparam name="T">Tipo de ação que será executada</typeparam>
        /// <param name="func">Ações que serão executadas</param>
        /// <returns></returns>
        protected IEnumerable<T> WithConnectionEnumerate<T>(Func<Connection, IEnumerable<T>> func)
        {
            if(this.ConnectionObject != null)
            {
                bool doOpen = this.ConnectionObject.State == ConnectionState.Closed;
                if(doOpen)
                    this.ConnectionObject.Open();
                foreach(T item in func(this.ConnectionObject))
                    yield return item;
                if(doOpen)
                    this.ConnectionObject.Close();
            }
            else if(this.ConnectionString != null)
            {
                using(Connection conn = new Connection(this.ConnectionString))
                {
                    conn.Open();
                    foreach(T item in func(conn))
                        yield return item;
                }
            }
            else
            {
                this.ConnectionObject = DbContext.CreateConnection();

                foreach(T item in func(ConnectionObject))
                    yield return item;
            }
        }
        #endregion
    }
}
