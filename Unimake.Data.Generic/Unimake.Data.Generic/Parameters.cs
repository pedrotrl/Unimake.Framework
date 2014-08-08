using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data.Common;
using System.Linq;
using Unimake.Data.Generic.Exceptions;
using System.Data;

namespace Unimake.Data.Generic
{
    public class Parameters: DbParameterCollection, IDataParameterCollection, ICloneable, IEnumerable<Parameter>
    {
        #region Construtores
        private Parameters(DbParameterCollection parameters)
        {
            foreach(DbParameter item in parameters)
            {
                this.Add(new Parameter(item));
            }
        }

        public Parameters()
        {
        }
        #endregion

        #region Locais
        private List<Parameter> mDbParameterCollection = new List<Parameter>();
        #endregion

        #region Add
        /// <summary>
        /// use este método para adiciona um objeto Parameter, pelo nome real da coluna
        /// <para>O método irá procurar pelo nome real da coluna. Se não encontrar retornara false</para>
        /// </summary>
        /// <param name="parameter">objeto parameter</param>
        /// <param name="fromSourceColumn">se true faz a pesquisa pelo nome real da coluna
        /// <para>Se false simplesmente adiciona o Parameter. Neste caso se o mesmo já existir sera retornado um erro</para>
        /// 
        /// </param>
        public virtual void Add(Parameter parameter, bool fromSourceColumn)
        {
            if(fromSourceColumn == true)
            {
                int parameterIndex = GetParameterIndex(parameter.SourceColumn);
                if(parameterIndex > -1)
                {
                    this[parameterIndex] = parameter;
                    return;
                }
            }

            //se o parâmetro já existir não adicionar novamente
            if(mDbParameterCollection.Count(p => p.ParameterName == parameter.ParameterName) == 0)
            {
                mDbParameterCollection.Add(parameter);
            }
        }

        public override int Add(object parameter)
        {
            return this.Add((Parameter)parameter);
        }

        public virtual int Add(Parameter parameter)
        {
            Add(parameter, false);
            return mDbParameterCollection.Count;
        }

        public virtual Parameter Add(string sourceColumn)
        {
            Parameter p = new Parameter(sourceColumn);
            this.Add(p);
            return p;
        }

        public virtual Parameter Add(string parameterName, GenericDbType dbType)
        {
            Parameter p = new Parameter(parameterName, dbType);
            this.Add(p);
            return p;
        }

        public virtual Parameter Add(string parameterName, object value)
        {
            Parameter p = new Parameter(parameterName, value);
            this.Add(p);
            return p;
        }

        public virtual Parameter Add(string parameterName, GenericDbType dbType, object value)
        {
            Parameter p = new Parameter(parameterName, dbType, value);
            this.Add(p);
            return p;
        }

        public virtual Parameter Add(string parameterName, GenericDbType dbType, string sourceColumn)
        {
            Parameter p = new Parameter(parameterName, dbType, sourceColumn);
            this.Add(p);
            return p;
        }

        public virtual Parameter Add(string parameterName, string sourceColumn)
        {
            Parameter p = new Parameter(parameterName, GenericDbType.Unknown, sourceColumn);
            this.Add(p);
            return p;
        }

        public virtual Parameter Add(string parameterName, string sourceColumn, object value)
        {
            Parameter p = new Parameter(parameterName, GenericDbType.Unknown, sourceColumn);
            p.Value = value;
            this.Add(p);
            return p;
        }

        public virtual Parameter Add(string parameterName, GenericDbType dbType, string sourceColumn, object value)
        {
            Parameter p = new Parameter(parameterName, dbType, sourceColumn, value);
            this.Add(p);
            return p;
        }

        #region AddRange
        public override void AddRange(Array values)
        {
            foreach(Parameter item in values)
            {
                this.Add(item);
            }
        }

        public void AddRange(Parameters parameters)
        {
            foreach(Parameter item in parameters)
                this.Add(item);
        }

        public virtual void AddRange(Parameter[] parameters)
        {
            foreach(Parameter item in parameters)
            {
                this.Add(item);
            }
        }
        #endregion

        #endregion

        #region IDataParameterCollection Members
        public override int IndexOf(string parameterName)
        {
            int ret = -1;
            int i = 0;
            foreach(Parameter item in mDbParameterCollection)
            {
                //aqui a pesquisa é feita pelo nome do parâmetro ou pelo source column
                if(item.ParameterName.Equals(parameterName, StringComparison.InvariantCultureIgnoreCase) ||
                    item.SourceColumn.Equals(parameterName, StringComparison.InvariantCultureIgnoreCase)
                    )
                {
                    ret = i;
                    break;
                }
                i++;
            }

            return ret;
        }

        public override void RemoveAt(string parameterName)
        {
            this.Remove(new Parameter(parameterName));
        }

        object IDataParameterCollection.this[string parameterName]
        {
            get { return this.mDbParameterCollection[this.IndexOf(parameterName)]; }
            set { this.mDbParameterCollection[this.IndexOf(parameterName)] = (Parameter)value; }
        }

        #endregion

        #region IList Members

        int IList.Add(object value)
        {
            return this.Add((Parameter)value);
        }

        public override bool Contains(object value)
        {
            return this.mDbParameterCollection.Contains((Parameter)value);
        }

        public override int IndexOf(object value)
        {
            return this.mDbParameterCollection.IndexOf((Parameter)value);
        }

        public override void Insert(int index, object value)
        {
            this.Insert(index, (Parameter)value);
        }

        public override bool IsFixedSize
        {
            get { return false; }
        }

        public void Remove(string value)
        {
            this.Remove(this[value]);
        }

        public override void Remove(object value)
        {
            this.Remove((Parameter)value);
        }

        public override void RemoveAt(int index)
        {
            this.mDbParameterCollection.RemoveAt(index);
        }

        public new virtual Parameter this[int index]
        {
            get { return (Parameter)this.mDbParameterCollection[index]; }
            set { this.mDbParameterCollection[index] = value; }
        }

        public new virtual Parameter this[string name]
        {
            get
            {
                int index = IndexOf(name);

                if(index == -1)
                    throw new FieldDoesntExist(name, ToString());

                return this[index];
            }
            set
            {
                int index = IndexOf(name);

                if(index == -1)
                    throw new FieldDoesntExist(name, ToString());

                this[index] = value;
            }
        }

        #endregion

        #region ICollection Members

        public override void CopyTo(Array array, int index)
        {
            this.mDbParameterCollection.CopyTo((Parameter[])array, index);
        }

        public override bool IsSynchronized
        {
            get { return false; }
        }

        public override object SyncRoot
        {
            get { return null; }
        }

        #endregion

        #region IList<Parameter> Members

        public virtual int IndexOf(Parameter item)
        {
            return this.IndexOf(item.ParameterName);
        }

        public virtual void Insert(int index, Parameter item)
        {
            this.mDbParameterCollection.Insert(index, item);
        }

        #endregion

        #region ICollection<Parameter> Members
        public override void Clear()
        {
            this.mDbParameterCollection.Clear();
        }

        public virtual void CopyTo(Parameter[] array, int arrayIndex)
        {
            this.mDbParameterCollection.CopyTo(array, arrayIndex + 1);
        }

        public override int Count
        {
            get { return this.mDbParameterCollection.Count; }
        }

        public override bool IsReadOnly
        {
            get { return true; }
        }

        public virtual bool Remove(Parameter item)
        {
            this.mDbParameterCollection.Remove(item);
            return true;
        }

        #endregion

        #region IDataParameterCollection Members
        public override bool Contains(string parameterName)
        {
            return this.IndexOf(parameterName) > -1;
        }

        #endregion

        #region ICollection<Parameter> Members
        public virtual bool Contains(Parameter item)
        {
            return this.Contains(item.ParameterName);
        }
        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion

        #region IEnumerable<Parameter> Members

        /*public new virtual IEnumerator<Parameter> GetEnumerator()
        {
            return mDbParameterCollection.GetEnumerator();
        }*/

        public override System.Collections.IEnumerator GetEnumerator()
        {
            return this.mDbParameterCollection.GetEnumerator();
        }
        #endregion

        #region Protected
        protected override DbParameter GetParameter(string parameterName)
        {
            return this.GetParameter(GetOrdinal(parameterName));
        }

        public int GetOrdinal(string parameterName)
        {
            return IndexOf(parameterName);
        }

        /// <summary>
        /// use este método para achar o key do item pelo seu nome real
        /// </summary>
        /// <param name="sourceColumn">nome real da coluna 'Tabela.Coluna'</param>
        /// <returns>Key da coluna</returns>
        public int GetParameterIndex(string sourceColumn)
        {
            int ret = -1;
            int i = 0;
            foreach(Parameter item in mDbParameterCollection)
            {
                if(item.SourceColumn.Equals(sourceColumn, StringComparison.InvariantCultureIgnoreCase))
                {
                    ret = i;
                    break;
                }
                i++;
            }

            return ret;
        }

        protected override DbParameter GetParameter(int index)
        {
            return mDbParameterCollection[index];
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            this.SetParameter(GetOrdinal(parameterName), value);
        }

        protected override void SetParameter(int index, DbParameter value)
        {
            mDbParameterCollection[index].Value = value;
        }
        #endregion

        #region Métodos
        /// <summary>
        /// verifica se o nome do parametro informado existe.
        /// <para>retorna true caso o parametro exista</para>
        /// </summary>
        /// <param name="parameterName">Nome do parâmetro</param>
        /// <returns>retorna true caso o parametro exista</returns>
        public bool Exists(string parameterName)
        {
            return this.Contains(parameterName);
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return ToString(mDbParameterCollection);
        }

        internal static string ToString(IEnumerable parameters)
        {
            string template = "";
            template += "---------------------------\r\n";
            template += "Name: {0}\r\n";
            template += "Source: {1}\r\n";
            template += "Type: {2}\r\n";
            template += "Value: {3}\r\n";
            template += "length: {4}\r\n";
            template += "Position: {5}\r\n";
            template += "---------------------------";

            string ret = "";
            int i = 0;
            string value = null;

            foreach(DbParameter item in parameters)
            {
                try
                {
                    value = item.Value == null ? "" : item.Value.ToString();
                }
                catch
                {
                    value = "";
                }

                ret += string.Format(template, item.ParameterName,
                                               item.SourceColumn,
                                               item.DbType.ToString(),
                                               value,
                                               string.IsNullOrEmpty(value) ? 0 : value.Length,
                                               i++);
            }

            return ret;
        }
        #endregion

        #region ICloneable members
        public Parameters Clone()
        {
            Parameters result = new Parameters();

            foreach(var item in this)
            {
                result.Add(item);
            }

            return result;

        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
        #endregion

        #region  IEnumerable<Parameter> members
        IEnumerator<Parameter> IEnumerable<Parameter>.GetEnumerator()
        {
            return mDbParameterCollection.GetEnumerator();
        }
        #endregion
    }
}
