using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unimake.Test.Exceptions;
using System.Configuration;
using System.Collections;
using System.Reflection;
using Unimake.Test.Base;

namespace Unimake.Test
{
    /// <summary>
    /// Classe de base para os testes. Implementa métodos que podem ser utilizados 
    /// durante os testes
    /// </summary>
    [TestClass]
    [System.Diagnostics.DebuggerStepThrough()]
    public abstract class BaseTest: IBaseTest
    {
        /// <summary>
        /// Cria um novo objeto
        /// </summary>
        public BaseTest()
        {
            Type derivedType = GetType();
            bool flag = false;
            foreach(object item in derivedType.GetCustomAttributes(false))
            {
                if(item is Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute)
                {
                    flag = true;
                    break;
                }
            }

            if(flag == false)
                throw new MustHaveTestClassAttribute(derivedType.Name);
        }

        /// <summary>
        /// Contexto atual do teste criado pelo VS
        /// </summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        /// <summary>
        /// Initialize the test
        /// </summary>
        [TestInitialize()]
        public virtual void BaseTestInitialize()
        {

        }

        /// <summary>
        /// Free the test resources
        /// </summary>
        [TestCleanup()]
        public virtual void BaseTestCleanup()
        {
            //
        }
        #endregion

        /// <summary>
        /// desenha uma linha tracejada com 230 traços
        /// </summary>
        public void DrawLine()
        {
            DrawLine(230);
        }

        /// <summary>
        /// desenha uma linha tracejada com o número de caracteres definidos em size
        /// </summary>
        /// <param name="size">número de caracteres para o tamanho da linha</param>
        public void DrawLine(int size)
        {
            TestContext.WriteLine("-".PadLeft(size, '-'));
        }

        /// <summary>
        /// Escreve uma linha formatada no contexto do teste
        /// </summary>
        /// <param name="format">formato a ser impresso no teste</param>
        /// <param name="args">argumentos</param>
        public void WriteLine(string format, params object[] args)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(format, args);

                if(args == null)
                    TestContext.WriteLine(format);
                else
                    TestContext.WriteLine(format, args);
            }
            catch(FormatException)
            {
                format = format
                    .Replace("{", "{0}")
                    .Replace("}", "{1}")
                    .Replace("{0{1}", "{0}");

                TestContext.WriteLine(format, "{", "}");
            }
        }

        /// <summary>
        /// Este método percorre as propriedades do objeto e escreve no contexto do teste seus valores, chamando o método ToString()
        /// </summary>
        /// <param name="element">Objeto que deverá ser lido</param>
        public void DumpElement(object element)
        {
            DumpElement(element, true);
        }

        /// <summary>
        /// Este método percorre as propriedades do objeto e escreve no contexto do teste seus valores, chamando o método ToString()
        /// </summary>
        /// <param name="element">Objeto que deverá ser lido</param>
        /// <param name="dumpIEnumerable">Se true, as propriedades do tipo IEnumerable serão escritas no contexto. Padrão true</param>
        public void DumpElement(object element, bool dumpIEnumerable)
        {
            string format = ElementToString(element, dumpIEnumerable);
            WriteLine(format);
        }

        /// <summary>
        /// Este método percorre as propriedades do objeto e retorna uma string com seus valores, chamando o método ToString()
        /// </summary>
        /// <param name="element">Objeto que deverá ser lido</param>
        /// <param name="dumpIEnumerable">Se true, as propriedades do tipo, IEnumerable serão escritas no contexto</param>
        public string ElementToString(object element, bool dumpIEnumerable)
        {
            StringBuilder sb = new StringBuilder();

            if(element != null)
            {
                Type t = element.GetType();
                bool isEnumerable = (element is IEnumerable &&
                            !(element is System.String)      //tipos System.String são IEnumerable e não entra nesta :)
                            );

                if(dumpIEnumerable && isEnumerable)
                {
                    try
                    {
                        sb.AppendLine("\t{0} Is IEnumerable: {1}"
                                        .Replace("{0}", t.Name)
                                        .Replace("{1}", "Count: " + ((IEnumerable<object>)element).Count()));

                        sb.AppendLine("Current values:");
                        sb.AppendLine("{");
                        sb.AppendLine(IEnumerableToString(element as IEnumerable));
                        sb.AppendLine("}");

                    }
                    catch(Exception ex)
                    {
                        sb.AppendLine("\t{0}: {1}"
                                .Replace("{0}", t.Name)
                                .Replace("{1}", "Exception raised: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message)));
                    }

                    return sb.ToString();
                }

                object result = null;
                sb.AppendLine(element.GetType().ToString() + " {");

                foreach(var item in t.GetProperties(BindingFlags.Instance | BindingFlags.Public).OrderBy(x => x.Name))
                {
                    try
                    {
                        result = item.GetValue(element, null);

                        //se a propriedade for uma propriedade indexada, apenas exibir a quantidade de itens que ela possui
                        isEnumerable = (result is IEnumerable &&
                            !(result is System.String)      //tipos System.String são IEnumerable e não entra nesta :)
                            );

                        if(result != null && isEnumerable)
                        {
                            int count = 0;

                            var pi = result.GetType().GetProperty("Count");

                            if(pi == null)
                                count = Convert.ToInt32(result.GetType().GetMethod("Count").Invoke(result, null));
                            else
                                count = Convert.ToInt32(pi.GetValue(result, null));

                            sb.AppendLine("\t{0} Is IEnumerable: {1}"
                                    .Replace("{0}", item.Name)
                                    .Replace("{1}", "Count: " + count));

                            if(dumpIEnumerable)
                            {
                                sb.AppendLine("Current values:");
                                sb.AppendLine("{");
                                sb.AppendLine(IEnumerableToString(result as IEnumerable));
                                sb.AppendLine("}");
                            }
                        }
                        else
                        {
                            sb.AppendLine("\t{0}: {1}"
                                    .Replace("{0}", item.Name)
                                    .Replace("{1}", result == null ? "[null value]" : result.ToString()));
                        }
                    }
                    catch(TargetParameterCountException)
                    {
                        //do nothing 
                    }
                    catch(Exception ex)
                    {
                        sb.AppendLine("\t{0}: {1}"
                                .Replace("{0}", item.Name)
                                .Replace("{1}", "Exception raised: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message)));
                    }
                }

                sb.AppendLine("}");
            }
            else
                sb.Append("Element is null");

            return sb.ToString();
        }

        /// <summary>
        /// Cria os itens enumerados e retorna uma string com os dados
        /// </summary>
        /// <param name="iEnumerable">tipo IEnumerable</param>
        /// <returns></returns>
        public string IEnumerableToString(IEnumerable iEnumerable)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            int i = 0;

            foreach(var item in iEnumerable)
            {
                sb.AppendFormat("// --------------- Item: {0} ---------------\r\n", ++i);
                sb.AppendFormat("\t{0}\r\n", ElementToString(item, false));
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}