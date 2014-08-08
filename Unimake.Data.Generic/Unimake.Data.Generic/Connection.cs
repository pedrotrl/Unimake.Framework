using System;
using System.Text;
using System.Data.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Unimake.Data.Generic.Exceptions;
using System.Runtime.InteropServices;
using System.Collections;
using Unimake.Data.Generic.Configuration;
using System.Data;

namespace Unimake.Data.Generic
{
    /// <summary>
    /// classe responsável pela conexão.
    /// </summary>
    /// <remarks>Cada tipo (ODBC, SQL, OleDb) depende de uma instancia esta classe trata estes tipos</remarks>
    public class Connection: DbConnection, ICloneable, IDisposable, IConnection
    {
        #region Variáveis Locais
        internal DbConnection mDbConnection;
        internal Transaction mTransaction;
        #endregion

        #region Propriedades
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Quantidade de conexões ativas
        /// </summary>
        public int CountOpen { get; private set; }

        /// <summary>
        /// quantidade de transações ativas
        /// </summary>
        public int CountTransaction { get; private set; }

        /// <summary>
        /// Transação corrente.
        /// </summary>
        public virtual Transaction Transaction
        {
            get { return mTransaction; }
            set { mTransaction = value; }
        }

        public DatabaseType DatabaseType
        {
            get
            {
                DatabaseType result = DatabaseType.SQLite;

                if(mDbConnection is Npgsql.NpgsqlConnection)
                    result = DatabaseType.PostgreSQL;
                else if(mDbConnection is System.Data.SQLite.SQLiteConnection)
                    result = DatabaseType.SQLite;
                else
                    throw new Exceptions.DatabaseTypeNotImplemented();

                return result;
            }
        }
        #endregion

        #region Construtores
        public Connection()
            : base()
        {
            if(!DataGenericSettings.Initialized)
                DataGenericSettings.Initialize();

            mDbConnection = _Connection(DataGenericSettings.Settings.DatabaseType);
            ConnectionString = DataGenericSettings.Settings.ConnectionString;
        }

        /// <summary>
        /// construtor que carrega a string de conexão
        /// </summary>
        /// <param name="connectionString">string de conexão</param>
        public Connection(string connectionString)
            : this()
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Cria uma string de conexão utilizando os parâmetros passados e instancia a conexão
        /// </summary>
        /// <param name="dbType">Tipo da base de dados que a string será criada</param>
        /// <param name="database">Nome da base de dados ou caminho do arquivo, se SQLite</param>
        /// <param name="moreConfig">Qualquer configuração adicional que deverá ser passada para a string de conexão</param>
        /// <exception cref="DatabaseTypeNotImplemented">Lançada quando for informado um tipo de base de dados inválido</exception>
        /// <returns>String de conexão montada</returns>
        public Connection(Generic.DatabaseType dbType, string database)
            : this(dbType, database, "", 0, "", "", "")
        {
        }
        /// <summary>
        /// Cria uma string de conexão utilizando os parâmetros passados e instancia a conexão
        /// </summary>
        /// <param name="dbType">Tipo da base de dados que a string será criada</param>
        /// <param name="database">Nome da base de dados ou caminho do arquivo, se SQLite</param>
        /// <param name="moreConfig">Qualquer configuração adicional que deverá ser passada para a string de conexão</param>
        /// <exception cref="DatabaseTypeNotImplemented">Lançada quando for informado um tipo de base de dados inválido</exception>
        /// <returns>String de conexão montada</returns>
        public Connection(Generic.DatabaseType dbType, string database, string moreConfig = "")
            : this(dbType, database, "", 0, "", "", moreConfig)
        {

        }

        /// <summary>
        /// Cria uma string de conexão utilizando os parâmetros passados e instancia a conexão
        /// </summary>
        /// <param name="dbType">Tipo da base de dados que a string será criada</param>
        /// <param name="database">Nome da base de dados ou caminho do arquivo, se SQLite</param>
        /// <param name="server">Nome ou IP do servidor</param>
        /// <param name="port">porta de acesso ao servidor</param>
        /// <param name="userId">Usuário de acesso ao banco de dados</param>
        /// <param name="password">Senha de acesso ao banco de dados</param>
        /// <param name="moreConfig">Qualquer configuração adicional que deverá ser passada para a string de conexão</param>
        /// <exception cref="DatabaseTypeNotImplemented">Lançada quando for informado um tipo de base de dados inválido</exception>
        /// <returns></returns>
        public Connection(Generic.DatabaseType dbType,
            string database, string server, int port, string userId, string password, string moreConfig = "")
            : this()
        {
            //aqui iremos forçar a recriar a conexão, pois pode ter criado errado
            mDbConnection = _Connection(dbType);
            //recriar a string de conexão
            ConnectionString = CreateConnectionString(dbType, database, server, port, userId, password, moreConfig);
        }

        internal Connection(DbConnection connection)
        {
            mDbConnection = connection;
        }

        /// <summary>
        /// cria um novo objeto de conexão para ser usado na classe
        /// </summary>
        /// <returns></returns>
        private static DbConnection _Connection(DatabaseType databaseType)
        {
            DbConnection dbConnection = null;
            switch(databaseType)
            {
                case DatabaseType.PostgreSQL:
                    dbConnection = new Npgsql.NpgsqlConnection();
                    break;
                case DatabaseType.SQLite:
                    dbConnection = new System.Data.SQLite.SQLiteConnection();
                    break;
                default:
                    throw new Exceptions.DatabaseTypeNotImplemented();
            }

            return dbConnection;
        }
        #endregion

        #region DbConnection Members
        public override int GetHashCode()
        {
            return mDbConnection.GetHashCode();
        }

        public override string DataSource
        {
            get { return mDbConnection.DataSource; }
        }

        public override string ServerVersion
        {
            get { return mDbConnection.ServerVersion; }
        }

        public override void ChangeDatabase(string databaseName)
        {
            mDbConnection.ChangeDatabase(databaseName);
        }

        public override void Close()
        {
            try
            {
                CountOpen--;

                //se a conexão estiver em uma transação. Não pode ser fechada
                if(CountTransaction > 0)
                {
                    if(CountOpen <= 0) CountOpen = 1;//aqui foi ajustado, pois se não fechou a conexão, ainda tem uma aberta
                    return;
                }

                //-------------------------------------------------------------------------
                // Se pode fechar a conexão, logo ...
                //-------------------------------------------------------------------------
                if(CountOpen <= 0)
                {
                    CountOpen = 0;
                    if(mDbConnection.State == ConnectionState.Open)
                    {
                        mDbConnection.Close();
                    }
                }
            }
            catch(ObjectDisposedException)
            {
                //nada
            }
            catch(NotSupportedException)
            {
                CountOpen++;
            }
        }

        private String mConnectionString = "";
        public override string ConnectionString
        {
            get { return mConnectionString; }
            set
            {
                mConnectionString = value;
                mDbConnection.ConnectionString = value;
            }
        }

        public override int ConnectionTimeout
        {
            get { return mDbConnection.ConnectionTimeout; }
        }

        public override string Database
        {
            get { return mDbConnection.Database; }
        }

        public virtual void Open(string connectionString)
        {
            this.ConnectionString = connectionString;
            Open();
        }

        public override void Open()
        {
            //Se por algum razão a conexão está fechada e o countOpen for maior que zero, não importa
            //Abrir a conexão e iniciar a contagem de aberturas
            if(CountOpen <= 0 || mDbConnection.State != ConnectionState.Open)
            {
                CountOpen = 1;

                if(mDbConnection.State != ConnectionState.Open)
                {
                    mDbConnection.Open();

                    //-------------------------------------------------------------------------
                    // Se for do tipo SQLIte, devemos habilitar o suporte a foreignKey
                    //-------------------------------------------------------------------------
                    if(DatabaseType == Generic.DatabaseType.SQLite)
                    {
                        using(DbCommand command = mDbConnection.CreateCommand())
                        {
                            //-------------------------------------------------------------------------
                            // Foi comentado até que a aplicação esteja estável
                            //-------------------------------------------------------------------------
                            command.CommandText = "PRAGMA foreign_keys = ON";
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            else
                CountOpen++;
        }

        public override ConnectionState State
        {
            get { return mDbConnection.State; }
        }
        #endregion

        #region ICloneable Members

        public virtual Connection Clone()
        {
            return (Connection)this.MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

        #region IDbConnection Members

        IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il)
        {
            return this.BeginTransaction(il);
        }

        IDbTransaction IDbConnection.BeginTransaction()
        {
            return this.BeginTransaction();
        }

        IDbCommand IDbConnection.CreateCommand()
        {
            return CreateCommand();
        }
        #endregion

        #region Create Command
        /// <summary>
        /// cria um objeto command para ser usado nesta conexão
        /// </summary>
        /// <returns></returns>
        public new Command CreateCommand()
        {
            Command result = null;
            result = new Command(mDbConnection.CreateCommand());
            result.BaseTable = "";

            //aqui devemos setar a conexão e ... -->> (1)
            result.Connection = this;

            //... -->> (1) a transação para que não dê erro na execução do comando
            result.Transaction = Transaction;
            return result;
        }

        /// <summary>
        /// cria um objeto command
        /// </summary>
        /// <param name="baseTable">tabela base para o objeto comando</param>
        /// <returns>objeto command</returns>
        public virtual Command CreateCommand(string baseTable)
        {
            Command result = new Command(this);
            result.BaseTable = baseTable;
            return result;
        }

        protected override DbCommand CreateDbCommand()
        {
            return (DbCommand)this.CreateCommand();
        }
        #endregion

        #region Transaction
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            Transaction = new Transaction(this.BeginTransaction(isolationLevel));
            return Transaction;
        }

        public new virtual Transaction BeginTransaction(IsolationLevel il)
        {
            Transaction = new Transaction(mDbConnection.BeginTransaction(il));
            return Transaction;
        }

        public new virtual Transaction BeginTransaction()
        {
            if(CountTransaction <= 0 || Transaction == null)
            {
                CountTransaction = 0;

                if(State == ConnectionState.Closed)
                    Open();

                Transaction = new Transaction(mDbConnection.BeginTransaction());
                Debug.Log("Transaction Begin");
            }

            CountTransaction++;

            return Transaction;
        }

        public virtual void CommitTransaction()
        {
            CountTransaction--;
            if(CountTransaction <= 0)
            {
                CountTransaction = 0;
                Transaction.Commit();
            }

        }

        public virtual void RollbackTransaction()
        {
            if(CountTransaction == 0)
                return;

            CountTransaction--;
            if(CountTransaction <= 0)
            {
                CountTransaction = 0;
                Transaction.Rollback();
            }
        }
        #endregion

        #region Overrides
        protected override void Dispose(bool disposing)
        {
            //se a conexão estiver em uma transação. Não faça nada
            if(CountTransaction <= 0 || Transaction.State == TransactionState.Committed)
            {
                IsDisposed = true;

                base.Dispose(disposing);
                if(disposing)
                {
                    CountTransaction = 0;
                    CountOpen = 0;
                    Close();
                }
            }
        }

        public override string ToString()
        {
            if(mDbConnection != null)
            {
                return "\r\n{\r\n\tDatabase: {0} \r\n\tConnectionTimeout: {1}\r\n\tDatabaseType: {2}\r\n\tTransaction: {{3}}\r\n}"
                    .Replace("{0}", mDbConnection.Database)
                    .Replace("{1}", mDbConnection.ConnectionTimeout.ToString())
                    .Replace("{2}", DatabaseType.ToString())
                    .Replace("{3}", mTransaction == null ? "Transaction is null" : mTransaction.ToString());

            }

            return "Connection is null";
        }
        #endregion

        /// <summary>
        /// Cria uma string de conexão utilizando os parâmetros passados e instancia a conexão
        /// </summary>
        /// <param name="dbType">Tipo da base de dados que a string será criada</param>
        /// <param name="database">Nome da base de dados ou caminho do arquivo, se SQLite</param>
        /// <param name="server">Nome ou IP do servidor</param>
        /// <param name="port">porta de acesso ao servidor</param>
        /// <param name="userId">Usuário de acesso ao banco de dados</param>
        /// <param name="password">Senha de acesso ao banco de dados</param>
        /// <param name="moreConfig">Qualquer configuração adicional que deverá ser passada para a string de conexão</param>
        /// <exception cref="DatabaseTypeNotImplemented">Lançada quando for informado um tipo de base de dados inválido</exception>
        /// <returns>String de conexão montada</returns>
        public static string CreateConnectionString(Generic.DatabaseType dbType,
            string database,
            string server,
            int port,
            string userId,
            string password,
            string moreConfig = "")
        {
            string connString = "";
            switch(dbType)
            {
                case DatabaseType.PostgreSQL:
                    connString = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};{5}",
                        server,
                        port,
                        userId,
                        password,
                        database,
                        moreConfig);
                    break;
                case DatabaseType.SQLite:
                    connString = String.Format("Data Source={0};{1}", database, moreConfig);
                    break;
                default:
                    throw new DatabaseTypeNotImplemented();
            }

            return connString;
        }
    }
}