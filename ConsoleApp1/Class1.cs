using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace StreamResponseApp
{
    class Program
    {
        private static readonly string API_KEY = "KhOviLZnaSeH8SBJnMC3LkO6";
        private static readonly string SECRET_KEY = "SdcaoyUVGVj5P4uhl2OUliFd9CCTPKD6";

        static async Task Main(string[] args)
        {
            try
            {
                string accessToken = await GetAccessToken();
                if (!string.IsNullOrEmpty(accessToken))
                {
                    await AskQuestionAsync(accessToken);
                }
                else
                {
                    Console.WriteLine("获取访问令牌失败。");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
            }
            Console.WriteLine("over。");
            Console.ReadKey();
        }

        private static async Task<string> GetAccessToken()
        {
            using (HttpClient client = new HttpClient())
            {
                //https://aip.baidubce.com/oauth/2.0/token?grant_type=client_credentials&client_id=
                //https://aip.baidubce.com/oauth/2.0/token?grant_type=client_credentials&client_id=
                string url = $"https://aip.baidubce.com/oauth/2.0/token?grant_type=client_credentials&client_id={API_KEY}&client_secret={SECRET_KEY}";
                HttpResponseMessage response = await client.PostAsync(url, null);
                string jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    dynamic json = JObject.Parse(jsonResponse);
                    return json.access_token;
                }
                else
                {
                    Console.WriteLine($"错误: {jsonResponse}");
                    return null;
                }
            }
        }

        private static async Task AskQuestionAsync(string accessToken)
        {
            //https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/completions_pro?access_token=
            //
            string url = $"https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/completions?access_token={accessToken}";
            //string url = $"https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/completions_pro?access_token={accessToken}";

            var payload = new
            {
                messages = new[]
                {
                    new {
                        role = "user",
                        content = "分析一下这个\n" + File.ReadAllText(@"C:\Users\Laster\Desktop\Copilot每日\output.txt"),
                        
                    }
                },
                system = "你是一个非常厉害的WindowsPE文件逆向分析专家，请根据我给出的汇编文件以及其他信息，分析出他的运行逻辑，API调用以及尝试最后把完整的还原成伪代码（c）,写出markdown分级报告，包含WindowsAPI的调用，以及它具体做了什么，不要写的太复杂。",

                disable_search = true,
                stream = true
            };

            string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);

            using (HttpClient client = new HttpClient())
            {
                // 可选: 增加超时时间以适应流式响应
                client.Timeout = TimeSpan.FromMinutes(10);

                using (var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json"))
                {
                    // 构建 HttpRequestMessage
                    var request = new HttpRequestMessage(HttpMethod.Post, url)
                    {
                        Content = content
                    };

                    // 使用 ResponseHeadersRead 选项
                    using (HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();

                        using (var stream = await response.Content.ReadAsStreamAsync())
                        using (var reader = new System.IO.StreamReader(stream))
                        {
                            while (!reader.EndOfStream)
                            {
                                var line = await reader.ReadLineAsync();
                                if (!string.IsNullOrWhiteSpace(line) && line.StartsWith("data:"))
                                {
                                    string jsonData = line.Substring(5).Trim();
                                    if (jsonData == "[DONE]")
                                    {
                                        break;
                                    }

                                    try
                                    {
                                        var jsonObject = JObject.Parse(jsonData);

                                        if (jsonObject["result"] != null)
                                        {
                                            // 逐步输出 'result' 字段内容
                                            Console.Write(jsonObject["result"].ToString());
                                        }

                                        // 可选: 检查是否已结束
                                        if (jsonObject["is_end"] != null && (bool)jsonObject["is_end"] == true)
                                        {
                                            break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"\n解析 JSON 时出错: {ex.Message}");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine(); // 流式输出完成后换行
        }
    }
}
