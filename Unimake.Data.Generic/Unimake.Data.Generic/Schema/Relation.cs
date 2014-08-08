using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Unimake.Data.Generic.Schema
{
    /// <summary>
    /// determina o tipo de delete para o relacionamento
    /// </summary>
    public enum DeleteAction
    {
        /// <summary>
        /// não fazer nada
        /// </summary>
        [Description("Não Fazer nada")]
        DoNothing,

        /// <summary>
        /// em cascata
        /// </summary>
        [Description("Cascata")]
        Cascade,

        /// <summary>
        /// Seta para Nulo
        /// </summary>
        [Description("Ajusta para nulo")]
        SetNull,

        /// <summary>
        /// Seta para padrão
        /// </summary>
        [Description("Ajusta para o padrão")]
        SetDefault,

        /// <summary>
        /// Seta para restrito
        /// </summary>
        [Description("Ajusta para restrito")]
        Restrict
    }

    public class Relation
    {
        #region Propriedades
        public string PrimaryTable { get; set; }
        public string PrimaryField { get; set; }
        public string ForeignTable { get; set; }
        public string ForeignField { get; set; }
        public DeleteAction DeleteAction { get; set; }
        #endregion

        #region Construtores
        public Relation() { }

        public Relation(string _primaryTable, string _primaryField,
            string _foreignTable, string _foreignField)
            : this()
        {
            PrimaryTable = _primaryTable;
            PrimaryField = _primaryField;
            ForeignTable = _foreignTable;
            ForeignField = _foreignField;
        }
        #endregion

        #region overrides

        #endregion
    }
}
