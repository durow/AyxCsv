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
    public class CsvLineWriterTests
    {
        [TestMethod]
        public void WriteTest()
        {
            var fields = new List<string>
            {
                "AAA",
                "BBB",
                "CC",
                "DDDD"
            };
            var writer = new CsvLineWriter();
            var actual = writer.WriteLine(fields);

            Assert.AreEqual("AAA,BBB,CC,DDDD", actual);
        }

        [TestMethod]
        public void WriteTest2()
        {
            var fields = new List<string>
            {
                "AAA",
                "B,B,B",
                "\"C\"C\"",
                "D,D\"D,D"
            };
            var writer = new CsvLineWriter();
            var actual = writer.WriteLine(fields);

            Assert.AreEqual("AAA,\"B,B,B\",\"\"\"C\"\"C\"\"\",\"D,D\"\"D,D\"", actual);
        }

        [TestMethod]
        public void WriteTestFinal()
        {
            var fields = new List<string>
            {
                "AAA",
                "",
                "",
                "B,B,B",
                "\"C\"C\"",
                "D,D\"D,D"
            };
            var writer = new CsvLineWriter();
            var actual = writer.WriteLine(fields);
            Assert.AreEqual("AAA,,,\"B,B,B\",\"\"\"C\"\"C\"\"\",\"D,D\"\"D,D\"", actual);

            var reader = new CsvLineReader();
            var readFields = reader.ReadLine(actual);
            for (int i = 0; i < fields.Count; i++)
            {
                Assert.AreEqual(fields[i], readFields[i]);
            }
        }
    }
}
