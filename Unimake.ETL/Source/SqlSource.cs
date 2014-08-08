using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Unimake.Data.Generic;

namespace Unimake.ETL.Source
{
    /// <summary>
    /// Objeto do tipo origem para bases de dados utilizando select
    /// </summary>
    public class SqlSource: SqlBase, ISource
    {
        /// <summary>
        /// Nome da tabela utilizada na clausula from. Assume que o tipo de comando é "TableDirect"
        /// </summary>
        /// <param name="tableName">Nome da tabela</param>
        /// <returns></returns>
        public SqlSource FromTable(string tableName)
        {
            this.CommandText = tableName;
            this.CommandType = CommandType.TableDirect;
            return this;
        }

        /// <summary>
        /// Assume que o comando utilizado será uma query. Assume que o tipo de comando é "Text"
        /// </summary>
        /// <param name="query">query que será executada</param>
        /// <returns></returns>
        public SqlSource FromQuery(string query)
        {
            this.CommandText = query;
            this.CommandType = CommandType.Text;
            return this;
        }

        /// <summary>
        /// Assume que o comando utilizado será uma stored procedure. Assume que o tipo de comando é "StoredProcedure"
        /// </summary>
        /// <param name="storedProcedure">Stored procedure que será executada</param>
        /// <returns></returns>
        public SqlSource FromStoredProcedure(string storedProcedure)
        {
            this.CommandText = storedProcedure;
            this.CommandType = CommandType.StoredProcedure;
            return this;
        }

        /// <summary>
        /// Define a string conexão desta origem
        /// </summary>
        /// <param name="connectionString">string de conexão</param>
        /// <returns></returns>
        public SqlSource Connection(string connectionString)
        {
            this.ConnectionString = connectionString;
            return this;
        }

        /// <summary>
        /// Define a conexão desta origem
        /// </summary>
        /// <param name="connection">Objeto do tipo conexão</param>
        /// <returns></returns>
        public SqlSource Connection(Connection connection)
        {
            this.ConnectionObject = connection;
            return this;
        }

        /// <summary>
        ///  Recupera todas as linhas desta origem
        /// </summary>
        public override IEnumerable<IRow> Rows
        {
            get
            {
                IEnumerable<IRow> rows = base.WithConnectionEnumerate<IRow>(conn => GetRows(conn));
                foreach(var r in rows)
                {
                    yield return r;
                }
            }
        }

        /// <summary>
        /// Executa o contexto de transformação do objeto
        /// </summary>
        /// <param name="transform">objeto do tipo "Transform" que sofrerá a transformação</param>
        /// <param name="action">Ação que será executada</param>
        void ISource.InTransformContext(ITransform transform, Action action)
        {
            action();
        }

        /// <summary>
        /// Retorna as linhas desta origem
        /// </summary>
        /// <param name="connection">Objeto do tipo conexão</param>
        /// <returns></returns>
        private IEnumerable<IRow> GetRows(Connection connection)
        {
            Command cmd = base.GetSelectCommand(connection);
            IDataReader rdr = cmd.ExecuteReader();
            while(rdr.Read())
            {
                yield return new DataReaderRow(rdr);
            }
        }
    }
}

