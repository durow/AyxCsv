using System;
using AyxCsv;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AyxCsvTests
{
    [TestClass]
    public class CsvLineReaderTests
    {
        [TestMethod]
        public void SimpleReadTest()
        {
            var test = "A,BBB,CC,DD,EEE,FF,GGGGG";
            var reader = new CsvLineReader();
            var actual = reader.ReadLine(test);

            Assert.AreEqual("A", actual[0]);
            Assert.AreEqual("BBB", actual[1]);
            Assert.AreEqual("CC", actual[2]);
            Assert.AreEqual("DD", actual[3]);
            Assert.AreEqual("EEE", actual[4]);
            Assert.AreEqual("FF", actual[5]);
            Assert.AreEqual("GGGGG", actual[6]);
        }

        [TestMethod]
        public void SimpleReadTest2()
        {
            var test = ",,A,,\"B,B,B\",CC,\"D,,D\",EEE,";
            var reader = new CsvLineReader();
            var actual = reader.ReadLine(test);

            Assert.AreEqual("", actual[0]);
            Assert.AreEqual("", actual[1]);
            Assert.AreEqual("A", actual[2]);
            Assert.AreEqual("", actual[3]);
            Assert.AreEqual("B,B,B", actual[4]);
            Assert.AreEqual("CC", actual[5]);
            Assert.AreEqual("D,,D", actual[6]);
            Assert.AreEqual("EEE", actual[7]);
        }

        [TestMethod]
        public void SimpleReadTestFinal()
        {
            var test = "A,,,,BBB,\"C,C,C\",\"D\"\"D\",\"\"\"EEE\"\"\"";
            var reader = new CsvLineReader();
            var actual = reader.ReadLine(test);

            Assert.AreEqual("A", actual[0]);
            Assert.AreEqual("", actual[1]);
            Assert.AreEqual("", actual[2]);
            Assert.AreEqual("", actual[3]);
            Assert.AreEqual("BBB", actual[4]);
            Assert.AreEqual("C,C,C", actual[5]);
            Assert.AreEqual("D\"D", actual[6]);
            Assert.AreEqual("\"EEE\"", actual[7]);

            var writer = new CsvLineWriter();
            var line = writer.WriteLine(actual);
            Assert.AreEqual(test, line);
        }

        [TestMethod]
        public void SimpleReadTestDirty()
        {
            var test = "A\"A\"A,\"B\"B\"B\",\"\"CCC\"\"";
            var reader = new CsvLineReader();
            var actual = reader.ReadLine(test);

            Assert.AreEqual("A\"A\"A", actual[0]);
            Assert.AreEqual("B\"B\"B", actual[1]);
            Assert.AreEqual("\"CCC\"", actual[2]);
        }

        [TestMethod]
        public void SeparatorTest()
        {
            var test = "AA \"B B B\" C,C   DDD";
            var reader = new CsvLineReader(' ');
            var actual = reader.ReadLine(test);

            Assert.AreEqual("AA", actual[0]);
            Assert.AreEqual("B B B", actual[1]);
            Assert.AreEqual("C,C", actual[2]);
            Assert.AreEqual("", actual[3]);
            Assert.AreEqual("", actual[4]);
            Assert.AreEqual("DDD", actual[5]);
        }
    }
}
