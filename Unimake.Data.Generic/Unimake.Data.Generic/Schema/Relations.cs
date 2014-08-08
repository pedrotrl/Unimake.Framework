using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Data.Generic.Schema
{
    public class Relations: List<Relation>
    {
        #region local
        public Connection Connection { get; set; }
        #endregion

        #region Delegates
        private delegate void GetRelationsHanlder(Table _table);

        #endregion

        #region Construtores
        public Relations() : base() { }

        public Relations(Connection connection, Table _table)
        {
            GetRelationsHanlder getRelations = null;
            Connection = connection;

            switch(connection.DatabaseType)
            {
                case DatabaseType.PostgreSQL:
                    getRelations = new GetRelationsHanlder(GetPostgreSQLRelations);
                    break;
                case DatabaseType.SQLite:
                    getRelations = new GetRelationsHanlder(GetSQLiteRelations);
                    break;
                default:
                    throw new NotImplementedException("DatabaseType not implemented:Relations");
            }

            getRelations.Invoke(_table);

        }
        #endregion

        #region Métodos getRelations
        private void GetPostgreSQLRelations(Table _table)
        {
            string sql = string.Format(@"SELECT p2.relname as pk_Table, a2.attname as pk_field, 
                                            p1.relname as fk_Table,a1.attname as fk_field,
                                            c.confdeltype as del_type
                                            FROM pg_constraint c, pg_namespace n, 
                                            pg_class p1, pg_class p2, 
                                            pg_attribute a1, pg_attribute a2 
                                            WHERE 
                                            c.contype = 'f' 
                                            AND c.confrelid > 0 
                                            AND c.connamespace = n.oid 
                                            AND c.conrelid = p1.oid 
                                            AND c.confrelid = p2.oid 
                                            AND c.conrelid = a1.attrelid 
                                            AND a1.attnum = ANY (c.conkey) 
                                            AND c.confrelid = a2.attrelid 
                                            AND a2.attnum = ANY (c.confkey) 
                                            AND LOWER(p2.relname) = '{0}'", _table.Name.ToLower());

            try
            {
                Connection.Open();

                Command cmd = new Command(sql, Connection,
                Connection.Transaction);

                DataReader rs = cmd.ExecuteReader();

                while(rs.Read())
                    this.Add(new Relation(rs.GetString("pk_Table"), rs.GetString("pk_Field"),
                        rs.GetString("fk_Table"), rs.GetString("fk_Field"))
                    {
                        DeleteAction = GetDeleteAction(rs.GetString("del_type"))
                    });
            }
            finally
            {
                Connection.Close();
            }
        }

        private void GetSQLiteRelations(Table _table)
        {
            string sql = string.Format(@"PRAGMA foreign_key_list('{0}')", _table.Name.ToUpper());

            Command cmd = new Command(sql, Connection,
                Connection.Transaction);

            DataReader rs = cmd.ExecuteReader();

            while(rs.Read())
            {
                this.Add(new Relation(rs.GetString("table"), rs.GetString("to"),
                    _table, rs.GetString("from"))
                    {
                        DeleteAction = GetDeleteAction(rs.GetString("on_delete"))
                    });
            }
        }

        /// <summary>
        /// retorna o delete action para os tipos de base de dados
        /// </summary>
        /// <param name="delType">tipo de delete retornado pela base de dados</param>
        /// <returns></returns>
        private DeleteAction GetDeleteAction(string delType)
        {
            delType = delType.ToLower();

            switch(Connection.DatabaseType)
            {
                #region pgsql
                case DatabaseType.PostgreSQL:
                    if(delType == "d")
                        return DeleteAction.SetDefault;
                    else if(delType == "n")
                        return DeleteAction.SetNull;
                    else if(delType == "r")
                        return DeleteAction.Restrict;
                    else if(delType == "c")
                        return DeleteAction.Cascade;
                    else
                        return DeleteAction.DoNothing;
                #endregion

                #region sqlServer
                case DatabaseType.SQLite:
                    if(delType == "restrict")
                        return DeleteAction.Restrict;
                    else if(delType == "set default")
                        return DeleteAction.SetDefault;
                    else if(delType == "set null")
                        return DeleteAction.SetNull;
                    else if(delType == "cascade")
                        return DeleteAction.Cascade;
                    else
                        return DeleteAction.DoNothing;
                #endregion

                #region notimplemented
                default:
                    throw new NotImplementedException("DatabaseType not implemented:GetDeleteAction");
                #endregion
            }
        }
        #endregion
    }
}
