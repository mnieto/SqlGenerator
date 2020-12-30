using System;

namespace SqlGenerator
{

    /// <summary>
    /// Generator factory to create Insert, Update... generator
    /// </summary>
    public interface IGeneratorFactory
    {
        /// <summary>
        /// Return the correct <see cref="IGenerator"/> implementation
        /// </summary>
        /// <param name="reader">Data reader</param>
        /// <param name="tableDef">Table schema</param>
        /// <param name="options"><see cref="DataSourceInformation"/> with specific SQL engine options</param>
        IGenerator GetGenerator(IReader reader, TableDef tableDef, DataSourceInformation options);

        /// <summary>
        /// Return the correct <see cref="IGenerator"/> implementation
        /// </summary>
        /// <param name="reader">Data reader</param>
        /// <param name="tableDef">Table schema</param>
        /// <param name="options">Method that configures a <see cref="DataSourceInformation"/> with specific SQL engine options</param>
        IGenerator GetGenerator(IReader reader, TableDef tableDef, Action<DataSourceInformation> options = null);
    }
}