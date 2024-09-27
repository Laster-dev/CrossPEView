namespace CrossPEView.Page;

using CrossPEView.Control;
using PeNet;
using static CrossPEView.Page.ExportPage;

public partial class SectionPage : ContentPage
{
    PeFile _pefile;
    public SectionPage(PeFile peFile)
    {
        InitializeComponent();
        _pefile = peFile;
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        if (_pefile.ExportedFunctions != null)
        {

            foreach (var section in _pefile.ImageSectionHeaders)
            {
                var function = new ExportedFunction
                {
                    Name = section.Name,
                    VA = section.VirtualAddress,
                    Size = section.VirtualSize,
                };

                var dllView = new DLLView("������"+function.Name, "VA:"+function.VA.ToString(), "��С��"+function.Size.ToString());

                ExpanderStack.Children.Add(dllView);
            }
        }
    }
    public class ExportedFunction
    {
        public string? Name { get; set; }
        public uint VA { get; set; }
        public uint Size { get; set; }
    }
}
