using SqlGenerator.Discover;
using SqlGenerator.Sources;

namespace SqlGenerator
{
    /// <summary>
    /// Enum the different data types distinguised by the sql generator
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        /// Field type not set. Will try to guess for each read value.
        /// </summary>
        /// <remarks>
        /// When <see cref="DiscoverStrategy"/> is <see cref="DiscoverStrategy.GuessDataType"/> and all scaned rows are <c>null</c>, FieldType remains Auto
        /// </remarks>
        Auto,

        /// <summary>
        /// If field has text or other field types have failed.
        /// </summary>
        Text,

        /// <summary>
        /// Field has only numeric values. Can be integer, float.... For text sources, decimal separator is the system default
        /// </summary>
        Numeric,

        /// <summary>
        /// Field seems to have date or date time values.
        /// </summary>
        /// <remarks>
        /// Do not support formats with month names. Date separator can be [/], [-] or [.] Hour separator is [:]
        /// Detect formats like yyyy-mm-dd, mm-dd-yyyy, dd-mm-yyyy. With year in 2 or 4 digits, and day and month with 1 or 2 digits
        /// Hour part can be hh:mm with 1 or 2 digits. Any trailing chars will be considered seconds part, which allow milliseconds
        /// 12 hours format with am/pm not allowed. Please use allways 24 hours format
        /// </remarks>
        DateTime,

        /// <summary>
        /// Field is considered boolean as it has only 1 or 2 values (apart of null)
        /// </summary>
        /// <remarks>
        /// The <see cref="FieldDef.Format"/> field will contain the text values for true/false, separated with [/]
        /// If <see cref="FieldDef.Format"/> is not specified, allowed pairs are: true/false, yes/no, y/n, 1/0, -1/0, x/&lt;space&gt;
        /// </remarks>
        Bool
    }
}
