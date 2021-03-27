using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using SqlGenerator.Discover;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator.Sources
{
    class ExcelSource : BaseSource
    {

        private ExcelWorksheet _ws;

        public ExcelSource(IOptions<Template> options, ILogger<ExcelSource> logger, IDiscoverFieldDefFactory discoverFieldDefFactory) 
            : base(options, logger, discoverFieldDefFactory) { }

        public override void Load(string fileName) {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            Logger.LogDebug($"Opening '{fileName}'...");

            var xls = new ExcelPackage(new System.IO.FileInfo(fileName));
            _ws = string.IsNullOrEmpty(Options.WorkSheetName) ?
                xls.Workbook.Worksheets[0] :
                xls.Workbook.Worksheets[Options.WorkSheetName];

            Logger.LogDebug($"Selected '{_ws.Name}' worksheet.");

            DiscoverTableDef();
        }

        protected override IEnumerable<string> GetHeaders(int row) {
            return _ws.Cells[row, 1, row, _ws.Cells.End.Column]
                .ToList()
                .Select(x => x.Value)
                .Cast<string>();
        }

        protected override string GetTableName() {
            return Options.TableName ?? _ws.Name;
        }

        protected override List<string[]> ReadBufferRows(int numRows) {
            var buffer = new List<string[]>();
            int cols = Headers.Count();
            for (int row = 2, i = 0; row <= _ws.Dimension.End.Row && i < numRows; row++, i++) {
                buffer.Add(GetLine(row, cols));
            }
            return buffer;
        }

        protected override (string[] data, bool more) ReadRow(int rowNumber) {
            if (rowNumber > _ws.Dimension.End.Row) {
                return (new string[TableDef.Fields.Count], false);
            }
            int cols = TableDef.Fields.Count;
            return (GetLine(rowNumber, cols), true);
        }

        private string[] GetLine(int row, int cols) {
            string[] line = new string[cols];
            for (int col = 1; col <= cols; col++) {
                line[col - 1] = _ws.Cells[row, col].Value?.ToString();
            }
            return line;
        }

    }
}
