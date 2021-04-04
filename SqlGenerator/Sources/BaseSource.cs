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
    /// <summary>
    /// Base class for data sources
    /// </summary>
    internal abstract class BaseSource : ISource, IReader {
        protected DiscoverStrategy InitialDiscoverStrategy;
        protected DiscoverStrategy DiscoverStrategy;

        protected Template Options { get; set; }
        protected ILogger Logger { get; set; }

        protected IDiscoverFieldDefFactory DiscoverFieldDefFactory { get; init; }

        /// <summary>
        /// Names of the columns. If source has no headers, the collection will be empty
        /// </summary>
        protected IEnumerable<string> Headers { get; set; }

        
        /// <summary>
        /// Contains the n first rows of data. These are used to guess the data type of each column
        /// </summary>
        /// <remarks>
        /// If there is no need to guess data type, <see cref="FieldsBuffer"/> will be null
        /// </remarks>
        protected List<string[]> FieldsBuffer;

        
        /// <summary>
        /// Stores the data of the current row when invoking the <see cref="Read"/> method. Current row is pointed by <see cref="CurrentRow"/> property
        /// </summary>
        protected string[] RowData { get; set; }

        /// <summary>
        /// Points to the current read possition
        /// </summary>
        /// <remarks>
        /// If you override the <see cref="Read"/> method you probably will need to increase this value in each read operation
        /// </remarks>
        public int CurrentRow { get; protected set; }

        /// <summary>
        /// Contains the table schema (definition)
        /// </summary>
        public TableDef TableDef { get; protected set; }

        /// <summary>
        /// Gets the name of the table
        /// </summary>
        public string TableName => TableDef?.TableName;

        /// <summary>
        /// <c>True</c> if table schema is loaded, otherwise it is <c>false</c>
        /// </summary>
        public bool IsLoaded => TableDef?.Fields.Count > 0;

        /// <summary>
        /// Contains, if any, the errors that occured while loading schema or reading data
        /// </summary>
        public List<string> Errors { get; init; }

        /// <summary>
        /// Gets the value of the column specified by name
        /// </summary>
        /// <param name="name">name of the column</param>
        /// <exception cref="NotSupportedException"> when the source has no headers</exception>
        /// <exception cref="IndexOutOfRangeException"> if the specified name doesn't correspond whith an existim column</exception>
        public string this[string name] { 
            get {
                CheckIsLoaded(true);
                if (Headers.Count() == 0)
                    throw new NotSupportedException("Can't access by name in a headerless source");
                int index = Headers.ToList().IndexOf(name);
                return RowData[index];

            }
        }

        /// <summary>
        /// Gets the value of the column specified by position
        /// </summary>
        /// <param name="i">index of the column</param>
        /// <exception cref="IndexOutOfRangeException"> if the specified index is out of the columns range</exception>
        public string this[int i] {
        get {
                CheckIsLoaded(true);
                return RowData[i];
            }
        }

        public BaseSource(IOptions<Template> options, ILogger logger, IDiscoverFieldDefFactory discoverFieldDefFactory) {
            Options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            InitialDiscoverStrategy = Options.DiscoverStrategy;
            DiscoverStrategy = Options.DiscoverStrategy;
            DiscoverFieldDefFactory = discoverFieldDefFactory ?? throw new ArgumentNullException(nameof(discoverFieldDefFactory));

            Headers = new List<string>();
            Errors = new List<string>();
        }


        /// <summary>
        /// Load the schema of the table from the specified file
        /// </summary>
        /// <param name="fileName">Name of the file to read from</param>
        /// <remarks>
        /// Inheritors must call to <see cref="DiscoverTableDef"/> method from <see cref="Load"/>
        /// </remarks>
        public abstract void Load(string fileName);


        public IReader GetReader() {
            return this;
        }


        /// <summary>
        /// Read the next row of data
        /// </summary>
        /// <returns><c>True</c> if there is mor data to read. <c>False</c> if there is no more data</returns>
        /// <remarks>
        /// Note to inheritors: Row information is stored in <see cref="RowData"/> property and <see cref="CurrentRow"/> is incremented in each call to <see cref="Read"/>
        /// </remarks>
        public virtual bool Read() {
            CheckIsLoaded(false);
            bool more = true;
            if (FieldsBuffer != null && CurrentRow < FieldsBuffer.Count) {
                RowData = FieldsBuffer[CurrentRow++];
            }  else {
                (RowData, more) = ReadRow(CurrentRow++);
            }
            return more;
        }

        /// <summary>
        /// Internally used by AsString, AsBoolean, AsNumber... methods to cast to the specified data type
        /// </summary>
        protected T GetField<T>(string name) {
            return ConvertTo<T>(TableDef[name]);
        }

        /// <summary>
        /// Internally used by AsString, AsBoolean, AsNumber... methods to cast to the specified data type
        /// </summary>
        protected T GetField<T>(int i) {
            return ConvertTo<T>(TableDef[i]);
        }

        /// <summary>
        /// Internally used by GetField{T} methods
        /// </summary>
        protected virtual T ConvertTo<T>(FieldDef fld) {
            switch (fld.FieldType) {
                case FieldType.Text:
                case FieldType.Numeric:
                    return (T)Convert.ChangeType(RowData[fld.OrdinalPosition], typeof(T));
                case FieldType.Bool:
                    var trueValues = new List<string>();
                    var falseValues = new List<string>();
                    if (string.IsNullOrEmpty(fld.Format)) {
                        trueValues.AddRange(new string[] { "true", "yes", "y", "1", "-1", "x" });
                        falseValues.AddRange(new string[] { "false", "no", "n", "0", " " });
                    } else {
                        string[] values = fld.Format.Split("/");
                        trueValues.Add(values[0]);
                        falseValues.Add(values[1]);
                    }
                    if (trueValues.Contains(RowData[fld.OrdinalPosition], StringComparer.CurrentCultureIgnoreCase))
                        return (T)Convert.ChangeType(bool.TrueString, typeof(T));
                    else if (falseValues.Contains(RowData[fld.OrdinalPosition], StringComparer.CurrentCultureIgnoreCase))
                        return (T)Convert.ChangeType(bool.FalseString, typeof(T));
                    else
                        throw new InvalidCastException($"Can't convert {RowData[fld.OrdinalPosition]} to boolean value");
                case FieldType.DateTime:
                    if (string.IsNullOrEmpty(fld.Format)) {
                        string value = RowData[fld.OrdinalPosition];
                        return (T)Convert.ChangeType(value, typeof(T));
                    } else {
                        DateTime d = DateTime.ParseExact(RowData[fld.OrdinalPosition], fld.Format, null);
                        return (T)Convert.ChangeType(d, typeof(T));
                    }
                default:
                    throw new InvalidCastException($"Not supported data type: {fld.FieldType}");
            }
        }


        /// <summary>
        /// Retrive the headers from the source
        /// </summary>
        /// <param name="row">Number of the row where the headers are located</param>
        protected abstract IEnumerable<string> GetHeaders(int row);

        /// <summary>
        /// Retrieve the table name from the source
        /// </summary>
        protected abstract string GetTableName();

        /// <summary>
        /// Read up to the first n rows to guess to data type of each column
        /// </summary>
        /// <param name="numRows">Number of rows to read</param>
        /// <remarks>
        /// Readed rows are stored in <see cref="FieldsBuffer"/>. The <see cref="Read"/> method gets transparently data from the buffer or from source file when needed
        /// </remarks>
        protected abstract List<string[]> ReadBufferRows(int numRows);
        
        /// <summary>
        /// Read the next row of data
        /// </summary>
        /// <param name="rowNumber">Number of row to be read</param>
        /// <returns>A tuple with data read and a boolean that indicates if there is more data or not</returns>
        protected abstract (string[] data, bool more) ReadRow(int rowNumber);

        /// <summary>
        /// Try different strategies to get the table's schema
        /// Auto => ConnectToDatabase => FielDefDescriptor => GuessDataType
        /// </summary>
        protected virtual void DiscoverTableDef() {
            if (DiscoverStrategy == DiscoverStrategy.Auto) {
                Logger.LogInformation($"Using {DiscoverStrategy.Auto} to guess fields information");
                if (Options.Fields.Count() == 0)
                    DiscoverStrategy = DiscoverStrategy.ConnectToDatabase;
                else
                    DiscoverStrategy = DiscoverStrategy.UseTemplate;
            }
            if (DiscoverStrategy == DiscoverStrategy.UseTemplate) {
                UseFieldDescriptorFromTemplate();
            }
            if (DiscoverStrategy == DiscoverStrategy.ConnectToDatabase) {
                if (!string.IsNullOrEmpty(Options.ConnectionString))
                    UseDatabase(Options.ConnectionString);
                else if (InitialDiscoverStrategy == DiscoverStrategy.Auto)
                    DiscoverStrategy = DiscoverStrategy.FieldDefDescriptor;
            }
            if (DiscoverStrategy == DiscoverStrategy.FieldDefDescriptor) {
                UseFieldDescriptor();
            }
            if (DiscoverStrategy == DiscoverStrategy.GuessDataType) {
                UseScan(Options.RowsToScan);
            }
        }

        /// <summary>
        /// Ensures that <see cref="TableDef"/> is valid and its information is not invalidated by subsequent calls to UseXXXX methods
        /// </summary>
        protected virtual void BuildTableDef() {
            if (Headers.Count() == 0)
                Headers = GetHeaders(1);

            TableDef = new TableDef {
                TableName = GetTableName()
            };
        }

        protected virtual void UseFieldDescriptorFromTemplate() {
            Logger.LogInformation($"Using {DiscoverStrategy.UseTemplate} to load fields information from template");
            BuildTableDef();
            CurrentRow = 2; //Skip headers and point to first row data

            //Assign ordinalPosition for those fields that don't have defined it
            int p = 0;
            Logger.LogDebug("Found {fields} field definitions:", Options.Fields.Count);
            foreach (var field in Options.Fields) {
                if (field.OrdinalPosition == 0)
                    field.OrdinalPosition = p;
                Logger.LogDebug("  " + field.ToString());
                p++;
            }
            bool hasDuplicates = Options.Fields
                .GroupBy(x => x.OrdinalPosition)
                .Where(g => g.Count() > 1)
                .Any();
            if (hasDuplicates) {
                throw new InvalidOperationException("Found duplicated OrdinalPosition for some fields");
            }

            TableDef.Fields.AddRange(Options.Fields);
        }

        /// <summary>
        /// Uses database strategy to get the table's schema
        /// </summary>
        /// <param name="connectionString">connection string neccesary to connect to the database</param>
        protected virtual void UseDatabase(string connectionString) {
            Logger.LogInformation($"Using {DiscoverStrategy.ConnectToDatabase} to guess fields information");
            try {
                throw new NotImplementedException();
            } catch (Exception ex) {
                Errors.Add("Error loading fieldDef from template: " + ex.Message);
                DiscoverStrategy = DiscoverStrategy.ConnectToDatabase;
            }
        }


        /// <summary>
        /// Uses <see cref="FieldDefParserStrategy"/> strategy to get the table's schema
        /// </summary>
        protected virtual void UseFieldDescriptor() {

            Logger.LogInformation($"Using {DiscoverStrategy.FieldDefDescriptor} to guess fields information");
            BuildTableDef();

            var dataTypeSampling = DiscoverFieldDefFactory.GetDiscoverStrategy(DiscoverStrategy);
            foreach (string descriptor in Headers) {
                var result = dataTypeSampling.GetFieldDef(descriptor);
                if (result.Success) {
                    result.Field.OrdinalPosition = TableDef.Fields.Count;
                    TableDef.Fields.Add(result.Field);
                } else {
                    var errors = result.Errors.Select(x => x.Message);
                    Logger.LogWarning($"Error parsing {descriptor} field: {string.Join(Environment.NewLine, errors)}");
                    Errors.AddRange(errors);
                }
            }
            if (TableDef.Fields.Count != Headers.Count()) {
                TableDef = null;
                if (InitialDiscoverStrategy == DiscoverStrategy.Auto) {
                    Errors.Clear();
                    DiscoverStrategy = DiscoverStrategy.GuessDataType;
                }
            }
        }

        /// <summary>
        /// Uses <see cref="SamplingStrategy"/> strategy to get the table's schema
        /// </summary>
        protected virtual void UseScan(int numRows) {

            Logger.LogInformation($"Using {DiscoverStrategy.GuessDataType} to guess fields information");
            BuildTableDef();

            FieldsBuffer = ReadBufferRows(numRows);
            int i = 0;
            var dataTypeSampling = DiscoverFieldDefFactory.GetDiscoverStrategy(DiscoverStrategy);
            foreach (string header in Headers) {
                var data = FieldsBuffer.Select(x => x[i]);
                var parseResult = dataTypeSampling.GetFieldDef(header, data);
                if (parseResult.Success) {
                    parseResult.Field.OrdinalPosition = TableDef.Fields.Count;
                    TableDef.Fields.Add(parseResult.Field);
                }
                i++;
            }
            
        }


        private void CheckIsLoaded(bool checkIfReaded) {
            if (!IsLoaded)
                throw new InvalidOperationException("Source is not loaded");
            if (checkIfReaded && RowData == null) {
                throw new InvalidOperationException($"You must invoke {nameof(Read)} before this operation");
            }

        }

        /// <summary>
        /// Inherited <see cref="IRecord.AsBoolean(string)"/>
        /// </summary>
        public bool AsBoolean(string name) {
            return GetField<bool>(name);
        }

        /// <summary>
        /// Inherited <see cref="IRecord.AsBoolean(int)"/>
        /// </summary>
        public bool AsBoolean(int i) {
            return GetField<bool>(i);
        }

        /// <summary>
        /// Inherited <see cref="IRecord.AsString(string)"/>
        /// </summary>
        public string AsString(string name) {
            return GetField<string>(name);
        }

        /// <summary>
        /// Inherited <see cref="IRecord.AsString(int)"/>
        /// </summary>
        public string AsString(int i) {
            return GetField<string>(i);
        }

        /// <summary>
        /// Inherited <see cref="IRecord.AsDateTime(string)"/>
        /// </summary>
        public DateTime AsDateTime(string name) {
            return GetField<DateTime>(name);
        }

        /// <summary>
        /// Inherited <see cref="IRecord.AsDateTime(int)"/>
        /// </summary>
        public DateTime AsDateTime(int i) {
            return GetField<DateTime>(i);
        }

        /// <summary>
        /// Inherited <see cref="IRecord.AsNumber(string)"/>
        /// </summary>
        public double AsNumber(string name) {
            return GetField<double>(name);
        }

        /// <summary>
        /// Inherited <see cref="IRecord.AsNumber(int)"/>
        /// </summary>
        public double AsNumber(int i) {
            return GetField<double>(i);
        }

        /// <summary>
        /// Inherited <see cref="IRecord.IsNull(string)"/>
        /// </summary>

        public bool IsNull(string name) {
            return InternalIsNull(TableDef[name]);
        }

        /// <summary>
        /// Inherited <see cref="IRecord.IsNull(int)"/>
        /// </summary>
        public bool IsNull(int i) {
            return InternalIsNull(TableDef[i]);
        }

        private bool InternalIsNull(FieldDef fld) {
            if (fld.FieldType == FieldType.Text) {
                return RowData[fld.OrdinalPosition] == null;
            } else if (fld.FieldType == FieldType.DateTime && RowData[fld.OrdinalPosition] == "0000-00-00 00:00:00") {
                //MySQL stores 'false' datetime values for NULL or invalid values. See: https://dev.mysql.com/doc/refman/5.6/en/sql-mode.html#sqlmode_no_zero_date
                return true;
            }
            return string.IsNullOrEmpty(RowData[fld.OrdinalPosition]);
        }
    }
}
