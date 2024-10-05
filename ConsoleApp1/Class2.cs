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

//namespace ExeDisassembler
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            Console.WriteLine("=== EXE 反汇编工具 ===");

//            if (args.Length == 0)
//            {
//                Console.WriteLine("用法: ExeDisassembler <exe文件路径>");
//                return;
//            }

//            string exePath = args[0];

//            if (!File.Exists(exePath))
//            {
//                Console.WriteLine($"错误: 找不到文件 {exePath}");
//                return;
//            }

//            try
//            {
//                // 读取整个文件为字节数组
//                byte[] fileBytes = File.ReadAllBytes(exePath);

//                // 解析 PE 文件
//                var peFile = new PeFile(exePath);

//                // 输出所有段的特征信息（用于调试）
//                Console.WriteLine("\n--- 段特征信息 ---");
//                foreach (var section in peFile.ImageSectionHeaders)
//                {
//                    Console.WriteLine($"段名: {section.Name}, 特征: {section.Characteristics}");
//                }

//                // 获取所有包含代码的可执行段
//                var execSections = peFile.ImageSectionHeaders
//                    .Where(s =>
//                        s.Characteristics.HasFlag(ScnCharacteristicsType.CntCode) &&
//                        s.Characteristics.HasFlag(ScnCharacteristicsType.MemExecute))
//                    .ToList();

//                if (!execSections.Any())
//                {
//                    Console.WriteLine("\n未在 PE 文件中找到包含代码的可执行段。");
//                    return;
//                }

//                // 自动识别架构并初始化相应的反汇编器
//                CapstoneDisassembler disassembler = null;
//                string architecture = "";

//                // 获取机器类型
//                var machine = peFile.ImageNtHeaders.FileHeader.Machine;
//                switch (machine)
//                {
//                    case MachineType.I386:
//                        // x86 (32-bit)
//                        using (var x86Disassembler = new CapstoneX86Disassembler(X86DisassembleMode.Bit32))
//                        {
//                            x86Disassembler.DisassembleSyntax = DisassembleSyntax.Intel;
//                            x86Disassembler.EnableInstructionDetails = true;
//                            architecture = "x86 (32-bit)";

//                            DisassembleAndPrint(X86DisassembleMode.Bit32, execSections, fileBytes, architecture);
//                        }
//                        break;

//                    case MachineType.Amd64:
//                        // x64 (64-bit)
//                        using (var x64Disassembler = new CapstoneX86Disassembler(X86DisassembleMode.Bit64))
//                        {
//                            x64Disassembler.DisassembleSyntax = DisassembleSyntax.Intel;
//                            x64Disassembler.EnableInstructionDetails = true;
//                            architecture = "x64 (64-bit)";

//                            DisassembleAndPrint(X86DisassembleMode.Bit64, execSections, fileBytes, architecture);
//                        }
//                        break;

//                    default:
//                        Console.WriteLine($"不支持的机器类型: {machine}");
//                        return;
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"反汇编过程中发生错误: {ex.Message}");
//            }
//        }

//        static void DisassembleAndPrint(X86DisassembleMode x86DisassembleMode, List<ImageSectionHeader> execSections, byte[] fileBytes, string architecture)
//        {
            
//            Console.WriteLine($"\n检测到的架构: {architecture}");
//            CapstoneX86Disassembler disassembler = CapstoneDisassembler.CreateX86Disassembler(x86DisassembleMode);
//            foreach (var section in execSections)
//            {
//                Console.WriteLine($"\n--- 段名: {section.Name} ---");

//                long pointerToRawData = section.PointerToRawData;
//                long sizeOfRawData = section.SizeOfRawData;

//                // 检查段数据是否超出文件范围
//                if (pointerToRawData + sizeOfRawData > fileBytes.Length)
//                {
//                    Console.WriteLine($"警告: 段 {section.Name} 的数据超出文件范围，跳过。");
//                    continue;
//                }

//                byte[] sectionData = new byte[sizeOfRawData];
//                Array.Copy(fileBytes, pointerToRawData, sectionData, 0, sizeOfRawData);

//                // 反汇编
//                var instructions = disassembler.Disassemble(sectionData);
//                foreach (var ins in instructions)
//                {
//                    Console.WriteLine($"{ins.Address:X}: \t {ins.Mnemonic} \t {ins.Operand}");
//                }
//            }
//        }
//    }
//}
