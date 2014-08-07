using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Unimake.Test.Exceptions;

namespace Unimake.Test
{
    /// <summary>
    /// Classe de base para testes de CRUD em tabelas.
    /// </summary>
    [TestClass]
    public abstract class TableTest: BaseTest
    {
        #region Propriedades
        /// <summary>
        /// Código retornado pelo método gravar
        /// </summary>
        public object PkIndex { get; set; }
        #endregion

        #region Construtores
        /// <summary>
        /// este construtor vai forçar que a classe derivada tenha os atributos necessários
        /// tanto na classe quanto nos métodos
        /// </summary>
        public TableTest()
            : base()
        {
            if(HasAttributes("RunCRUDTest") == false)
                throw new MustHaveTestMethodAttribute("RunCRUDTest");
        }
        #endregion

        #region Validação
        private bool HasAttributes(string methodName)
        {
            bool flag = false;

            Type derivedType = GetType();
            MethodInfo method = derivedType.GetMethod(methodName);

            foreach(object item in method.GetCustomAttributes(false))
            {
                if(item is Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute)
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }

        #endregion

        #region Abstratos
        /// <summary>
        /// Executa o teste unitário CRUD padrão
        /// </summary>
        [TestMethod]
        public abstract void RunCRUDTest();

        /// <summary>
        /// Gravação (Crud)
        /// </summary>
        public abstract void Gravar();

        /// <summary>
        /// Leitura (cRud)
        /// </summary>
        public abstract void Popular();

        /// <summary>
        /// Atualização (crUd)
        /// </summary>
        public abstract void Editar();

        /// <summary>
        /// Exclusão (cruD)
        /// </summary>
        public abstract void Excluir();
        #endregion
    }
}