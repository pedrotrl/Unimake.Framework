using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.ETL.Transform
{
    /// <summary>
    /// Lista de erros que ocorreram ao executar a transfromação
    /// </summary>
    public class RowValidation
    {
        private IDictionary<string, string> _errors;

        /// <summary>
        /// Erros encontrados
        /// </summary>
        public IDictionary<string, string> Errors
        {
            get
            {
                if(_errors == null)
                    _errors = new Dictionary<string, string>();
                return _errors;
            }
        }

        /// <summary>
        /// Se true, ocorreram erros na transformação
        /// </summary>
        public bool HasErrors
        {
            get
            {
                return _errors != null && _errors.Count > 0;
            }
        }
    }
}