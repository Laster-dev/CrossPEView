﻿using PeNet.FileParser;

namespace PeNet.Header.Net.MetaDataTables
{
    public class Param : AbstractTable
    {
        public Param(IRawFile peFile, long offset, HeapSizes heapSizes, IndexSize indexSizes) 
            : base(peFile, offset, heapSizes, indexSizes)
        {
            Flags = (ushort) ReadSize(2);
            Sequence = (ushort) ReadSize(2);
            Name = ReadSize(HeapSizes.String);
        }

        public ushort Flags {get;}
        public ushort Sequence {get;}
        public uint Name {get;}
    }
}
