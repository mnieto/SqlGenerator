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
using Moq;

namespace SqlGenerator.Test
{
    public class SourceTest
    {

        [Fact]
        public void isLoaded_is_false_until_data_is_readed() {
            var discoverFactory = new Mock<IDiscoverFieldDefFactory>();
            discoverFactory.Setup(x => x.GetDiscoverStrategy(It.IsAny<DiscoverStrategy>()))
                .Returns(new FieldDefParserStrategy());

            var settings = Options.Create(new Template {
                DiscoverStrategy = DiscoverStrategy.FieldDefDescriptor,
                TableName = "MyTable"
            });
            var sut = new TestSource(settings, NullLogger<SourceTest>.Instance, discoverFactory.Object, SampleData.FieldDescriptorData());
            Assert.False(sut.IsLoaded);
        }

        [Fact]
        public void source_with_errors_has_no_fields() {
            var discoverFactory = new Mock<IDiscoverFieldDefFactory>();
            discoverFactory.Setup(x => x.GetDiscoverStrategy(It.IsAny<DiscoverStrategy>()))
                .Returns(new FieldDefParserStrategy());

            var settings = Options.Create(new Template {
                DiscoverStrategy = DiscoverStrategy.FieldDefDescriptor,
                TableName = "MyTable"
            });
            var sut = new TestSource(settings, NullLogger<SourceTest>.Instance, discoverFactory.Object, SampleData.ErroneousFieldDescriptorData());
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
            var discoverFactory = new Mock<IDiscoverFieldDefFactory>();
            discoverFactory.Setup(x => x.GetDiscoverStrategy(It.IsAny<DiscoverStrategy>()))
                .Returns(new FieldDefParserStrategy());

            var settings = Options.Create(new Template {
                DiscoverStrategy = DiscoverStrategy.FieldDefDescriptor,
                TableName ="MyTable"
            });
            var sut = new TestSource(settings, NullLogger<SourceTest>.Instance, discoverFactory.Object, SampleData.FieldDescriptorData());
            sut.Load("imput filename");
            Assert.True(sut.IsLoaded);
            Assert.Equal(settings.Value.TableName, sut.TableName);
            Assert.Equal(5, sut.TableDef.Fields.Count);
            Assert.Collection(sut.TableDef.Keys, x => Assert.Equal("Id", x));
        }

        [Fact]
        public void must_be_loaded_before_read_data() {
            var discoverFactory = new Mock<IDiscoverFieldDefFactory>();
            discoverFactory.Setup(x => x.GetDiscoverStrategy(It.IsAny<DiscoverStrategy>()))
                .Returns(new FieldDefParserStrategy());

            var settings = Options.Create(new Template {
                DiscoverStrategy = DiscoverStrategy.FieldDefDescriptor,
                TableName = "MyTable"
            });
            var sut = new TestSource(settings, NullLogger<SourceTest>.Instance, discoverFactory.Object, SampleData.FieldDescriptorData());
            Assert.Throws<InvalidOperationException>(() => sut.Read());
        }

        [Fact]
        public void must_be_readed_before_get_data() {
            var discoverFactory = new Mock<IDiscoverFieldDefFactory>();
            discoverFactory.Setup(x => x.GetDiscoverStrategy(It.IsAny<DiscoverStrategy>()))
                .Returns(new FieldDefParserStrategy());

            var settings = Options.Create(new Template {
                DiscoverStrategy = DiscoverStrategy.FieldDefDescriptor,
                TableName = "MyTable"
            });
            var sut = new TestSource(settings, NullLogger<SourceTest>.Instance, discoverFactory.Object, SampleData.FieldDescriptorData());
            sut.Load("imput filename");
            Assert.Throws<InvalidOperationException>(() => sut[0]);
        }


        [Fact]
        public void can_read_data() {
            var discoverFactory = new Mock<IDiscoverFieldDefFactory>();
            discoverFactory.Setup(x => x.GetDiscoverStrategy(It.IsAny<DiscoverStrategy>()))
                .Returns(new FieldDefParserStrategy());

            var settings = Options.Create(new Template {
                DiscoverStrategy = DiscoverStrategy.FieldDefDescriptor,
                TableName = "MyTable"
            });
            var sut = new TestSource(settings, NullLogger<SourceTest>.Instance, discoverFactory.Object, SampleData.ReadData());
            sut.Load("imput filename");
            bool more = sut.Read();
            Assert.True(more);
            Assert.Equal(1, sut.AsNumber(0));
            Assert.True(sut.AsBoolean(2));
            Assert.Null(sut.AsString(3));
            Assert.Equal(new DateTime(1970, 2, 24), sut.AsDateTime(4));

            more = sut.Read();
            Assert.True(more);

            more = sut.Read();
            Assert.False(more);

        }
    }


    internal class TestSource : BaseSource
    {

        private (List<string> Header, List<string[]> Data) sampleData;
        public TestSource(IOptions<Template> options,
                          ILogger logger,
                          IDiscoverFieldDefFactory discoverFactory,
                          (List<string> Header, List<string[]> Data) sampleData) :
            base(options, logger, discoverFactory) {

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

        protected override (string[] data, bool more) ReadRow(int rowNumber) {
            string[] data = new string[TableDef.Fields.Count];
            if (rowNumber >= sampleData.Data.Count) {
                return (data, false);
            }
            int cols = TableDef.Fields.Count;
            data = sampleData.Data[rowNumber];
            return (data, true);
        }
    }
}
