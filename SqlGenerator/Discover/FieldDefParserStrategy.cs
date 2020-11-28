using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlGenerator.Discover
{

    internal class FieldDefParserStrategy : IFieldDefStrategy
    {
        
        //Error codes
        public const int ERR_FIELD_TYPE = 1;
        public const int ERR_FIELD_NAME = 2;
        public const int ERR_BOOL_ATTR = 3;
        public const int ERR_NUMERIC_ATTR = 4;
        public const int ERR_TEXT_ATTR = 5;
        public const int ERR_NUM_TOKENS = 6;
        public const int ERR_UNEXPECTED = 7;


        /// <summary>
        /// Translate char from <see cref="patternFieldType"/> to <see cref="FieldType"/>
        /// </summary>
        private static Dictionary<char, FieldType> dataTypes = new Dictionary<char, FieldType> {
            {'t', FieldType.Text },
            {'n', FieldType.Numeric },
            {'d', FieldType.DateTime },
            {'b', FieldType.Bool }
        };

        //Parsing rules for each type of token
        private Regex patternFieldType = new Regex(@"^k*[tndb]$");      //Mandatory: k for key field and letter for datatype
        private Regex patternNullable = new Regex(@"^n{0,2}$");         //Optional:  n or empty => nullable; nn => not nullable
        private Regex patternName = new Regex(@"[-_ \d\p{L}]+");        //Mandatory: Any _,-,{espace},letter or digit
                                                                        //Optional:  Format: b - values for true/false
                                                                        //                   t - max size
                                                                        //                   d - datetime format

        /// <summary>Which token are we parsing now</summary>
        private int CurrentToken { get; set; } = 0;

        /// <summary>Descriptor Which we are parsing now</summary>
        private string Descriptor { get; set; }

        /// <summary>Current parse result</summary>
        private ParseResult result { get; set; }


        public ParseResult GetFieldDef(string fieldName, IEnumerable<string> data = null) {
            result = new ParseResult();
            CurrentToken = 0;
            Descriptor = fieldName;

            var tokens = Tokenize(fieldName).ToList();
            if (tokens.Count < 2 || tokens.Count > 4) {
                AddError(ERR_NUM_TOKENS, $"Field descriptor must have between 2 and 4 tokens. Found {tokens.Count}");
                return result;
            }

            try {
                ParseFieldType(patternFieldType, tokens[CurrentToken]);
                ParseNullable(patternNullable, tokens[CurrentToken]);
                ParseName(patternName, tokens[CurrentToken]);
                string attributes = CurrentToken < tokens.Count ? tokens[CurrentToken] : null;
                ParseAttributes(attributes);
            } catch (Exception ex) {
                AddError(ERR_UNEXPECTED, $"Unexpected error: {ex.Message}");
            }

            if (!result.Success) {
                result.Field = null;
            }
            return result;
        }


        public IEnumerable<string> Tokenize(string fieldMeta, char sep = '|') {
            var sb = new StringBuilder();
            int i = 0;
            while (i < fieldMeta.Length) {
                char c = fieldMeta[i];
                if (c == sep) {
                    if (i + 1 < fieldMeta.Length && fieldMeta[i + 1] == sep) {
                        sb.Append(c);
                        i++;
                    } else {
                        string token = sb.ToString();
                        sb.Clear();
                        if (token != string.Empty)
                            yield return token;
                    }
                } else {
                    sb.Append(c);
                }
                i++;
            }
            string lastToken = sb.ToString();
            if (lastToken != string.Empty)
                yield return sb.ToString();
        }


        private ParseResult ParseFieldType(Regex regex, string token) {
            var match = regex.Match(token.ToLower());
            int i = 0;
            if (match.Success) {
                if (match.Value[i] == 'k') {
                    result.Field.IsKey = true;
                    i++;
                }
                result.Field.FieldType =  dataTypes[match.Value[i]];
            } else {
                AddError(ERR_FIELD_TYPE, "Cannot match field type. It must be an optional k for key fields followed by one of these chars: t|n|d|b => text,number,date,bool.");
            }
            CurrentToken++;
            return result;
        }

        private ParseResult ParseNullable(Regex regex, string token) {
            var match = regex.Match(token.ToLower());
            if (match.Success) {
                result.Field.IsNullable = match.Value == "n";
                CurrentToken++;
            }
            return result;
        }

        private ParseResult ParseName(Regex regex, string token) {
            var match = regex.Match(token);
            if (match.Success) {
                result.Field.Name = match.Value.Trim();
                //TODO: Naming correctness is database engine dependant
            } else {
                AddError(ERR_FIELD_NAME, $"Field name must composed by numbers, letters, space, - and _");
            }
            CurrentToken++;
            return result;
        }

        private ParseResult ParseAttributes(string token) {
            switch (result.Field.FieldType) {
                case FieldType.Bool:
                    if (token != null) {
                        if (token.Split('/').Length != 2) {
                            AddError(ERR_BOOL_ATTR, $"Boolean values must be separated with /. Firt token will match with true and second with false.");
                        }
                        result.Field.Format = token;
                    }
                    break;
                case FieldType.DateTime:
                    result.Field.Format = token;
                    break;
                case FieldType.Numeric:
                    if (token != null)
                        AddError(ERR_NUMERIC_ATTR, $"Numeric fields have not attributes.");
                    break;
                case FieldType.Text:
                    if (token != null) {
                        int length;
                        if (int.TryParse(token, out length)) {
                            result.Field.MaxLength = length;
                        } else {
                            AddError(ERR_TEXT_ATTR, $"Text fields attribute must be an integer with the field length.");
                        }
                    }
                    break;
            }
            CurrentToken++;
            return result;
        }

        private void AddError(int code, string message) {
            result.Errors.Add(new ParseError(code, $"[{Descriptor}] {message}"));
        }

    }
}
