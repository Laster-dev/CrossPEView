using PeNet;
namespace CrossPEView.Page;
using System.Security.Cryptography.X509Certificates;
public partial class DoMainPage : ContentPage
{
    PeFile pefile { get; set; }
    byte[] m_bytes;
    public DoMainPage(byte[] bytes)
    {
        InitializeComponent();
        m_bytes = bytes;
        pefile = new PeFile(bytes);

        // 获取相关信息
        m_filesize.Text = FormatFileSize(pefile.FileSize);

        if (pefile.IsDll)
        {
            m_filetype.Text = "DLL";
        }
        else if (pefile.IsDriver)
        {
            m_filetype.Text = "Driver";
        }
        else
        {
            m_filetype.Text = "EXE";
            var signature = m_filetype.AutomationId;
        }

        m_targetMachine.Text = pefile.Is32Bit ? "i386" : "AMD64";
        m_timestamp.Text = DateTimeOffset.FromUnixTimeSeconds(pefile.ImageNtHeaders.FileHeader.TimeDateStamp).DateTime.ToString("yyyy-MM-dd HH:mm:ss");
        m_baseImage.Text = "0x"+pefile.ImageNtHeaders.OptionalHeader.ImageBase.ToString("X");
        m_checksum.Text = $"0x{pefile.ImageNtHeaders.OptionalHeader.CheckSum:X}";

        m_subsystem.Text = pefile.ImageNtHeaders.OptionalHeader.Subsystem.ToString();
        m_subsystemVersion.Text = pefile.ImageNtHeaders.OptionalHeader.MajorSubsystemVersion.ToString();
        m_MD5.Text = $"Md5:\n{pefile.Md5}";;
        m_sha1.Text = $"Sha1:\n{pefile.Sha1}";
        m_sha256.Text = $"Sha256: \n{pefile.Sha256}";

    }

    // 文件大小格式化方法
    private string FormatFileSize(long size)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = size;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
    private async void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        
        await Navigation.PushAsync(new ImportPage(pefile));


    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ExportPage(pefile));
    }

    private async void Button_Clicked_2(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SignerPage(m_bytes));
    }

    private async void Button_Clicked_3(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SectionPage(pefile));
    }
}