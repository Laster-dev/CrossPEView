using Microsoft.Maui.Controls;

namespace CrossPEView.Control
{
    public partial class DLLView : ContentView
    {
        public DLLView(string Name, string Hint, string IATOffset)
        {
            InitializeComponent();
            m_Name.Text = "º¯ÊýÃû³Æ:   "+Name; 
            ID.Text = "ÐòºÅ:   "+Hint;
            m_IATOffset.Text = "Æ«ÒÆÁ¿:   " + IATOffset;
        }
    }
}
