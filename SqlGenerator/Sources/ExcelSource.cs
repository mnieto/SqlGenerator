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

        public ExcelSource(IOptions<Specification> options, ILogger<ExcelSource> logger, IFieldDefStrategy dataTypeSampling) 
            : base(options, logger, dataTypeSampling) { }

        public override void Load(string fileName) {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            Logger.LogDebug($"Opening '{fileName}'...");

            using var xls = new ExcelPackage(new System.IO.FileInfo(fileName));
            _ws =  string.IsNullOrEmpty(Options.WorkSheetName) ?
                xls.Workbook.Worksheets[Options.WorkSheetName] :
                xls.Workbook.Worksheets[1];

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
            int cols = TableDef.Fields.Count;
            for (int row = 2, i = 0; row <= _ws.Cells.End.Row && i < numRows; row++, i++) {
                string[] line = _ws.Cells[row, 1, row, cols].Select(x => x.Value?.ToString()).ToArray();
                buffer.Add(line);
            }
            return buffer;
        }

    }
}
