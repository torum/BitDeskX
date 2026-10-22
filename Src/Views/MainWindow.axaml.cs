using Avalonia.Controls;
using BitDesk.Models;
using BitDesk.Services;
using BitDesk.Services.Contracts;
using BitDesk.ViewModels;
using BitDesk.Views.ChartPages;
using FluentAvalonia.UI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BitDesk.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _mainViewModel;

    private readonly Dictionary<string, Type> _pageMap = new()
    {
        { "BitDesk.Views.ChartPages.BtcJpyPage", typeof(Views.ChartPages.BtcJpyPage) },
        { "BitDesk.Views.ChartPages.XrpJpyPage", typeof(Views.ChartPages.XrpJpyPage) },
        { "BitDesk.Views.ChartPages.EthJpyPage", typeof(Views.ChartPages.EthJpyPage) },
        { "BitDesk.Views.MultipleView", typeof(Views.MultipleView) },
    };

    private readonly List<(string Tag, Type Page)> _pages =
    [
        (PairCodes.btc_jpy.ToString(), typeof(BtcJpyPage)),
        (PairCodes.xrp_jpy.ToString(), typeof(XrpJpyPage)),
        (PairCodes.eth_jpy.ToString(), typeof(EthJpyPage)),
        ("multiple", typeof(MultipleView)),
        /*
        (PairCodes.ltc_jpy.ToString(), typeof(LtcJpyPage)),
        (PairCodes.mona_jpy.ToString(), typeof(MonaJpyPage)),
        (PairCodes.bcc_jpy.ToString(), typeof(BccJpyPage)),
        (PairCodes.xlm_jpy.ToString(), typeof(XlmJpyPage)),
        (PairCodes.qtum_jpy.ToString(), typeof(QtumJpyPage)),
        (PairCodes.bat_jpy.ToString(), typeof(BatJpyPage)),
        (PairCodes.omg_jpy.ToString(), typeof(OmgJpyPage)),
        (PairCodes.xym_jpy.ToString(), typeof(XymJpyPage)),
        (PairCodes.link_jpy.ToString(), typeof(LinkJpyPage)),
        (PairCodes.mkr_jpy.ToString(), typeof(MkrJpyPage)),
        (PairCodes.boba_jpy.ToString(), typeof(BobaJpyPage)),
        (PairCodes.enj_jpy.ToString(), typeof(EnjJpyPage)),
        (PairCodes.matic_jpy.ToString(), typeof(MaticJpyPage)),
        (PairCodes.dot_jpy.ToString(), typeof(DotJpyPage)),
        (PairCodes.doge_jpy.ToString(), typeof(DogeJpyPage)),
        (PairCodes.astr_jpy.ToString(), typeof(AstrJpyPage)),
        (PairCodes.ada_jpy.ToString(), typeof(AdaJpyPage)),
        (PairCodes.avax_jpy.ToString(), typeof(AvaxJpyPage)),
        (PairCodes.axs_jpy.ToString(), typeof(AxsJpyPage)),
        (PairCodes.flr_jpy.ToString(), typeof(FlrJpyPage)),
        (PairCodes.sand_jpy.ToString(), typeof(SandJpyPage)),
        ("settings", typeof(SettingsPage)),
        */
    ];

    private readonly IBitbankStreamService _tickerService;
    private readonly IConfigurationService _configurationService;
    private readonly INavigationService _navigationService;
    private bool _shutdownStarted;
    private bool _allowClose;

    // Parameterless constructor for XAML Previewer
    public MainWindow()
    {
        _mainViewModel = App.GetService<MainViewModel>();
        this.DataContext = _mainViewModel;
        _tickerService = App.GetService<IBitbankStreamService>();
        _configurationService = App.GetService<IConfigurationService>();
        _navigationService = App.GetService<INavigationService>();

        InitializeComponent();

        this.Title = "BitDeskX";
        this.WindowDecorations = WindowDecorations.Full;
        this.ExtendClientAreaToDecorationsHint = true;

        _navigationService.Initialize(RootFrame, _pageMap);

        this.Closing += MainWindow_Closing;
    }

    protected async override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        if (_navigationService.NavigateTo("BitDesk.Views.ChartPages.BtcJpyPage", null))
        {
            //NavigateViewControl.MenuItems.FirstOrDefault(x => x.Tag?.ToString() == "btc_jpy")?.Selected = true;

            NavigateViewControl.SelectedItem = NavigateViewControl.MenuItems.OfType<FANavigationViewItem>().Where(x => x.Tag?.ToString()?.Equals("btc_jpy") == true).FirstOrDefault();//.First();

            var _activePair = Enum.TryParse<PairCodes>("btc_jpy", out var parsedPair);
            if (_activePair)
            {
                await _mainViewModel.SetSelectedPairFromCodeAsync(parsedPair);
            }
        }
        /*
        var pair = _mainViewModel.Pairs.FirstOrDefault(p => p.PairCode == BitDesk.Models.PairCodes.btc_jpy);

        if (pair is not null)
        {
            _navigationService.NavigateTo("BitDesk.Views.ChartPages.BtcJpyPage", pair);
            //this.RootFrame.Navigate(typeof(Views.ChartPages.BtcJpyPage), pair);
        }
        */

        await _mainViewModel.InitializeAndStartAsync();
    }

    protected override void OnClosed(EventArgs e)
    {
        this.Closing -= MainWindow_Closing;

        base.OnClosed(e);
    }

    private async void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (_allowClose)
        {
            return;
        }

        if (_shutdownStarted)
        {
            e.Cancel = true;
            return;
        }

        _shutdownStarted = true;

        // Cancel the closing event to prevent the window from closing immediately
        e.Cancel = true;
        // Hide the window instead of closing it
        this.Hide();

        // Wait for the ticker service to stop gracefully
        await _tickerService.StopAsync();

        // After the service has stopped, allow the window to close
        _allowClose = true;
        Close();
    }

    private void NavigationView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is not FANavigationView)
        {
            return;
        }

        if (this.NavigateViewControl is null)
        {
            return;
        }

        if (this.DataContext is not MainViewModel vm)
        {
            return;
        }
        /*
        vm.SelectedNodeMenu = vm.MainMenuItems.FirstOrDefault();
        vm.SelectedNodeMenu?.Selected = true;

        if (vm.IsNavigationViewMenuOpen)
        {
            foreach (var fuga in vm.MainMenuItems)
            {
                if (fuga is NodeMenuLibrary lib)
                {
                    lib.Expanded = true;
                }
                else if (fuga is NodeMenuPlaylists plt)
                {
                    plt.Expanded = true;
                }
            }
        }
        */
    }

    private void NavigationView_SelectionChanged(object? sender, FluentAvalonia.UI.Controls.FANavigationViewSelectionChangedEventArgs e)
    {
        if (sender is not FluentAvalonia.UI.Controls.FANavigationView)
        {
            return;
        }

        if (this.DataContext is not MainViewModel vm)
        {
            return;
        }

        /*
        if (e.SelectedItem is null)
        {
            vm.SelectedNodeMenu = null;
            return;
        }

        if (e.SelectedItem is NodeTree)
        {
            vm.SelectedNodeMenu = e.SelectedItem as NodeTree;
        }
        else
        {
            Debug.WriteLine("e.SelectedItem is not NodeTree. @NavigationView_SelectionChanged");
            vm.SelectedNodeMenu = null;
        }
        */
    }

    private async void NavigationView_ItemInvoked(object? sender, FluentAvalonia.UI.Controls.FANavigationViewItemInvokedEventArgs e)
    {
        if (sender is not FluentAvalonia.UI.Controls.FANavigationView nv)
        {
            Debug.WriteLine("NavigationView_ItemInvoked: sender is not FANavigationView.");
            return;
        }

        if (this.DataContext is not MainViewModel vm)
        {
            Debug.WriteLine("NavigationView_ItemInvoked: DataContext is not MainViewModel.");
            return;
        }
        if (e.IsSettingsInvoked == true)
        {
            // pass mainviewmodel for setting page.
            //NavigationFrame.Navigate(typeof(SettingsPage), _mainViewModel);

            return;
        }

        if (e.InvokedItemContainer is null && (e.InvokedItemContainer?.Tag is null))
        {
            Debug.WriteLine("NavigationView_ItemInvoked: InvokedItemContainer or its Tag is null.");
            return;
        }

        if (e.InvokedItemContainer is not FANavigationViewItem invokedItem)
        {
            Debug.WriteLine($"NavigationView_ItemInvoked: InvokedItem is not FANavigationViewItem. {e.InvokedItem}");
            return;
        }

        //Debug.WriteLine($"NavigationView_ItemInvoked: InvokedItem.Tag = {invokedItem.Tag?.ToString()}");

        var selectedItem = nv.MenuItems.OfType<FANavigationViewItem>().Where(x => x.Tag?.ToString()?.Equals(invokedItem.Tag?.ToString()) == true).FirstOrDefault();//.First();

        if (selectedItem is null)
        {
            Debug.WriteLine("NavigationView_ItemInvoked: SelectedItem is null.");
            return;
        }




        var pageInfo = _pages.FirstOrDefault(p => p.Tag == invokedItem.Tag?.ToString());

        if (_navigationService.NavigateTo(pageInfo.Page?.FullName, null))
        {
            //Debug.WriteLine($"NavigationView_ItemInvoked: Navigation to pageInfo successful.");
            nv.SelectedItem = selectedItem;

            if (invokedItem.Tag?.ToString() == "multiple")
            {
                // Handle multiple view logic here

                await _mainViewModel.SetSelectedAllPairsFromCodeAsync();
            }
            else
            {
                var _activePair = Enum.TryParse<PairCodes>(invokedItem.Tag?.ToString(), out var parsedPair);
                if (_activePair)
                {
                    await _mainViewModel.SetSelectedPairFromCodeAsync(parsedPair);
                }
                else
                {
                    Debug.WriteLine($"NavigationView_ItemInvoked: Failed to parse PairCodes from tag '{invokedItem.Tag?.ToString()}'.");
                }
            }
        }
        else
        {
            Debug.WriteLine($"NavigationView_ItemInvoked: Navigation to pageInfo failed.");
        }

        /*
        // Not really "invoked" now that we use clicking on the settings icon....
        if (e.IsSettingsInvoked == true)
        {
            //await vm.GetCacheFolderSizeAsync();
            await RefreshCacheFolderSizeSafeAsync(vm);

            nv.Content = App.GetService<SettingsPage>();

            return;
        }

        var mainView = App.GetService<MainView>();
        if (nv.Content is not MainView)
        {
            nv.Content = mainView;
        }
        */
    }

    private void RootFrame_Navigated(object sender, FluentAvalonia.UI.Navigation.FANavigationEventArgs e)
    {
        if (e.Content is BtcJpyPage btcJpyPage)
        {
            btcJpyPage.InitializeNavigationParameter(e.Parameter);
        }
        else if (e.Content is EthJpyPage ethJpyPage)
        {
            ethJpyPage.InitializeNavigationParameter(e.Parameter);
        }
        else if (e.Content is XrpJpyPage xrpJpyPage)
        {
            xrpJpyPage.InitializeNavigationParameter(e.Parameter);
        }

    }
}