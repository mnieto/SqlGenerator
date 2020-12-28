using System;
using System.IO;

namespace SqlGenerator
{
    /// <summary>
    /// Base absract class for SQL generators
    /// </summary>
    public abstract class GeneratorBase : IGenerator
    {
        /// <summary>
        /// <see cref="IReader"/> that provides the data to transform in SQL instructions
        /// </summary>
        public IReader Reader { get; protected set; }
        
        /// <summary>
        /// Definition (Schema) of the table
        /// </summary>
        public TableDef TableDef { get; protected set; }
        
        /// <summary>
        /// Datasource metadata information like fields quotes, literal quotes, sentences separator...
        /// </summary>
        protected DataSourceInformation Information { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reader"><see cref="IReader"/> that provides de data</param>
        /// <param name="tableDef">Table schema</param>
        /// <param name="options">Datasource metadata information</param>
        public GeneratorBase(IReader reader, TableDef tableDef, DataSourceInformation options) {
            Reader = reader;
            TableDef = tableDef;
            Information = new DataSourceInformation();
            Information = options;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reader"><see cref="IReader"/> that provides de data</param>
        /// <param name="tableDef">Table schema</param>
        /// <param name="options">Allows to configure datasource metadata. By default MS SQL Server configuration is taken</param>
        public GeneratorBase(IReader reader, TableDef tableDef, Action<DataSourceInformation> options = null) {
            Reader = reader;
            TableDef = tableDef;
            Information = new DataSourceInformation();
            if (options != null) {
                options(Information);
            }
        }

        /// <summary>
        /// Generate the SQL sentences
        /// </summary>
        /// <param name="writer"><see cref="TextWriter"/> where the SQL sentences are written</param>
        public void Generate(TextWriter writer) {
            while (Reader.Read()) {
                string sql = GenerateSQL();
                writer.WriteLine(sql);
            }
        }

        /// <summary>
        /// Generates a single SQL sentence based on the current row in the <see cref="Reader"/>
        /// </summary>
        /// <returns></returns>
        public abstract string GenerateSQL();

        /// <summary>
        /// Generate del literal SQL representation for the specified <paramref name="fld"/> depending on the field data type
        /// </summary>
        /// <param name="fld"><see cref="FieldDef"/> with the field definition</param>
        /// <returns>formated string according with the field data type containing the field value</returns>
        protected string GenerateFieldValue(FieldDef fld) {
            if (fld.IsNullable && Reader.IsNull(fld.OrdinalPosition))
                return Information.NullValue;
            switch (fld.FieldType) {
                case FieldType.Text:
                    return QuoteLiteral(Reader.AsString(fld.OrdinalPosition));
                case FieldType.Bool:
                    return Reader.AsBoolean(fld.OrdinalPosition) ? Information.TrueValue : Information.FalseValue;
                case FieldType.Numeric:
                    return Reader.AsNumber(fld.OrdinalPosition).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case FieldType.DateTime:
                    string format = Information.DateTimeFormat;
                    if (!string.IsNullOrEmpty(fld.Format))
                        format = fld.Format;
                    string dateWithFormat = Reader.AsDateTime(fld.OrdinalPosition).ToString(format);
                    return string.Format(Information.DateTimeTemplate, dateWithFormat);
                default:
                    throw new NotSupportedException($"{fld.FieldType} not supported");
            }
        }

        /// <summary>
        /// Quote identifier like table or field names
        /// </summary>
        /// <param name="identifier">value to be quoted</param>
        protected string QuoteIndentifier(string identifier) {
            char begin = Information.QuoteChars[0];
            char end = Information.QuoteChars[1];
            return string.Concat(begin, Escape(identifier, end), end);
        }

        /// <summary>
        /// Quote a string literal. Escapes the quote char if it is neccesary
        /// </summary>
        /// <param name="literal">literal value</param>
        protected string QuoteLiteral(string literal) {
            char q = Information.StringLiteralChar;
            return string.Concat(q, Escape(literal, q), q);
        }

        private string Escape(string value, char c) {
            string cc = new string(c, 2);
            return value.Replace(c.ToString(), cc);
        }
    }
}