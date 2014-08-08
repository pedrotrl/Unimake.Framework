using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unimake.ETL.Source;
using Unimake.ETL.Destination;
using Unimake.ETL.Transform;

namespace Unimake.Test.ETL
{
    [TestClass]
    public class ObjectDestinationTest: BaseTest
    {
        [TestMethod]
        public void NCMDestinationTest()
        {
            SqlSource source = new SqlSource()
                                   .FromQuery("SELECT GUID, EGUID, NCM, Descricao FROM cad_NCM LIMIT 100");

            ObjectDestination<NCMJustForTest> destination = new ObjectDestination<NCMJustForTest>()
                                                                .ProcessWithAction((s, r) =>
                                                                {
                                                                    s.Save();
                                                                    WriteLine(s.ToString());
                                                                });

            Transform transform = new Transform(source, destination)
                                      .Map("GUID", "GUID")
                                      .Map("EGUID", "EGUID")
                                      .Map("NCM", "NCM", (e) =>
                                      {
                                          return e;
                                      })
                                      .Map("Descricao", "Descricao")
                                      .Execute();
        }
    }
}
