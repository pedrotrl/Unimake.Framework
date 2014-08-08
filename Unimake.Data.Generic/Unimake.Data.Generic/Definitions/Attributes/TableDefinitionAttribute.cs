using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Data.Generic.Definitions.Attributes
{
    /// <summary>
    /// definições da tabela
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class TableDefinitionAttribute: Attribute
    {
        readonly string tableName;
        readonly string primaryKey;
        readonly string foreignKey;

        /// <summary>
        /// se true este campo pode ser utilizado em um insert mesmo se for chave primária
        /// </summary>
        readonly bool useInsert;

        /// <summary>
        /// define as configurações de uma tabela para uma classe
        /// </summary>
        /// <param name="tableName">nome da tabela</param>
        /// <param name="primaryKey">nome do campo chave primária</param>
        public TableDefinitionAttribute(string tableName, string primaryKey)
        {
            this.tableName = tableName;
            this.primaryKey = primaryKey;
        }

        /// <summary>
        /// define as configurações de uma tabela para uma classe
        /// </summary>
        /// <param name="tableName">nome da tabela</param>
        /// <param name="primaryKey">nome do campo chave primária</param>
        /// <param name="useInsert">se true este campo pode ser utilizado em um insert mesmo se for chave primária</param>
        public TableDefinitionAttribute(string tableName, string primaryKey, bool useInsert)
        {
            this.tableName = tableName;
            this.primaryKey = primaryKey;
            this.useInsert = useInsert;
        }

        /// <summary>
        /// define as configurações de uma tabela para uma classe
        /// </summary>
        /// <param name="tableName">nome da tabela</param>
        public TableDefinitionAttribute(string tableName)
        {
            this.tableName = tableName;
        }

        /// <summary>
        /// define as configurações de uma tabela para uma classe
        /// </summary>
        /// <param name="tableName">nome da tabela</param>
        /// <param name="primaryKey">nome do campo chave primária</param>
        public TableDefinitionAttribute(string tableName, string primaryKey, string foreignKey)
        {
            this.tableName = tableName;
            this.primaryKey = primaryKey;
            this.foreignKey = foreignKey;
        }

        /// <summary>
        /// Nome da tabela
        /// </summary>
        public string TableName
        {
            get { return tableName; }
        }

        /// <summary>
        /// nome do campo de chave primária
        /// </summary>
        public string PrimaryKey
        {
            get { return primaryKey; }
        }

        /// <summary>
        /// Retorna a lista de campos que esta tabela contem
        /// </summary>
        public Parameters Fields
        {
            get { return Schema.Tables.TableList[TableName].Fields; }
        }

        /// <summary>
        /// se true este campo pode ser utilizado em um insert mesmo se for chave primária
        /// </summary>
        public bool UseInsert
        {
            get { return useInsert; }
        }

        public override string ToString()
        {
            return TableName;
        }
    }
}
