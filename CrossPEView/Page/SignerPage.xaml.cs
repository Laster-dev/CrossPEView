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
            // ������ʱ�ļ��Ա�ʹ��X509Certificate��
            tempFilePath = Path.GetTempFileName();
            File.WriteAllBytes(tempFilePath, peFileBytes);

            // ���Լ���ǩ��֤��
            X509Certificate2 certificate = null;
            try
            {
                certificate = new X509Certificate2(tempFilePath);
            }
            catch (CryptographicException ex)
            {
                Signer.Text = "����֤��ʱ��������: " + ex.Message;
                return;
            }

            string str = "";

            // ���ǩ����Ϣ
            str += "ǩ����: " + certificate.Subject + "\n";
            str += "�䷢��: " + certificate.Issuer + "\n";

            // ��֤ǩ����Ч��
            bool isValid = VerifySignature(tempFilePath);
            str += "ǩ����Ч��: " + (isValid ? "��Ч" : "��Ч") + "\n";

            Signer.Text = str;
        }
        catch (Exception ex)
        {
            // �����쳣����ʾ������Ϣ
            Signer.Text = "��������: " + ex.Message;
        }
        finally
        {
            // ɾ����ʱ�ļ�
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
            Console.WriteLine("��֤ǩ��ʱ��������: " + ex.Message);
            return false;
        }
    }

}