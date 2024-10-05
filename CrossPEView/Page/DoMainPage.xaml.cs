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
        // ��ȡ�����Ϣ
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
                m_PDB.Text = "δ�ҵ�PDB��Ϣ";
            }
        }
        else
        {
            m_PDB.Text = "û�е���Ŀ¼��Ϣ";
        }


        // ��ȡͼ��
        IEnumerable<byte[]> icons = pefile.Icons();
        if (icons != null && icons.Any())
        {
            // ��ȡ����ͼ��
            byte[] iconData = icons.OrderByDescending(icon => GetIconSize(icon)).First();

            using (var ms = new MemoryStream(iconData))
            {
                var imageSource = ImageSource.FromStream(() => new MemoryStream(iconData));
                // ��ͼ�����ݼ��ص� Image �ؼ�
                iconImage.Source = imageSource;
            }
        }

    }

    // ������������ȡͼ��ĳߴ�
    private int GetIconSize(byte[] iconData)
    {
        // ����ͼ�����ݸ�ʽΪICO��������ͷ���Ի�ȡͼ��ߴ�
        using (var ms = new MemoryStream(iconData))
        using (var reader = new BinaryReader(ms))
        {
            // ����ICO�ļ�ͷ��
            ms.Seek(4, SeekOrigin.Begin);

            // ��ȡͼ������
            int iconCount = reader.ReadUInt16();

            // ����ÿ��ͼ�����Ϣ
            for (int i = 0; i < iconCount; i++)
            {
                byte width = reader.ReadByte();
                byte height = reader.ReadByte();
                // ��ȡ������Ϣ������ͼ��ߴ磨��������Կ�����Ϊ�ߴ磩
                int size = width * height; // ���Ը�������������㷽ʽ

                // ����ʣ�ಿ���Զ�ȡ��һ��ͼ��
                reader.BaseStream.Seek(16, SeekOrigin.Current); // ��������һ��ͼ�����ʼλ��

                // ����ߴ�������������ش�С
                return size;
            }
        }

        return 0; // Ĭ�Ϸ���0
    }
    // �ļ���С��ʽ������
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