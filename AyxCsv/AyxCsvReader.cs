using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace AyxCsv
{
    public class AyxCsvReader
    {
        public bool HaveHeader { get; set; } = true;
        public char Separator { get; set; } = ',';
        private CsvLineReader lineReader;

        public AyxCsvReader(char separator = ',', bool haveHeader = true)
        {
            Separator = separator;
            HaveHeader = haveHeader;
            lineReader = new CsvLineReader(separator);
        }

        public DataTable ReadCsvFile(string filename, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Default;

            var result = new DataTable();

            using (var fs = new FileStream(filename, FileMode.Open))
            {
                using (var reader = new StreamReader(fs, encoding))
                {
                    if (reader.EndOfStream)
                        return result;

                    if (HaveHeader)
                    {
                        var header = reader.ReadLine();
                        AddColumns(result, header);
                    }

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        AddRow(result, line);
                    }
                }
            }

            return result;
        }

        public DataTable ReadCsvString(IEnumerable<string> lines)
        {
            var result = new DataTable();

            if (lines.Count() == 0)
                return result;

            if (HaveHeader)
            {
                AddColumns(result, lines.First());
            }

            foreach (var line in lines)
            {
                AddRow(result, line);
            }

            return result;
        }

        private void AddColumns(DataTable table, string header)
        {
            var headers = lineReader.ReadLine(header);
            foreach (var item in headers)
            {
                table.Columns.Add(item);
            }
        }

        private void AddRow(DataTable table, string line)
        {
            var fields = lineReader.ReadLine(line);

            if (table.Columns.Count < fields.Count)
            {
                var gap = fields.Count - table.Columns.Count;
                for (int i = 0; i < gap; i++)
                {
                    table.Columns.Add();
                }
            }

            var row = table.NewRow();
            for (int i = 0; i < fields.Count; i++)
            {
                row[i] = fields[i];
            }
            table.Rows.Add(row);
        }
    }
}
