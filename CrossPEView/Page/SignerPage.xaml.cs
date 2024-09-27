namespace CrossPEView.Page;

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
public partial class SignerPage : ContentPage
{
    public SignerPage(byte[] peFileBytes)
    {
        InitializeComponent();

        string tempFilePath = null;
        try
        {
            // 创建临时文件以便使用X509Certificate类
            tempFilePath = Path.GetTempFileName();
            File.WriteAllBytes(tempFilePath, peFileBytes);

            // 尝试加载签名证书
            X509Certificate2 certificate = null;
            try
            {
                certificate = new X509Certificate2(tempFilePath);
            }
            catch (CryptographicException ex)
            {
                Signer.Text = "加载证书时发生错误: " + ex.Message;
                return;
            }

            string str = "";

            // 输出签名信息
            str += "签名者: " + certificate.Subject + "\n";
            str += "颁发者: " + certificate.Issuer + "\n";

            // 验证签名有效性
            bool isValid = VerifySignature(tempFilePath);
            str += "签名有效性: " + (isValid ? "有效" : "无效") + "\n";

            Signer.Text = str;
        }
        catch (Exception ex)
        {
            // 处理异常并显示错误信息
            Signer.Text = "发生错误: " + ex.Message;
        }
        finally
        {
            // 删除临时文件
            if (tempFilePath != null && File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    public static bool VerifySignature(string filePath)
    {
        try
        {
            X509Certificate2 certificate = new X509Certificate2(filePath);
            X509Chain chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
            chain.ChainPolicy.VerificationTime = DateTime.Now;

            return chain.Build(certificate);
        }
        catch (Exception ex)
        {
            Console.WriteLine("验证签名时发生错误: " + ex.Message);
            return false;
        }
    }

}