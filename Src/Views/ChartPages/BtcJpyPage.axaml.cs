using Avalonia.Controls;
using BitDesk.ViewModels;
using System.Diagnostics;
using System.Linq;

namespace BitDesk.Views.ChartPages;

public partial class BtcJpyPage : UserControl
{   
    public BtcJpyPage()
    {
        var mainViewModel = App.GetService<MainViewModel>();
        var pairViewModel = mainViewModel.Pairs.FirstOrDefault(p => p.PairCode == Models.PairCodes.btc_jpy);
        if (pairViewModel != null)
        {
            this.DataContext = pairViewModel;
        }
        else
        {
            Debug.WriteLine("BtcJpyPage: PairViewModel for btc_jpy not found.");
        }

        InitializeComponent();

        this.Loaded += BtcJpyPage_Loaded;
    }

    private void BtcJpyPage_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //Debug.WriteLine("BtcJpyPage loaded. DataContext: " + (this.DataContext?.ToString() ?? "null"));
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