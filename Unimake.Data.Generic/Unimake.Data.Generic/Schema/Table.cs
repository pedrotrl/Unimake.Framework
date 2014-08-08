using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;

namespace Unimake.Data.Generic.Schema
{
    public class Table
    {
        #region Locais
        public Connection Connection { get; set; }
        #endregion

        #region Operators
        public static implicit operator String(Table t)
        {
            if(t != null)
                return t.ToString();

            return "";
        }
        #endregion

        #region Atributos
        private Relations mRelations = null;
        #endregion

        #region Propriedades
        public Parameters Fields { get; private set; }
        public string Name { get; private set; }

        /// <summary>
        /// retorna os relacionamentos entre as tabelas especificadas
        /// </summary>
        /// <returns></returns>
        public Relations Relations
        {
            get
            {
                if(mRelations == null)
                    mRelations = new Unimake.Data.Generic.Schema.Relations(Connection, this);

                return mRelations;
            }

        }
        #endregion

        #region Construtores
        internal Table(Connection connection, string tableName)
        {
            if(connection.DatabaseType == DatabaseType.PostgreSQL)
                GetFieldsFromPGSql(connection, tableName);
            else if(connection.DatabaseType == DatabaseType.SQLite)
                GetFieldsFromSQLite(connection, tableName);

        }

        private void GetFieldsFromPGSql(Connection connection, string tableName)
        {
            Fields = new Parameters();
            Connection = connection;

            Command cmd = connection.CreateCommand(tableName);
            cmd.CommandText = "SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE ";
            cmd.CommandText += "FROM information_schema.columns ";
            cmd.CommandText += "WHERE  TABLE_NAME = '" + tableName + "' ";
            cmd.CommandText += "ORDER BY ORDINAL_POSITION";

            DataReader rs = cmd.ExecuteReader();

            while(rs.Read())
            {
                Parameter f = new Parameter(rs["COLUMN_NAME"].ToString());

                f.SourceColumn = f.ParameterName;
                f.TableName = tableName;
                f.GenericDbType = f.ToGenericDbType(rs["DATA_TYPE"].ToString());

                #region IS_NULLABLE
                //os valores de IS_NULLABLE podem ser diferentes em algumas bases de dados, logo temos
                //que validar todos os tipos possíveis
                string isNullable = rs["IS_NULLABLE"].ToString().ToLower();

                if(isNullable == "0")
                    isNullable = false.ToString();
                else if(isNullable == "yes")
                    isNullable = true.ToString();
                else if(isNullable == "no")
                    isNullable = false.ToString();
                else if(isNullable != "0")
                    isNullable = true.ToString();

                f.IsNullable = Convert.ToBoolean(isNullable);
                #endregion

                int size = 0;
                int.TryParse(rs["CHARACTER_MAXIMUM_LENGTH"].ToString(), out size);
                f.Size = size;
                Fields.Add(f);
            }

            Name = tableName;
        }

        private void GetFieldsFromSQLite(Connection connection, string tableName)
        {
            Fields = new Parameters();
            Connection = connection;

            Command cmd = connection.CreateCommand(tableName);
            cmd.CommandText = String.Format("PRAGMA table_info('{0}')", tableName);

            DataReader rs = cmd.ExecuteReader();

            while(rs.Read())
            {
                string type = rs["type"].ToString();

                Parameter f = new Parameter(rs["name"].ToString());
                f.SourceColumn = f.ParameterName;
                f.TableName = tableName;
                f.GenericDbType = f.ToGenericDbType(type);

                #region IS_NULLABLE
                //os valores de IS_NULLABLE podem ser diferentes em algumas bases de dados, logo temos
                //que validar todos os tipos possíveis
                f.IsNullable = rs["notNull"].ToString() == "1" ? false : true;
                #endregion

                int size = 0;
                //aqui o maxlength tem que ser definido pelo campo (
                if(type.Contains('('))
                {
                    type = type.Substring(type.IndexOf('(') + 1);
                    type = type.Substring(0, type.IndexOf(')'));
                    int.TryParse(type, out size);

                    f.Size = size;
                }

                Fields.Add(f);
            }

            Name = tableName;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return Name;
        }
        #endregion
    }

}
