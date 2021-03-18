using System;
using System.Collections.Generic;
using Xunit;
using CommandLineParser.Configuration;
using CommandLine;

namespace CommandLineParser.Configuration.Test
{
    public class ConfigurationTest
    {

        [Fact]
        public void CmdLineOptionAttribute() {
            var config = new TestConfig {
                Name = "Anne",
                BirthDate = new DateTime(1980, 12, 21)
            };
            config.DeliveryAddress.Street = "Main Street";
            config.DeliveryAddress.PostalCode = "10180";
            config.DeliveryAddress.State = "MyState";

            var sut = new CmdLineOptionAttribute(typeof(TestConfig), "DeliveryAddress", "Street");
            string path = sut.GetPropertyPath();
            Assert.Equal("TestConfig:DeliveryAddress:Street", path);
        }


        [Fact]
        public void BindingTest() {
            var config = new TestConfig {
                Name = "Anne",
                BirthDate = new DateTime(1980, 12, 21)
            };
            config.DeliveryAddress.Street = "Main Street";
            config.DeliveryAddress.PostalCode = "10180";
            config.DeliveryAddress.State = "MyState";


            var cmd = new CmdOptions {
                Name = "Richard",
                DeliveryState = "OtherState"
            };


            var sut = new CommandLineConfigurationProvider<CmdOptions, TestConfig>(cmd);
            sut.Load();

            string value = null;
            Assert.True(sut.TryGet("TestConfig:Name", out value));
            Assert.Equal("Richard", value);
            Assert.True(sut.TryGet("TestConfig:DeliveryAddress:State", out value));
            Assert.Equal("OtherState", value);
        }


        [Fact]
        public void CommandLineMapperTest() {

            var config = new TestConfig {
                Name = "Anne",
                BirthDate = new DateTime(1980, 12, 21)
            };
            config.DeliveryAddress.Street = "Main Street";
            config.DeliveryAddress.PostalCode = "10180";
            config.DeliveryAddress.State = "MyState";

            var cmd = new CmdOptions {
                Name = "Richard",
                DeliveryState = "OtherState"
            };

            var configurator = new CommandLineMapper<CmdOptions, TestConfig>(cmd);
            configurator.Map(x => x.DeliveryState, x => x.DeliveryAddress.State);

            Assert.Equal("TestConfig:DeliveryAddress:State", configurator.Path("DeliveryState"));
            Assert.Equal("OtherState", configurator.Value("DeliveryState"));
        }


        [Fact]
        public void MapperBindingTest() {

            var config = new TestConfig {
                Name = "Anne",
                BirthDate = new DateTime(1980, 12, 21)
            };
            config.DeliveryAddress.Street = "Main Street";
            config.DeliveryAddress.PostalCode = "10180";
            config.DeliveryAddress.State = "MyState";

            var cmd = new CmdOptions {
                Name = "Richard",
                DeliveryState = "OtherState"
            };

            var sut = new CommandLineConfigurationProvider<CmdOptions, TestConfig>(cmd, mapper => {
                mapper
                    .Map(x => x.DeliveryState, x => x.DeliveryAddress.State)
                    .Map(x => x.Name, x => x.Name);
            });
            sut.Load();

            string value = null;
            Assert.True(sut.TryGet("TestConfig:DeliveryAddress:State", out value));
            Assert.Equal("OtherState", value);
            value = null;
            Assert.True(sut.TryGet("TestConfig:Name", out value));
            Assert.Equal("Richard", value);
        }


        [Fact]
        public void MapperWithDefaultOptionValues() {
            var config = new TestConfig {
                Name = "Anne",
                BirthDate = new DateTime(1980, 12, 21),
                DeliveryMethod = DeliveryMethod.Standard
            };
            config.DeliveryAddress.Street = "Main Street";
            config.DeliveryAddress.PostalCode = "10180";
            config.DeliveryAddress.State = "MyState";

            var cmd = new CmdOptions {
                DeliveryState = "OtherState"
            };

            var sut = new CommandLineConfigurationProvider<CmdOptions, TestConfig>(cmd, mapper => {
                mapper
                    .Map(x => x.DeliveryState, x => x.DeliveryAddress.State)
                    .Map(x => x.DeliveryPostalCode, x => x.DeliveryAddress.PostalCode)
                    .Map(x => x.DeliveryMethod, x => x.DeliveryMethod)
                    .Map(x => x.Name, x => x.Name);
            });
            sut.Load();

            string value = null;
            Assert.True(sut.TryGet("TestConfig:DeliveryAddress:State", out value));
            Assert.Equal("OtherState", value);

            //Values not set in CmdOptions are missing in the provider collection. If they exist 
            //in a configuration file they will be added by the configuration provider
            Assert.False(sut.TryGet("TestConfig:DeliveryAddress:PostalCode", out value));
            Assert.False(sut.TryGet("TestConfig:DeliveryMethod", out value));
            Assert.False(sut.TryGet("TestConfig:Name", out value));

        }
    }

    internal class CmdOptions
    {
        [CmdLineOption(typeof(TestConfig), "Name")]
        public string Name { get; set; }

        [Option('d', "delivery-method", Default = DeliveryMethod.Express)]
        [CmdLineOption(typeof(TestConfig), "DeliveryMethod")]
        public DeliveryMethod DeliveryMethod { get; set; } = DeliveryMethod.Express;

        [CmdLineOption(typeof(TestConfig), "DeliveryAddress", "Street")]
        public string DeliveryStreet { get; set; }
        [CmdLineOption(typeof(TestConfig), "DeliveryAddress", "PostalCode")]
        public string DeliveryPostalCode { get; set; }
        [CmdLineOption(typeof(TestConfig), "DeliveryAddress", "State")]
        public string DeliveryState { get; set; }
    }

    internal class TestConfig
    {
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime? SingOutDate { get; set; }
        public Address DeliveryAddress { get; set; } = new Address();
        public DeliveryMethod DeliveryMethod { get; set; }
        public List<string> Tags { get; set; } = new List<string>();

    }

    internal class Address
    {
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
    }

    public enum DeliveryMethod
    {
        Inhouse,
        Standard,
        Express
    }
}
