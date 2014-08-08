using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Data.Generic.Definitions
{
    /// <summary>
    /// Classe de apoio para carregar as propriedades somente quando 
    /// forem utilizadas
    /// <typeparam name="T">Tipo de propriedade que deverá ser tratado para retornar mais tarde </typeparam>
    /// </summary>
    public sealed class LazyHelper: IDisposable
    {
        #region Locais
        /// <summary>
        /// Objetos que serão carregados posteriormente
        /// </summary>
        IDictionary<string, object> dic = new Dictionary<string, object>();
        #endregion

        #region get/set
        /// <summary>
        ///  Recupera o objeto e retorna
        /// </summary>
        /// <typeparam name="T">Tipo de objeto que deverá ser recuperado</typeparam>
        /// <param name="key">chave em que este objeto está </param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            if(Contains(key))
            {
                dynamic lazy = dic[key];
                return lazy.Value;
            }

            return default(T);
        }

        /// <summary>
        /// Seta a propriedade que deverá ser carregada posteriormente
        /// </summary>
        /// <typeparam name="T">Tipo de objeto esperado pela propriedade</typeparam>
        /// <param name="key">chave para recuperar o valor da propriedade</param>
        /// <param name="func">Item que deverá ser adicionado ao Lazy</param>
        public void Set<T>(string key, Func<T> func)
        {
            dic[key] = new LazyItem<T>(func);
        }

        /// <summary>
        ///Adiciona uma nova chamada ao tipo Lazy
        ///<para>É apenas uma alisa para o método Set&gt;T&lt;()</para>
        /// </summary>
        /// <typeparam name="T">Tipo de objeto que será adicionado</typeparam>
        /// <param name="key">chave do objeto adicionado</param>
        /// <param name="func">Expressão que deverá ser chamada ao utilizar o método Get&gt;T&lt;()</param>
        public void Add<T>(string key, Func<T> func)
        {
            Set<T>(key, func);
        }

        /// <summary>
        /// Retorna true se encontrou a chave informada
        /// </summary>
        /// <param name="key">Chave para pesquisa</param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return dic.Count(w => w.Key == key) > 0;
        }
        #endregion

        #region IDisposable members
        //------------------------------------------------------------------------
        // TODO: Não esqueça de implementar a interface IDisposable nesta classe
        //------------------------------------------------------------------------
        /// <summary>
        /// Retorna true se este objeto foi descarregado
        /// </summary>	
        public bool Disposed { get; private set; }

        ~LazyHelper()
        {
            Dispose(false);
        }

        void Dispose(bool disposing)
        {
            if(disposing)
            {
                dic.Clear();
                GC.SuppressFinalize(this);
            }

            Disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
