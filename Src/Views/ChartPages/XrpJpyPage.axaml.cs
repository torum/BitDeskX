using Avalonia.Controls;
using BitDesk.ViewModels;
using System.Diagnostics;
using System.Linq;

namespace BitDesk.Views.ChartPages;

public partial class XrpJpyPage : UserControl
{   
    public XrpJpyPage()
    {
        var mainViewModel = App.GetService<MainViewModel>();
        var pairViewModel = mainViewModel.Pairs.FirstOrDefault(p => p.PairCode == Models.PairCodes.xrp_jpy);
        if (pairViewModel != null)
        {
            this.DataContext = pairViewModel;
        }
        else
        {
            Debug.WriteLine("XrpJpyPage: PairViewModel for xrp_jpy not found.");
        }

        InitializeComponent();

        this.Loaded += XrpJpyPage_Loaded;
    }

    private void XrpJpyPage_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //Debug.WriteLine("XrpJpyPage loaded. DataContext: " + (this.DataContext?.ToString() ?? "null"));
    }

    // A custom method to receive the parameter
    public void InitializeNavigationParameter(object parameter)
    {
        if (parameter is null)
        {
            return;
        }
    }

}