using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator
{
    /// <summary>
    /// Generate insert SQL sentences for each data row
    /// </summary>
    public class InsertGenerator : GeneratorBase, IGenerator
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reader"><see cref="IReader"/> that provides de data</param>
        /// <param name="tableDef">Table schema</param>
        /// <param name="options">Datasource metadata information</param>
        public InsertGenerator(IReader reader, TableDef tableDef, DataSourceInformation options) :
            base(reader, tableDef, options)  {  }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reader"><see cref="IReader"/> that provides de data</param>
        /// <param name="tableDef">Table schema</param>
        /// <param name="options">Allows to configure datasource metadata. By default MS SQL Server configuration is taken</param>
        public InsertGenerator(IReader reader, TableDef tableDef, Action<DataSourceInformation> options = null) :
            base(reader, tableDef, options) { }



        /// <summary>
        /// Inherited <see cref="GeneratorBase.GenerateSQL"/>
        /// </summary>
        public override string GenerateSQL() {
            var sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            sb.Append(QuoteIndentifier(TableDef.TableName));
            sb.Append(" (");
            foreach (FieldDef fld in TableDef.Fields) {
                sb.Append(QuoteIndentifier(fld.Name));
                if (fld.OrdinalPosition < TableDef.Fields.Count - 1) {
                    sb.Append(", ");
                }
            }
            sb.Append(") VALUES (");

            foreach (FieldDef fld in TableDef.Fields) {
                sb.Append(GenerateFieldValue(fld));
                if (fld.OrdinalPosition < TableDef.Fields.Count - 1) {
                    sb.Append(", ");
                }
            }
            sb.Append(')');
            sb.Append(Information.StatementSeparator);

            return sb.ToString();
        }
    }
}
