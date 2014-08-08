using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Threading;
using Unimake.Data.Generic.Configuration;

namespace Unimake.Data.Generic
{
    public class Command: DbCommand, ICloneable, IDisposable, IDbCommand
    {
        #region Operadores
        /// <summary>
        /// Adiciona um comando à este comando
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns>Um novo comando adicionado à este</returns>
        public static Command operator +(Command lhs, Command rhs)
        {
            lhs.Parameters.AddRange(rhs.Parameters);
            return lhs.Clone();
        }
        #endregion

        #region Construtores
        /// <summary>
        /// Cria um novo objeto do tipo DbCommando com base no tipo de conexão
        /// </summary>
        /// <returns></returns>
        private static DbCommand _Command()
        {
            DbCommand result = null;
            if(DataGenericSettings.Settings.DatabaseType == DatabaseType.SQLite)
                result = new System.Data.SQLite.SQLiteCommand();
            else if(DataGenericSettings.Settings.DatabaseType == DatabaseType.PostgreSQL)
                result = new Npgsql.NpgsqlCommand();

            return result;
        }
        /// <summary>
        /// Constrói um novo comando com base no objeto base DbCommand
        /// </summary>
        /// <param name="command">Objeto base, DBCommand</param>
        internal Command(DbCommand command)
        {
            mCommand = command;
            CommandTimeout = 600;
        }

        /// <summary>
        /// Constrói um novo command
        /// </summary>
        /// <param name="commandText">SQL que será utilizado no comando</param>
        /// <param name="connection">Conexão associado ao comando</param>
        public Command(string commandText, Unimake.Data.Generic.Connection connection)
            : this(commandText, connection, null)
        {
        }

        /// <summary>
        /// Constroi um novo command
        /// </summary>
        /// <param name="commandText">SQL que será utilizado no comando</param>
        /// <param name="connection">Conexão associada ao comando</param>
        /// <param name="transaction">Transação associada ao comando</param>
        public Command(string commandText, Unimake.Data.Generic.Connection connection, Unimake.Data.Generic.Transaction transaction)
            : this(connection.CreateCommand().mCommand)
        {
            CommandText = commandText;
            Connection = connection;
            Transaction = transaction;
        }

        /// <summary>
        /// Constroi um novo command
        /// </summary>
        /// <param name="connection">Conexão associada ao comando</param>
        /// <param name="transaction">Transação associada ao comando</param>
        public Command(Unimake.Data.Generic.Connection connection, Unimake.Data.Generic.Transaction transaction)
            : this(connection.CreateCommand().mCommand)
        {
            Connection = connection;
            Transaction = transaction;
        }

        /// <summary>
        /// Constroi um novo command
        /// </summary>
        /// <param name="connection">Conexão associada ao comando</param>
        public Command(Unimake.Data.Generic.Connection connection)
            : this(connection.CreateCommand().mCommand)
        {
            Connection = connection;
        }

        public Command()
            : this(_Command())
        {
        }
        #endregion

        #region Locais
        internal DbCommand mCommand = null;
        private Transaction mTransaction;
        private Connection mConnection;
        private Parameters mParameters = new Parameters();
        #endregion

        #region Propriedades
        /// <summary>
        /// usado dentro do comando Execute... da classe Connection
        /// <para>Este é o nome da classe base da clausula SQL</para>
        /// </summary>
        public string BaseTable { get; set; }
        #endregion

        #region DbCommand Members
        public override void Cancel()
        {
            this.mCommand.Cancel();
        }

        public override string CommandText
        {
            get { return mCommand.CommandText; }
            set { mCommand.CommandText = value; }
        }

        public override int CommandTimeout
        {
            get { return mCommand.CommandTimeout; }
            set { mCommand.CommandTimeout = value; }
        }

        public override CommandType CommandType
        {
            get { return mCommand.CommandType; }
            set { mCommand.CommandType = value; }
        }

        public new virtual Parameters Parameters
        {
            get { return mParameters; }
        }

        public override bool DesignTimeVisible
        {
            get { return mCommand.DesignTimeVisible; }
            set { mCommand.DesignTimeVisible = value; }
        }

        public new virtual DataReader ExecuteReader()
        {
            Debug.ExecuteReaderStart(this);
            DataReader result = null;
            bool flag = false;

            try
            {
                if(Connection.State == ConnectionState.Closed)
                {
                    flag = true;
                    Connection.Open();
                }

                ParseCommand();
                result = new DataReader(mCommand.ExecuteReader(), mCommand.CommandText);
            }
            catch(Exception ex)
            {
                Debug.ExecuteReaderError(this, ex);
                throw;
            }
            finally
            {
                if(flag)
                    Connection.Close();
            }

            Debug.ExecuteReaderFinish(this);
            return result;
        }

        public virtual DataReader ExecuteReader(string commandText)
        {
            this.CommandText = commandText;
            return this.ExecuteReader();
        }

        public override int ExecuteNonQuery()
        {
            bool flag = false;
            int result = 0;

            Debug.ExecuteNonQueryStart(this);

            try
            {
                if(Connection.State == ConnectionState.Closed)
                {
                    flag = true;
                    Connection.Open();
                }

                ParseCommand();
                result = mCommand.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                Debug.ExecuteNonQueryError(this, ex);
                throw;
            }
            finally
            {
                if(flag)
                    Connection.Close();
            }

            Debug.ExecuteNonQueryFinish(this);
            return result;
        }

        public override object ExecuteScalar()
        {
            bool flag = false;

            try
            {
                if(Connection.State == ConnectionState.Closed)
                {
                    flag = true;
                    Connection.Open();
                }
                ParseCommand();
                return mCommand.ExecuteScalar();
            }
            finally
            {
                if(flag)
                    Connection.Close();
            }
        }

        public override void Prepare()
        {
            this.mCommand.Prepare();
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get { return mCommand.UpdatedRowSource; }
            set { mCommand.UpdatedRowSource = value; }
        }

        public new virtual Transaction Transaction
        {
            get
            {
                //se for nulo, pega a transaction da conexão atual.
                if(mTransaction == null)
                    mTransaction = Connection.Transaction;

                //se a transação atual já estiver finalizada pega a transação atual
                if(mTransaction != null && mTransaction.State == TransactionState.Committed)
                    mTransaction = Connection.Transaction;
                return (Transaction)mTransaction;
            }
            set
            {
                mTransaction = value;
                mCommand.Transaction = value == null ? null :
                                       value.State == TransactionState.Begin ? value.dbTransaction : null;
            }
        }

        public new virtual Connection Connection
        {
            get { return mConnection; }
            set
            {
                mConnection = value;
                mCommand.Connection = value == null ? null : value.mDbConnection;
            }
        }
        #endregion

        #region ICloneable Members

        public virtual Command Clone()
        {
            return (Command)this.MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region IDisposable Members

        ~Command()
        {
            Dispose();
        }

        #endregion

        #region IDbCommand Members

        IDbConnection IDbCommand.Connection
        {
            get
            {
                return this.Connection;
            }
            set
            {
                this.Connection = (Connection)value;
            }
        }

        public new virtual IDbDataParameter CreateParameter()
        {
            return mCommand.CreateParameter();
        }

        public new virtual DataReader ExecuteReader(CommandBehavior behavior)
        {
            bool flag = false;
            DataReader result = null;
            DbConnection connection = mCommand.Connection;

            try
            {
                if(connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                    flag = true;
                }
                ParseCommand();
                result = new DataReader(mCommand.ExecuteReader(behavior), mCommand.CommandText);
            }
            finally
            {
                if(flag)
                    connection.Close();
            }

            return result;
        }

        IDataReader IDbCommand.ExecuteReader()
        {
            return mCommand.ExecuteReader();
        }

        IDataParameterCollection IDbCommand.Parameters
        {
            get { return mCommand.Parameters; }
        }

        IDbTransaction IDbCommand.Transaction
        {
            get
            {
                return this.Transaction;
            }
            set
            {
                this.Transaction = (Transaction)value;
            }
        }

        #endregion

        #region Métodos
        private void ParseCommand()
        {
            if((mCommand.Connection == null &&
                this.Connection != null) ||
                mCommand.Connection.State == ConnectionState.Closed)
                mCommand.Connection = this.Connection.mDbConnection;

            if(mCommand.Transaction == null &&
                this.Transaction != null &&
                Transaction.State == TransactionState.Begin)
                mCommand.Transaction = this.Transaction.dbTransaction;

            //aqui os parâmetros deverão ser reajustados, pois podem ter sido modificados durante o processo
            mCommand.Parameters.Clear();

            foreach(Parameter parameter in this.Parameters)
                mCommand.Parameters.Add(parameter.ToParameterType());
        }

        private System.Data.Odbc.OdbcType ToOdbcType(DbType dbType)
        {
            switch(dbType)
            {
                case DbType.Boolean:
                    return System.Data.Odbc.OdbcType.TinyInt;
                case DbType.Byte:
                case DbType.SByte:
                    return OdbcType.SmallInt;

                case DbType.Currency:
                case DbType.Decimal:
                case DbType.Double:
                case DbType.Single:
                case DbType.VarNumeric:
                    return System.Data.Odbc.OdbcType.Decimal;

                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                    return System.Data.Odbc.OdbcType.Int;

                case DbType.Object:
                case DbType.Binary:
                    return System.Data.Odbc.OdbcType.Binary;

                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                case DbType.Time:
                    return System.Data.Odbc.OdbcType.DateTime;

                case DbType.Xml:
                case DbType.Guid:
                case DbType.String:
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.StringFixedLength:
                default:
                    return System.Data.Odbc.OdbcType.NVarChar;
            }
        }

        private System.Data.OleDb.OleDbType ToOleDbType(DbType dbType)
        {
            switch(dbType)
            {
                case DbType.Boolean:
                    return System.Data.OleDb.OleDbType.TinyInt;
                case DbType.Byte:
                case DbType.SByte:
                    return OleDbType.SmallInt;

                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                    return System.Data.OleDb.OleDbType.Date;

                case DbType.Currency:
                case DbType.Decimal:
                case DbType.Double:
                case DbType.Single:
                case DbType.VarNumeric:
                    return System.Data.OleDb.OleDbType.Decimal;

                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                    return System.Data.OleDb.OleDbType.Integer;

                case DbType.Object:
                case DbType.Binary:
                    return System.Data.OleDb.OleDbType.Binary;

                case DbType.Time:
                    return System.Data.OleDb.OleDbType.DBTimeStamp;

                case DbType.Xml:
                case DbType.Guid:
                case DbType.String:
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.StringFixedLength:
                default:
                    return System.Data.OleDb.OleDbType.VarWChar;
            }
        }

        #endregion

        #region Protegidos
        protected override DbParameter CreateDbParameter()
        {
            return (DbParameter)this.CreateParameter();
        }

        protected override DbConnection DbConnection
        {
            get { return mCommand.Connection; }
            set
            {
                mConnection = (Connection)value;
                mCommand.Connection = value;
            }
        }

        protected override DbParameterCollection DbParameterCollection
        {
            get { return mCommand.Parameters; }
        }

        protected override DbTransaction DbTransaction
        {
            get { return Transaction == null ? null : Transaction.dbTransaction; }
            set { Transaction = new Transaction(value); }
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return (DbDataReader)(this.ExecuteReader(behavior) as IDataReader);
        }

        #endregion

        #region Overrides
        public override string ToString()
        {
            return "Command {CommandText: {0}\r\n\tParameters: {1}\r\n\tConnection {\r\n\t {2}} }"
                .Replace("{0}", mCommand.CommandText)
                .Replace("{1}", Parameters.ToString(mCommand.Parameters))
                .Replace("{2}", Connection == null ? "Connection is null" : Connection.ToString());
        }
        #endregion
    }
}
