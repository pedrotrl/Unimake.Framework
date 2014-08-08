using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Data.Generic.Definitions.Attributes
{
    /// <summary>
    /// Identifica os valores padrões para os tipos de campos que não são informados nas classes
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class DefaultValueAttribute: Attribute
    {
        #region propriedades
        GenericDbType _type = GenericDbType.Unknown;
        /// <summary>
        /// Tipo definido na base de dados
        /// </summary>
        public GenericDbType Type
        {
            get
            {
                if(_type == GenericDbType.Unknown)
                    _type = GenericDbType.Unknown.DetectTypeFromValue(Value);

                return _type;
            }
            private set { _type = value; }
        }
        /// <summary>
        /// Valor padrão do campo
        /// </summary>
        public object Value { get; private set; }
        /// <summary>
        /// Nome do campo
        /// </summary>
        public string FieldName { get; private set; }
        #endregion

        #region Construtores
        /// <summary>
        /// Instancia o atributo
        /// </summary>
        /// <param name="fieldName">Nome do campo </param>
        /// <param name="value">valor do campo</param>
        public DefaultValueAttribute(string fieldName, object value)
            : base()
        {
            FieldName = fieldName;
            Value = value;
        }
        /// <summary>
        /// Instancia o atributo
        /// </summary>
        /// <param name="fieldName">Nome do campo </param>
        /// <param name="value">valor do campo</param>
        /// <param name="type">Tipo de dado do campo</param>
        public DefaultValueAttribute(string fieldName, object value, GenericDbType type)
            : base()
        {
            Type = type;
            FieldName = fieldName;
            Value = value;
        }
        #endregion
    }
}
