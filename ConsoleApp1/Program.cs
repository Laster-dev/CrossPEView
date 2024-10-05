//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.IO;
//class Program
//{
//    static void Main()
//    {
//        // 假设 byte[] 数据包含一些字符串和非字符串数据
//        byte[] byteArray = File.ReadAllBytes(@"C:\Users\Laster\Desktop\hfs2.3.exe");
//        // 设置最小字符串长度
//        int minLength = 8;

//        // 提取 ASCII 字符串
//        List<string> asciiStrings = ExtractAsciiStrings(byteArray, minLength);
//        Console.WriteLine("ASCII Strings:");
//        foreach (var s in asciiStrings)
//        {
//            Console.WriteLine(s);
//        }

//        // 提取 Unicode (UTF-16 LE) 字符串
//        List<string> unicodeStrings = ExtractUnicodeStrings(byteArray, minLength);
//        Console.WriteLine("Unicode Strings:");
//        foreach (var s in unicodeStrings)
//        {
//            Console.WriteLine(s);
//        }
//        Console.ReadLine();
//    }

//    static List<string> ExtractAsciiStrings(byte[] data, int minLength)
//    {
//        List<string> result = new List<string>();
//        StringBuilder currentString = new StringBuilder();

//        foreach (byte b in data)
//        {
//            // 判断是否为可打印的 ASCII 字符（32到126之间）
//            if (b >= 32 && b <= 126)
//            {
//                currentString.Append((char)b);
//            }
//            else
//            {
//                // 如果当前字符串长度符合最小要求，添加到结果
//                if (currentString.Length >= minLength)
//                {
//                    result.Add(currentString.ToString());
//                }
//                currentString.Clear();
//            }
//        }

//        // 检查最后一个字符串
//        if (currentString.Length >= minLength)
//        {
//            result.Add(currentString.ToString());
//        }

//        return result;
//    }

//    static List<string> ExtractUnicodeStrings(byte[] data, int minLength)
//    {
//        List<string> result = new List<string>();
//        StringBuilder currentString = new StringBuilder();

//        // 处理字节对（UTF-16 LE：每个字符由2个字节组成）
//        for (int i = 0; i < data.Length - 1; i += 2)
//        {
//            // 组合低字节和高字节，形成一个 UTF-16 字符
//            ushort unicodeChar = BitConverter.ToUInt16(data, i);

//            // 判断是否为可打印的 Unicode 字符（包括标准的ASCII范围）
//            if (unicodeChar >= 32 && unicodeChar <= 126)
//            {
//                currentString.Append((char)unicodeChar);
//            }
//            else
//            {
//                // 如果当前字符串长度符合最小要求，添加到结果
//                if (currentString.Length >= minLength)
//                {
//                    result.Add(currentString.ToString());
//                }
//                currentString.Clear();
//            }
//        }

//        // 检查最后一个字符串
//        if (currentString.Length >= minLength)
//        {
//            result.Add(currentString.ToString());
//        }

//        return result;
//    }
//}
