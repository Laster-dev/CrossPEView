﻿using PeNet.FileParser;
using PeNet.Header.Pe;
using Xunit;

namespace PeNet.Test.Header.Pe
{

    public class ImageResourceDirectoryTest
    {
        [Fact]
        public void ImageResourceDirectoryConstructorWorks_Test()
        {
            var fakeParent = new ImageResourceDirectoryEntry(null, null, 0, 0);

            var resourceDirectory = new ImageResourceDirectory(new BufferFile(RawStructures.RawResourceDirectory), fakeParent, 2, 2, RawStructures.RawResourceDirectory.Length);

            Assert.Equal((uint) 0x33221100, resourceDirectory.Characteristics);
            Assert.Equal((uint) 0x77665544, resourceDirectory.TimeDateStamp);
            Assert.Equal((ushort) 0x9988, resourceDirectory.MajorVersion);
            Assert.Equal((ushort) 0xbbaa, resourceDirectory.MinorVersion);
            Assert.Equal((ushort) 0x0001, resourceDirectory.NumberOfNameEntries);
            Assert.Equal((ushort) 0x0001, resourceDirectory.NumberOfIdEntries);
            Assert.Equal((uint) 0x44332211, resourceDirectory.DirectoryEntries[0].Name);
            Assert.Equal((uint) 0x00000011, resourceDirectory.DirectoryEntries[0].OffsetToData);
            Assert.Equal((uint) 0x44332222 & 0xFFFF, resourceDirectory.DirectoryEntries[1].ID);
            Assert.Equal((uint) 0x00000022, resourceDirectory.DirectoryEntries[1].OffsetToData);
            Assert.Equal(fakeParent, resourceDirectory.Parent);
        }
    }
}
