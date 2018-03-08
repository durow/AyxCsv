using AyxCsv;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AyxCsvTests.AyxCsvWriterTests;

namespace AyxCsvTests
{
    [TestClass]
    public class AyxCsvReaderTests
    {
        [TestMethod]
        public void ReadCsvTest()
        {
            var reader = new AyxCsvReader();
            var dt = reader.ReadCsvFileDataTable("test.csv");

            Assert.IsTrue(dt.Rows.Count > 0);
        }

        [TestMethod]
        public void ReadCsvTestAdvanced()
        {
            var reader = new AyxCsvReader();
            var dt = reader.ReadCsvFileDataTable("adv.csv");
            Assert.IsTrue(dt.Rows.Count > 0);

            var writer = new AyxCsvWriter();
            writer.WriteCsvFile("adv_out.csv", dt);
        }

        [TestMethod]
        public void ReadCsvFileDynamicTest()
        {
            var reader = new AyxCsvReader();
            var list = reader.ReadCsvFileDynamic("adv.csv").ToList();

            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void ReadCsvFileGenericTest()
        {
            var reader = new AyxCsvReader();
            var list = reader.ReadCsvFileGeneric<TestData>("adv.csv").ToList();

            Assert.IsTrue(list.Count > 0);
        }
    }
}
