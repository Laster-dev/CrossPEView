namespace CrossPEView.Page;

using PeNet;
using System.Security.Cryptography;
using PeNet.Header.Authenticode;

public partial class SignerPage : ContentPage
{
    public SignerPage(PeFile peFileBytes)
    {
        InitializeComponent();
        LoadSignerInfo(peFileBytes);  // ����ǩ����Ϣ
    }

    private void LoadSignerInfo(PeFile peFile)
    {
        // ��ȡ Authenticode ��Ϣ
        var authenticodeInfo = new AuthenticodeInfo(peFile);

        // ���½����ϵ���Ϣ
        UpdateUI(authenticodeInfo);
    }

    private void UpdateUI(AuthenticodeInfo authenticodeInfo)
    {
        // ����ǩ��״̬
        string statusText = authenticodeInfo.IsAuthenticodeValid ? "Valid" : "Not Valid";
        StatusLabel.Text = statusText;
        StatusLabel.TextColor = authenticodeInfo.IsAuthenticodeValid ? Colors.Green : Colors.Red;

        // ����ǩ�������к�
        SerialNumberLabel.Text = authenticodeInfo.SignerSerialNumber ?? "N/A";

        // ��ȡǩ��֤����Ϣ
        var signingCertificate = authenticodeInfo.SigningCertificate;
        SubjectLabel.Text = signingCertificate?.Subject ?? "N/A";
        IssuerLabel.Text = signingCertificate?.Issuer ?? "N/A";
        ThumbprintLabel.Text = signingCertificate?.Thumbprint ?? "N/A";
    }
}
