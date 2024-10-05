//using System;
//using System.Collections.Generic;
//using System.IO;

//class FullDisassembler
//{
//    // 单字节操作码映射表
//    private static Dictionary<byte, string> opcodeMap = new Dictionary<byte, string>
//    {
//        { 0xB8, "mov eax, imm32" }, // Load immediate value into eax
//        { 0xB9, "mov ecx, imm32" }, // Load immediate value into ecx
//        { 0xBA, "mov edx, imm32" }, // Load immediate value into edx
//        { 0xBB, "mov ebx, imm32" }, // Load immediate value into ebx
//        { 0xC7, "mov reg/mem, imm32" }, // Move immediate value into register/memory
//        { 0x89, "mov reg/mem, reg" },  // Move register to memory or another register
//        { 0x8B, "mov reg, reg/mem" },  // Move memory or another register to register
//        { 0x05, "add eax, imm32" },    // Add immediate value to eax
//        { 0x83, "add/sub/cmp reg/mem, imm8" }, // Add/subtract/compare reg/mem with imm8
//        { 0xE9, "jmp rel32" },         // Jump relative 32-bit offset
//        { 0xEB, "jmp rel8" },          // Jump relative 8-bit offset
//        { 0xC3, "ret" },               // Return from procedure
//        { 0xF4, "hlt" },               // Halt processor
//        { 0x90, "nop" },               // No operation
//        // 更多的单字节指令
//    };

//    // 两字节扩展操作码映射 (0x0F 前缀)
//    private static Dictionary<byte, string> extendedOpcodeMap = new Dictionary<byte, string>
//    {
//        { 0x0F, "extended" },    // 两字节操作码前缀
//        { 0x84, "jz rel32" },    // Jump if zero (32-bit offset)
//        { 0x85, "jnz rel32" },   // Jump if not zero (32-bit offset)
//        { 0xAF, "imul reg, reg/mem" }, // Signed multiply
//        { 0xB6, "movzx reg, reg/mem" }, // Move byte to register with zero extension
//        { 0xBE, "movsx reg, reg/mem" }, // Move byte to register with sign extension
//        // 更多的两字节指令
//    };

//    // 寄存器映射表
//    private static readonly string[] registers32 = { "eax", "ecx", "edx", "ebx", "esp", "ebp", "esi", "edi" };
//    private static readonly string[] registers64 = { "rax", "rcx", "rdx", "rbx", "rsp", "rbp", "rsi", "rdi" };

//    // 前缀字节映射
//    private static Dictionary<byte, string> prefixMap = new Dictionary<byte, string>
//    {
//        { 0xF3, "rep" },   // Repeat string operation
//        { 0xF2, "repne" }, // Repeat string operation while not equal
//        { 0x66, "operand-size override" }, // 16-bit operand size in 32-bit mode
//        { 0x67, "address-size override" }, // 16-bit address size in 32-bit mode
//    };

//    // 反汇编逻辑
//    static void Main(string[] args)
//    {
//        if (args.Length == 0)
//        {
//            Console.WriteLine("请提供要反汇编的exe文件路径。");
//            return;
//        }

//        string filePath = args[0];
//        try
//        {
//            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
//            using (BinaryReader reader = new BinaryReader(fs))
//            {
//                // 简单的PE解析，找到代码段 (text 节)
//                fs.Seek(0x3C, SeekOrigin.Begin);
//                int peHeaderOffset = reader.ReadInt32();  // PE header 的偏移
//                fs.Seek(peHeaderOffset, SeekOrigin.Begin);
//                uint peSignature = reader.ReadUInt32();
//                if (peSignature != 0x00004550)  // 检查是否为 "PE\0\0"
//                {
//                    Console.WriteLine("该文件不是有效的PE文件。");
//                    return;
//                }

//                // 跳过COFF和可选头
//                fs.Seek(0xF8, SeekOrigin.Current); // 节表通常从这里开始
//                reader.ReadBytes(8); // 忽略节名
//                reader.ReadUInt32(); // 忽略 VirtualSize
//                reader.ReadUInt32(); // 忽略 VirtualAddress
//                uint sizeOfRawData = reader.ReadUInt32();
//                uint pointerToRawData = reader.ReadUInt32();

//                // 读取代码段数据
//                fs.Seek(pointerToRawData, SeekOrigin.Begin);
//                byte[] code = reader.ReadBytes((int)sizeOfRawData);
//                byte[] code = File.ReadAllBytes(filePath);
//                // 反汇编机器码
//                Console.WriteLine("反汇编结果: ");
//                int index = 0;
//                while (index < code.Length)
//                {
//                    byte opcode = code[index];

//                    // 检查是否是前缀字节
//                    if (prefixMap.ContainsKey(opcode))
//                    {
//                        Console.WriteLine($"0x{index:X8}: {prefixMap[opcode]}");
//                        index++;
//                        continue;
//                    }

//                    // 检查是否是单字节操作码
//                    if (opcodeMap.ContainsKey(opcode))
//                    {
//                        Console.WriteLine($"0x{index:X8}: {opcodeMap[opcode]}");
//                        index += GetInstructionSize(opcode);  // 跳过指令
//                    }
//                    // 检查是否是两字节操作码 (0x0F 前缀)
//                    else if (opcode == 0x0F)
//                    {
//                        byte nextOpcode = code[index + 1];
//                        if (extendedOpcodeMap.ContainsKey(nextOpcode))
//                        {
//                            Console.WriteLine($"0x{index:X8}: {extendedOpcodeMap[nextOpcode]}");
//                            index += GetInstructionSize(nextOpcode, true);  // 跳过两字节指令
//                        }
//                        else
//                        {
//                            Console.WriteLine($"0x{index:X8}: 未知两字节指令 0x{nextOpcode:X2}");
//                            index += 2;
//                        }
//                    }
//                    else
//                    {
//                        Console.WriteLine($"0x{index:X8}: 未知指令 0x{opcode:X2}");
//                        index++;
//                    }
//                }
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"反汇编时发生错误: {ex.Message}");
//        }
//    }

//    // 获取指令的大小（可以根据操作码进行判断）
//    static int GetInstructionSize(byte opcode, bool isExtended = false)
//    {
//        if (isExtended)
//        {
//            switch (opcode)
//            {
//                case 0x84: return 6;  // jz rel32
//                case 0x85: return 6;  // jnz rel32
//                case 0xAF: return 2;  // imul reg, reg/mem
//                case 0xB6: return 3;  // movzx reg, reg/mem
//                case 0xBE: return 3;  // movsx reg, reg/mem
//                default: return 2;    // 未知指令，假设两字节指令大小
//            }
//        }
//        else
//        {
//            switch (opcode)
//            {
//                case 0xB8: return 5;  // mov eax, imm32
//                case 0x89: return 2;  // mov reg/mem, reg
//                case 0x8B: return 2;  // mov reg, reg/mem
//                case 0x05: return 5;  // add eax, imm32
//                case 0x83: return 3;  // add/sub/cmp/and/or reg/mem, imm8
//                case 0xE9: return 5;  // jmp rel32
//                case 0xEB: return 2;  // jmp rel8
//                case 0xC3: return 1;  // ret
//                case 0xF4: return 1;  // hlt
//                default: return 1;    // 未知指令，假设1字节大小
//            }
//        }
//    }
//}
