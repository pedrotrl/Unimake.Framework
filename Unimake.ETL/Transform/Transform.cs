using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unimake.ETL.Enuns;

namespace Unimake.ETL.Transform
{
    /// <summary>
    /// Classe responsável pela transformação dos dados que seão salvos
    /// </summary>
    public class Transform: ITransform
    {
        #region propriedades
        /// <summary>
        /// Define o mapeamento que foi utilizado para este transformação. Ex: de-> para
        /// </summary>
        public virtual Dictionary<string, string> TransformMap { get; protected set; }
        /// <summary>
        /// Objeto de origem que foi usado para esta transformação
        /// </summary>
        public virtual ISource Source { get; protected set; }
        /// <summary>
        /// Funções utilizadas para a transformação do valor
        /// </summary>
        public Dictionary<string, Func<object, object>> TransformFuncs { get; private set; }
        #endregion

        #region locais
        private IDestination _dest;
        #endregion

        #region ações
        /// <summary>
        /// Valida a linha do objeto
        /// </summary>
        public Action<IRow, RowValidation> DoRowValidation { get; set; }

        /// <summary>
        /// Retorna a operação que deverá ser executada por esta linha
        /// </summary>
        public Func<IRow, RowOperation> GetRowOperation { get; set; }
        #endregion

        #region eventos
        /// <summary>
        /// Lançado quando inicia a transformação do objeto
        /// </summary>
        protected event Action<Transform> Start;

        /// <summary>
        /// Lançado quando acontecer erro na transformação do objeto
        /// </summary>
        protected event Action<Transform, Exception> Error;

        /// <summary>
        /// Lançado quando a transformação do objeto foi completada
        /// </summary>
        protected event Action<Transform> Complete;

        /// <summary>
        /// Lançado quando a linha para transformação é inválida
        /// </summary>
        public event Action<IRow, RowValidation> RowInvalid;

        /// <summary>
        /// Lançado anates da transformação da linha
        /// </summary>
        protected event Action<IRow, RowOperation> RowBeforeProcess;

        /// <summary>
        /// Lançado quando acontece um erro ao transformar a linha
        /// </summary>
        protected event Action<IRow, RowOperation, Exception> RowError;

        /// <summary>
        /// Lançado quando a transformação da linha aconteceu com sucesso
        /// </summary>
        protected event Action<IRow, RowOperation> RowSuccess;

        /// <summary>
        /// Lançado depois que ocorreu a transformação da linha
        /// </summary>
        protected event Action<IRow, RowOperation> RowAfterProcess;
        #endregion

        #region construtores
        /// <summary>
        /// Instancia um novo objeto do tipo Transformação
        /// </summary>
        /// <param name="source">Origem do registro</param>
        /// <param name="destination">O objeto source será transformado neste destino</param>
        public Transform(ISource source, IDestination destination)
        {
            Source = source;
            _dest = destination;
            TransformMap = new Dictionary<string, string>();
            TransformFuncs = new Dictionary<string, Func<object, object>>();
            this.DoRowValidation = (row, v) => { };
            this.GetRowOperation = row => RowOperation.Process;
        }
        #endregion

        #region mapeamento
        /// <summary>
        /// Tenta mapear o objeto de destino com o objeto de destino
        /// </summary>
        public Transform AutoMap()
        {
            AutoMap(x =>
            {
                if(x != null)
                    return new DestinationMapping() { Field = x };
                return null;
            });
            return this;
        }

        /// <summary>
        /// Tenta mapear o objeto de destino com o objeto de destino
        /// </summary>
        /// <param name="getMapping">Ação responsável por mapear o objeto</param>
        /// <returns></returns>
        public Transform AutoMap(Func<string, string> getMapping)
        {
            AutoMap(x =>
            {
                if(x != null)
                    return new DestinationMapping() { Field = getMapping(x) };
                return null;
            });
            return this;
        }

        /// <summary>
        /// Tenta mapear o objeto de destino com o objeto de destino
        /// </summary>
        /// <param name="getMapping">Ação responsável por mapear o objeto</param>
        /// <returns></returns>
        public Transform AutoMap(Func<string, DestinationMapping> getMapping)
        {
            IFieldNames sourceFieldNamesInfo = Source as IFieldNames;
            IFieldNames destinationFieldNamesInfo = _dest as IFieldNames;
            if(sourceFieldNamesInfo == null)
                throw new InvalidOperationException("Cannot automap source.");
            else
            {
                var sourceFields = sourceFieldNamesInfo.GetFieldNames();
                var srcAndDestMappings = from f in sourceFields
                                         let m = getMapping(f)
                                         where m != null
                                         select new KeyValuePair<string, DestinationMapping>(f, m);
                if(destinationFieldNamesInfo != null)
                {
                    // Find fields with match in destination
                    var destinationFields = destinationFieldNamesInfo.GetFieldNames();
                    var matchedMappings = from srcAndMap in srcAndDestMappings
                                          join f2 in destinationFieldNamesInfo.GetFieldNames() on srcAndMap.Value.Field equals f2
                                          select srcAndMap;
                    foreach(var srcAndMap in matchedMappings)
                    {
                        Map(srcAndMap.Key, srcAndMap.Value.Field, srcAndMap.Value.Transform);
                    }
                }
                else
                {
                    // Assume all fields have destinations
                    // i.e. match blindly, may cause errors depending on destination
                    foreach(var srcAndMap in srcAndDestMappings)
                    {
                        Map(srcAndMap.Key, srcAndMap.Value.Field, srcAndMap.Value.Transform);
                    }
                }
                return this;
            }
        }

        /// <summary>
        /// Mapeia o objeto de origem e assume que  o de destino será igual
        /// </summary>
        /// <param name="sourceField">Nome do campo mapeado</param>
        /// <returns></returns>
        public Transform Map(string sourceField)
        {
            TransformMap[sourceField] = sourceField;
            if(TransformFuncs.ContainsKey(sourceField))
                TransformFuncs.Remove(sourceField);
            return this;
        }

        /// <summary>
        /// Mapeia o objeto de origem e assume que  o de destino será igual
        /// </summary>
        /// <param name="sourceField">Nome do campo mapeado</param>
        /// <param name="destinationField">Nome do campo de destino para o mapeamento</param>
        /// <returns></returns>
        public Transform Map(string sourceField, string destinationField)
        {
            TransformMap[sourceField] = destinationField;
            if(TransformFuncs.ContainsKey(sourceField))
                TransformFuncs.Remove(sourceField);
            return this;
        }

        /// <summary>
        /// Mapeia o objeto de origem e assume que  o de destino será igual
        /// </summary>
        /// <param name="sourceField">Nome do campo mapeado de origem</param>
        /// <param name="destinationField">Nome do campo de destino para o mapeamento</param>
        /// <param name="transformFunc">Função que irá transformar o campo</param>
        /// <returns></returns>
        public Transform Map(string sourceField, string destinationField, Func<object, object> transformFunc)
        {
            TransformMap[sourceField] = destinationField;
            if(transformFunc != null)
                TransformFuncs[sourceField] = transformFunc;
            return this;
        }

        /// <summary>
        /// Mapeia o campo de origem para o campo de destino
        /// </summary>
        /// <param name="sourceField">Nome do campo mapeado de origem</param>
        /// <param name="destinationField">Nome do campo de destino para o mapeamento</param>
        /// <param name="transformFunc">Função que irá transformar o campo</param>
        /// <typeparam name="T">Tipo de objeto que deverá ser transformado</typeparam>
        /// <returns></returns>
        public Transform Map<T>(string sourceField, string destinationField, Func<T, T> transformFunc)
        {
            TransformMap[sourceField] = destinationField;
            if(transformFunc != null)
                TransformFuncs[sourceField] = (x => transformFunc((T)x));
            return this;
        }

        /// <summary>
        /// Mapeia o campo de origem para o campo de destino
        /// </summary>
        /// <typeparam name="TOriginal">Tipo do campo origem</typeparam>
        /// <typeparam name="TTransformed">Tipo do campo para ser transformado</typeparam>
        /// <param name="sourceField">Nome do campo de origem</param>
        /// <param name="destinationField">Nome do campo de destino</param>
        /// <param name="transformFunc">Função de transformação dos campos</param>
        /// <returns></returns>
        public Transform Map<TOriginal, TTransformed>(string sourceField,
            string destinationField, Func<TOriginal, TTransformed> transformFunc)
        {
            TransformMap[sourceField] = destinationField;
            if(transformFunc != null)
                TransformFuncs[sourceField] = (x => transformFunc((TOriginal)x));
            return this;
        }

        /// <summary>
        /// Remove o mapeamento existente entre origem e destino
        /// </summary>
        /// <param name="sourceField"></param>
        /// <returns></returns>
        public Transform Unmap(string sourceField)
        {
            if(TransformMap.ContainsKey(sourceField))
                TransformMap.Remove(sourceField);
            if(TransformFuncs.ContainsKey(sourceField))
                TransformFuncs.Remove(sourceField);
            return this;
        }
        #endregion

        /// <summary>
        /// Valida se a linha transformada atende aos requisitos
        /// </summary>
        /// <param name="action">Ação que será executada para a validação dos campos</param>
        /// <returns></returns>
        public Transform Validate(Action<IRow, RowValidation> action)
        {
            this.DoRowValidation = action;
            return this;
        }

        /// <summary>
        /// Determina a operação que deverá ser realizada entre a transformação
        /// </summary>
        /// <param name="func">Função que retorna a operação que deverá ser executada</param>
        /// <returns></returns>
        public Transform DetermineOperation(Func<IRow, RowOperation> func)
        {
            this.GetRowOperation = func;
            return this;
        }

        #region eventos
        /// <summary>
        /// Lançado quando inicia a transformação do objeto
        /// </summary>
        public Transform OnStart(Action<Transform> action)
        {
            this.Start += action;
            return this;
        }

        /// <summary>
        /// Lançado quando acontecer erro na transformação do objeto
        /// </summary>
        public Transform OnError(Action<Transform, Exception> action)
        {
            this.Error += action;
            return this;
        }

        /// <summary>
        /// Lançado quando a transformação do objeto foi completada
        /// </summary>
        public Transform OnComplete(Action<Transform> action)
        {
            this.Complete += action;
            return this;
        }

        /// <summary>
        /// Lançado quando a linha para transformação é inválida
        /// </summary>
        public Transform OnRowInvalid(Action<IRow, RowValidation> action)
        {
            this.RowInvalid += action;
            return this;
        }

        /// <summary>
        /// Lançado antes da transformação da linha
        /// </summary>
        public Transform OnRowBeforeProcess(Action<IRow, RowOperation> action)
        {
            this.RowBeforeProcess += action;
            return this;
        }

        /// <summary>
        /// Lançado depois que ocorreu a transformação da linha
        /// </summary>
        public Transform OnRowAfterProcess(Action<IRow, RowOperation> action)
        {
            this.RowAfterProcess += action;
            return this;
        }

        /// <summary>
        /// Lançado quando acontece um erro ao transformar a linha
        /// </summary>
        public Transform OnRowError(Action<IRow, RowOperation, Exception> action)
        {
            this.RowError += action;
            return this;
        }

        /// <summary>
        /// Lançado quando a transformação da linha aconteceu com sucesso
        /// </summary>
        public Transform OnRowSuccess(Action<IRow, RowOperation> action)
        {
            this.RowSuccess += action;
            return this;
        }
        #endregion

        /// <summary>
        /// Executa o processo de transformação do objeto e retorna o resultado transformado
        /// </summary>
        /// <returns></returns>
        public Transform Execute()
        {
            if(TransformMap.Count == 0)
                throw new InvalidOperationException("No mappings defined.");

            if(this.Start != null)
                this.Start(this);

            try
            {
                Source.InTransformContext(this, () =>
                {
                    _dest.InTransformContext(this, () =>
                    {
                        foreach(var inputRow in Source.Rows)
                        {
                            RowValidation rowValidation = new RowValidation();
                            DoRowValidation(inputRow, rowValidation);
                            if(rowValidation.HasErrors)
                            {
                                if(this.RowInvalid != null)
                                    this.RowInvalid(inputRow, rowValidation);
                            }
                            else
                            {
                                RowOperation rowOp = this.GetRowOperation(inputRow);
                                if(rowOp != RowOperation.Ignore)
                                {
                                    DictionaryRow transformedRow = new DictionaryRow();
                                    foreach(var mapping in TransformMap)
                                    {
                                        object rowValue = inputRow[mapping.Key];
                                        Func<object, object> transformFunc;
                                        if(TransformFuncs.TryGetValue(mapping.Key, out transformFunc))
                                            rowValue = transformFunc(rowValue);
                                        transformedRow[mapping.Value] = rowValue;
                                    }

                                    ProcessTransformedRow(rowOp, transformedRow);
                                }
                            }
                        }
                    });
                });
            }
            catch(Exception exTransform)
            {
                if(this.Error != null)
                    this.Error(this, exTransform);
                else
                    throw exTransform;
            }

            if(this.Complete != null)
                this.Complete(this);

            return this;
        }

        /// <summary>
        /// Executa ação definida para o registro no objeto de destino
        /// </summary>
        /// <param name="rowOp">Operação que deverás er executada</param>
        /// <param name="transformedRow">Linha que foi transformada</param>
        public void ProcessTransformedRow(RowOperation rowOp, DictionaryRow transformedRow)
        {
            if(this.RowBeforeProcess != null)
                this.RowBeforeProcess(transformedRow, rowOp);

            try
            {
                switch(rowOp)
                {
                    case RowOperation.Process:
                        _dest.Process(transformedRow);
                        break;
                    case RowOperation.Insert:
                        _dest.Insert(transformedRow);
                        break;
                    case RowOperation.Update:
                        _dest.Update(transformedRow);
                        break;
                    case RowOperation.Delete:
                        _dest.Delete(transformedRow);
                        break;
                }

                if(this.RowSuccess != null && rowOp != RowOperation.Ignore)
                    this.RowSuccess(transformedRow, rowOp);
            }
            catch(Exception exRow)
            {
                if(this.RowError != null)
                    this.RowError(transformedRow, rowOp, exRow);
                else
                    throw exRow;
            }

            if(this.RowAfterProcess != null)
                this.RowAfterProcess(transformedRow, rowOp);
        }

        #region ITransform members
        IEnumerable<string> ITransform.GetMappedSourceFields()
        {
            return TransformMap.Keys;
        }

        IEnumerable<string> ITransform.GetMappedDestinationFields()
        {
            return TransformMap.Values;
        }

        object ITransform.Execute()
        {
            return this.Execute();
        }

        /// <summary>
        /// Executa o evento de linha inválida
        /// </summary>
        /// <param name="inputRow">linha atual, que causou a invalidação</param>
        /// <param name="rowValidation">Resultado da validação</param>
        void ITransform.RaiseRowInvalid(IRow inputRow, RowValidation rowValidation)
        {
            if(RowInvalid != null)
                RowInvalid(inputRow, rowValidation);
        }
        #endregion
    }
}
