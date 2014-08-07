using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Unimake.Test.Base
{
    interface IBaseTest
    {
        void DrawLine();
        void DrawLine(int size);
        void DumpElement(object element);
        void DumpElement(object element, bool dumpIEnumerable);
        string ElementToString(object element, bool dumpIEnumerable);
        string IEnumerableToString(System.Collections.IEnumerable iEnumerable);
        void WriteLine(string format, params object[] args);

        #region Additional test attributes
        Microsoft.VisualStudio.TestTools.UnitTesting.TestContext TestContext { get; set; }

        [TestInitialize()]
        void BaseTestInitialize();

        [TestCleanup()]
        void BaseTestCleanup();
        #endregion
    }
}
