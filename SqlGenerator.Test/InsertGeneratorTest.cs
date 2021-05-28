using SqlGenerator.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Moq;


namespace SqlGenerator.Test
{
    public class InsertGeneratorTest
    {
        [Fact]
        public void basic_insert() {
            var source = new Mock<IReader>();
            source.Setup(x => x.Read())
                .Returns(false);
            source.Setup(x => x.AsNumber(0))
                .Returns(10);
            source.Setup(x => x.AsString(1))
                .Returns("John's house");
            source.Setup(x => x.AsBoolean(2))
                .Returns(true);
            var tbDef = new TableDef {
                TableName = "TestTable"
            };
            tbDef.Fields.AddRange(new List<FieldDef> {
                new FieldDef {
                    Name = "Id",
                    FieldType = FieldType.Numeric,
                    IsKey = true,
                    IsNullable = false,
                    OrdinalPosition = 0
                },
                new FieldDef {
                    Name = "Name",
                    FieldType = FieldType.Text,
                    OrdinalPosition = 1
                },
                new FieldDef {
                    Name = "Enabled",
                    FieldType = FieldType.Bool,
                    OrdinalPosition = 2
                }
            });
            var sut = new InsertGenerator(source.Object, NullLogger<InsertGenerator>.Instance, tbDef);
            string sql = sut.GenerateSQL();

            Assert.Equal("INSERT INTO [TestTable] ([Id], [Name], [Enabled]) VALUES (10, 'John''s house', 1);", sql);
        }

        [Fact]
        public void empty_source_generates_empty_output() {
            var sw = new System.IO.StringWriter();

            var source = new Mock<IReader>();
            source.Setup(x => x.Read())
                .Returns(false);
            var tbDef = new TableDef {
                TableName = "TestTable"
            };
            tbDef.Fields.AddRange(new List<FieldDef> {
                new FieldDef {
                    Name = "Id",
                    FieldType = FieldType.Numeric,
                    IsKey = true,
                    IsNullable = false,
                    OrdinalPosition = 0
                },
                new FieldDef {
                    Name = "Name",
                    FieldType = FieldType.Text,
                    OrdinalPosition = 1
                },
                new FieldDef {
                    Name = "Enabled",
                    FieldType = FieldType.Bool,
                    OrdinalPosition = 2
                }
            });

            var sut = new InsertGenerator(source.Object, NullLogger<InsertGenerator>.Instance, tbDef);
            sut.Generate(sw);

            Assert.Equal(string.Empty, sw.ToString());

        }


        [Fact]
        public void n_rows_source_generates_n_lines_output() {
            var sw = new System.IO.StringWriter();

            var source = new Mock<IReader>();
            source.SetupSequence(x => x.Read())
                .Returns(true)
                .Returns(true)
                .Returns(false);
            source.SetupSequence(x => x.AsNumber(0))
                .Returns(10)
                .Returns(11);
            source.SetupSequence(x => x.AsString(1))
                .Returns("John's house")
                .Returns("Moe tavern");
            source.SetupSequence(x => x.AsBoolean(2))
                .Returns(true)
                .Returns(false);


            var tbDef = new TableDef {
                TableName = "TestTable"
            };
            tbDef.Fields.AddRange(new List<FieldDef> {
                new FieldDef {
                    Name = "Id",
                    FieldType = FieldType.Numeric,
                    IsKey = true,
                    IsNullable = false,
                    OrdinalPosition = 0
                },
                new FieldDef {
                    Name = "Name",
                    FieldType = FieldType.Text,
                    OrdinalPosition = 1
                },
                new FieldDef {
                    Name = "Enabled",
                    FieldType = FieldType.Bool,
                    OrdinalPosition = 2
                }
            });

            var sut = new InsertGenerator(source.Object, NullLogger<InsertGenerator>.Instance, tbDef);
            sut.Generate(sw);
            string[] lines = sw.ToString().Split(sw.NewLine, StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(2, lines.Length);

        }



        [Fact]
        public void source_with_null_values() {
            var sw = new System.IO.StringWriter();

            var source = new Mock<IReader>();
            source.SetupSequence(x => x.Read())                     //Will return 2 records
                .Returns(true)
                .Returns(true)
                .Returns(false);
            source.Setup(x => x.AsNumber(0))                        //Setup values for first record
                .Returns(10);
            source.Setup(x => x.AsString(1))
                .Returns("John's house");
            source.Setup(x => x.AsBoolean(2))
                .Returns(true);
            source.SetupSequence(x => x.IsNull(It.IsAny<int>()))    //The IsNull method is called for each nullable field before try to convert the value
                .Returns(false)                                     //First row has values
                .Returns(false)
                .Returns(false)
                .Returns(true)                                      //Second row has all fields NULL
                .Returns(true)
                .Returns(true);

            var tbDef = new TableDef {
                TableName = "TestTable"
            };
            tbDef.Fields.AddRange(new List<FieldDef> {              //Al fiellds defined as nullable for this test
                new FieldDef {
                    Name = "Id",
                    FieldType = FieldType.Numeric,
                    OrdinalPosition = 0
                },
                new FieldDef {
                    Name = "Name",
                    FieldType = FieldType.Text,
                    OrdinalPosition = 1
                },
                new FieldDef {
                    Name = "Enabled",
                    FieldType = FieldType.Bool,
                    OrdinalPosition = 2
                }
            });

            var sut = new InsertGenerator(source.Object, NullLogger<InsertGenerator>.Instance, tbDef);
            sut.Generate(sw);
            string[] lines = sw.ToString().Split(sw.NewLine, StringSplitOptions.RemoveEmptyEntries);
            Assert.Collection(lines,
                x => Assert.Equal("INSERT INTO [TestTable] ([Id], [Name], [Enabled]) VALUES (10, 'John''s house', 1);", x),
                x => Assert.Equal("INSERT INTO [TestTable] ([Id], [Name], [Enabled]) VALUES (NULL, NULL, NULL);", x));

        }

        [Fact]
        public void long_text_is_truncated_at_maxLenght_position() {
            var sw = new System.IO.StringWriter();

            var source = new Mock<IReader>();
            source.SetupSequence(x => x.Read())                     //Will return 1 record
                .Returns(true)
                .Returns(true)
                .Returns(false);
            source.SetupSequence(x => x.AsNumber(0))                       
                .Returns(1)
                .Returns(2);
            source.SetupSequence(x => x.AsString(1))
                .Returns("0123456789")
                .Returns("012");


            var tbDef = new TableDef {
                TableName = "TestTable"
            };
            tbDef.Fields.AddRange(new List<FieldDef> {              //Al fiellds defined as nullable for this test
                new FieldDef {
                    Name = "Id",
                    FieldType = FieldType.Numeric,
                    OrdinalPosition = 0
                },
                new FieldDef {
                    Name = "Name",
                    FieldType = FieldType.Text,
                    MaxLength = 5,
                    OrdinalPosition = 1
                }
            });

            var sut = new InsertGenerator(source.Object, NullLogger<InsertGenerator>.Instance, tbDef);
            sut.Generate(sw);
            string[] lines = sw.ToString().Split(sw.NewLine, StringSplitOptions.RemoveEmptyEntries);
            Assert.Collection(lines,
                x => Assert.Equal("INSERT INTO [TestTable] ([Id], [Name]) VALUES (1, '01234');", x),
                x => Assert.Equal("INSERT INTO [TestTable] ([Id], [Name]) VALUES (2, '012');", x));
        }

    }
}

