using AyxCsv;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyxCsvTests
{
    [TestClass]
    public class AyxCsvReaderTests
    {
        [TestMethod]
        public void ReadCsvTest()
        {
            var reader = new AyxCsvReader();
            var dt = reader.ReadCsvFile("test.csv");

            Assert.IsTrue(dt.Rows.Count > 0);
        }

        [TestMethod]
        public void ReadCsvTestAdvanced()
        {
            var reader = new AyxCsvReader();
            var dt = reader.ReadCsvFile("adv.csv");
            Assert.IsTrue(dt.Rows.Count > 0);

            var writer = new AyxCsvWriter();
            writer.WriteCsvFile("adv_out.csv", dt);
        }
    }
}
