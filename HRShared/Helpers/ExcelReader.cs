using ClosedXML.Excel;
using Newtonsoft.Json;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HRShared.Helpers
{
    public interface IExcelReader
    {
        Task<DataTable> ReadExcelFile(Stream fileStream);
        string DataTableToJSONWithJSONNet(DataTable table);
        string SerializeDataTableToJSON(DataTable table);
        Task<Stream> GenerateExcel(DataTable data, string templateName);
    }

    public class ExcelReader : IExcelReader
    {
        public async Task<Stream> GenerateExcel(DataTable data, string templateName)
        {
            try
            {
                var outputStream = new MemoryStream();
                var workbook = new XLWorkbook();

                var worksheet = workbook.Worksheets.Add(data, templateName);
                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(outputStream);

                return outputStream;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<DataTable> ReadExcelFile(Stream fileStream)
        {
            try
            {
                var dt = new DataTable();
                using (var workbook = new XLWorkbook(fileStream))
                {
                    var worksheet = workbook.Worksheet(1); //Worksheets.ElementAt(0);
                    string readRange = "1:1";

                    var rowsUsed = worksheet.RowsUsed();
                    var headerRowPosition = 0; //1;
                    for (int i = headerRowPosition; i <= rowsUsed.Count(); i++) // column header is starting from row 3
                    {
                        var row = rowsUsed.ElementAtOrDefault(i);
                        if (row != null)
                        {
                            if (i == headerRowPosition)
                            {
                                readRange = $"{headerRowPosition}:{row.LastCellUsed().Address.ColumnNumber}";

                                //temp fix
                                if (readRange == "0:27")
                                    readRange = readRange.Replace("0", "1");

                                foreach (var cell in row.Cells(readRange))
                                {
                                    // stripping the spaces off to get the property name
                                    var columnName = new string(cell.Value.ToString().Where(c => c != '*' && !char.IsWhiteSpace(c)).ToArray());
                                    dt.Columns.Add(columnName);
                                }
                            }
                            else
                            {
                                dt.Rows.Add();
                                int cellIndex = 0;
                                foreach (var cell in row.Cells(readRange))
                                {
                                    dt.Rows[dt.Rows.Count - 1][cellIndex] = cell.Value.ToString();
                                    cellIndex++;
                                }
                            }
                        }
                    }
                }
                return dt;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string SerializeDataTableToJSON(DataTable table)
        {
            var options = new JsonSerializerOptions
            {
                //  IgnoreNullValues = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            options.Converters.Add(new DateTimeConverterFactory());
            options.Converters.Add(new DataTableJsonConverter());

            return System.Text.Json.JsonSerializer.Serialize(table, options); // JsonConvert.SerializeObject(table);
        }

        public string DataTableToJSONWithJSONNet(DataTable table)
        {
            return JsonConvert.SerializeObject(table);
        }
    }
}