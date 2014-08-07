using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unimake.Test.Base
{
    /// <summary>
    /// Formulário de base para testes de componentes e forms
    /// </summary>
    [TestClass]
    public partial class FormBaseTest: Form, IBaseTest
    {
        /// <summary>
        /// Inicia a classe de base de testes de forms
        /// </summary>
        public FormBaseTest()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Executa o teste definido para o formulário
        /// </summary>
        [TestMethod]
        public virtual void RunTest()
        {
        }

        #region IBaseTest members
        /// <summary>
        /// Classe usada para acessar os dados de teste da base 
        /// </summary>
        [TestClass]
        private class LocalTest: BaseTest
        {

        }

        LocalTest test = new LocalTest();

        /// <summary>
        /// desenha uma linha tracejada com 230 traços
        /// </summary>
        public void DrawLine()
        {
            test.DrawLine();
        }

        /// <summary>
        /// desenha uma linha tracejada com o número de caracteres definidos em size
        /// </summary>
        /// <param name="size">número de caracteres para o tamanho da linha</param>
        public void DrawLine(int size)
        {
            test.DrawLine(size);
        }

        /// <summary>
        /// Este método percorre as propriedades do objeto e escreve no contexto do teste seus valores, chamando o método ToString()
        /// </summary>
        /// <param name="element">Objeto que deverá ser lido</param>
        public void DumpElement(object element)
        {
            test.DumpElement(element);
        }

        /// <summary>
        /// Este método percorre as propriedades do objeto e escreve no contexto do teste seus valores, chamando o método ToString()
        /// </summary>
        /// <param name="element">Objeto que deverá ser lido</param>
        /// <param name="dumpIEnumerable">Se true, as propriedades do tipo IEnumerable serão escritas no contexto. Padrão true</param>
        public void DumpElement(object element, bool dumpIEnumerable)
        {
            test.DumpElement(element, dumpIEnumerable);
        }

        /// <summary>
        /// Este método percorre as propriedades do objeto e retorna uma string com seus valores, chamando o método ToString()
        /// </summary>
        /// <param name="element">Objeto que deverá ser lido</param>
        /// <param name="dumpIEnumerable">Se true, as propriedades do tipo, IEnumerable serão escritas no contexto</param>
        public string ElementToString(object element, bool dumpIEnumerable)
        {
            return test.ElementToString(element, dumpIEnumerable);
        }

        /// <summary>
        /// Cria os itens enumerados e retorna uma string com os dados
        /// </summary>
        /// <param name="iEnumerable">tipo IEnumerable</param>
        /// <returns></returns>
        public string IEnumerableToString(System.Collections.IEnumerable iEnumerable)
        {
            return test.IEnumerableToString(iEnumerable);
        }

        /// <summary>
        /// Escreve uma linha formatada no contexto do teste
        /// </summary>
        /// <param name="format">formato a ser impresso no teste</param>
        /// <param name="args">argumentos</param>
        public void WriteLine(string format, params object[] args)
        {
            test.WriteLine(format, args);
        }

        #region Additional test attributes
        /// <summary>
        /// Contexto atual do teste criado pelo VS
        /// </summary>
        public TestContext TestContext
        {
            get { return test.TestContext; }
            set { test.TestContext = value; }
        }

        /// <summary>
        /// Initialize the test
        /// </summary>
        [TestInitialize()]
        public virtual void BaseTestInitialize()
        {
            test.BaseTestInitialize();
        }

        /// <summary>
        /// Free the test resources
        /// </summary>
        [TestCleanup()]
        public virtual void BaseTestCleanup()
        {
            test.BaseTestCleanup();
        }
        #endregion
        #endregion
    }
}
