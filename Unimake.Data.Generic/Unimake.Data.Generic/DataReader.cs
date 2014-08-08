using System.Collections.Generic;
using System.Data.Common;
using System.Collections;
using Unimake.Data.Generic.Exceptions;
using System.Data;
using System;

namespace Unimake.Data.Generic
{
    public class DataReader: MarshalByRefObject, IDataReader,
        IEnumerable<DataDictionary>,
        IDisposable
    {
        #region Statics
        /// <summary>
        /// usado para os valores nulos no banco de dados
        /// </summary>
        public static readonly string NULLString = "NULL";
        #endregion

        #region Locais
        protected internal List<Object> Dados = new List<Object>();
        protected internal int mDepth = 0;
        protected internal DataTable mGetSchemaTable;
        protected internal int mRecordsAffected;
        protected internal int mFieldCount = 0;
        protected internal List<string> mGetDataTypeName = new List<string>();
        protected internal List<Type> mGetFieldType = new List<Type>();
        protected internal List<string> mGetName = new List<string>();
        /// <summary>
        /// string de comando utilizada para a geração do DataReader
        /// </summary>
        internal string mCommandText = "";
        #endregion

        #region Propriedades
        private int mCurrentPosition = -1;
        public int CurrentPosition
        {
            get { return mCurrentPosition + 1; }
            set { mCurrentPosition = value - 1; }
        }
        public virtual bool HasRows
        {
            get { return Dados.Count > 0 ? true : false; }
        }
        #endregion

        #region Operadores
        #endregion

        #region Construtores
        internal DataReader()
        {
            IsClosed = true;
            IsReady = false;
        }

        internal DataReader(DbDataReader dr, string commandText)
            : this()
        {
            Fill(dr, commandText);
        }

        internal virtual void Fill(DbDataReader dr, string commandText)
        {
            lock(dr)
            {
                mCommandText = commandText;
                object[] Values = null;
                Dados.Clear();

                mFieldCount = dr.FieldCount;

                #region Propriedades de coleção
                for(int i = 0; i < FieldCount; i++)
                {
                    mGetDataTypeName.Add(dr.GetDataTypeName(i));
                    mGetFieldType.Add(dr.GetFieldType(i));
                    mGetName.Add(dr.GetName(i));
                }
                #endregion

                #region Propriedades Comuns
                mDepth = dr.Depth;
                mRecordsAffected = dr.RecordsAffected;
                mFieldCount = dr.FieldCount;
                #endregion

                #region Dados
                if(dr.HasRows)
                {
                    dr.Read();
                    mGetSchemaTable = dr.GetSchemaTable();

                    do
                    {
                        Values = new object[mFieldCount];
                        dr.GetValues(Values);
                        Dados.Add(Values);
                    } while(dr.Read());
                }
                #endregion

                #region Fim
                dr.Close();
                IsClosed = false;
                IsReady = false;
                MoveTo(MoveToPos.BOF);
                #endregion
            }
        }
        #endregion

        #region IDataReader Members

        public virtual void Close()
        {
            IsReady = false;
            IsClosed = true;
            Dados.Clear();
        }

        public virtual int Depth
        {
            get { return mDepth; }
        }

        public virtual DataTable GetSchemaTable()
        {
            return mGetSchemaTable;
        }

        public virtual bool IsClosed { get; internal set; }
        private bool mIsReady = false;
        public virtual bool IsReady
        {
            get
            {
                if(mCurrentPosition < 0)
                    mIsReady = false;
                return mIsReady;
            }
            internal set { mIsReady = value; }
        }
        public virtual bool NextResult()
        {
            //return mDataReader.NextResult();
            throw new NotImplementedException();
        }

        public virtual bool Read()
        {
            if(mCurrentPosition >= (Dados.Count - 1))
                IsReady = false;
            else
            {
                mCurrentPosition++;
                IsReady = true;
            }
            return IsReady;
        }

        public virtual int RecordsAffected
        {
            get { return mRecordsAffected; }
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            try
            {
                if(Dados != null)
                    Dados.Clear();
                Dados = null;
            }
            catch { }
        }

        void IDisposable.Dispose()
        {
            this.Dispose();
        }

        #endregion

        #region IDataRecord Members

        public virtual int FieldCount
        {
            get { return mFieldCount; }
        }

        public virtual bool GetBoolean(int i)
        {
            return this.GetValue<bool>(i);
        }

        public virtual bool GetBoolean(string name)
        {
            return this.GetValue<bool>(name);
        }

        public virtual byte GetByte(int i)
        {
            return this.GetValue<byte>(i);
        }

        public virtual long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            //return mDataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
            throw new NotImplementedException();
        }

        public virtual char GetChar(int i)
        {
            return this.GetValue<char>(i);
        }

        public virtual long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            //return mDataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
            throw new NotImplementedException();
        }

        public virtual IDataReader GetData(int i)
        {
            return (IDataReader)this[i];
        }

        public virtual string GetDataTypeName(int i)
        {
            //return mDataReader.GetDataTypeName(i);
            return mGetDataTypeName[i];
        }

        public virtual DateTime GetDateTime(int i)
        {
            return this.GetValue<DateTime>(i);
        }

        public virtual decimal GetDecimal(int i)
        {
            return this.GetValue<decimal>(i);
        }

        public virtual double GetDouble(int i)
        {
            return this.GetValue<double>(i);
        }

        public virtual Type GetFieldType(int i)
        {
            //return mDataReader.GetFieldType(i);
            return mGetFieldType[i];
        }

        public virtual float GetFloat(int i)
        {
            return this.GetValue<float>(i);
        }

        public virtual Guid GetGuid(int i)
        {
            //return mDataReader.GetGuid(i);
            throw new NotImplementedException();
        }

        public virtual short GetInt16(int i)
        {
            return this.GetValue<short>(i);
        }

        public virtual int GetInt32(int i)
        {
            return this.GetValue<int>(i);
        }

        public virtual long GetInt64(int i)
        {
            return this.GetValue<long>(i);
        }

        public virtual string GetName(int i)
        {
            //return mDataReader.GetName(i);
            return mGetName[i];
        }

        public virtual int GetOrdinal(string name)
        {
            for(int i = 0; i < mGetName.Count; i++)
            {
                if(name.Equals(mGetName[i], StringComparison.InvariantCultureIgnoreCase))
                    return i;
            }

            return -1;
        }

        public virtual string GetString(int i)
        {
            return this.GetValue<string>(i);
        }

        public virtual object GetValue(int i)
        {
            return this[i];
        }

        public virtual int GetValues(object[] values)
        {
            object[] result = (object[])Dados[mCurrentPosition];

            for(int i = 0; i < values.Length; i++)
                values[i] = GetValue(i);

            return values.Length;
        }

        public virtual bool IsDBNull(int i)
        {
            //return mDataReader.IsDBNull(i);
            if(this[i] == null)
                return true;

            return false;
        }

        public virtual object this[string name]
        {
            get
            {
                object ret = null;
                try
                {
                    ret = this[GetOrdinal(name)];
                }
                catch
                {
                    throw new FieldDoesntExist(name);
                }
                return ret;
            }
        }

        public virtual object this[int i]
        {
            get
            {
                object result = null;

                if(RecordCount <= 0)
                    return result;
                else
                {
                    if(IsReady == false)
                        Read();

                    result = ((object[])Dados[mCurrentPosition])[i];

                    if(result != null && GetFieldType(i) == typeof(System.String))
                        result = Utilities.PrepareValue<string>(result);

                    return result;
                }
            }
        }

        public virtual byte GetByte(string name)
        {
            return this.GetValue<byte>(name);
        }

        public virtual char GetChar(string name)
        {
            return this.GetValue<char>(name);
        }

        public virtual IDataReader GetData(string name)
        {
            return (IDataReader)this[name];
        }

        public virtual DateTime GetDateTime(string name)
        {
            return this.GetValue<DateTime>(name);
        }

        public virtual decimal GetDecimal(string name)
        {
            return this.GetValue<decimal>(name);
        }

        public virtual double GetDouble(string name)
        {
            return this.GetValue<double>(name);
        }

        public virtual float GetFloat(string name)
        {
            return this.GetValue<float>(name);
        }

        public virtual short GetInt16(string name)
        {
            return this.GetValue<short>(name);
        }

        public virtual int GetInt32(string name)
        {
            return this.GetValue<int>(name);
        }

        public virtual long GetInt64(string name)
        {
            return this.GetValue<long>(name);
        }

        public virtual string GetString(string name)
        {
            return this.GetValue<string>(name);
        }

        public virtual object GetValue(string name)
        {
            int i = GetOrdinal(name);
            if(i == -1)
                return default(object);
            return GetValue(i);
        }

        public virtual int GetInt(int i)
        {
            return GetInt32(i);
        }

        public virtual int GetInt(string name)
        {
            return GetInt32(name);
        }

        /// <summary>
        /// retorna o valor em formato ENUM
        /// </summary>
        /// <typeparam name="TEnum">Tipo esperado de retorno</typeparam>
        /// <param name="name">nome do campo</param>
        /// <returns>Enum</returns>
        public virtual TEnum GetEnum<TEnum>(string name)
        {
            return this.GetValue<TEnum>(name);
        }

        /// <summary>
        /// retorna o valor em formato ENUM
        /// </summary>
        /// <typeparam name="TEnum">Tipo esperado de retorno</typeparam>
        /// <param name="name">nome do campo</param>
        /// <returns>Enum</returns>
        public virtual TEnum GetEnum<TEnum>(int i)
        {
            string ret = GetString(i);
            if(ret.Length > 0)
                return (TEnum)Enum.Parse(typeof(TEnum), ret);

            return default(TEnum);
        }

        #endregion

        #region Métodos
        /// <summary>
        /// retorna uma string pelo nome do campo
        /// </summary>
        /// <param name="fieldName">nome do campo</param>
        /// <returns></returns>
        public string ToString(string name)
        {
            return GetString(name);
        }

        /// <summary>
        /// verifica se o campo existe.
        /// </summary>
        /// <param name="fieldName">nome do campo</param>
        /// <returns>true se o campo existir</returns>
        public bool HasField(string fieldName)
        {

            if(string.IsNullOrEmpty(fieldName))
                return false;
            bool ret = false;
            try
            {
                foreach(string item in mGetName)
                {
                    if(item.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ret = true;
                        break;
                    }

                }
            }
            catch
            {
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// este método é chamado no TableObject.Populate (DataReader dataReader) 
        /// para garantir que o tableobject ficara sempre com o datareader atualizado
        /// </summary>
        /// <param name="rs"></param>
        internal protected void SetDataReader(DataReader dataReader)
        {
            Dados = dataReader.Dados;
            mDepth = dataReader.mDepth;
            mGetSchemaTable = dataReader.mGetSchemaTable;
            mRecordsAffected = dataReader.mRecordsAffected;
            mFieldCount = dataReader.mFieldCount;
            mGetDataTypeName = dataReader.mGetDataTypeName;
            mGetFieldType = dataReader.mGetFieldType;
            mGetName = dataReader.mGetName;
            mCurrentPosition = dataReader.mCurrentPosition;
            mCommandText = dataReader.mCommandText;
            IsReady = dataReader.IsReady;
            IsClosed = dataReader.IsClosed;
            PageSize = dataReader.PageSize;
        }
        #endregion

        #region Movimenta o registro
        public virtual bool MoveFirst()
        {
            if(HasRows)
            {
                mCurrentPosition = 0;
                IsReady = true;
            }
            else
                IsReady = false;

            return IsReady;
        }

        public virtual bool MoveNext()
        {
            if(HasRows)
            {
                mCurrentPosition++;
                if(mCurrentPosition >= (Dados.Count - 1))
                {
                    mCurrentPosition = Dados.Count - 1;
                    IsReady = false;
                }
                else
                    IsReady = true;
            }
            else
                IsReady = false;

            return IsReady;
        }

        public virtual bool MoveLast()
        {
            if(HasRows)
            {
                mCurrentPosition = Dados.Count - 1;
                IsReady = true;
            }
            else
                IsReady = false;

            return IsReady;
        }

        public virtual bool MovePrevious()
        {
            if(HasRows)
            {
                mCurrentPosition--;
                if(mCurrentPosition < 0)
                {
                    mCurrentPosition = -1;
                    IsReady = false;
                }
                else
                    IsReady = true;
            }
            else
                IsReady = false;

            return IsReady;
        }

        /// <summary>
        /// move o registro para o início (BOF) ou final (EOF)
        /// </summary>
        /// <param name="moveTo">posição</param>
        /// <returns>true se moveu o registro</returns>
        public virtual bool MoveTo(MoveToPos moveToPos)
        {
            int pos = -1;
            if(moveToPos == MoveToPos.EOF)
            {
                pos = RecordCount + 1;
            }

            return MoveTo(pos);
        }

        /// <summary>
        /// move o registro para a posição informada
        /// </summary>
        /// <param name="position">posição desejada</param>
        /// <returns>retorna true se mover o registro para a posição desejada</returns>
        public virtual bool MoveTo(int position)
        {
            if(HasRows)
            {
                mCurrentPosition = position;
                if(position < 0)
                    IsReady = false;
                else if(position > Dados.Count)
                    IsReady = false;
                else if(position >= 0 || position <= (Dados.Count - 1))
                    IsReady = true;
            }
            else
                IsReady = false;

            return IsReady;
        }

        public virtual int RecordCount
        {
            get
            {
                if(HasRows)
                {
                    return Dados.Count;

                }
                return 0;
            }
        }
        #endregion

        #region Paginação
        /// <summary>
        /// tamanho da página de registro. O menor valor é 1 registro por página
        /// </summary>
        public int PageSize { get; set; }
        public int CurrentPage
        {
            get { return GetCurrentPage(mCurrentPosition); }
            set { MoveToPage(value); }
        }

        private int GetCurrentPage(int mCurrentPosition)
        {
            if(PageSize == 0) return 0;
            return Convert.ToInt32((mCurrentPosition / PageSize) + .6f);
        }

        public int PageCount
        {
            get
            {
                if(PageSize <= 0)
                    PageSize = 1;
                return Convert.ToInt32(Dados.Count / PageSize + .6f);
            }
        }

        /// <summary>
        /// retorna um DataReader contendo apenas os registro da página atual
        /// </summary>
        /// <returns>datareader</returns>
        public DataReader GetFromPage()
        {
            return GetFromPage(CurrentPage);
        }

        /// <summary>
        /// retorna um DataReader contendo os registro da página passada no parâmetro
        /// </summary>
        /// <param name="pageNumber">número da página</param>
        /// <returns></returns>
        public DataReader GetFromPage(int pageNumber)
        {
            DataReader ret = new DataReader();
            List<Object> _Dados = new List<Object>();
            ret.SetDataReader(this);
            ret.CurrentPage = pageNumber;

            int curPage = pageNumber;
            int curPos = ret.mCurrentPosition;

            while(curPage == GetCurrentPage(curPos))
            {
                _Dados.Add(Dados[curPos++]);
                if(curPos > Dados.Count)
                    break;

            }

            ret.Dados = _Dados;

            return ret;
        }

        #region Movimenta o registro por páginas
        public virtual bool MoveFirstPage()
        {
            if(HasRows)
            {
                mCurrentPosition = 0;
                IsReady = true;
            }
            else
                IsReady = false;

            return IsReady;
        }

        public virtual bool MoveNextPage()
        {
            if(HasRows)
            {
                if(CurrentPage <= PageCount)
                {
                    mCurrentPosition = (PageSize * (CurrentPage - 1)) + PageSize;
                    IsReady = true;
                }
                else
                    IsReady = false;
            }
            else
                IsReady = false;

            return IsReady;
        }

        public virtual bool MoveLastPage()
        {
            if(HasRows)
            {
                mCurrentPosition = PageSize * (PageCount - 1);
                IsReady = true;
            }
            else
                IsReady = false;

            return IsReady;
        }

        public virtual bool MovePreviousPage()
        {
            if(HasRows)
            {
                if(CurrentPage <= 0)
                {
                    mCurrentPosition = -1;
                    IsReady = false;
                }
                else
                {
                    mCurrentPosition = (PageSize * (CurrentPage - 1)) - PageSize;
                    IsReady = true;
                }
            }
            else
                IsReady = false;

            return IsReady;
        }

        public virtual bool MoveToPage(int _pageNumber)
        {
            if(_pageNumber <= 0 || _pageNumber > PageCount)
                throw new InvalidPageNumber();
            mCurrentPosition = PageSize * (_pageNumber - 1);
            IsReady = true;
            return IsReady;
        }
        #endregion
        #endregion

        #region IEnumerator<object> members
        public IEnumerator<DataDictionary> GetEnumerator()
        {
            for(int i = 0; i < Dados.Count; i++)
            {
                DataDictionary result = new DataDictionary();

                object[] values = Dados[i] as object[];

                for(int j = 0; j < values.Length; j++)
                {
                    result[GetName(j)] = values[j];
                }

                yield return result;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Dados.GetEnumerator();
        }
        #endregion
    }
}