using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.ComponentModel;
using Unimake.Data.Generic.Definitions.Mapping;
using System.Data;

namespace Unimake.Data.Generic
{
    public class Parameter: DbParameter, IConvertible
    {
        #region Locais
        DbParameter _dbParameter = null;
        DbParameter dbParameter
        {
            get
            {
                if(_dbParameter == null)
                    _dbParameter = ToParameterType(false);

                return _dbParameter;
            }
        }
        private GenericDbType mType = GenericDbType.Object;
        #endregion

        #region Propriedades
        private string mTableName = "";

        /// <summary>
        /// retorna o nome da tabela que o campo pertence.
        /// <para>usa como base o . para separar o nome da tabela do nome do campo. Logo o SourceColumn deverá ser pasasdo como NomeTabela.NomeCampo</para>
        /// </summary>
        public string TableName
        {
            get
            {
                if(string.IsNullOrEmpty(SourceColumn) == false)
                {
                    if(SourceColumn.Contains("."))
                        mTableName = SourceColumn.Split('.')[0];
                }

                return mTableName;
            }
            set { mTableName = value; }
        }
        #endregion

        #region Construtores
        public Parameter()
            : base()
        {
        }

        public Parameter(string parameterName, GenericDbType type, string sourceColumn, object value)
            : base()
        {
            ParameterName = parameterName;
            SourceColumn = sourceColumn;
            Value = value;
            GenericDbType = type;
        }

        public Parameter(string parameterName, GenericDbType type, string sourceColumn)
            : base()
        {
            ParameterName = parameterName;
            SourceColumn = sourceColumn;
            Value = null;
            GenericDbType = type;
        }

        public Parameter(string parameterName, GenericDbType type)
            : base()
        {
            ParameterName = parameterName;
            SourceColumn = parameterName;
            Value = null;
            GenericDbType = type;
        }

        public Parameter(string parameterName, object value)
            : base()
        {
            ParameterName = parameterName;
            SourceColumn = "";
            Value = value;
            GenericDbType = GenericDbType.String;
        }

        public Parameter(string parameterName)
            : base()
        {
            ParameterName = parameterName;
            SourceColumn = "";
            Value = null;
            GenericDbType = GenericDbType.String;
        }

        public Parameter(string parameterName, GenericDbType type, object value)
            : base()
        {
            ParameterName = parameterName;
            SourceColumn = "";
            Value = value;
            GenericDbType = type;
        }

        public Parameter(DbParameter parameter)
            : base()
        {
            ParameterName = parameter.ParameterName;
            SourceColumn = parameter.SourceColumn;
            Value = parameter.Value;
            GenericDbType = ToGenericDbType(parameter.DbType);
        }
        #endregion

        #region DbParameter Members
        /// <summary>
        /// tipo do campo
        /// </summary>
        public Unimake.Data.Generic.GenericDbType GenericDbType
        {
            get { return mType; }
            set
            {
                mType = value;
                dbParameter.DbType = ToDbType(value);
            }
        }

        public override System.Data.ParameterDirection Direction
        {
            get { return dbParameter.Direction; }
            set { dbParameter.Direction = value; }
        }

        /// <summary>
        /// se true o campo pode ser nulo
        /// </summary>
        public override bool IsNullable
        {
            get { return dbParameter.IsNullable; }
            set { dbParameter.IsNullable = value; }
        }

        /// <summary>
        /// nome do parametro, um alias.
        /// </summary>
        public override string ParameterName
        {
            get { return dbParameter.ParameterName; }
            set { dbParameter.ParameterName = value; }
        }

        /// <summary>
        /// coloca o GenericDbType para string (default)
        /// </summary>
        public void ResetGenericDbType()
        {
            this.GenericDbType = GenericDbType.String;
        }

        /// <summary>
        /// tamanho do campo no banco de dados
        /// </summary>
        public override int Size
        {
            get { return dbParameter.Size; }
            set { dbParameter.Size = value; }
        }

        /// <summary>
        /// nome original da coluna
        /// </summary>
        public override string SourceColumn
        {
            get
            {
                if(string.IsNullOrEmpty(dbParameter.SourceColumn))
                    return ParameterName;

                return dbParameter.SourceColumn;
            }
            set { dbParameter.SourceColumn = value; }
        }

        public string Name
        {
            get { return SourceColumn; }
        }

        public override bool SourceColumnNullMapping
        {
            get { return dbParameter.SourceColumnNullMapping; }
            set { dbParameter.SourceColumnNullMapping = value; }
        }

        public override System.Data.DataRowVersion SourceVersion
        {
            get { return dbParameter.SourceVersion; }
            set { dbParameter.SourceVersion = value; }
        }

        /// <summary>
        /// valor da coluna
        /// </summary>
        public override object Value
        {
            get { return dbParameter.Value ?? DBNull.Value; }
            set
            {
                #region Define val
                object val = value;

                if(val != null)
                {
                    try
                    {
                        if(val is IMappingType)
                        {
                            IMappingType mapping = value as IMappingType;
                            val = mapping.ConvertToDbValue();
                        }

                        dbParameter.Value = val;
                    }
                    catch(InvalidCastException)
                    {
                        Type t = val.GetType();

                        if(t.GetCustomAttributes(false).Count(w =>
                                            w is System.ComponentModel.TypeConverterAttribute) > 0)
                        {
                            TypeConverter conv = TypeDescriptor.GetConverter(t);
                            if(conv.CanConvertFrom(GenericDbType.GetTypeValue()))
                            {
                                try
                                {
                                    val = conv.ConvertFrom(val);
                                    dbParameter.Value = val;
                                }
                                catch
                                {
                                    if(val != null)
                                        dbParameter.Value = val.ToString();
                                }
                            }else
                                val = value.ToString();
                        }
                        else
                            val = value.ToString();

                        dbParameter.Value = val;
                    }
                }
                #endregion

                GenericDbType = ToGenericDbType(Value.GetType());
            }
        }

        public override DbType DbType
        {
            get { return dbParameter.DbType; }
            set
            {
                dbParameter.DbType = value;
                mType = ToGenericDbType(dbParameter.DbType);
            }
        }

        public override void ResetDbType()
        {
            DbType = DbType.String;
            GenericDbType = GenericDbType.String;
        }
        #endregion

        #region Métodos
        public Type GetFieldType()
        {
            switch(GenericDbType)
            {
                case GenericDbType.Integer:
                    return typeof(int);
                case GenericDbType.Float:
                    return typeof(float);
                case GenericDbType.Boolean:
                    return typeof(bool);
                case GenericDbType.Date:
                case GenericDbType.Time:
                case GenericDbType.DateTime:
                case GenericDbType.TimeStamp:
                    return typeof(DateTime);
                case GenericDbType.String:
                    return typeof(string);
                case GenericDbType.Bit:
                case GenericDbType.Byte:
                    return typeof(byte);
                case GenericDbType.Object:
                case GenericDbType.Unknown:
                default:
                    return typeof(object);
            }
        }

        public Generic.GenericDbType ToGenericDbType(Type type)
        {
            if(type == typeof(bool))
            {
                if(Unimake.Data.Generic.Configuration.DataGenericSettings.Settings.UseBoolAsInt)
                    return Generic.GenericDbType.Integer;

                return Unimake.Data.Generic.GenericDbType.Boolean;
            }

            if(type == typeof(byte))
                return GenericDbType.Byte;

            if(type == typeof(decimal) ||
               type == typeof(double) ||
               type == typeof(Single) ||
               type == typeof(float))
                return Unimake.Data.Generic.GenericDbType.Float;

            if(type == typeof(Int16) ||
               type == typeof(Int32) ||
               type == typeof(Int64) ||
               type == typeof(int) ||
               type == typeof(IntPtr) ||
               type == typeof(uint) ||
               type == typeof(UInt16) ||
               type == typeof(UInt32) ||
               type == typeof(UInt64) ||
               type == typeof(UIntPtr) ||
               type == typeof(ulong) ||
               type == typeof(long))
                return Unimake.Data.Generic.GenericDbType.Integer;

            if(type == typeof(DateTime))
                return Unimake.Data.Generic.GenericDbType.DateTime;

            return Unimake.Data.Generic.GenericDbType.String;
        }

        public GenericDbType ToGenericDbType(DbType value)
        {
            switch(value)
            {
                case DbType.Boolean:
                    if(Unimake.Data.Generic.Configuration.DataGenericSettings.Settings.UseBoolAsInt)
                        return Generic.GenericDbType.Integer;

                    return Unimake.Data.Generic.GenericDbType.Boolean;
                case DbType.Byte:
                case DbType.SByte:
                    return GenericDbType.Byte;

                case DbType.Currency:
                case DbType.Decimal:
                case DbType.Double:
                case DbType.Single:
                case DbType.VarNumeric:
                    return Unimake.Data.Generic.GenericDbType.Float;

                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                    return Unimake.Data.Generic.GenericDbType.Integer;

                case DbType.Object:
                case DbType.Binary:
                    return Unimake.Data.Generic.GenericDbType.Unknown;

                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                case DbType.Time:
                    return Unimake.Data.Generic.GenericDbType.Date;

                case DbType.Xml:
                case DbType.Guid:
                case DbType.String:
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.StringFixedLength:
                default:
                    return Unimake.Data.Generic.GenericDbType.String;
            }
        }

        public GenericDbType ToGenericDbType(string type)
        {
            GenericDbType ret = GenericDbType.String;

            if(Utilities.IsNumeric(type))
            {
                int t = 0;
                int.TryParse(type, out t);
                ret = ToGenericDbType((DbType)t);
            }
            else
            {
                type = type.ToUpper().Split(new string[] { " " }, StringSplitOptions.None)[0];

                if(type.Contains("(")) type = type.Split(new string[] { "(" }, StringSplitOptions.None)[0];

                switch(type)
                {
                    case "_BYTE":
                    case "BYTE":
                    case "BYTEA":
                    case "_BYTEA":
                        ret = GenericDbType.Byte;
                        break;
                    case "BOOL":
                    case "BOOLEAN":
                        ret = GenericDbType.Boolean;
                        break;

                    case "NUMERIC":
                    case "DECIMAL":
                    case "FLOAT4":
                    case "_FLOAT4":
                    case "REAL":
                        ret = GenericDbType.Float;
                        break;

                    case "TIMETZ":
                        ret = GenericDbType.Time;
                        break;

                    case "DATE":
                        ret = GenericDbType.Date;
                        break;

                    case "TIMESTAMPTZ":
                    case "TIMESTAMP":
                    case "TIME":
                    case "DATETIME":
                        ret = GenericDbType.TimeStamp;
                        break;

                    case "TEXT":
                    case "_TEXT":
                    case "VARCHAR":
                    case "BPCHAR":
                    case "CHAR":
                    case "_CHAR":
                    case "CHARACTER":
                    case "CHARACTER VARYING":
                        ret = GenericDbType.String;
                        break;
                    case "INT4":
                    case "INT2":
                    case "INT8":
                    case "_INT4":
                    case "_INT2":
                    case "_INT8":
                    case "BIGINT":
                    case "SMALLINT":
                    case "INTEGER":
                    case "INT":
                        ret = GenericDbType.Integer;
                        break;
                    default:
                        ret = GenericDbType.String;
                        break;
                }
            }

            return ret;
        }

        public DbType ToDbType(GenericDbType value)
        {
            switch(value)
            {
                case GenericDbType.Integer:
                    return DbType.Int32;
                case GenericDbType.Float:
                    return DbType.Double;
                case GenericDbType.Boolean:
                    if(Unimake.Data.Generic.Configuration.DataGenericSettings.Settings.UseBoolAsInt)
                        return DbType.Int16;
                    return DbType.Boolean;
                case GenericDbType.Date:
                case GenericDbType.Time:
                case GenericDbType.TimeStamp:
                case GenericDbType.DateTime:
                    return DbType.DateTime;
                case GenericDbType.String:
                    return DbType.String;
                case GenericDbType.Bit:
                    return DbType.Boolean;
                case GenericDbType.Byte:
                    return DbType.Byte;
                case GenericDbType.Object:
                    return DbType.Object;
                case GenericDbType.Unknown:
                    return DbType.Object;
                default:
                    return DbType.String;
            }

        }

        /// <summary>
        /// converte o parametro para o tipo esperado
        /// </summary>
        /// <param name="copy">Se true, copia os valores deste objeto</param>
        /// <returns></returns>
        internal DbParameter ToParameterType(bool copy = true)
        {
            DbParameter result = null;

            #region Detecta o tipo de parametro
            switch(Configuration.DataGenericSettings.Settings.DatabaseType)
            {
                case DatabaseType.PostgreSQL:
                    result = new Npgsql.NpgsqlParameter();
                    break;
                case DatabaseType.SQLite:
                    result = new System.Data.SQLite.SQLiteParameter();
                    break;

                default:
                    throw new Exceptions.DatabaseTypeNotDefined();
            }
            #endregion

            #region Copia o objeto
            if(copy)
            {
                result.ParameterName = ParameterName;
                result.DbType = DbType;
                result.SourceColumn = SourceColumn;
                result.Value = Value;
            }
            #endregion

            return result;
        }
        #endregion

        #region IConvertible Members

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return ToBoolean();
        }

        public byte ToByte(IFormatProvider provider)
        {
            return ToByte();
        }

        public char ToChar(IFormatProvider provider)
        {
            return ToChar();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return ToDateTime();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return ToDecimal();
        }

        public double ToDouble(IFormatProvider provider)
        {
            return ToDouble();
        }

        public short ToInt16(IFormatProvider provider)
        {
            return ToInt16();
        }

        public int ToInt32(IFormatProvider provider)
        {
            return ToInt32();
        }

        public long ToInt64(IFormatProvider provider)
        {
            return ToInt64();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return ToSByte();
        }

        public float ToSingle(IFormatProvider provider)
        {
            return ToSingle();
        }

        public string ToString(IFormatProvider provider)
        {
            return ToString();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return ToType(conversionType);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return ToUInt16();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return ToUInt32();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return ToUInt64();
        }

        public bool ToBoolean()
        {
            bool ret = false;
            object valor = Value;
            int i = 0;
            if(valor == null || valor.ToString().Length == 0)
                return false;
            if(Char.IsNumber(valor.ToString(), 0))
            {
                int.TryParse(valor.ToString(), out i);
                i = Math.Abs(i);
                if(i == 0)
                    ret = false;
                else
                    return true;
            }
            else
                bool.TryParse(valor.ToString(), out ret);
            return ret;
        }

        public byte ToByte()
        {
            byte ret = 0;
            byte.TryParse(Value.ToString(), out ret);
            return ret;
        }

        public char ToChar()
        {
            char ret = new char();
            char.TryParse(Value.ToString(), out ret);
            return ret;
        }

        public DateTime ToDateTime()
        {
            DateTime ret = DateTime.MinValue;
            DateTime.TryParse(Value.ToString(), out ret);
            return ret;
        }

        public decimal ToDecimal()
        {
            decimal ret = 0;
            decimal.TryParse(Value.ToString(), out ret);
            return ret;
        }

        public double ToDouble()
        {
            double ret = 0;
            double.TryParse(Value.ToString(), out ret);
            return ret;
        }

        public short ToInt16()
        {
            Int16 ret = 0;
            Int16.TryParse(Value.ToString(), out ret);
            return ret;
        }

        public int ToInt32()
        {
            Int32 ret = 0;
            Int32.TryParse(Value.ToString(), out ret);
            return ret;
        }

        public long ToInt64()
        {
            Int64 ret = 0;
            Int64.TryParse(Value.ToString(), out ret);
            return ret;
        }

        public int ToInt()
        {
            return ToInt32();
        }

        public sbyte ToSByte()
        {
            sbyte ret = 0;
            sbyte.TryParse(Value.ToString(), out ret);
            return ret;

        }

        public float ToSingle()
        {
            float ret = 0;
            float.TryParse(Value.ToString(), out ret);
            return ret;
        }

        public object ToType(Type conversionType)
        {
            return Value;
        }

        public ushort ToUInt16()
        {
            UInt16 ret = 0;
            UInt16.TryParse(Value.ToString(), out ret);
            return ret;
        }

        public uint ToUInt32()
        {
            UInt32 ret = 0;
            UInt32.TryParse(Value.ToString(), out ret);
            return ret;

        }

        public ulong ToUInt64()
        {
            UInt64 ret = 0;
            UInt64.TryParse(Value.ToString(), out ret);
            return ret;
        }

        public TEnum ToEnum<TEnum>()
        {
            return (TEnum)Enum.Parse(typeof(TEnum), Value.ToString());
        }

        #endregion

        #region Overrides
        public override string ToString()
        {
            if(Value == null || Value.ToString().Length == 0)
                return "";

            return Value.ToString();
        }
        #endregion
    }
}
