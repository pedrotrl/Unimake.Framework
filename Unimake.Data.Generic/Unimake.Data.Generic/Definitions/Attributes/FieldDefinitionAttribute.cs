using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Unimake.Data.Generic.Definitions.Attributes
{
    /// <summary>
    /// definições para os campos de uma tabela
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class FieldDefinitionAttribute : Attribute
    {
        /// <summary>
        /// instancia a definição para o campo
        /// </summary>
        /// <param name="fieldName">nome do campo que deverá ser usado no momendo de montar o CRUD</param>
        public FieldDefinitionAttribute(string fieldName)
            : this(fieldName, true)
        {
        }

        /// <summary>
        /// instancia a definição para o campo
        /// </summary>
        /// <param name="fieldName">nome do campo que deverá ser usado no momendo de montar o CRUD</param>
        /// <param name="isTableField">Se true é um campo de tabela e será usado dentro do CRUD</param>
        public FieldDefinitionAttribute(string fieldName, bool isTableField)
            : this(null, fieldName, false, isTableField)
        {
        }

        /// <summary>
        /// instancia o campo como sendo parte de um join.
        /// <para>Por default o campo não é um TableField</para>
        /// </summary>
        /// <param name="joinTable">nome da tabela no JOIN</param>
        /// <param name="fieldName">nome do campo, será concatenado com o joinTable</param>
        /// <param name="isJoinField">se true usa este campo em um join. Opcional, valor padrão true</param>
        public FieldDefinitionAttribute(string joinTable, string fieldName, bool isJoinField = true, bool isTableField = true)
        {
            this.FieldName = fieldName;
            this.IsJoinField = isJoinField;
            this.JoinTable = joinTable;
            this.IsTableField = isTableField;
        }

        /// <summary>
        /// instancia a definição para o campo
        /// </summary>
        /// <param name="isTableField">Se true é um campo de tabela e será usado dentro do CRUD</param>
        public FieldDefinitionAttribute(bool isTableField)
            : this(null, null, false, isTableField)
        {
        }

        /// <summary>
        /// nome do campo que deverá ser usado no momendo de montar o CRUD
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// apelido do campo que deverá ser usado no momendo de montar o SELECT
        /// </summary>
        public string FieldAlias { get; set; }

        /// <summary>
        /// retorna true se for um campo de tabela
        /// </summary>
        public bool IsTableField { get; set; }

        /// <summary>
        /// se true, usa este campo em uma clausula JOIN
        /// </summary>
        public bool IsJoinField { get; set; }

        /// <summary>
        /// nome da tabela no join, será concatenado com o nome do campo (FieldName)
        /// </summary>
        public string JoinTable { get; set; }
    }
}
