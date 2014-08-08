using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unimake.ETL.Source;
using Unimake.Data.Generic;

namespace Unimake.Test.ETL
{
    [TestClass]
    public class SqlSourceTest: BaseTest
    {
        [TestMethod]
        public void SelectSourceTest()
        {
            SqlSource source = new SqlSource()
                                   .Connection(DbContext.CreateConnection())
                                   .FromQuery("SELECT * FROM cad_NCM LIMIT 100");

            foreach(var item in source.Rows)
            {
                string line = "";

                foreach(var field in source.GetFieldNames())
                {
                    line += item[field] +  "".PadRight(15);
                }

                WriteLine(line);
            }
        }
    }
}
