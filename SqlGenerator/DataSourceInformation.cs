using System;

namespace SqlGenerator
{
    /// <summary>
    /// Exposes information specific for the database provider
    /// </summary>
    public class DataSourceInformation
    {
        private string _quoteChars = "[]";

        /// <summary>
        /// String of 2 chars length used for quote indentifiers (fields, table names..)
        /// </summary>
        public string QuoteChars {
            get { return _quoteChars; }
            set {
                if (value == null || value.Length != 2)
                    throw new ArgumentException("Value must be 2 chars lenght");
                _quoteChars = value;
            }
        } 

        /// <summary>
        /// Char used to quote literals
        /// </summary>
        public char StringLiteralChar { get; set; } = '\'';
        
        /// <summary>
        /// String used as statement separator
        /// </summary>
        public string StatementSeparator { get; set; } = ";";
        
        /// <summary>
        /// Literal used for true value in boolean fields
        /// </summary>
        public string TrueValue { get; set; } = "1";
        
        /// <summary>
        /// Literal used for false value in boolean fields
        /// </summary>
        public string FalseValue { get; set; } = "0";
        
        /// <summary>
        /// Literal used for null values
        /// </summary>
        public string NullValue { get; set; } = "NULL";
        
        /// <summary>
        /// Expression used in the database engine to convert a string literal to DateTime
        /// </summary>
        public string DateTimeTemplate { get; set; } = "CONVERT(DATETIME, '{0}', 126)";
        
        /// <summary>
        /// DateTime string format
        /// </summary>
        public string DateTimeFormat { get; set; } = "yyyy-MM-ddTHH:mm:ss";
    }
}