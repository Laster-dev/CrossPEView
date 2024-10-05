using PeNet;
namespace CrossPEView.Page;
using System.Drawing;
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
            iconImage.Source = "dll.png";
        }
        else if (pefile.IsDriver)
        {
            m_filetype.Text = "Driver";
            iconImage.Source = "dll.png";
        }
        else
        {
            m_filetype.Text = "EXE";
            iconImage.Source = "exe.png";
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
        var debugDirectory = pefile.ImageDebugDirectory;

        if (debugDirectory != null)
        {
            var pdbInfo = debugDirectory
                .FirstOrDefault(idb => idb.CvInfoPdb70 != null)?
                .CvInfoPdb70;

            if (pdbInfo != null)
            {
                m_PDB.Text = pdbInfo.PdbFileName;
            }
            else
            {
                m_PDB.Text = "未找到PDB信息";
            }
        }
        else
        {
            m_PDB.Text = "没有调试目录信息";
        }


        // 获取图标
        IEnumerable<byte[]> icons = pefile.Icons();
        if (icons != null && icons.Any())
        {
            // 获取最大的图标
            byte[] iconData = icons.OrderByDescending(icon => GetIconSize(icon)).First();

            using (var ms = new MemoryStream(iconData))
            {
                var imageSource = ImageSource.FromStream(() => new MemoryStream(iconData));
                // 将图标数据加载到 Image 控件
                iconImage.Source = imageSource;
            }
        }

    }

    // 辅助方法：获取图标的尺寸
    private int GetIconSize(byte[] iconData)
    {
        // 假设图标数据格式为ICO，解析其头部以获取图标尺寸
        using (var ms = new MemoryStream(iconData))
        using (var reader = new BinaryReader(ms))
        {
            // 跳过ICO文件头部
            ms.Seek(4, SeekOrigin.Begin);

            // 读取图标数量
            int iconCount = reader.ReadUInt16();

            // 解析每个图标的信息
            for (int i = 0; i < iconCount; i++)
            {
                byte width = reader.ReadByte();
                byte height = reader.ReadByte();
                // 获取其他信息并计算图标尺寸（这里假设以宽高相乘为尺寸）
                int size = width * height; // 可以根据需求调整计算方式

                // 跳过剩余部分以读取下一个图标
                reader.BaseStream.Seek(16, SeekOrigin.Current); // 跳过到下一个图标的起始位置

                // 如果尺寸符合条件，返回大小
                return size;
            }
        }

        return 0; // 默认返回0
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
        await Navigation.PushAsync(new SignerPage(pefile));
    }

    private async void Button_Clicked_3(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SectionPage(pefile));
    }

    private async void Button_Clicked_4(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AnalysisPage(m_bytes));
    }
}