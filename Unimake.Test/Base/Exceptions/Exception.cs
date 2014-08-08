using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Test.Exceptions
{
    /// <summary>
    /// Exceção lançada quando o atributo "TestMethod" não está definido para um método que deverá ser usado no teste
    /// </summary>
    public class MustHaveTestMethodAttribute: Exception
    {
        private string MethodName = "";

        /// <summary>
        /// Inicia a exceção com o nome do método que não possui o atributo
        /// </summary>
        /// <param name="methodName">Nome do método que não possui o atributo</param>
        public MustHaveTestMethodAttribute(string methodName)
        {
            MethodName = methodName;
        }

        /// <summary>
        /// Mensagem de erro.
        /// </summary>
        public override string Message
        {
            get
            {
                return MethodName + " Must have TestMethod attribute";
            }
        }
    }

    /// <summary>
    /// Exceção lançada quando uma classe herda de BaseTest mas não define o atributo "TestClass"
    /// </summary>
    public class MustHaveTestClassAttribute: Exception
    {
        private string ClassName = "";

        /// <summary>
        /// Inicializa uma instância desta classe com o nome da classe que está com problema.
        /// </summary>
        /// <param name="className">Nome da classe que está com erro</param>
        public MustHaveTestClassAttribute(string className)
        {
            ClassName = className;
        }

        /// <summary>
        /// Mensagem de erro.
        /// </summary>
        public override string Message
        {
            get
            {
                return ClassName + " Must have TestClass attribute";
            }
        }
    }
}
