using Microsoft.Extensions.Logging.Abstractions;
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
    public class DataTypeSamplingTest {
        [Fact]
        public void auto_field() {
            var fld = GetFieldDef(0);
            Assert.Equal(FieldType.Text, fld.FieldType);
            Assert.Null(fld.Format);
        }

        [Fact]
        public void numeric_field() {
            var fld = GetFieldDef(1);
            Assert.Equal(FieldType.Numeric, fld.FieldType);
            Assert.Null(fld.Format);
        }

        [Fact]
        public void bool_field() {
            var fld = GetFieldDef(2);
            Assert.Equal(FieldType.Bool, fld.FieldType);
            Assert.Equal("T/F", fld.Format);
        }

        [Fact]
        public void text_field() {
            var fld = GetFieldDef(3);
            Assert.Equal(FieldType.Text, fld.FieldType);
            Assert.Null(fld.Format);
        }

        [Fact]
        public void date_field() {
            var fld = GetFieldDef(4);
            Assert.Equal(FieldType.DateTime, fld.FieldType);
            Assert.Null(fld.Format);
        }

        private FieldDef GetFieldDef(int i) {
            var (header, data) = SampleData.SamplingData();
            var sut = new SamplingStrategy(new NullLogger<SamplingStrategy>());
            return sut.GetFieldDef(header[i], data.Select(x => x[i])).Field;

        }

    }

}
