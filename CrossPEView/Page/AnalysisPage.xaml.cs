namespace CrossPEView.Page;
using Helper;
using PeNet;

public partial class AnalysisPage : ContentPage
{
    byte[] _filebytes;

    public AnalysisPage(byte[] filebytes)
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
        CapstoneHelper capstoneHelper = new CapstoneHelper(_filebytes);
        outstringlabel.Text = capstoneHelper.GetASM();
    }
}