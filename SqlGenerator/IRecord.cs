using System;

namespace SqlGenerator
{
    /// <summary>
    /// Provides access to the column values of current row for a <see cref="IReader"/>
    /// </summary>
    public interface IRecord
    {
        /// <summary>
        /// Get access to the nth column
        /// </summary>
        /// <param name="i">index of the column</param>
        /// <returns>string representation of the column</returns>
        public string this[int i] { get; }

        /// <summary>
        /// Get the column value with the specified name
        /// </summary>
        /// <param name="name">name of the column</param>
        /// <returns>string representation of the column</returns>        
        public string this[string name] { get; }

        /// <summary>
        /// Gets the value of the specified column as a boolean
        /// </summary>
        public bool AsBoolean(string name);

        /// <summary>
        /// Gets the value of the specified column as a boolean
        /// </summary>
        public bool AsBoolean(int i);

        /// <summary>
        /// Gets the value of the specified column as a string
        /// </summary>
        public string AsString(string name);

        /// <summary>
        /// Gets the value of the specified column as a string
        /// </summary>
        public string AsString(int i);

        /// <summary>
        /// Gets the value of the specified column as a DateTime
        /// </summary>
        public DateTime AsDateTime(string name);

        /// <summary>
        /// Gets the value of the specified column as a DateTime
        /// </summary>
        public DateTime AsDateTime(int i);

        /// <summary>
        /// Gets the value of the specified column as a double
        /// </summary>
        public double AsNumber(string name);

        /// <summary>
        /// Gets the value of the specified column as a double
        /// </summary>
        public double AsNumber(int i);

        /// <summary>
        /// Returns <c>true</c> if column contains null value
        /// </summary>
        public bool IsNull(string name);

        /// <summary>
        /// Returns <c>true</c> if column contains null value
        /// </summary>
        public bool IsNull(int i);

    }
}