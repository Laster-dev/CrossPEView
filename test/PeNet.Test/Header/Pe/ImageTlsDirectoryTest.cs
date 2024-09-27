﻿using PeNet.FileParser;
using PeNet.Header.Pe;
using Xunit;

namespace PeNet.Test.Header.Pe
{
    
    public class ImageTlsDirectoryTest
    {
        [Fact]
        public void ImageTlsDirectory64ConstructorWorks_Test()
        {
            var tlsDirectory = new ImageTlsDirectory(new BufferFile(RawStructures.RawTlsDirectory64), 2, true);

            Assert.Equal((ulong) 0x7766554433221100, tlsDirectory.StartAddressOfRawData);
            Assert.Equal((ulong) 0xbbaa998877665544, tlsDirectory.EndAddressOfRawData);
            Assert.Equal((ulong) 0x221100ffeeddccbb, tlsDirectory.AddressOfIndex);
            Assert.Equal((ulong) 0xaa99887766554433, tlsDirectory.AddressOfCallBacks);
            Assert.Equal((uint) 0x44332211, tlsDirectory.SizeOfZeroFill);
            Assert.Equal((uint) 0x99887766, tlsDirectory.Characteristics);
        }

        [Fact]
        public void ImageTlsDirectory32ConstructorWorks_Test()
        {
            ImageTlsDirectory tlsDirectory = new ImageTlsDirectory(new BufferFile(RawStructures.RawTlsDirectory32), 2, false);

            Assert.Equal((ulong)0x33221100, tlsDirectory.StartAddressOfRawData);
            Assert.Equal((ulong)0x77665544, tlsDirectory.EndAddressOfRawData);
            Assert.Equal((ulong)0xeeddccbb, tlsDirectory.AddressOfIndex);
            Assert.Equal((ulong)0x66554433, tlsDirectory.AddressOfCallBacks);
            Assert.Equal((uint)0x44332211, tlsDirectory.SizeOfZeroFill);
            Assert.Equal((uint)0x99887766, tlsDirectory.Characteristics);
        }
    }
}