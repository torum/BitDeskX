using Avalonia.Controls;
using BitDesk.ViewModels;
using System.Diagnostics;
using System.Linq;

namespace BitDesk.Views.ChartPages;

public partial class EthJpyPage : UserControl
{   
    public EthJpyPage()
    {
        var mainViewModel = App.GetService<MainViewModel>();
        var pairViewModel = mainViewModel.Pairs.FirstOrDefault(p => p.PairCode == Models.PairCodes.eth_jpy);
        if (pairViewModel != null)
        {
            this.DataContext = pairViewModel;
        }
        else
        {
            Debug.WriteLine("EthJpyPage: PairViewModel for eth_jpy not found.");
        }

        InitializeComponent();

        this.Loaded += EthJpyPage_Loaded;
    }

    private void EthJpyPage_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //Debug.WriteLine("EthJpyPage loaded. DataContext: " + (this.DataContext?.ToString() ?? "null"));
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