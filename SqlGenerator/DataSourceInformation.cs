using System;

namespace SqlGenerator
{
    public class DataSourceInformation
    {
        private string _quoteChars = "[]";
        public string QuoteChars {
            get { return _quoteChars; }
            set {
                if (value == null || value.Length != 2)
                    throw new ArgumentException("Value must be 2 chars lenght");
                _quoteChars = value;
            }
        } 

        public char StringLiteralChar { get; set; } = '\'';
        public string StatementSeparator { get; set; } = ";";
        public string TrueValue { get; set; } = "1";
        public string FalseValue { get; set; } = "0";
        public string NullValue { get; set; } = "NULL";
        public string DateTimeTemplate { get; set; } = "CONVERT(DATETIME, '{0}', 126)";
        public string DateTimeFormat { get; set; } = "yyyy-MM-ddTHH:mm:ss";
    }
}