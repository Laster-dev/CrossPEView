﻿using System;
using System.Collections.Generic;
using PeNet.FileParser;

namespace PeNet.Header.Resource
{
    /// <summary>
    ///     Collection of GroupIconDirectoryEntries.
    /// </summary>
    public class GroupIconDirectory : AbstractStructure
    {
        private bool _entriesParsed;
        private readonly long _sizeInBytes;
        private List<GroupIconDirectoryEntry>? _directoryEntries;

        /// <summary>
        ///     Create a GroupIconDirectory instance.
        /// </summary>
        /// <param name="peFile">A PE file.</param>
        /// <param name="offset">Offset of a GroupIconDirectory in the PE file.</param>
        public GroupIconDirectory(IRawFile peFile, ResourceLocation location)
            : base(peFile, location.Offset)
        {
            _sizeInBytes = location.Size;
        }

        /// <summary>
        ///     Always zero.
        /// </summary>
        public ushort IdReserved
        {
            get => PeFile.ReadUShort(Offset);
            set => PeFile.WriteUShort(Offset, value);
        }

        /// <summary>
        ///     Always one for icons.
        /// </summary>
        public ushort IdType
        {
            get => PeFile.ReadUShort(Offset + 0x2);
            set => PeFile.WriteUShort(Offset + 0x2, value);
        }

        /// <summary>
        ///     Contains the number of included GroupIconDirectoryEntries.
        /// </summary>
        public ushort IdCount
        {
            get => PeFile.ReadUShort(Offset + 0x4);
            set => PeFile.WriteUShort(Offset + 0x4, value);
        }

        /// <summary>
        ///     Array with the different directory entries.
        /// </summary>
        public IEnumerable<GroupIconDirectoryEntry> DirectoryEntries
        {
            get
            {
                if (_entriesParsed)
                    return _directoryEntries.OrEmpty();

                _entriesParsed = true;
                return _directoryEntries = ParseDirectoryEntries();
            }
        }

        private List<GroupIconDirectoryEntry> ParseDirectoryEntries()
        {
            var numEntries = IdCount;
            var currentOffset = Offset + 0x6;
            var maxOffset = Math.Min(PeFile.Length, Offset + _sizeInBytes);
            if (currentOffset + numEntries * GroupIconDirectoryEntry.Size > maxOffset)
            {   // Max number of GroupIconDirectoryEntry that fit into the GroupIconDirectory frame.
                numEntries = (ushort)((maxOffset - currentOffset) / GroupIconDirectoryEntry.Size);
            }
            var parsedArray = new List<GroupIconDirectoryEntry>();
            for (ushort i = 0; i < numEntries; ++i)
            {
                parsedArray.Add(new GroupIconDirectoryEntry(PeFile, currentOffset));
                currentOffset += GroupIconDirectoryEntry.Size;
            }

            return parsedArray;
        }
    }
}
