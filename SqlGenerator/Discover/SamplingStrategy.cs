using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlGenerator.Discover
{
    internal class SamplingStrategy : IFieldDefStrategy
    {
        private ILogger<SamplingStrategy> Logger { get; set; }
        public SamplingStrategy(ILogger<SamplingStrategy> logger) {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ParseResult GetFieldDef(string fieldName, IEnumerable<string> data) {
            var counters = new Dictionary<FieldType, int>() {
                { FieldType.Auto, 0 },
                { FieldType.Bool, 0 },
                { FieldType.DateTime, 0 },
                { FieldType.Numeric, 0 },
                { FieldType.Text, 0 }
            };
            var distinct = new HashSet<string>();
            double num;

            //number followed by tipical date separator followed by number, the same separator and number
            //optionaly separated by space (or T) and hh:mm pattern followed by optional seconds.millisenconds and optional time offset
            //                                   1-4d  sep      1-2d sep 1-4-d   sp|T   hh       mm       ss     .dec     time offset hh   mm
            Regex datePattern = new Regex(@"^(\d{1,4}([-\/.]))\d{1,2}\2\d{1,4}((\s+|T)\d{1,2}:\d{1,2}(:\d{1,2})*(\.\d+)*(\s*[-+]\d{1,2}(:\d{1,2})*)*)*$");

            foreach (string value in data) {
                if (string.IsNullOrEmpty(value)) {
                    counters[FieldType.Auto]++;
                } else if (datePattern.IsMatch(value)) {
                    //Date must be prior than double because 24.02.70 is detected as valid double
                    counters[FieldType.DateTime]++;
                    //TODO: try to infer the date time format using groups in the regex and add it to a HasSet
                } else if (double.TryParse(value, out num)) {
                    counters[FieldType.Numeric]++;
                } else {
                    counters[FieldType.Text]++;
                    distinct.Add(value);
                }
            }
            var result = new ParseResult();
            result.Field.Name = fieldName;

            if (counters[FieldType.Auto] == data.Count()) {
                result.Field.FieldType = FieldType.Auto;
            } else if (counters[FieldType.Numeric] + counters[FieldType.Auto] == data.Count()) {
                result.Field.FieldType = FieldType.Numeric;
            } else if (counters[FieldType.DateTime] + counters[FieldType.Auto] == data.Count()) {
                result.Field.FieldType = FieldType.DateTime;
            } else if (distinct.Count <= 2) {
                result.Field.FieldType = FieldType.Bool;
                result.Field.Format = string.Join('/', distinct).ToUpper();
            } else {
                result.Field.FieldType = FieldType.Text;
            }

            return result;
        }
    }
}
