using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unimake.Data.Generic.Model;
using Unimake.Data.Generic.Definitions.Attributes;
using Unimake.Data.Generic.Definitions.Mapping;
using System.Reflection;
using System.Data.Common;
using Unimake.Data.Generic.Definitions;

namespace Unimake.Data.Generic.Factory
{
    public class CommandFactory
    {
        #region locais
        private Connection connection;
        private Type currentType;
        private IBaseModel model;
        private Transaction transaction;
        private ForeignKeys foreignKeys;
        #endregion

        #region ações
        public Action<Command> BeforeCreateCommandAction { get; set; }
        public Action<Command, string> BeforePrepareSelectAction { get; set; }
        #endregion

        public CommandFactory(Connection connection,
            Transaction transaction,
            Type currentType,
            IBaseModel model)
        {
            this.connection = connection;
            this.currentType = currentType;
            this.model = model;
            this.transaction = transaction;
        }

        public CommandFactory(Connection connection,
           Transaction transaction,
           Type currentType,
           IBaseModel model,
           ForeignKeys foreignKeys)
        {
            this.connection = connection;
            this.currentType = currentType;
            this.model = model;
            this.transaction = transaction;
            this.foreignKeys = foreignKeys;
        }

        public CommandFactory(IBaseModel model)
        {
            this.currentType = model.GetType();
            this.model = model;
        }

        public virtual Command CreateCommand(bool insert)
        {
            string commandText = "";
            Command command = connection.CreateCommand();
            command.Transaction = transaction;
            string[] fields;
            string[] values;

            //aqui iremos determinar quais os objetos que irão compor este comando
            TableDefinitionAttribute tableDefinition = currentType.GetCustomAttributes(typeof(TableDefinitionAttribute), false)[0] as TableDefinitionAttribute;
            commandText += "{command} {tablename} {command2};";

            //aqui não podemos filtrar as propriedades apenas pela instancia, pois as mesmas podem vir de heranças
            var props = currentType.GetProperties(BindingFlags.Instance |
                                                  BindingFlags.Public |
                                                  BindingFlags.NonPublic).Where(p =>
                                                  {
                                                      //---------------------------------------------------------------------------
                                                      // garantir que apenas os campos desta tabela sejam passadas como propriedade
                                                      // Podem estar vindo de uma herança
                                                      //---------------------------------------------------------------------------
                                                      string fieldName = GetFieldName(p, tableDefinition, false);
                                                      return tableDefinition.Fields.Contains(fieldName);
                                                  });
            if(BeforeCreateCommandAction != null)
                BeforeCreateCommandAction(command);

            #region Campos
            foreach(var item in props)
            {
                if(CanUseField(item, tableDefinition))
                {
                    command.Parameters.Add(CreateParameter(tableDefinition, item, insert));
                }
            }
            #endregion

            #region Default Values
            IList<DefaultValueAttribute> defaultValues = (from x in currentType.GetCustomAttributes(typeof(DefaultValueAttribute), false)
                                                          select x as DefaultValueAttribute).ToList<DefaultValueAttribute>();

            if(defaultValues.Count > 0)
            {
                foreach(DefaultValueAttribute defaultValue in defaultValues)
                {
                    command.Parameters.Add(CreateParameter(defaultValue.FieldName, "@" + defaultValue.FieldName, defaultValue.Value, insert));
                }
            }
            #endregion

            #region chaves estrangeiras
            //na nossa aplicação usamos o GUID como pk e sempre temos ter as chaves estrangeiras que serão do tipo inteiro 
            //recuperar a fk

            ForeignKeyAttribute[] fks = GetForeignKey();

            //para cada um retornado, temos que validar se existe na definição

            if(fks != null)
            {
                foreach(ForeignKeyAttribute fk in fks.Where(f =>
                    //---------------------------------------------------------------------------
                    // garantir que apenas os campos desta tabela sejam passadas como propriedade
                    // Podem estar vindo de uma herança
                    //---------------------------------------------------------------------------
                        tableDefinition.Fields.Contains(f.ForeignKey) &&
                        foreignKeys.ContainsKey(f.TableName)))
                {
                    string fieldName = fk.ForeignKey;
                    string parameterName = "@" + fk.ForeignKey;
                    object value = foreignKeys[fk.TableName];
                    command.Parameters.Add(CreateParameter(fieldName, parameterName, value, insert));
                }
            }
            #endregion

            #region fields/ values
            fields = new string[command.Parameters.Count];
            values = new string[command.Parameters.Count];
            #endregion

            #region insert
            if(insert)
            {
                for(int i = 0; i < command.Parameters.Count; i++)
                {
                    fields[i] = command.Parameters[i].SourceColumn;
                    values[i] = command.Parameters[i].ParameterName;
                }

                commandText = commandText
                                .Replace("{command}", "INSERT INTO")
                                .Replace("{command2}", "(" + fields.Join() + ") VALUES (" + values.Join() + ")")
                                .Replace("{tablename}", tableDefinition.TableName.ToLower());
            }
            #endregion

            #region update
            else
            {
                for(int i = 0; i < command.Parameters.Count; i++)
                {
                    fields[i] = String.Format("{0} = {1}", command.Parameters[i].SourceColumn, command.Parameters[i].ParameterName);
                }

                commandText = "UPDATE {0} SET {1} WHERE {2}";
                commandText = string.Format(commandText, tableDefinition.TableName, fields.Join(),
                                            GetUpdateWhere(command, Utilities.DbUtils.GetPrimaryKeyValue(model)));
            }
            #endregion

            command.CommandText = commandText;

            return command;
        }

        /// <summary>
        /// Cria um parâmetro com base nas informações passadas e retorna
        /// </summary>
        /// <param name="tableDefinition">Definição da tabela</param>
        /// <param name="item">Item de propriedade para recuperar os nomes e informações do campo.</param>
        /// <param name="insert">Se true, os parâmetros fields e values estarão preparados para o insert, caso contrário, update</param>
        /// <returns></returns>
        private Parameter CreateParameter(TableDefinitionAttribute tableDefinition,
            PropertyInfo item,
            bool insert)
        {
            string parameterName;
            string fieldName = GetFieldName(item, tableDefinition, false);
            parameterName = GetParameterName(item);
            object value = item.GetValue(model, null);

            return CreateParameter(fieldName, parameterName, value, insert);
        }

        /// <summary>
        /// Cria um parâmetro com base nas informações passadas e retorna
        /// </summary>
        /// <param name="tableDefinition">Definição da tabela</param>
        /// <param name="fieldName">Nome que o campo deverá ter</param>
        /// <param name="parameterName">Nome do parâmetro para o campo</param>
        /// <param name="value">Valor do campo</param>
        /// <param name="insert">Se true, os parâmeros fields e values estarão preparados para o insert, caso contrário, update</param>
        /// <returns></returns>
        protected virtual Parameter CreateParameter(string fieldName,
            string parameterName,
            object value,
            bool insert)
        {
            Parameter result = new Parameter();
            result.SourceColumn = fieldName;
            result.ParameterName = parameterName;
            GenericDbType dbType = GenericDbType.Unknown;

            if(value is IMappingType)
            {
                value = (value as IMappingType).ConvertToDbValue();
            }
            else if(value is DateTime)
            {
                DateTime d = DateTime.Parse(value.ToString());

                if(d <= DateTime.MinValue)
                    value = DBNull.Value;
                else
                    value = d;

                dbType = GenericDbType.DateTime;
            }
            else if(value is Enum)
            {
                value = (int)value;
                dbType = GenericDbType.Integer;
            }

            if(value == null)
                value = DBNull.Value;

            if(dbType == GenericDbType.Unknown)
                dbType = dbType.DetectTypeFromValue(value);

            result.Value = value;
            result.GenericDbType = dbType;

            return result;
        }

        private ForeignKeyAttribute[] GetForeignKey()
        {
            ForeignKeyAttribute[] attribs = (from fk in currentType.GetCustomAttributes(typeof(ForeignKeyAttribute), true)
                                             select fk as ForeignKeyAttribute).ToArray();
            return attribs;
        }

        private string GetParameterName(PropertyInfo item)
        {
            object[] attribs = item.GetCustomAttributes(typeof(FieldDefinitionAttribute), false);

            if(attribs.Count() > 0)
            {
                FieldDefinitionAttribute f = attribs[0] as FieldDefinitionAttribute;
                return "@" + f.FieldName;
            }

            return "@" + item.Name;
        }

        public Command CreateCommandDelete(TableDefinitionAttribute tableDefinition, IID id)
        {
            Command command = connection.CreateCommand();
            command.Transaction = transaction;
            string pkName = GetPKName(tableDefinition);
            string commandText = "";

            commandText = string.Format("DELETE FROM {0} WHERE {1} = @where_1;", tableDefinition.TableName, pkName);

            command.CommandText = commandText;
            command.Parameters.Add(new Parameter("@where_1", GenericDbType.String)
            {
                SourceColumn = pkName,
                Value = id.ToString()
            });

            return command;
        }

        private string GetPKName(TableDefinitionAttribute tableDefinition)
        {
            //recupera a definição da tabela
            return string.Format("{0}.{1}", tableDefinition.TableName, tableDefinition.PrimaryKey);
        }

        private bool CanUseField(System.Reflection.PropertyInfo item, TableDefinitionAttribute tableDefinition)
        {
            //valida se pode usar este campo
            object[] attribs = item.GetCustomAttributes(typeof(FieldDefinitionAttribute), false);

            if(attribs.Count() > 0)
                if(!(attribs[0] as FieldDefinitionAttribute).IsTableField)
                    return false;

            //recupera o nome do campo
            string fieldName = GetFieldName(item, tableDefinition);

            //se o campo for a chave primária não pode ser usado
            if(!tableDefinition.UseInsert && fieldName.Equals(GetPKName(tableDefinition), StringComparison.InvariantCultureIgnoreCase))
                return false;

            return true;
        }

        /// <summary>
        /// retorna true se for um tablefield
        /// </summary>
        /// <param name="item">propriedade da classe</param>
        /// <returns></returns>
        private bool IsTableField(System.Reflection.PropertyInfo item)
        {
            return IsTableField(item, false);
        }

        /// <summary>
        /// retorna true se for um tablefield
        /// </summary>
        /// <param name="item">propriedade da classe</param>
        /// <param name="returnIfJoin">se true valida se o campo é um join e retorna    </param>
        /// <returns></returns>
        private bool IsTableField(System.Reflection.PropertyInfo item, bool returnIfJoin)
        {
            //valida se pode usar este campo
            object[] attribs = item.GetCustomAttributes(typeof(FieldDefinitionAttribute), false);

            if(attribs.Count() > 0)
            {
                FieldDefinitionAttribute f = attribs[0] as FieldDefinitionAttribute;

                if(returnIfJoin)
                {
                    return f.IsJoinField || f.IsTableField;
                }
                return f.IsTableField;
            }

            return true;
        }

        private string GetFieldName(System.Reflection.PropertyInfo item, TableDefinitionAttribute tableDefinition)
        {
            return GetFieldName(item, tableDefinition, true);
        }

        private string GetFieldName(System.Reflection.PropertyInfo item, TableDefinitionAttribute tableDefinition,
            bool withTableName)
        {
            object[] attribs = item.GetCustomAttributes(typeof(FieldDefinitionAttribute), false);

            string name = "";
            string table = "";

            if(attribs.Count() > 0)
            {
                FieldDefinitionAttribute f = attribs[0] as FieldDefinitionAttribute;

                if(f.IsJoinField)
                {
                    table = f.JoinTable;
                    name = f.FieldName;
                }
                else
                {
                    table = tableDefinition.TableName;
                    name = f.FieldName;
                }
            }
            else
            {
                table = tableDefinition.TableName;
                name = item.Name;
            }

            if(withTableName)
                return string.Format("{0}.{1}", table, name);

            return name;
        }

        private IList<SelectField> GetSelectAux(Type type)
        {
            object[] attribs = type.GetCustomAttributes(typeof(SelectDefinitionAttribute), false);
            if(attribs.Count() > 0)
            {
                SelectDefinitionAttribute a = attribs[0] as SelectDefinitionAttribute;
                return a.SelectFields;
            }

            return null;
        }

        protected virtual object GetUpdateWhere(Command command, IID id)
        {
            object[] attribs = currentType.GetCustomAttributes(typeof(TableDefinitionAttribute), false);
            TableDefinitionAttribute a = attribs[0] as TableDefinitionAttribute;
            command.Parameters.Add(new Parameter("@" + a.PrimaryKey, GenericDbType.String)
            {
                SourceColumn = a.PrimaryKey,
                Value = id.ConvertToDbValue()
            });

            return string.Format("{0} = @{0}", a.PrimaryKey);
        }


        private string GetFrom()
        {
            IEnumerable<MyHierarchicalType> models = Utilities.DbUtils.GetModels(model);
            string from = "";

            foreach(var modelType in models.OrderBy(o => o.Order))
            {
                if(modelType.Type.GetCustomAttributes(typeof(FromDefinitionAttribute), false).Count() > 0)
                {
                    object[] attribs = modelType.Type.GetCustomAttributes(typeof(FromDefinitionAttribute), false);

                    FromDefinitionAttribute j = attribs[0] as FromDefinitionAttribute;

                    string result = "";

                    foreach(var item in j.Joins)
                    {
                        result += item + " ";
                    }

                    from += from.Length == 0 ? j.From + " " + result : result;
                }
            }

            return "FROM " + from;
        }

        private string GetGroupBy()
        {
            object[] attribs = currentType.GetCustomAttributes(typeof(GroupByDefinitionAttribute), false);

            if(attribs.Count() > 0)
            {
                GroupByDefinitionAttribute g = attribs[0] as GroupByDefinitionAttribute;
                return g.GroupBy;
            }

            return "";
        }

        internal void PrepareCommandSelect(TableDefinitionAttribute tableDefinition,
            Where where, OrderBy order, IEnumerable<DbParameter> whereParameters, ref Command command)
        {
            #region Variáveis
            string sqlWhere = "";
            var props = model.GetType().GetProperties();

            //-------------------------------------------------------------------------
            // Salva os campos que já foram adicionados para não adicionar novamente
            //-------------------------------------------------------------------------
            Dictionary<string, SelectField> fields = new Dictionary<string, SelectField>();

            string commandText = "";
            string selectFields = "";

            commandText = @"SELECT {selectFields} {from} {where} {limit}";

            if(where == null) where = new Where();
            #endregion

            #region Select

            foreach(MyHierarchicalType hType in Utilities.DbUtils.GetModels(model).OrderByDescending(o => o.Order))
            {
                //recuperar todos os selectedsFields
                IList<SelectField> selectList = GetSelectAux(hType.Type);

                if(selectList != null)
                {
                    foreach(SelectField item in selectList)
                    {
                        if(!fields.ContainsKey(item.FieldAlias))
                        {
                            selectFields += item.ToString() + ",";
                            fields.Add(item.FieldAlias, item);
                        }
                    }
                }
            }

            if(BeforePrepareSelectAction != null)
                BeforePrepareSelectAction(command, selectFields);

            selectFields = selectFields.Trim();

            if(!string.IsNullOrEmpty(selectFields))
                selectFields = selectFields.Substring(0, selectFields.Length - 1);

            #region where
            if(whereParameters != null && whereParameters.Count() > 0)
            {
                foreach(Parameter item in whereParameters)
                {
                    command.Parameters.Add(item);
                }
            }

            model.PrepareReader(command, where);

            if(where.Count > 0)
            {
                sqlWhere = " WHERE " + where.ToString();

                foreach(Parameter p in where.Parameters)
                {
                    command.Parameters.Add(p);
                }
            }
            #endregion

            #endregion

            #region Return
            commandText = commandText.Replace("{from}", GetFrom())
                                         .Replace("{selectFields}", selectFields)
                                         .Replace("{where}", sqlWhere)
                                         .Replace("{limit}", where.Limit.ToString());

            command.CommandText = commandText;
            #endregion
        }
    }
}
