using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;

namespace Unimake.Data.Generic
{
    public class Transaction: DbTransaction, IDbTransaction, ICloneable, IDisposable
    {
        #region Propriedades
        public TransactionState State { get; internal set; }
        #endregion

        #region Construtores
        internal Transaction(DbTransaction transaction)
        {
            this.dbTransaction = transaction;
            State = TransactionState.Begin;
        }
        #endregion

        #region locais
        internal DbTransaction dbTransaction;
        #endregion

        #region IDbTransaction Members

        public override void Commit()
        {
            dbTransaction.Commit();
            State = TransactionState.Committed;
            Debug.Log("Transaction Commit");
        }

        public new virtual Connection Connection
        {
            get { return (Connection)dbTransaction.Connection; }
        }

        IDbConnection IDbTransaction.Connection
        {
            get { return (IDbConnection)this.Connection; }
        }

        public override IsolationLevel IsolationLevel
        {
            get { return dbTransaction.IsolationLevel; }
        }

        public override void Rollback()
        {
            dbTransaction.Rollback();
            State = TransactionState.Rolledback;
            Debug.Log("Transaction Rollback");
        }

        #endregion

        #region IDisposable Members

        public new virtual void Dispose()
        {
            dbTransaction.Dispose();
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this.Dispose();
        }

        #endregion

        #region ICloneable Members
        public virtual Transaction Clone()
        {
            return (Transaction)this.MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

        #region Overrides
        protected override DbConnection DbConnection
        {
            get { return dbTransaction.Connection; }
        }

        public override string ToString()
        {
            return String.Format("\tState: {0}", State);
        }
        #endregion
    }
}
