﻿using PeNet.FileParser;
using PeNet.Header.Pe;
using Xunit;

namespace PeNet.Test.Header.Pe
{
    
    public class WinCertificateTest
    {
        [Fact]
        public void WinCertificateConstructorWorks_Test()
        {
            var winCertifiacte = new WinCertificate(new BufferFile(RawStructures.RawWinCertificate), 2);
            Assert.Equal((uint) 0x0000000b, winCertifiacte.DwLength);
            Assert.Equal((ushort) 0x5544, winCertifiacte.WRevision);
            Assert.Equal((ushort) 0x7766, (ushort) winCertifiacte.WCertificateType);
            Assert.Equal((byte) 0x11, winCertifiacte.BCertificate[0]);
            Assert.Equal((byte) 0x22, winCertifiacte.BCertificate[1]);
            Assert.Equal((byte) 0x33, winCertifiacte.BCertificate[2]);
        }
    }
}