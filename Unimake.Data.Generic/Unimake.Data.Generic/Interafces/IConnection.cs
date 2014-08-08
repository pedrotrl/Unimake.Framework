using System;
using System.Data;

namespace Unimake.Data.Generic
{
    interface IConnection: IDbConnection
    {
        Connection Clone();
        void CommitTransaction();
        Command CreateCommand(string baseTable);
        string DataSource { get; }
        void Open(string connectionString);
        void RollbackTransaction();
        string ServerVersion { get; }
        Transaction Transaction { get; set; }
    }
}
