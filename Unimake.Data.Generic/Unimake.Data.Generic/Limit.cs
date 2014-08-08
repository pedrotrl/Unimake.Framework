using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Data.Generic
{
    public struct Limit
    {
        #region Operadores
        /// <summary>
        /// Este operador instancia inciando do registro um até o offset informado
        /// </summary>
        /// <param name="offset">quantidade de registros que deverá ser exibida</param>
        /// <returns></returns>
        public static implicit operator Limit(int from)
        {
            return new Limit(from, 0);
        }
        #endregion

        #region Propriedades
        private int _from;
        /// <summary>
        /// Posição inicial do registro
        /// </summary>
        public int From
        {
            get { return _from; }
        }

        private int _offSet;
        /// <summary>
        /// Posição em que deverá ser iniciada a busca dos registros.
        /// </summary>
        public int OffSet { get { return _offSet; } }
        #endregion

        #region Construtores
        /// <summary>
        /// Instancia o objeto 
        /// </summary>
        /// <param name="from">posição inicial dos registros</param>
        public Limit(int from)
        {
            if(from < -1)
                throw new ArgumentOutOfRangeException("from", "Valor inválido para o argumento from");

            _from = from;
            _offSet = -1;
        }

        /// <summary>
        /// Instancia o objeto 
        /// </summary>
        /// <param name="from">posição inicial dos registros</param>
        public Limit(int from, int offset)
        {
            if(from < -1)
                throw new ArgumentOutOfRangeException("from", "Valor inválido para o argumento from");

            if(offset <= -1)
                throw new ArgumentOutOfRangeException("offset", "Valor inválido para o argumento offset");

            _from = from;
            _offSet = offset;
        }
        #endregion

        #region overrides/ new
        public override string ToString()
        {
            if(From == 0 && OffSet == 0) return "";

            string result = "";
            //-------------------------------------------------------------------------
            // É obrigatório informar o from, caso contrário, não tem offset
            //-------------------------------------------------------------------------
            if(From > -1)
            {
                string format = "\r\nLIMIT {0}";

                if(OffSet > -1)
                {

                    switch(Configuration.DataGenericSettings.Settings.DatabaseType)
                    {
                        case DatabaseType.PostgreSQL:

                            format += " OFFSET {1}";
                            break;
                        case DatabaseType.SQLite:
                        default:
                            format = "\r\nLIMIT {1}, {0}";
                            break;
                    }
                }

                format += "\r\n";

                result = format.Contains("{1}") ?
                    String.Format(format, From, OffSet) :
                    String.Format(format, From);
            }

            return result;
        }
        #endregion
    }
}
