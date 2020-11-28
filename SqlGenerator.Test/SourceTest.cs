using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SqlGenerator.Discover;
using SqlGenerator.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqlGenerator.Test
{
    public class SourceTest
    {

        [Fact]
        public void isLoaded_is_false_until_data_is_readed() {
            var settings = Options.Create(new Specification {
                DiscoverStrategy = DiscoverStrategy.FieldDefDescriptor,
                TableName = "MyTable"
            });
            var sut = new TestSource(settings, NullLogger<SourceTest>.Instance, new FieldDefParserStrategy(), SampleData.FieldDescriptorData());
            Assert.False(sut.IsLoaded);
        }

        [Fact]
        public void source_with_errors_has_no_fields() {
            var settings = Options.Create(new Specification {
                DiscoverStrategy = DiscoverStrategy.FieldDefDescriptor,
                TableName = "MyTable"
            });
            var sut = new TestSource(settings, NullLogger<SourceTest>.Instance, new FieldDefParserStrategy(), SampleData.ErroneousFieldDescriptorData());
            sut.Load("imput filename");
            Assert.False(sut.IsLoaded);         //Source with errors will not have fields and thus, IsLoaded status is false
            Assert.Null(sut.TableDef);          //Source with errors will not have a defined TableDef
            Assert.Collection(sut.Errors,
                x => Assert.Contains("ThisNotHaveDataType", x),
                x => Assert.Contains("t|seemText|badFormat", x)
            );
        }

        [Fact]
        public void load_with_field_descriptor() {
            var settings = Options.Create(new Specification {
                DiscoverStrategy = DiscoverStrategy.FieldDefDescriptor,
                TableName ="MyTable"
            });
            var sut = new TestSource(settings, NullLogger<SourceTest>.Instance, new FieldDefParserStrategy(), SampleData.FieldDescriptorData());
            sut.Load("imput filename");
            Assert.True(sut.IsLoaded);
            Assert.Equal(settings.Value.TableName, sut.TableName);
            Assert.Equal(5, sut.TableDef.Fields.Count);
            Assert.Collection(sut.TableDef.Keys, x => Assert.Equal("Id", x));
        }

    }


    internal class TestSource : BaseSource
    {

        private (List<string> Header, List<string[]> Data) sampleData;
        public TestSource(IOptions<Specification> options,
                                         ILogger logger,
                                         IFieldDefStrategy dataTypeSampling,
                                         (List<string> Header, List<string[]> Data) sampleData) :
            base(options, logger, dataTypeSampling) {

            this.sampleData = sampleData;
        }

        public override void Load(string fileName) {
            DiscoverTableDef();
        }

        protected override IEnumerable<string> GetHeaders(int row) {
            return sampleData.Header;
        }

        protected override string GetTableName() {
            return Options.TableName;
        }

        protected override List<string[]> ReadBufferRows(int numRows) {
            return sampleData.Data;
        }
    }
}
