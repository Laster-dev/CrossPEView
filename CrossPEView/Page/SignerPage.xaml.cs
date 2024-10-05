namespace CrossPEView.Page;

using PeNet;
using System.Security.Cryptography;
using PeNet.Header.Authenticode;

public partial class SignerPage : ContentPage
{
    public SignerPage(PeFile peFileBytes)
    {
        InitializeComponent();
        LoadSignerInfo(peFileBytes);  // 加载签名信息
    }

    private void LoadSignerInfo(PeFile peFile)
    {
        // 获取 Authenticode 信息
        var authenticodeInfo = new AuthenticodeInfo(peFile);

        // 更新界面上的信息
        UpdateUI(authenticodeInfo);
    }

    private void UpdateUI(AuthenticodeInfo authenticodeInfo)
    {
        // 更新签名状态
        string statusText = authenticodeInfo.IsAuthenticodeValid ? "Valid" : "Not Valid";
        StatusLabel.Text = statusText;
        StatusLabel.TextColor = authenticodeInfo.IsAuthenticodeValid ? Colors.Green : Colors.Red;

        // 更新签名者序列号
        SerialNumberLabel.Text = authenticodeInfo.SignerSerialNumber ?? "N/A";

        // 获取签名证书信息
        var signingCertificate = authenticodeInfo.SigningCertificate;
        SubjectLabel.Text = signingCertificate?.Subject ?? "N/A";
        IssuerLabel.Text = signingCertificate?.Issuer ?? "N/A";
        ThumbprintLabel.Text = signingCertificate?.Thumbprint ?? "N/A";
    }
}
