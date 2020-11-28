using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator.Test
{
    internal static class SampleData
    {
        public static (List<string> Header, List<string[]> Data) SamplingData() {
            var header = new List<string> { "auto", "numeric", "bool", "text", "date" };
            var data = new List<string[]> {
                //            auto, numeric, bool,   text,    date
                new string[] {null, "8",     "t",    "t",     "24.02.70"},
                new string[] {null, "8.73",  "f" ,   "f",     "30/12/1966 13:20"},
                new string[] {null, null,    null,   "f",     null},
                new string[] {null, "-77.3", "t",    "other", "2020.11.23"},
                new string[] {null, "0" ,    "t",    null,    "14-09-04 20:37:52.9 +03:00" }
            };
            return (header, data);
        }

        public static (List<string> Header, List<string[]> Data) FieldDescriptorData() {
            var header = new List<string> { "kn|nn|Id", "n|numeric", "b|bool", "t|seemText", "d|n|MyDates" };
            var data = new List<string[]> {
                //            id, numeric, bool,   text,    date
                new string[] { "1", "8",     "t",    "t",     "24.02.70" },
                new string[] { "2", "8.73",  "f" ,   "f",     "30/12/1966 13:20" },
                new string[] { "3", null,    null,   "f",     null },
                new string[] { "4", "-77.3", "t",    "other", "2020.11.23" },
                new string[] { "5", "0" ,    "t",    null,    "14-09-04 20:37:52.9 +03:00" }
            };
            return (header, data);
        }

        public static (List<string> Header, List<string[]> Data) ErroneousFieldDescriptorData() {
            var header = new List<string> { "kn|nn|Id", "ThisNotHaveDataType", "b|bool", "t|seemText|badFormat", "d|n|MyDates" };
            var data = new List<string[]> {
                //            id, numeric, bool,   text,    date
                new string[] { "1", "8",     "t",    "t",     "24.02.70" },
                new string[] { "2", "8.73",  "f" ,   "f",     "30/12/1966 13:20" },
                new string[] { "3", null,    null,   "f",     null },
                new string[] { "4", "-77.3", "t",    "other", "2020.11.23" },
                new string[] { "5", "0" ,    "t",    null,    "14-09-04 20:37:52.9 +03:00" }
            };
            return (header, data);
        }
    }
}
