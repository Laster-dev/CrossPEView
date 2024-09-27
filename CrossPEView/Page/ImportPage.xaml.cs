using PeNet;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Views;
using CrossPEView.Control;

namespace CrossPEView.Page
{
    public partial class ImportPage : ContentPage
    {
        PeFile _pefile;

        public ObservableCollection<ImportedFunctionGroup> FunctionGroups { get; set; }

        public ImportPage(PeFile peFile)
        {
            InitializeComponent();
            _pefile = peFile;
            FunctionGroups = new ObservableCollection<ImportedFunctionGroup>();
        }

        private void ContentPage_Loaded(System.Object sender, System.EventArgs e)
        {
            if (_pefile.ImportedFunctions != null)
            {
                foreach (var importFunction in _pefile.ImportedFunctions)
                {
                    var function = new Function
                    {
                        FunctionName = importFunction.Name,
                        Hint = importFunction.Hint,
                        IATOffset = importFunction.IATOffset,
                    };

                    var group = FunctionGroups.FirstOrDefault(g => g.ModuleName == importFunction.DLL);
                    if (group == null)
                    {
                        group = new ImportedFunctionGroup { ModuleName = importFunction.DLL, Functions = new ObservableCollection<Function>() };
                        FunctionGroups.Add(group);
                    }
                    group.Functions.Add(function);
                }
            }

            foreach (var group in FunctionGroups)
            {
                var expander = new Expander
                {
                    Header = new Label
                    {
                        Text = group.ModuleName,
                        FontAttributes = FontAttributes.Bold,
                        BackgroundColor = Color.FromRgba("#1e1e1e"),
                        Padding = new Thickness(10),
                        TextColor = Colors.White,
                    },
                    // 创建一个新的StackLayout，并设置其背景颜色、内边距和外边距
                    Content = new StackLayout
                    {
                        // 设置背景颜色为#1e1e1e
                        BackgroundColor = Color.FromRgba("#1e1e1e"),
                        // 设置内边距为10
                        Padding = 3,
                        // 设置外边距为5
                        Spacing = 2
                    }
                };

                foreach (var func in group.Functions)
                {
                    var dllView = new DLLView(func.FunctionName, func.Hint.ToString(), func.IATOffset.ToString())
                    {
                        Margin = new Thickness(0, 5) // ���Ӽ��
                    };
                    (expander.Content as StackLayout).Children.Add(dllView);
                }

                ExpanderStack.Children.Add(expander);
            }

        }

    }

    public static class ViewExtensions
    {
        public static T BindToGrid<T>(this T view, int row, int column) where T : View
        {
            Grid.SetRow(view, row);
            Grid.SetColumn(view, column);
            return view;
        }
    }

    public class ImportedFunctionGroup
    {
        public string ModuleName { get; set; }
        public ObservableCollection<Function> Functions { get; set; }
    }

    public class Function
    {
        public string FunctionName { get; set; }
        public int Hint { get; set; }
        public uint IATOffset { get; set; }
    }
}
