using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Unimake.Data.Generic
{
    /// <summary>
    /// Tipos de SGBD
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// SGBD PostgreSQL
        /// </summary>
        [Description("PostgreSQL")]
        PostgreSQL,

        /// <summary>
        /// SGBD SQLite
        /// </summary>
        [Description("SQLite")]
        SQLite,
    }

    /// <summary>
    /// indica a posição que deverá ser movida o registro
    /// </summary>
    public enum MoveToPos
    {
        /// <summary>
        /// move para o inicio do registro
        /// </summary>
        BOF,

        /// <summary>
        /// move para o fim do registro
        /// </summary>
        EOF

    }

    public enum GenericDbType
    {
        Integer,
        Float,
        Boolean,
        Date,
        Time,
        DateTime,
        TimeStamp,
        String,
        Bit,
        Byte,
        Object,
        Unknown
    }

    public enum OrderByClassification
    {
        /// <summary>
        /// Nenhuma
        /// </summary>
        [Description("None")]
        None,

        /// <summary>
        /// Ascendente
        /// </summary>
        [Description("ASC")]
        Ascending,

        /// <summary>
        /// descendente
        /// </summary>
        [Description("DESC")]
        Descending
    }

    public enum ComparisonType
    {
        /// <summary>
        /// a == b
        /// </summary>
        [Description("a == b")]
        Equal,

        /// <summary>
        /// a &lt;&gt; b
        /// </summary>
        [Description("a <> b")]
        NotEqual,

        /// <summary>
        /// a &gt; b
        /// </summary>
        [Description("a > b")]
        GreaterThan,

        /// <summary>
        /// a &gt;= b
        /// </summary>
        [Description("a >= b")]
        GreaterEqual,

        /// <summary>
        /// a &lt; b
        /// </summary>
        [Description("a < b")]
        LessThan,

        /// <summary>
        /// a &lt;= b
        /// </summary>
        [Description("a <= b")]
        LessEqual,

        /// <summary>
        /// a like 'b%'
        /// </summary>
        [Description("a like 'b%'")]
        StartsWith,

        /// <summary>
        /// a like '%b'
        /// </summary>
        [Description("a like '%b'")]
        EndsWith,

        /// <summary>
        /// a like '%b%'
        /// </summary>
        [Description("a like '%b%'")]
        Having,

        /// <summary>
        /// BETWEEN value1 and value2
        /// </summary>
        [Description("BETWEEN value1 and value2")]
        Between,

        /// <summary>
        /// IN(value1, value2,valueN)
        /// </summary>
        [Description("IN(value1, value2,valueN)")]
        IN
    }

    /// <summary>
    /// estados da transaçào
    /// </summary>
    public enum TransactionState
    {
        /// <summary>
        /// iniciou o begin transaction
        /// </summary>
        Begin,

        /// <summary>
        /// transação foi confirmada
        /// </summary>
        Committed,

        /// <summary>
        /// transação foi desfeita
        /// </summary>
        Rolledback

    }
}
