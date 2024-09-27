using Microsoft.Maui.Controls;

namespace CrossPEView.Control
{
    public partial class DLLView : ContentView
    {
        public DLLView(string Name, string Hint, string IATOffset)
        {
            InitializeComponent();
            m_Name.Text = Name; 
            ID.Text = Hint;
            m_IATOffset.Text = IATOffset;
        }
    }
}
