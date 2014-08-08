using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unimake.Data.Generic.Test
{
    [TestClass]
    public class DataReaderTest
    {
        [TestMethod]
        public void ExecuteTestReader()
        {
            DataReader dr = DbContext.CreateConnection().ExecuteReader("SELECT * FROM cad_NCM");

            while(dr.Read())
            {
                System.Diagnostics.Debug.WriteLine(dr[0]);
            }
        }
    }
}
