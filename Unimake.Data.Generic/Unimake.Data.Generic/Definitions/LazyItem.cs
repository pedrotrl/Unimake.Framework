using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Data.Generic.Definitions
{
    /// <summary>
    /// Item da coleção Lazy, utilizado para manter os dados de forma tardia
    /// </summary>
    /// <typeparam name="T">Tipo que será adicionado</typeparam>
    internal sealed class LazyItem<T>
    {
        #region propriedades
        /// <summary>
        /// Retorna true se o item já foi criado uma vez
        /// </summary>
        public bool IsValueCreated { get; private set; }

        /// <summary>
        /// Método que deverá ser chamado para retornar  o valor esperado
        /// </summary>
        public Func<T> Func { get; private set; }

        T value = default(T);
        /// <summary>
        /// Retorna o valor esperado 
        /// </summary>
        public T Value
        {
            get
            {
                if(IsValueCreated)
                    return value;

                value = Func.Invoke();
                IsValueCreated = true;
                return value;
            }
        }
        #endregion

        #region construtores
        /// <summary>
        /// Inicia um novo LazyItem e define o método que será invocado
        /// </summary>
        /// <param name="func">Método que será invocado ao chamar a propriedade Value</param>
        public LazyItem(Func<T> func)
        {
            IsValueCreated = false;
            Func = func;
        }
        #endregion
    }
}