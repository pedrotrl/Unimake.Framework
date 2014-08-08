using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Test.ETL
{
    public class NCMJustForTest
    {
        #region Propriedades
        public int ID { get; set; }
        /// <summary>
        /// Número do NCM.
        /// </summary>
        public string NCM { get; set; }
        /// <summary>
        /// Descrição do NCM.
        /// </summary>
        public string Descricao { get; set; }
        #endregion

        #region Método
        public int Save()
        {
            ID = new Random().Next();
            return ID;
        }
        #endregion

        #region overrrides
        public override string ToString()
        {

            return String.Format("ID: {0}\t\tNCM: {1}\t\tDescrição: {2}", ID, NCM, Descricao);
        }
        #endregion
    }
}
