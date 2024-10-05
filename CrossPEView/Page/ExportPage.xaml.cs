namespace CrossPEView.Page;
using CrossPEView.Control;
using PeNet;


public partial class ExportPage : ContentPage
{
    PeFile _pefile;
    public ExportPage(PeFile peFile)
    {
        InitializeComponent();
        _pefile = peFile;
        LoadSignerInfo();
    }
    private void LoadSignerInfo()
    {
        if (_pefile.ExportedFunctions != null)
        {
            foreach (var exportFunction in _pefile.ExportedFunctions)
            {
                var function = new ExportedFunction
                {
                    FunctionName = exportFunction.Name,
                    ordinal = exportFunction.Ordinal,
                    Address = exportFunction.Address,
                };

                var dllView = new DLLView(function.FunctionName,
                                          "–Ú∫≈£∫" + function.ordinal.ToString(),
                                          "µÿ÷∑:" + function.Address.ToString());

                ExpanderStack.Children.Add(dllView);
            }
        }
    }
    

    public class ExportedFunction
    {
        public string? FunctionName { get; set; }
        public ushort ordinal { get; set; }
        public uint Address { get; set; }
    }
}