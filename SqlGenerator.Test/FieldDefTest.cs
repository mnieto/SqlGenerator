using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlGenerator;
using SqlGenerator.Discover;
using Xunit;

namespace SqlGenerator.Test
{
    public class FieldDefTest
    {

        [Fact]
        public void parse_text_with_invalid_size() {
            string test = "t|name|forty";

            var result = GetResult(test);
            Assert.Contains(result.Errors, x => x.Code == FieldDefParserStrategy.ERR_TEXT_ATTR);
            Assert.False(result.Success);
        }

        [Fact]
        public void parse_text_without_size() {
            string test = "t|name";

            var result = GetResult(test);
            Assert.Equal("name", result.Field.Name);
            Assert.Equal(FieldType.Text, result.Field.FieldType);
            Assert.False(result.Field.IsKey);
            Assert.True(result.Field.IsNullable);
            Assert.Equal(0, result.Field.MaxLength);
            Assert.Null(result.Field.Format);
            
        }

        [Fact]
        public void parse_text_with_size() {
            string test = "t|name|40";

            var result = GetResult(test);
            Assert.Equal("name", result.Field.Name);
            Assert.Equal(FieldType.Text, result.Field.FieldType);
            Assert.False(result.Field.IsKey);
            Assert.True(result.Field.IsNullable);
            Assert.Equal(40, result.Field.MaxLength);
            Assert.Null(result.Field.Format);
        }


        [Fact]
        public void parse_text_with_size_and_key_field() {
            string test = "kt|name|40";

            var result = GetResult(test);
            Assert.Equal("name", result.Field.Name);
            Assert.Equal(FieldType.Text, result.Field.FieldType);
            Assert.True(result.Field.IsKey);
            Assert.True(result.Field.IsNullable);
            Assert.Equal(40, result.Field.MaxLength);
            Assert.Null(result.Field.Format);
        }


        [Fact]
        public void parse_numeric_not_null_and_key_field() {
            string test = "kn|nn|name";

            var result = GetResult(test);
            Assert.Equal("name", result.Field.Name);
            Assert.Equal(FieldType.Numeric, result.Field.FieldType);
            Assert.True(result.Field.IsKey);
            Assert.False(result.Field.IsNullable);
            Assert.Equal(0, result.Field.MaxLength);
            Assert.Null(result.Field.Format);
        }

        [Fact]
        public void parse_numeric_nullable_with_error_attributes() {
            string test = "n|n|name|4";

            var result = GetResult(test);
            Assert.Contains(result.Errors, x => x.Code == FieldDefParserStrategy.ERR_NUMERIC_ATTR);
        }


        [Fact]
        public void parse_bool_not_null_with_valid_attributes() {
            string test = "b|nn|name|Yes/No";

            var result = GetResult(test);
            Assert.Equal("name", result.Field.Name);
            Assert.Equal(FieldType.Bool, result.Field.FieldType);
            Assert.False(result.Field.IsKey);
            Assert.False(result.Field.IsNullable);
            Assert.Equal(0, result.Field.MaxLength);
            Assert.Equal("Yes/No", result.Field.Format);
        }


        [Fact]
        public void parse_bool_not_null_with_invalid_attributes() {
            string test = "b|nn|name|01";

            var result = GetResult(test);
            Assert.Contains(result.Errors, x => x.Code == FieldDefParserStrategy.ERR_BOOL_ATTR);
        }


        private ParseResult GetResult(string test) {
            var sut = new FieldDefParserStrategy();
            return sut.GetFieldDef(test);
        }

        [Fact]
        public void tokenize() {
            string test = "one|two|three";
            FieldDefParserStrategy sut = new FieldDefParserStrategy();
            Assert.Collection(sut.Tokenize(test),
                x => Assert.Equal("one", x),
                x => Assert.Equal("two", x), 
                x => Assert.Equal("three", x)
            );
        }

        [Fact]
        public void tokenize_with_escaped_char() {
            string test = "one|two|th||ree";
            FieldDefParserStrategy sut = new FieldDefParserStrategy();
            Assert.Collection(sut.Tokenize(test),
                x => Assert.Equal("one", x),
                x => Assert.Equal("two", x),
                x => Assert.Equal("th|ree", x)
            );
        }


        [Fact]
        public void tokenize_with__starting_empty_item() {
            string test = "|one|two|three";
            FieldDefParserStrategy sut = new FieldDefParserStrategy();
            Assert.Collection(sut.Tokenize(test),
                x => Assert.Equal("one", x),
                x => Assert.Equal("two", x),
                x => Assert.Equal("three", x)
            );
        }


        [Fact]
        public void tokenize_with__trailing_empty_item() {
            string test = "|one|two|three|";
            FieldDefParserStrategy sut = new FieldDefParserStrategy();
            Assert.Collection(sut.Tokenize(test),
                x => Assert.Equal("one", x),
                x => Assert.Equal("two", x),
                x => Assert.Equal("three", x)
            );
        }

    }
}
