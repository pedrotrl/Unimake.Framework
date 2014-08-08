using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Data.Generic.Definitions.Attributes
{
    /// <summary>
    /// Atributo para definição de chaves estrangeiras
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class ForeignKeyAttribute: Attribute
    {
        /// <summary>
        /// Nome da tabela pai que está associada com a filha
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// Nome do campo relacionado na tabela filha
        /// </summary>
        public string ForeignKey { get; private set; }

        /// <summary>
        /// Instancia o atributo com os valores obrigatórios
        /// </summary>
        /// <param name="tableName">Nome da tabela pai que está associada com a filha</param>
        /// <param name="foreignKey">Nome do campo relacionado na tabela filha</param>
        public ForeignKeyAttribute(string tableName, string foreignKey)
        {
            this.ForeignKey = foreignKey;
            this.TableName = tableName;
        }
    }
}
