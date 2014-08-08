using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unimake.Data.Generic.Definitions.Attributes;
using Unimake.Data.Generic.Model;
using Unimake.Data.Generic.Factory;
using Unimake.Data.Generic.Definitions.Mapping;
using Unimake.Data.Generic.Definitions;

namespace Unimake.Data.Generic
{

    /// <summary>
    /// Classe responsável por determinar, preparar e executar quais ações podem ser executadas na base de dados
    /// </summary>
    public static class DbContext
    {
        #region Actions
        public static Action<IBaseModel> PreparePopulateAction { get; set; }
        #endregion

        #region propriedades
        /// <summary>
        /// Cria uma nova conexão e retorna
        /// </summary>
        /// <param name="open">se true, cria e abre a conexão</param>
        /// <param name="_new">Se true, não usa a instancia atual, retorna uma nova</param>
        /// <returns></returns>
        public static Connection CreateConnection(bool open = true, bool _new = false)
        {
            return ConnectionFactory.CreateConnection(open, _new);
        }
        #endregion

        #region Métodos privados
        /// <summary>
        /// Retorna o primeiro TableDefinitionAttribute encontrado no modelo
        /// </summary>
        /// <param name="model">Modelo para pesquisar o TableDefinitionAttribute</param>
        /// <param name="first">Se true, retorna o nome da primeira tabela na hierarquia das classes</param>
        /// <returns></returns>
        internal static TableDefinitionAttribute FindTableDefinition(IBaseModel model, bool first)
        {
            var q = (from m in Utilities.DbUtils.GetModels(model).OrderByDescending(o => o.Order)
                     where m.Type.GetCustomAttributes(typeof(TableDefinitionAttribute), false).Count() > 0
                     select m.Type.GetCustomAttributes(typeof(TableDefinitionAttribute), false)[0]
                     as TableDefinitionAttribute);

            return first ? q.First() : q.Last();
        }

        /// <summary>
        /// Retorna o primeiro TableDefinitionAttribute encontrado no modelo
        /// </summary>
        /// <param name="model">Modelo para pesquisar o TableDefinitionAttribute</param>
        /// <param name="first">Se true, retorna o nome da primeira tabela na hierarquia das classes</param>
        /// <returns></returns>
        internal static TableDefinitionAttribute FindTableDefinition(IBaseModel model)
        {
            return FindTableDefinition(model, true);
        }

        /// <summary>
        /// Retorna o primeiro TableDefinitionAttribute encontrado no modelo
        /// </summary>
        /// <param name="type">Tipo definido que possui o atributo TableDefinition</param>
        /// <returns></returns>
        private static TableDefinitionAttribute FindTableDefinition(Type type)
        {
            return type.GetCustomAttributes(typeof(TableDefinitionAttribute), false)[0] as TableDefinitionAttribute;
        }
        #endregion

        #region Métodos públicos
        /// <summary>
        /// Retorna um registro pelo GUID. Null se não encontrar nada
        /// </summary>
        /// <param name="id">identificador do registro</param>
        /// <returns></returns>
        public static IBaseModel Get(object id, IBaseModel model)
        {
            TableDefinitionAttribute tableDefinition = FindTableDefinition(model);

            Where w = new Where();
            w.Add(tableDefinition.PrimaryKey, id.ToString());

            IList<IBaseModel> result = Find(model, w);

            if(result.Count > 0)
                return result[0];

            return null;
        }

        /// <summary>
        /// Exclui um registro da base de dados
        /// </summary>
        /// <param name="id">identificador do registro</param>
        public static void Delete(IID id, IBaseModel model)
        {
            try
            {
                model.Connection = DbContext.CreateConnection();
                model.Connection.BeginTransaction();

                //Validar
                model.ValidateDelete();

                TableDefinitionAttribute tableDefinition = FindTableDefinition(model);

                CommandFactory cf = new CommandFactory(model.Connection,
                    model.Connection.Transaction,
                    model.GetType(), model, null);

                using(Command command = cf.CreateCommandDelete(tableDefinition, id))
                {
                    command.ExecuteNonQuery();
                }

                model.Connection.CommitTransaction();
            }
            catch
            {
                model.Connection.RollbackTransaction();
                throw;
            }
            finally
            {
                if(model.Connection != null)
                    model.Connection.Close();
            }
        }

        /// <summary>
        /// Salva o registro na base de dados
        /// </summary>
        /// <param name="model"></param>
        public static IID Save(IBaseModel model)
        {
            IID result = null;
            bool oldNew = model.New;
            IID fk = null;
            ForeignKeys foreignKeys = new ForeignKeys();
            bool inserting = false;
            IEnumerable<MyHierarchicalType> models = null;
            TableDefinitionAttribute tableDefinition = null;

            try
            {
                //se este model for do tipo IChildModel
                //deve-se usar a conexão do Parent e cria uma foreignKey
                if(model.IsChildModel())
                {
                    dynamic m = model;

                    if(m.Parent != null)
                    {
                        model.Connection = m.Parent.Connection;

                        models = Utilities.DbUtils.GetModels(m.Parent as IBaseModel);

                        foreach(var modelType in models)
                        {
                            if(modelType.Type.GetCustomAttributes(typeof(TableDefinitionAttribute), false).Count() > 0)
                            {
                                tableDefinition = modelType.Type.GetCustomAttributes(typeof(TableDefinitionAttribute), false)[0] as TableDefinitionAttribute;

                                fk = Utilities.DbUtils.GetPrimaryKeyValue(m);

                                //adicionar o foreign key, se necessário usar no command
                                foreignKeys.Add(tableDefinition.TableName, fk);
                            }
                        }
                    }
                }

                model.Connection.Open();
                model.Connection.BeginTransaction();

                //Detectar se é uma inserção
                //se o modelo é marcado como não atualizável, deverá sempre sofrer insert
                object[] notUpdatable = model.GetType().GetCustomAttributes(typeof(NotUpdatableAttribute), true);

                inserting = notUpdatable.Count() > 0 || model.New;

                //Validar
                model.Validate(!inserting);
                model.BeforeSave(!inserting);

                //revalidar o insert, pois pode ter sido modificado pelo Validate e BeforeSave
                inserting = notUpdatable.Count() > 0 || model.New;

                models = Utilities.DbUtils.GetModels(model);

                //para todos os modelos retornados, iremos buscar as tabelas que compoe estes modelos
                foreach(var modelType in models.OrderByDescending(o => o.Order))
                {
                    if(modelType.Type.GetCustomAttributes(typeof(TableDefinitionAttribute), false).Count() > 0)
                    {
                        tableDefinition = modelType.Type.GetCustomAttributes(typeof(TableDefinitionAttribute), false)[0] as TableDefinitionAttribute;

                        CommandFactory cf = new CommandFactory(model.Connection,
                            model.Connection.Transaction,
                            modelType.Type, model, foreignKeys);

                        using(Command command = cf.CreateCommand(inserting))
                        {
                            //Preparar o comando antes de executar
                            model.PrepareCommand(command, !inserting);

                            //executar o comando
                            command.ExecuteNonQuery();

                            //se esta tabela possuir GUID, deverá ser armazenada para referência como chave estrangeira
                            if(Utilities.DbUtils.Tables[tableDefinition.TableName].Fields.Contains(tableDefinition.PrimaryKey))
                            {
                                fk = Utilities.DbUtils.GetPrimaryKeyValue(model);
                            }

                            if(!result.IsValid()) result = fk;

                            //adicionar o foreign key, se necessário usar no command
                            foreignKeys.Add(tableDefinition.TableName, fk);
                        }
                    }
                }

                model.AfterSave(!inserting);
                model.Connection.CommitTransaction();
                model.New = false;
            }
            catch
            {
                model.Connection.RollbackTransaction();
                model.New = oldNew;
                throw;
            }
            finally
            {
                model.Connection.Close();
            }

            model.New = false;
            return result;
        }

        /// <summary>
        /// Encontra todos os registos que satisfaçam a condição
        /// </summary>
        /// <param name="w">condição para buscar os registros</param>
        /// <param name="model">Modelo base</param>
        /// <typeparam name="T">Tipo de modelo que deverá ser retornado</typeparam>
        /// <returns></returns>
        public static List<T> Find<T>(T model, Where w = null)
            where T: IBaseModel
        {
            return Find<T>(model, model.Connection, w);
        }

        /// <summary>
        /// Encontra todos os registos que satisfaçam a condição
        /// </summary>
        /// <param name="w">condição para buscar os registros</param>
        /// <param name="model">Modelo base</param>
        /// <typeparam name="T">Tipo de modelo que deverá ser retornado</typeparam>
        /// <param name="connection">Conexão que será utilizada para buscar os dados</param>
        /// <returns></returns>
        public static List<T> Find<T>(T model, Connection connection, Where w)
            where T: IBaseModel
        {
            List<T> result = new List<T>();
            IList<IID> ids = new List<IID>();

            try
            {
                connection.Open();
                model.Connection = connection;
                TableDefinitionAttribute tableDefinition = FindTableDefinition(model);
                //recuperar o campo GUID da tabela mais básica
                Command command = new Command();
                new CommandFactory(model).PrepareCommandSelect(tableDefinition, w, null, null, ref command);
                command.Connection = model.Connection;
                DataReader dataReader = command.ExecuteReader();

                while(dataReader.Read())
                {
                    if(model is IParentModel)
                    {
                        IID id = Utilities.DbUtils.GetPrimaryKeyValue(model);

                        if(ids.Contains(id))
                            continue;

                        ids.Add(id);
                    }

                    model.Populate(dataReader);
                    result.Add(model);
                }
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        /// <summary>
        /// Encontra todos os registos que satisfaçam a condição
        /// </summary>
        /// <param name="w">condição para buscar os registros</param>
        /// <param name="whereParameters">Parâmetros utilizados na clausula where</param>
        /// <param name="model">Modelo base</param>
        /// <typeparam name="T">Tipo de modelo que deverá ser retornado</typeparam>
        /// <returns></returns>
        public static List<T> Find<T>(T model, IEnumerable<Parameter> whereParameters = null, Where w = null)
            where T: IBaseModel
        {
            List<T> result = new List<T>();

            try
            {
                model.Connection = DbContext.CreateConnection();
                TableDefinitionAttribute tableDefinition = FindTableDefinition(model);
                //recuperar o campo GUID da tabela mais básica
                Command command = new Command();
                new CommandFactory(model).PrepareCommandSelect(tableDefinition, w, null, whereParameters, ref command);
                command.Connection = model.Connection;
                DataReader dataReader = command.ExecuteReader();
                while(dataReader.Read())
                {
                    model.Populate(dataReader);
                    result.Add(model);
                }
            }
            finally
            {
                model.Connection.Close();
            }

            return result;
        }

        /// <summary>
        /// Prepara os valores e retorna os em um formato de exibição para o usuário
        /// <para>Como padrão retorna os três primeiros campos do select que foi criado</para>
        /// </summary>
        /// <param name="w">Filtro, se necessário. Não é obrigatório e pode ser nulo</param>
        /// <param name="model">Modelo base</param>
        /// <typeparam name="T">Tipo de modelo que deverá ser retornado</typeparam>
        /// <param name="pageSize">Tamanho da página de registros</param>
        /// <param name="executeCommand">Se true, irá executar o comando e retornar os dados</param>
        /// <returns>Retorna os valores em um um formato de exibição para o usuário</returns>
        public static IDisplayValues GetDisplayValues(IParentModel model, Where w = null, bool executeCommand = true, int pageSize = 100)
        {
            if(w == null) w = new Where
            {
                Limit = pageSize
            };

            IDisplayValues result = new DisplayValues(model);
            DataReader dataReader = null;

            try
            {
                model.Connection = DbContext.CreateConnection();
                TableDefinitionAttribute tableDefinition = FindTableDefinition(model);
                //recuperar o campo GUID da tabela mais básica
                Command command = new Command();
                new CommandFactory(model).PrepareCommandSelect(tableDefinition, w, null, null, ref command);
                command.Connection = model.Connection;
                dataReader = executeCommand ? command.ExecuteReader() : command.ExecuteReader(System.Data.CommandBehavior.SchemaOnly);
            }
            finally
            {
                model.Connection.Close();
            }

            result.Where = w;
            result.DataReader = dataReader;
            return result;
        }

        /// <summary>
        /// Preenche o modelo com os dados de base
        /// </summary>
        /// <param name="model">modelo que deverá ser preenchido</param>
        /// <param name="dataReader">DataReader com os dados que deverão ser passados ao ojeto</param>
        internal static void Populate(IParentModel model, DataReader dataReader)
        {
            model.CurrentDataReader = dataReader;
            model.New = false;
            if(PreparePopulateAction == null)
                throw new NotImplementedException("PreparePopulateAction was not implemented for this DbContext. Please implement the action DbContext.PreparePopulateAction");

            PreparePopulateAction(model);
        }

        /// <summary>
        /// Preenche o modelo pelo identificador do registro
        /// </summary>
        /// <param name="id">identificador do registro</param>
        /// <returns></returns>
        public static void Populate(IBaseModel model, IID id)
        {
            Where w = new Where();

            try
            {
                model.Connection = DbContext.CreateConnection();
                TableDefinitionAttribute tableDefinition = null;
                //recuperar o campo ID da tabela mais básica
                tableDefinition = FindTableDefinition(model);
                w.Add(tableDefinition.TableName + "." + tableDefinition.PrimaryKey, id);
                Command command = new Command();
                new CommandFactory(model).PrepareCommandSelect(tableDefinition, w, null, null, ref command);
                command.Connection = model.Connection;
                Populate(model, command.ExecuteReader());
            }
            finally
            {
                model.Connection.Close();
            }
        }

        /// <summary>
        /// Preenche o modelo com o primeiro registro encontrado
        /// </summary>
        ///<param name="model">Model que deverá ser populado</param>
        ///<param name="w">Filtro para pesquisar os registros</param>
        /// <returns></returns>
        public static void Populate(IBaseModel model, Where w)
        {
            try
            {
                model.Connection = DbContext.CreateConnection();
                TableDefinitionAttribute tableDefinition = null;
                //recuperar o campo ID da tabela mais básica
                tableDefinition = FindTableDefinition(model);
                Command command = new Command();
                new CommandFactory(model).PrepareCommandSelect(tableDefinition, w, null, null, ref command);
                command.Connection = model.Connection;
                Populate(model, command.ExecuteReader());

            }
            finally
            {
                model.Connection.Close();
            }
        }

        /// <summary>
        /// Preenche o modelo com o primeiro registro encontrado
        /// </summary>
        ///<param name="model">Model que deverá ser populado</param>
        ///<param name="dataReader">Objeto datareader com os dados que serão preenchidos</param>
        /// <returns></returns>
        public static void Populate(IBaseModel model, DataReader dataReader)
        {
            try
            {
                if(dataReader.Read())
                {
                    model.Populate(dataReader);
                    model.New = false;
                }
            }
            finally
            {
                model.Connection.Close();
            }
        }

        /// <summary>
        /// Valida o objeto antes de salvar na base de dados.
        /// <para>retorna o erro se o objeto não puder ser salvo.</para>
        /// </summary>
        /// <param name="updating">se true está atualizando</param>
        /// <param name="model">modelo base para ser validado</param>
        /// <returns>Erro se o objeto não puder ser salvo</returns>
        ///<remarks>Neste método não se pode chamar o validate do model, pois este já é chamaado e isso causaria um loop infinito</remarks>
        public static void Validate(bool updating, IBaseModel model)
        {
            //nada. Por enquanto
        }

        /// <summary>
        /// Valida o objeto antes de excluir
        /// <para>retorna o erro se o objeto não puder ser excluído.</para>
        /// </summary>
        /// <param name="model">modelo base para ser validado</param>
        /// <returns>Erro, se não puder ser feito um delete</returns>
        ///<remarks>Neste método não se pode chamar o validate do model, pois este já é chamaado e isso causaria um loop infinito</remarks>
        public static void ValidateDelete(IBaseModel model)
        {
            //nada. Por enquanto
        }
        #endregion
    }
}
