﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AyxCsv
{
    public class AyxCsvReader
    {
        public bool HasHeaders { get; set; } = true;
        public char Separator { get; set; } = ',';
        private CsvLineReader lineReader;

        public AyxCsvReader(char separator = ',', bool haveHeader = true)
        {
            Separator = separator;
            HasHeaders = haveHeader;
            lineReader = new CsvLineReader(separator);
        }

        public DataTable ReadCsvFileDataTable(string filename, Encoding encoding = null)
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

                    if (HasHeaders)
                    {
                        var header = reader.ReadLine();
                        AddColumns(result, header);
                    }

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            continue;
                        AddRow(result, line);
                    }
                }
            }

            return result;
        }

        public IEnumerable<dynamic> ReadCsvFileDynamic(string filename, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Default;

            using (var fs = new FileStream(filename, FileMode.Open))
            {
                using (var reader = new StreamReader(fs, encoding))
                {
                    if (reader.EndOfStream)
                        yield break;

                    var headerLine = reader.ReadLine();
                    var headers = lineReader.ReadLine(headerLine);

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            continue;

                        yield return GetDynamic(line, headers);
                    }
                }
            }
        }

        public IEnumerable<T> ReadCsvFileGeneric<T>(string filename, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Default;

            if (typeof(T) == typeof(Object))
            {
                var list = ReadCsvFileDynamic(filename, encoding);
                foreach (dynamic item in list)
                {
                    yield return (T)(Object)item;
                }
            }
            else
            {
                using (var fs = new FileStream(filename, FileMode.Open))
                {
                    using (var reader = new StreamReader(fs, encoding))
                    {
                        if (reader.EndOfStream)
                            yield break;

                        var headerLine = reader.ReadLine();
                        var headers = lineReader.ReadLine(headerLine);
                        var mapping = GetPropertyMapping<T>(headers);

                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (string.IsNullOrEmpty(line))
                                continue;

                            yield return GetInstance<T>(line, mapping);
                        }
                    }
                }
            }
        }

        public IEnumerable<string[]> ReadCsvFileArray(string filename, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Default;

            using (var fs = new FileStream(filename, FileMode.Open))
            {
                using (var reader = new StreamReader(fs, encoding))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            continue;

                        yield return lineReader.ReadLine(line).ToArray();
                    }
                }
            }
        }

        public IEnumerable<Dictionary<string, string>> ReadCsvFileDict(string filename, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Default;

            using (var fs = new FileStream(filename, FileMode.Open))
            {
                using (var reader = new StreamReader(fs, encoding))
                {
                    if (reader.EndOfStream)
                        yield break;

                    var headerLine = reader.ReadLine();
                    var headers = lineReader.ReadLine(headerLine);

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            continue;

                        yield return GetDict(line, headers);
                    }
                }
            }
        }

        public Dictionary<string, Dictionary<string, string>> ReadCsvFileDoubleDict(string filename, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Default;

            using (var fs = new FileStream(filename, FileMode.Open))
            {
                using (var reader = new StreamReader(fs, encoding))
                {
                    var result = new Dictionary<string, Dictionary<string, string>>();
                    if (reader.EndOfStream)
                        return result;

                    var headerLine = reader.ReadLine();
                    var headers = lineReader.ReadLine(headerLine);

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            continue;

                        var kv = GetKV(line, headers);
                        result.Add(kv.Key, kv.Value);
                    }
                    return result;
                }
            }
        }

        public DataTable ReadCsvString(IEnumerable<string> lines)
        {
            var result = new DataTable();

            if (lines.Count() == 0)
                return result;

            if (HasHeaders)
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

        private dynamic GetDynamic(string line, List<string> headers)
        {
            var fields = lineReader.ReadLine(line);
            dynamic d = new ExpandoObject();
            var dict = d as IDictionary<string, object>;

            for (int i = 0; i < headers.Count; i++)
            {
                if (i >= fields.Count)
                    break;

                dict[headers[i]] = fields[i];
            }

            return d;
        }

        private Dictionary<string, string> GetDict(string line, List<string> headers)
        {
            var dict = new Dictionary<string, string>();
            var fields = lineReader.ReadLine(line);

            for (int i = 0; i < headers.Count; i++)
            {
                if (i >= fields.Count)
                    break;

                dict[headers[i]] = fields[i];
            }

            return dict;
        }

        private KeyValuePair<string, Dictionary<string, string>> GetKV(string line, List<string> headers)
        {
            var dict = new Dictionary<string, string>();
            var fields = lineReader.ReadLine(line);
            var key = fields[0];

            for (int i = 1; i < headers.Count; i++)
            {
                if (i >= fields.Count)
                    break;

                dict[headers[i]] = fields[i];
            }
            return new KeyValuePair<string, Dictionary<string, string>>(key, dict);
        }

        private Dictionary<PropertyInfo, int> GetPropertyMapping<T>(List<string> headers)
        {
            var result = new Dictionary<PropertyInfo, int>();
            var props = typeof(T).GetProperties()
                .Where(p => p.PropertyType.IsValueType || p.PropertyType == typeof(string))
                .Where(p => headers.Contains(p.Name))
                .ToList();

            foreach (var prop in props)
            {
                var index = headers.IndexOf(prop.Name);
                result.Add(prop, index);
            }

            return result;
        }

        private T GetInstance<T>(string line, Dictionary<PropertyInfo, int> mapping)
        {
            var fields = lineReader.ReadLine(line);
            var result = Activator.CreateInstance<T>();
            foreach (var item in mapping)
            {
                var v = ParseValue(item.Key.PropertyType, fields[item.Value]);
                item.Key.SetValue(result, v);
            }
            return result;
        }

        private object ParseValue(Type t, string value)
        {
            if (t == typeof(bool))
                return Convert.ToBoolean(value);
            if (t == typeof(byte))
                return Convert.ToByte(value);
            if (t == typeof(char))
                return Convert.ToChar(value);
            if (t == typeof(DateTime))
                return Convert.ToDateTime(value);
            if (t == typeof(decimal))
                return Convert.ToDecimal(value);
            if (t == typeof(double))
                return Convert.ToDouble(value);
            if (t == typeof(Int16))
                return Convert.ToInt16(value);
            if (t == typeof(Int32))
                return Convert.ToInt32(value);
            if (t == typeof(Int64))
                return Convert.ToInt64(value);
            if (t == typeof(sbyte))
                return Convert.ToSByte(value);
            if (t == typeof(float))
                return Convert.ToSingle(value);
            if (t == typeof(UInt16))
                return Convert.ToUInt16(value);
            if (t == typeof(UInt32))
                return Convert.ToUInt32(value);
            if (t == typeof(UInt64))
                return Convert.ToUInt64(value);
            return value;
        }
    }
}
