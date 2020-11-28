using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlGenerator.Discover;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlGenerator.Sources
{
    internal abstract class BaseSource : ISource {
        protected DiscoverStrategy DiscoverStrategy;

        protected Specification Options { get; set; }
        protected ILogger Logger { get; set; }

        protected IFieldDefStrategy DataTypeSampling {get; init; }

        protected IEnumerable<string> Headers { get; set; }

        protected List<string[]> FieldsBuffer;

        public TableDef TableDef { get; protected set; }

        public string TableName => TableDef?.TableName;

        public bool IsLoaded => TableDef?.Fields.Count > 0;

        public List<string> Errors { get; init; }


        public BaseSource(IOptions<Specification> options, ILogger logger, IFieldDefStrategy dataTypeSampling) {
            Options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            DiscoverStrategy = Options.DiscoverStrategy;
            DataTypeSampling = dataTypeSampling ?? throw new ArgumentNullException(nameof(dataTypeSampling));

            Headers = new List<string>();
            Errors = new List<string>();
        }

        public abstract void Load(string fileName);

        protected abstract IEnumerable<string> GetHeaders(int row);
        protected abstract string GetTableName();
        protected abstract List<string[]> ReadBufferRows(int numRows);

        protected virtual void DiscoverTableDef() {
            if (DiscoverStrategy == DiscoverStrategy.ConnectToDatabase) {
                if (!string.IsNullOrEmpty(Options.ConnectionString))
                    UseDatabase(Options.ConnectionString);
                else
                    DiscoverStrategy = DiscoverStrategy.FieldDefDescriptor;
            }
            if (DiscoverStrategy == DiscoverStrategy.FieldDefDescriptor) {
                UseFieldDescriptor();
            }
            if (DiscoverStrategy == DiscoverStrategy.FieldNamesOnly) {
                UseFieldNames();
                if (Options.RowsToScan.GetValueOrDefault(0) > 0) {
                    DiscoverStrategy = DiscoverStrategy.GuestDataType;
                }
            }
            if (DiscoverStrategy == DiscoverStrategy.GuestDataType) {
                if (Options.RowsToScan.GetValueOrDefault(0) == 0)
                    throw new InvalidOperationException($"{nameof(Options.RowsToScan)} option is mandatory when {nameof(DiscoverStrategy)} is {nameof(DiscoverStrategy.GuestDataType)}.");
                UseScan(Options.RowsToScan.Value);
            }
        }

        protected virtual void BuildTableDef() {
            if (Headers.Count() == 0)
                Headers = GetHeaders(1);

            TableDef = new TableDef {
                TableName = GetTableName()
            };
        }

        protected virtual void UseDatabase(string connectionString) {
            throw new NotImplementedException();
        }

        protected virtual void UseFieldDescriptor() {

            BuildTableDef();

            foreach (string descriptor in Headers) {
                var result = DataTypeSampling.GetFieldDef(descriptor);
                if (result.Success) {
                    TableDef.Fields.Add(result.Field);
                } else {
                    Errors.AddRange(result.Errors.Select(x => x.Message));
                }
            }
            if (TableDef.Fields.Count != Headers.Count()) {
                TableDef = null;
                if (Options.DiscoverStrategy == DiscoverStrategy.Auto) {
                    DiscoverStrategy = DiscoverStrategy.FieldNamesOnly;
                }
            }
        }

        protected virtual void UseFieldNames() {

            BuildTableDef();

            foreach (string header in Headers) {
                TableDef.Fields.Add(new FieldDef { Name = header });
            }
        }

        protected virtual void UseScan(int numRows) {

            BuildTableDef();

            FieldsBuffer = ReadBufferRows(numRows);
            int i = 0;
            foreach (string header in Headers) {
                var data = FieldsBuffer.Select(x => x[i++]);
                var parseResult = DataTypeSampling.GetFieldDef(header, data);
                if (parseResult.Success) {
                    TableDef.Fields.Add(parseResult.Field);
                }
            }

        }
            

    }
}
