using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Net;
using System.IO;
using Microsoft.Maui.Controls;
using CrossPEView.Page; // 确保使用 Microsoft.Maui.Controls 命名空间
using Uri = Android.Net.Uri;
using Android.Provider;

namespace CrossPEView
{
    [Activity(Theme = "@style/Maui.SplashTheme",
              MainLauncher = true,
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density,
              LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(new[] { Intent.ActionSend, Intent.ActionView },
                  Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
                  DataMimeType = "*/*",    // 支持所有文件类型
                  DataScheme = "file")]     // 支持文件协议
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // 在主线程中处理 Intent
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Intent?.Action == Intent.ActionView)
                {
                    HandleIntent(Intent);
                }
            });
        }

        protected override void OnResume()
        {
            base.OnResume();

            // 在主线程中处理 OnNewIntent
            MainThread.BeginInvokeOnMainThread(() =>
            {
                OnNewIntent(Intent);
            });
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            // 在主线程中处理 Intent
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (intent?.Action == Intent.ActionView)
                {
                    HandleIntent(intent);
                }
            });
        }

        private void HandleIntent(Intent intent)
        {
            try
            {
                // 获取传递的文件 Uri
                Uri fileUri = intent.Data;
                if (fileUri != null)
                {
                    // 将文件读取为 byte[]
                    byte[] fileBytes = ReadFileBytes(fileUri);

                    // 打开 DoMainPage 页面并传递文件字节数组
                    OpenDoMainPage(fileBytes);
                }
            }
            catch (Exception ex)
            {
                // 捕获所有异常并记录日志，防止因异常而导致的崩溃
                Console.WriteLine($"HandleIntent Exception: {ex.Message}");
            }
        }

        private byte[] ReadFileBytes(Uri uri)
        {
            try
            {
                // 获取文件输入流
                using var stream = Platform.CurrentActivity.ContentResolver.OpenInputStream(uri);
                using var memoryStream = new MemoryStream();

                // 将文件内容复制到内存流
                stream.CopyTo(memoryStream);

                // 返回文件的字节数组
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                // 捕获所有异常并记录日志
                Console.WriteLine($"ReadFileBytes Exception: {ex.Message}");
                return Array.Empty<byte>();
            }
        }

        private void OpenDoMainPage(byte[] fileBytes)
        {
            // 在主线程上执行页面导航操作
            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    // 创建 DoMainPage 实例，并传递文件的字节数组
                    var mainPage = new DoMainPage(fileBytes);

                    // 使用 MauiApplication 进行页面导航
                    var navigation = (App.Current as App)?.MainPage?.Navigation;
                    if (navigation != null)
                    {
                        navigation.PushAsync(mainPage);
                    }
                }
                catch (Exception ex)
                {
                    // 捕获所有异常并记录日志
                    
                    Console.WriteLine($"OpenDoMainPage Exception: {ex.Message}");
                }
            });
        }
    }
}
