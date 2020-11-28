using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlGenerator.Discover
{
    /// <summary>
    /// Result of parsing a FieldDef descriptor
    /// </summary>
    internal class ParseResult
    {
        /// <summary>
        /// If no errors, Gets an nitialized and valid <see cref="FieldDef"/>. If there is any error, this field is <c>null</c>
        /// </summary>
        public FieldDef Field { get; set; } = new FieldDef();
        
        /// <summary>
        /// <c>true</c> if no parsing errors
        /// </summary>
        public bool Success => Errors.Count == 0;

        /// <summary>
        /// Readable error list as multiline string
        /// </summary>
        public string ErrorMessage => string.Join(Environment.NewLine, Errors.Select(x => x.Message));

        /// <summary>
        /// Parse errors found. If <see cref="Success"/> this collection is empty
        /// </summary>
        public List<ParseError> Errors { get; } = new List<ParseError>();

    }

    internal class ParseError
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public ParseError(int code, string message) {
            Code = code;
            Message = message;
        }
    }
}
