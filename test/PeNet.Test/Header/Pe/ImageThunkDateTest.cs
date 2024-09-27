﻿using PeNet.FileParser;
using PeNet.Header.Pe;
using Xunit;

namespace PeNet.Test.Header.Pe
{
    
    public class IMAGE_THUNK_DATA_Test
    {
        [Fact]
        public void ImageThunkData64ConstructorWorks_Test()
        {
            var thunkData64 = new ImageThunkData(new BufferFile(RawStructures.RawThunkData64), 2, true);

            Assert.Equal((ulong) 0x7766554433221100, thunkData64.AddressOfData);
            Assert.Equal((ulong) 0x7766554433221100, thunkData64.ForwarderString);
            Assert.Equal((ulong) 0x7766554433221100, thunkData64.Function);
            Assert.Equal((ulong) 0x7766554433221100, thunkData64.Ordinal);
        }

        [Fact]
        public void ImageThunkData32ConstructorWorks_Test()
        {
            var thunkData32 = new ImageThunkData(new BufferFile(RawStructures.RawThunkData32), 2, false);

            Assert.Equal((ulong) 0x33221100, thunkData32.AddressOfData);
            Assert.Equal((ulong) 0x33221100, thunkData32.ForwarderString);
            Assert.Equal((ulong) 0x33221100, thunkData32.Function);
            Assert.Equal((ulong) 0x33221100, thunkData32.Ordinal);
        }
    }
}