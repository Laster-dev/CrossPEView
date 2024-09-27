using CommunityToolkit.Maui.Alerts;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using CrossPEView.Page;
namespace CrossPEView.Page
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }
        string PefilePath { get; set; }

        private async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
            // iOS平台，值为"public.my.comic.extension"
            { DevicePlatform.iOS, new[] { "public.my.comic.extension" } },
            // Android平台，允许选择任意文件
            { DevicePlatform.Android, new[] { "*/*" } },
            // WinUI平台，值为".dll", ".exe"
            { DevicePlatform.WinUI, new[] { ".dll", ".exe" } },
            // Tizen平台，值为"*/*"
            { DevicePlatform.Tizen, new[] { "*/*" } },
            // macOS平台，值为"cbr", "cbz"
            { DevicePlatform.macOS, new[] { "cbr", "cbz" } },
                });

                PickOptions options = new()
                {
                    PickerTitle = "选择一个可执行文件",
                    FileTypes = customFileType,
                };

                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    // 检查文件扩展名
                    if (result.FileName.EndsWith("exe", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith("dll", StringComparison.OrdinalIgnoreCase))
                    {
                        using var stream = await result.OpenReadAsync();
                        var image = ImageSource.FromStream(() => stream); // 如果需要显示图像
                    }

                    PefilePath = result.FullPath;

                    await Navigation.PushAsync(new DoMainPage(File.ReadAllBytes(PefilePath)));
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }


        private void GoToSecondPage_Clicked(System.Object sender, System.EventArgs e)
        {

        }
    }

}
