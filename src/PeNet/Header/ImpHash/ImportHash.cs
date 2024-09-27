﻿using System.Collections.Generic;
using System.Text;
using PeNet.Crypto;
using PeNet.Header.Pe;

namespace PeNet.Header.ImpHash
{
    /// <summary>
    ///     Mandiant’s imphash convention requires the following:
    ///     Resolving ordinals to function names when they appear.
    ///     Converting both DLL names and function names to all lowercase.
    ///     Removing the file extensions from imported module names.
    ///     Building and storing the lowercased strings in an ordered list.
    ///     Generating the MD5 hash of the ordered list.
    ///     oleaut32, ws2_32 and wsock32 can resolve ordinals to functions names.
    ///     The implementation is equal to the python module "pefile" 1.2.10-139
    ///     https://code.google.com/p/pefile/
    /// </summary>
    public class ImportHash
    {
        /// <summary>
        ///     Create an import hash object from the imported functions of a
        ///     PE file.
        /// </summary>
        /// <param name="importedFunctions"></param>
        public ImportHash(ICollection<ImportFunction>? importedFunctions)
        {
            ImpHash = importedFunctions is null ? null : ComputeImpHash(importedFunctions);
        }

        /// <summary>
        ///     The import hash of the PE file as a string.
        /// </summary>
        public string? ImpHash { get; }


        private static string? ComputeImpHash(ICollection<ImportFunction> importedFunctions)
        {
            if (importedFunctions == null || importedFunctions.Count == 0)
                return null;

            var list = new List<string>();
            foreach (var impFunc in importedFunctions)
            {
                var tmp = FormatLibraryName(impFunc.DLL);
                tmp += FormatFunctionName(impFunc);

                list.Add(tmp);
            }

            // Concatenate all imports to one string separated by ','.
            var imports = string.Join(",", list);

            //var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(imports);
            return Hash.ComputeHash(inputBytes, Algorithm.Md5);
        }

        private static string FormatLibraryName(string libraryName)
        {
            var exts = new List<string> {"ocx", "sys", "dll"};
            var parts = libraryName.ToLower().Split('.');
            var libName = "";

#if NET48 || NETSTANDARD2_0
            if (parts.Length > 1 && exts.Contains(parts[parts.Length - 1]))
#else
            if (parts.Length > 1 && exts.Contains(parts[^1]))
#endif
            {
                for (var i = 0; i < parts.Length - 1; i++)
                {
                    libName += parts[i];
                    libName += ".";
                }
            }
            else
            {
                foreach (var p in parts)
                {
                    libName += p;
                    libName += ".";
                }
            }

            return libName;
        }

        private static string FormatFunctionName(ImportFunction impFunc)
        {
            var tmp = "";
            if (impFunc.Name == null) // Import by ordinal
            {
                if (impFunc.DLL.ToLower() == "oleaut32.dll")
                {
                    tmp += OrdinalSymbolMapping.Lookup(OrdinalSymbolMapping.Module.Oleaut32, impFunc.Hint);
                }
                else if (impFunc.DLL.ToLower() == "ws2_32.dll")
                {
                    tmp += OrdinalSymbolMapping.Lookup(OrdinalSymbolMapping.Module.Ws2_32, impFunc.Hint);
                }
                else if (impFunc.DLL.ToLower() == "wsock32.dll")
                {
                    tmp += OrdinalSymbolMapping.Lookup(OrdinalSymbolMapping.Module.Wsock32, impFunc.Hint);
                }
                else // cannot resolve ordinal to a function name
                {
                    tmp += "ord";
                    tmp += impFunc.Hint.ToString();
                }
            }
            else // Import by name
            {
                tmp += impFunc.Name;
            }

            return tmp.ToLower();
        }
    }
}