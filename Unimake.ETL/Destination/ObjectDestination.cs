using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unimake.ETL.Transform;
using Unimake.ETL.Enuns;
using System.Reflection;

namespace Unimake.ETL.Destination
{
    /// <summary>
    /// Utilize este destino quando precisar converter para um tipo object
    /// </summary>
    /// <typeparam name="T">Define o tipo de objeto que deverá ser instanciado</typeparam>
    public class ObjectDestination<T>: DestinationBase<ObjectDestination<T>>
        where T: class, new()
    {
        #region locais
        /// <summary>
        /// Objeto que é criado e retornado por este destino
        /// </summary>
        public virtual T ObjectResult { get; private set; }
        #endregion

        #region Propriedades
        /// <summary>
        /// Processa o objeto de destino
        /// </summary>
        protected virtual new Action<T, IRow> DoProcess { get; set; }
        #endregion

        #region sobrecargas
        /// <summary>
        /// Ação que será processada no destino
        /// </summary>
        /// <param name="action">ação que será executada para este método</param>
        public virtual ObjectDestination<T> ProcessWithAction(Action<T, IRow> action)
        {
            DoProcess = action;
            return this;
        }
        #endregion

        #region IWithActionDestination<ObjectDestination> members
        /// <summary>
        /// Ação que será processada no destino
        /// </summary>
        /// <param name="action">ação que será executada para este método</param>
        public override ObjectDestination<T> ProcessWithAction(Action<IRow> action)
        {
            base.DoProcess = action;
            return this;
        }

        /// <summary>
        /// Executa o processo de inserção no objeto de destino
        /// </summary>
        /// <param name="action">ação que será executada para este método</param>
        public override ObjectDestination<T> InsertWithAction(Action<IRow> action)
        {
            DoInsert = action;
            return this;
        }

        /// <summary>
        /// Executa o processo de atualização no objeto de destino
        /// </summary>
        /// <param name="action">ação que será executada para este método</param>
        public override ObjectDestination<T> UpdateWithAction(Action<IRow> action)
        {
            DoUpdate = action;
            return this;
        }

        /// <summary>
        /// Executa o processo de exclusão no objeto de destino
        /// </summary>
        /// <param name="action">ação que será executada para este método</param>
        public override ObjectDestination<T> DeleteWithAction(Action<IRow> action)
        {
            DoDelete = action;
            return this;
        }
        #endregion

        #region overrides
        //-------------------------------------------------------------------------
        // A princípio iremos tratar apenas o "Process", 
        // pois aqui iremos tentar popular os dados do objeto
        //-------------------------------------------------------------------------

        /// <summary>
        /// Processa o destino
        /// </summary>
        /// <param name="row">Linha que deverá ser processada por este destino</param>
        public override void Process(IRow row)
        {
            this.DoProcess(ObjectResult, row);
        }

        /// <summary>
        /// Indica que está no contexto de transformação do objeto
        /// </summary>
        /// <param name="context">Contexto de transformação</param>
        /// <param name="action">Ação que será executada para a transformação</param>
        protected override void InTransformContext(Unimake.ETL.ITransform context, System.Action action)
        {
            ObjectResult = new T();

            foreach(var inputRow in context.Source.Rows)
            {
                RowValidation rowValidation = new RowValidation();
                context.DoRowValidation(inputRow, rowValidation);
                if(rowValidation.HasErrors)
                {
                    context.RaiseRowInvalid(inputRow, rowValidation);
                }
                else
                {
                    RowOperation rowOp = context.GetRowOperation(inputRow);
                    if(rowOp != RowOperation.Ignore)
                    {
                        DictionaryRow transformedRow = new DictionaryRow();
                        foreach(var mapping in context.TransformMap)
                        {
                            object rowValue = inputRow[mapping.Key];
                            Func<object, object> transformFunc;
                            if(context.TransformFuncs.TryGetValue(mapping.Key, out transformFunc))
                                rowValue = transformFunc(rowValue);
                            transformedRow[mapping.Value] = rowValue;

                            //aqui temos que preencher o objeto com suas propriedades
                            SetValue(mapping, rowValue);
                        }

                        context.ProcessTransformedRow(rowOp, transformedRow);
                    }
                }
            }
        }

        private void SetValue(KeyValuePair<string, string> mapping, object rowValue)
        {
            SetValue(String.IsNullOrEmpty(mapping.Value) ? mapping.Key : mapping.Value, rowValue);
        }

        private void SetValue(string propertyName, object rowValue)
        {
            Type t = ObjectResult.GetType();
            PropertyInfo pi = t.GetProperty(propertyName, BindingFlags.Public |
                                                          BindingFlags.Instance |
                                                          BindingFlags.IgnoreCase);

            if(pi != null && pi.CanWrite)
                pi.SetValue(ObjectResult, rowValue, null);
        }
        #endregion
    }
}
