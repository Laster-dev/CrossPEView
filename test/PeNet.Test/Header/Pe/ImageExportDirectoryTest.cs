﻿using PeNet.FileParser;
using PeNet.Header.Pe;
using Xunit;

namespace PeNet.Test.Header.Pe
{
    
    public class ImageExportDirectoryTest
    {
        [Fact]
        public void ImageExportDirectoryConstructorWorks_Test()
        {
            var exportDirectory = new ImageExportDirectory(new BufferFile(RawStructures.RawExportDirectory), 2);

            Assert.Equal((uint) 0x33221100, exportDirectory.Characteristics);
            Assert.Equal((uint) 0x77665544, exportDirectory.TimeDateStamp);
            Assert.Equal((ushort) 0x9988, exportDirectory.MajorVersion);
            Assert.Equal((ushort) 0xbbaa, exportDirectory.MinorVersion);
            Assert.Equal(0xffeeddcc, exportDirectory.Name);
            Assert.Equal((uint) 0x55443322, exportDirectory.Base);
            Assert.Equal((uint) 0x44332211, exportDirectory.NumberOfFunctions);
            Assert.Equal(0x88776655, exportDirectory.NumberOfNames);
            Assert.Equal(0xccbbaa99, exportDirectory.AddressOfFunctions);
            Assert.Equal((uint) 0x00ffeedd, exportDirectory.AddressOfNames);
            Assert.Equal((uint) 0x55443322, exportDirectory.AddressOfNameOrdinals);
        }
    }
}