using PeNet;
namespace CrossPEView.Page;

public partial class DoMainPage : ContentPage
{
    PeFile pefile { get; set; }
    public DoMainPage(byte[] bytes)
	{
		InitializeComponent();
        pefile = new PeFile(bytes);
    }
    private async void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        
        await Navigation.PushAsync(new ImportPage(pefile));

    }
}