﻿using System;
using PeNet.FileParser;
using PeNet.Header.Net;
using Xunit;

namespace PeNet.Test.Header.Net
{

    public class MetaDataTablesHdrTest
    {
        [Fact]
        public void MetaDataTablesHdrConstructorWorks_Test()
        {
            var metaDataTablesHdr = new MetaDataTablesHdr(new BufferFile(RawDotNetStructures.RawMetaDataTablesHdr), 0x2);

            Assert.Equal((uint) 0x55443322, metaDataTablesHdr.Reserved1);
            Assert.Equal((byte) 0x66, metaDataTablesHdr.MajorVersion);
            Assert.Equal((byte) 0x77, metaDataTablesHdr.MinorVersion);
            Assert.Equal((byte) 0x88, metaDataTablesHdr.HeapSizes);
            Assert.Equal((byte) 0x99, metaDataTablesHdr.Reserved2);
            Assert.Equal(0xffffffffffffffff, (ulong) metaDataTablesHdr.MaskValid);
            Assert.Equal(0xaa99887766554433, metaDataTablesHdr.MaskSorted);
        }

        [Fact]
        public void ReadMetaDataTablesHdrWithoutExtraData()
        {
            var peFile = new PeFile("./Binaries/HelloWorld.exe");
            var tablesStream = peFile.MetaDataStreamTablesHeader!;
            var stringsStream = peFile.MetaDataStreamString!;

            Assert.False(tablesStream.HasExtraData);
            Assert.Null(tablesStream.ExtraData);
            Assert.Equal("HelloWorld.exe", stringsStream.GetStringAtIndex(tablesStream.Tables.Module![0].Name));
        }

        [Fact]
        public void ReadMetaDataTablesHdrWithExtraData()
        {
            var peFile = new PeFile("./Binaries/HelloWorld.TablesStream.ExtraData.exe");
            var tablesStream = peFile.MetaDataStreamTablesHeader!;
            var stringsStream = peFile.MetaDataStreamString!;

            Assert.True(tablesStream.HasExtraData);
            Assert.Equal(12345678u, tablesStream.ExtraData);
            Assert.Equal("HelloWorld.exe", stringsStream.GetStringAtIndex(tablesStream.Tables.Module![0].Name));
        }

        [Fact]
        public void WriteNullExtraDataToHeaderWithoutExtraDataShouldNotThrow()
        {
            var peFile = new PeFile("./Binaries/HelloWorld.exe");
            var tablesStream = peFile.MetaDataStreamTablesHeader!;

            tablesStream.ExtraData = null;
            Assert.Null(tablesStream.ExtraData);
        }

        [Fact]
        public void WriteNonNullExtraDataToHeaderWithExtraDataShouldNotThrow()
        {
            var peFile = new PeFile("./Binaries/HelloWorld.TablesStream.ExtraData.exe");
            var tablesStream = peFile.MetaDataStreamTablesHeader!;

            tablesStream.ExtraData = 1234u;
            Assert.Equal(1234u, tablesStream.ExtraData);
        }

        [Fact]
        public void WriteNonNullExtraDataToHeaderWithoutExtraDataShouldThrow()
        {
            var peFile = new PeFile("./Binaries/HelloWorld.exe");
            var tablesStream = peFile.MetaDataStreamTablesHeader!;

            Assert.Throws<InvalidOperationException>(() => tablesStream.ExtraData = 1234u);
        }

        [Fact]
        public void WriteNullExtraDataToHeaderWithExtraDataShouldThrow()
        {
            var peFile = new PeFile("./Binaries/HelloWorld.TablesStream.ExtraData.exe");
            var tablesStream = peFile.MetaDataStreamTablesHeader!;

            Assert.Throws<InvalidOperationException>(() => tablesStream.ExtraData = null);
        }
    }
}