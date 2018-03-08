﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AyxCsv
{
    public class AyxCsvWriter
    {
        public char Separator { get; set; } = ',';
        public bool HaveHeaders { get; set; } = true;
        private CsvLineWriter lineWriter;

        public AyxCsvWriter(char separator = ',', bool haveHeaders = true)
        {
            Separator = separator;
            HaveHeaders = haveHeaders;
            lineWriter = new CsvLineWriter(separator);
        }

        public void WriteCsvFile(string filename, DataTable table, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Default;


            using (var fs = new FileStream(filename, FileMode.Create))
            {
                using (var writer = new StreamWriter(fs, encoding))
                {
                    if (HaveHeaders)
                    {
                        var headerLine = GetHeaderLine(table);
                        writer.WriteLine(headerLine);
                    }

                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        writer.WriteLine(GetContentLine(table.Rows[i]));
                    }
                }
            }
        }

        public void WriteCsvFile<T>(string filename, IEnumerable<T> list, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Default;

            var props = typeof(T).GetProperties()
                .Where(p => p.PropertyType.IsValueType || p.PropertyType == typeof(string))
                .ToList();
            using (var fs = new FileStream(filename, FileMode.Create))
            {
                using (var writer = new StreamWriter(fs, encoding))
                {
                    if (HaveHeaders)
                    {
                        var headerLine = GetHeaderLine(props);
                        writer.WriteLine(headerLine);
                    }

                    foreach (var item in list)
                    {
                        writer.WriteLine(GetContentLine(item, props));
                    }
                }
            }
        }

        private string GetHeaderLine(DataTable table)
        {
            var list = new List<string>();

            for (int i = 0; i < table.Columns.Count; i++)
            {
                list.Add(table.Columns[i].ColumnName);
            }

            return lineWriter.WriteLine(list);
        }

        private string GetHeaderLine(List<PropertyInfo> propList)
        {
            var list = new List<string>();

            for (int i = 0; i < propList.Count; i++)
            {
                list.Add(propList[i].Name);
            }

            return lineWriter.WriteLine(list);
        }

        private string GetContentLine(DataRow row)
        {
            var list = new List<string>();

            for (int i = 0; i < row.Table.Columns.Count; i++)
            {
                list.Add(row[i].ToString());
            }

            return lineWriter.WriteLine(list);
        }

        private string GetContentLine(object o, List<PropertyInfo> propList)
        {
            var list = new List<string>();

            for (int i = 0; i < propList.Count; i++)
            {
                list.Add(propList[i].GetValue(o).ToString());
            }

            return lineWriter.WriteLine(list);
        }
    }
}
