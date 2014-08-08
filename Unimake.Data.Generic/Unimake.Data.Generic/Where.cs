using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Unimake.Data.Generic.Definitions;

namespace Unimake.Data.Generic
{
    /// <summary>
    /// Filtros para os objetos da base de dados
    /// </summary>
    public sealed class Where: List<string>, IDisposable, ICloneable
    {
        #region Propriedades
        /// <summary>
        /// Parâmetros auxiliares deste where
        /// </summary>
        public Parameters Parameters { get; private set; }

        /// <summary>
        /// Limite de registros que será para trazer
        /// </summary>
        public Limit Limit { get; set; }
        #endregion

        #region Construtores
        public Where()
        {
            Parameters = new Parameters();
        }
        #endregion

        #region Add
        /// <summary>
        /// Adiciona um novo where a esta lista
        /// </summary>
        /// <param name="field">nome do campo</param>
        /// <param name="id">valor do campo</param>
        internal void Add(string field, IID id)
        {
            base.Add(String.Format("{0} = '{1}'", field, id));
        }

        /// <summary>
        /// Adiciona um novo where a esta lista
        /// </summary>
        /// <param name="where">Filtro que será utilizado no registro</param>
        /// <param name="parameter">Parâmetro que será utilizado neste where</param>
        public void Add(string where, Parameter parameter)
        {
            base.Add(where);
            Parameters.Add(parameter);
        }

        /// <summary>
        /// Adiciona um novo where a esta lista
        /// </summary>
        /// <param name="where">Filtro que será utilizado no registro</param>
        ///<param name="parameters">Lista de parâmetros que serão adicionados à coleção</param>
        public void Add(string where, IEnumerable<Parameter> parameters)
        {
            base.Add(where);
            Parameters.AddRange(parameters.ToArray());
        }

        /// <summary>
        /// Adiciona um novo where a esta lista
        /// </summary>
        /// <param name="field">nome do campo</param>
        /// <param name="value">valor do campo</param>
        public void Add(string field, string value)
        {
            base.Add(String.Format("{0} = '{1}'", field, value));
        }

        /// <summary>
        /// Adiciona um novo where a esta lista
        /// </summary>
        /// <param name="field">nome do campo</param>
        /// <param name="value">valor do campo</param>
        public void Add(string field, int value)
        {
            base.Add(String.Format("{0} = {1}", field, value));
        }

        /// <summary>
        /// Adiciona um novo where a esta lista
        /// </summary>
        /// <param name="field">nome do campo</param>
        /// <param name="value">valor do campo</param>
        public void Add(string field, double value)
        {
            base.Add(String.Format("{0} = '{1}'", field, value));
        }

        /// <summary>
        /// Adiciona um novo where a esta lista
        /// <para>Irá assumir 1: Verdadeiro e 0: Falso</para>
        /// </summary>
        /// <param name="field">nome do campo</param>
        /// <param name="value">valor do campo</param>
        public void Add(string field, bool value)
        {
            base.Add(String.Format("{0} = {1}", field, value ? 1 : 0));
        }

        /// <summary>
        /// Adiciona um novo where a esta lista
        /// </summary>
        /// <param name="field">nome do campo</param>
        /// <param name="dataInicial">primeira data</param>
        /// <param name="dataFinal">segunda data</param>
        public void Add(string field, DateTime dataInicial, DateTime dataFinal)
        {
            base.Add(String.Format("{0} BETWEEN '{1}' AND '{2}'", field, dataInicial, dataFinal));
        }

        /// <summary>
        /// Adiciona um novo where a esta lista
        /// </summary>
        /// <param name="field">nome do campo</param>
        /// <param name="vlrInicial">primeira valor</param>
        /// <param name="vlrFinal">segunda valor</param>
        public void Add(string field, int vlrInicial, int vlrFinal)
        {
            base.Add(String.Format("{0} BETWEEN '{1}' AND '{2}'", field, vlrInicial, vlrFinal));
        }

        /// <summary>
        /// Adiciona um novo where a esta lista
        /// </summary>
        /// <param name="field">nome do campo</param>
        /// <param name="vlrInicial">primeir valor</param>
        /// <param name="vlrFinal">segundo valor</param>
        public void Add(string field, double vlrInicial, double vlrFinal)
        {
            base.Add(String.Format("{0} BETWEEN '{1}' AND '{2}'", field, vlrInicial, vlrFinal));
        }

        /// <summary>
        /// Adiciona um novo where a esta lista
        /// </summary>
        /// <param name="field">nome do campo</param>
        /// <param name="data">data a ser procurada</param>
        public void Add(string field, DateTime data)
        {
            base.Add(String.Format("{0} = '{1}'", field, data));
        }
        #endregion

        #region override/ new
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string s = "";

            foreach(string item in this)
            {
                s = item.Trim();

                if(!s.EndsWith("or", StringComparison.InvariantCultureIgnoreCase) ||
                   !s.EndsWith("and", StringComparison.InvariantCultureIgnoreCase))
                {
                    sb.Append(s + " AND ");
                }
            }

            s = sb.ToString();

            if(s.EndsWith("or", StringComparison.InvariantCultureIgnoreCase))
                s = s.Substring(0, s.Length - 2);

            else if(s.EndsWith("and ", StringComparison.InvariantCultureIgnoreCase))
                s = s.Substring(0, s.Length - 4);

            return s;
        }
        #endregion

        #region IDisposable members
        ~Where()
        {
            Dispose(false);
        }

        void Dispose(bool disposing)
        {
            if(Parameters != null)
                Parameters.Clear();

            if(disposing)
                GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        #region ICloneable members
        public Where Clone()
        {
            Where result = new Where();
            result.Limit = new Limit(Limit.From, Limit.OffSet);

            #region where
            foreach(var item in this)
            {
                result.Add(item);
            }
            #endregion

            #region parameters
            foreach(Parameter item in Parameters)
            {
                result.Parameters.Add(new Parameter(item));
            }
            #endregion


            return result;
        }
        #endregion

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}