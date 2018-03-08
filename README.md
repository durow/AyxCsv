# AyxCsv
a simple csv reader and writer

## How to use
``` C#
var reader = new AyxCsvReader();
var table = reader.ReadCsvFile("test.csv"); //read csv file as a DataTable

var writer = new AyxCsvWriter();
writer.WriteCsvFile("out.csv",table) //write DataTable to a csv file.
writer.WriteCsvFile<T>("out.csv",List<T> list) //write list to a csv file.
```
