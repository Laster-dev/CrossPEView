//using Gee.External.Capstone.X86;
//using System;
//using System.IO;
//using System.Linq;
//using PeNet;
//using PeNet.Header.Pe;
//using Gee.External.Capstone;
//using Gee.External.Capstone.Arm;
//using Gee.External.Capstone.Arm64;
//using Gee.External.Capstone.X86;
//using System.Collections.Generic;
//using Microsoft.SqlServer.Server;

//namespace CrossPEView.Helper
//{
//    internal class CapstoneHelper
//    {
//        public static void Main() {
//            CapstoneHelper capstoneHelper = new CapstoneHelper(File.ReadAllBytes(@"C:\Users\Laster\Desktop\1.bin.exe"));
//            Console.WriteLine(capstoneHelper.GetASM());
//        }


        
//        static byte[] peFilebyte;
//        public CapstoneHelper(byte[] _peFilebyte)
//        {
//            peFilebyte = _peFilebyte;
//        }
//        public string GetASM()
//        {
//            string output = "";
//            string architecture = "";
//            PeFile peFile = new PeFile(peFilebyte);
//            // 输出所有段的特征信息（用于调试）
//            output += ("\n--- 段特征信息 ---") + "\n";
//            foreach (var section in peFile.ImageSectionHeaders)
//            {
//                output +=($"段名: {section.Name}, 特征: {section.Characteristics}") + "\n";
//            }

//            // 获取所有包含代码的可执行段
//            var execSections = peFile.ImageSectionHeaders
//                .Where(s =>
//                    s.Characteristics.HasFlag(ScnCharacteristicsType.CntCode) &&
//                    s.Characteristics.HasFlag(ScnCharacteristicsType.MemExecute))
//                .ToList();

//            if (!execSections.Any())
//            {
//                return ("\n未在 PE 文件中找到包含代码的可执行段。");
//            }
//            // 获取机器类型
//            var machine = peFile.ImageNtHeaders.FileHeader.Machine;
//            switch (machine)
//            {
//                case MachineType.I386:
//                    // x86 (32-bit)
//                    using (var x86Disassembler = new CapstoneX86Disassembler(X86DisassembleMode.Bit32))
//                    {
//                        x86Disassembler.DisassembleSyntax = DisassembleSyntax.Intel;
//                        x86Disassembler.EnableInstructionDetails = true;
//                        architecture = "x86 (32-bit)";

//                        output += DisassembleAndPrint(X86DisassembleMode.Bit32, execSections, peFilebyte, architecture) + "\n";
//                    }
//                    break;

//                case MachineType.Amd64:
//                    // x64 (64-bit)
//                    using (var x64Disassembler = new CapstoneX86Disassembler(X86DisassembleMode.Bit64))
//                    {
//                        x64Disassembler.DisassembleSyntax = DisassembleSyntax.Intel;
//                        x64Disassembler.EnableInstructionDetails = true;
//                        architecture = "x64 (64-bit)";

//                        output += DisassembleAndPrint(X86DisassembleMode.Bit64, execSections, peFilebyte, architecture) + "\n";
//                    }
//                    break;

//                default:
//                    return ($"不支持的机器类型: {machine}");
//            }
//            return (output);
//        }
//        static string DisassembleAndPrint(X86DisassembleMode x86DisassembleMode, List<ImageSectionHeader> execSections, byte[] fileBytes, string architecture)
//        {
//            string output = "";
//            output += ($"\n检测到的架构: {architecture}") + "\n";
//            CapstoneX86Disassembler disassembler = CapstoneDisassembler.CreateX86Disassembler(x86DisassembleMode);
//            foreach (var section in execSections)
//            {
//                output += ($"\n--- 段名: {section.Name} ---") + "\n";

//                long pointerToRawData = section.PointerToRawData;
//                long sizeOfRawData = section.SizeOfRawData;

//                // 检查段数据是否超出文件范围
//                if (pointerToRawData + sizeOfRawData > fileBytes.Length)
//                {
//                    output += ($"警告: 段 {section.Name} 的数据超出文件范围，跳过。") + "\n";
//                    continue;
//                }

//                byte[] sectionData = new byte[sizeOfRawData];
//                Array.Copy(fileBytes, pointerToRawData, sectionData, 0, sizeOfRawData);

//                // 反汇编
//                var instructions = disassembler.Disassemble(sectionData, (long)section.VirtualAddress);
//                foreach (var ins in instructions)
//                {
//                    output += ($"{ins.Address:X}: \t {ins.Mnemonic} \t {ins.Operand}")+"\n";
//                }
//            }
//            return output;
//        }
//    }
//}
