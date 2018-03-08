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
    public class AyxCsvWriterTests
    {
        [TestMethod]
        public void WriteCsvTest()
        {
            var reader = new AyxCsvReader();
            var dt = reader.ReadCsvFile("test.csv");
            Assert.IsTrue(dt.Rows.Count > 0);

            var writer = new AyxCsvWriter();
            writer.WriteCsvFile("result.csv", dt);
        }

        [TestMethod]
        public void WriteCsvTestGeneric()
        {
            var writer = new AyxCsvWriter();
            var data = GetTestData(100).ToList();
            writer.WriteCsvFile("adv.csv", data);
        }

        private IEnumerable<TestData> GetTestData(int count)
        {
            var rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < count; i++)
            {
                yield return new TestData
                {
                    IntCol = rand.Next(),
                    StrCol = "Street,City"+i,
                    DbCol = rand.NextDouble(),
                    FlCol = (float)rand.NextDouble(),
                    DeCol = (decimal)rand.NextDouble(),
                    DtCol = DateTime.Now,
                };
            }
        }

        class TestData
        {
            public int IntCol { get; set; }
            public string StrCol { get; set; }
            public double DbCol { get; set; }
            public float FlCol { get; set; }
            public decimal DeCol { get; set; }
            public DateTime DtCol { get; set; }
        }
    }
}
