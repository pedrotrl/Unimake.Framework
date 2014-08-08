using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Data.Generic
{
    public class OrderBy
    {
        #region Locais
        /// <summary>
        /// Nome do campo para ordenação
        /// </summary>
        public string FieldName { get; private set; }
        #endregion

        #region Propriedades
        /// <summary>
        /// true para retornar os registros de forma ascendente. ou false
        /// </summary>
        public bool Ascending { get; set; }
        #endregion

        #region Construtores
        /// <summary>
        /// instancia o objeto Order
        /// </summary>
        /// <param name="fieldName">nome do campo para a ordenação desejada</param>
        public OrderBy(string fieldName)
        {
            Ascending = true;
            this.FieldName = fieldName;
        }

        /// <summary>
        /// instancia o objeto Order
        /// </summary>
        /// <param name="order">ordem desejada</param>
        /// <param name="ascending">se true é ascendente</param>
        public OrderBy(string order, bool ascending)
        {
            Ascending = ascending;
            this.FieldName = order;
        }
        #endregion

        #region Operadores
        public static implicit operator OrderBy(string order)
        {
            return new OrderBy(order);
        }

        public static implicit operator string(OrderBy order)
        {
            if(order == null)
                return "";
            return order.FieldName;
        }

        public static bool operator ==(OrderBy rhs, OrderBy lhs)
        {
            object r = (object)rhs;
            object l = (object)lhs;

            if(r == null && l == null)
                return true;

            if(r == null)
                return false;

            return rhs.Equals(lhs);
        }

        public static bool operator !=(OrderBy rhs, OrderBy lhs)
        {
            return !(rhs == lhs);
        }
        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            if(obj == null || (GetType() != obj.GetType() && !(obj is String)))
                return false;

            OrderBy result = null;

            // 
            // aqui não podemos usar o ToString diretamente pois o mesmo é sobrescrito e retorna o 
            // DESC caso seja ordenação descendente
            //
            if(obj is String)
                result = obj.ToString();
            else
                result = obj as OrderBy;

            return FieldName.Equals(result.FieldName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ FieldName.GetHashCode();
        }

        public override string ToString()
        {
            if(string.IsNullOrEmpty(FieldName))
                return "";

            if(Ascending)
                return FieldName;

            return FieldName + " DESC";
        }
        #endregion
    }
}
