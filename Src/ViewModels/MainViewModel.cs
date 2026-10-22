using Avalonia.Controls;
using BitDesk.Models;
using BitDesk.Services;
using BitDesk.Services.Contracts;
using CommunityToolkit.Mvvm.ComponentModel;
using SocketIOClient;
using SocketIOClient.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BitDesk.ViewModels;

internal partial class MainViewModel : ObservableObject
{
    private readonly IBitbankStreamService _tickerService;
    private readonly IConfigurationService _configurationService;

    public MainViewModel()
    {
        // This constructor is for design-time data context in XAML preview.
        // It should not be used in production code.

        // Sutisfiy the non-null for design-time purposes.
        _tickerService = App.GetService<IBitbankStreamService>();
        _configurationService = App.GetService<IConfigurationService>();
    }

    public MainViewModel(IBitbankStreamService tickerService, IConfigurationService configurationService)
    {
        _tickerService = tickerService;
        _configurationService = configurationService;

        _tickerService.TickerUpdated += OnTickerService_TickerUpdated;

#if DEBUG
        IsSaveLog = true;

#else
        IsSaveLog = false;
#endif
    }

    #region == Properties ==

    public bool IsSaveLog { get; }
    public bool IsNavigationViewMenuOpen { get; set; } = true;


    #region == Pairs ==

    public Dictionary<string, PairCodes> GetPairs { get; set; } = new Dictionary<string, PairCodes>()
        {
            {"btc_jpy", PairCodes.btc_jpy},
            {"xrp_jpy", PairCodes.xrp_jpy},
            //{"eth_btc", Pairs.eth_btc},
            {"eth_jpy", PairCodes.eth_jpy},
            //{"ltc_btc", Pairs.ltc_btc},
            {"ltc_jpy", PairCodes.ltc_jpy},
            {"mona_jpy", PairCodes.mona_jpy},
            //{"mona_btc", Pairs.mona_btc},
            {"bcc_jpy", PairCodes.bcc_jpy},
            //{"bcc_btc", Pairs.bcc_btc},
            {"xlm_jpy", PairCodes.xlm_jpy},
            {"qtum_jpy", PairCodes.qtum_jpy},
            {"bat_jpy", PairCodes.bat_jpy},
            {"omg_jpy", PairCodes.omg_jpy},
            {"xym_jpy", PairCodes.xym_jpy},
            {"link_jpy", PairCodes.link_jpy},
            {"mkr_jpy", PairCodes.mkr_jpy},
            {"boba_jpy", PairCodes.boba_jpy},
            {"enj_jpy", PairCodes.enj_jpy},
            {"matic_jpy", PairCodes.matic_jpy},
            {"dot_jpy", PairCodes.dot_jpy},
            {"doge_jpy", PairCodes.doge_jpy},
            {"astr_jpy", PairCodes.astr_jpy},
            {"ada_jpy", PairCodes.ada_jpy},
            {"avax_jpy", PairCodes.avax_jpy},
            {"axs_jpy", PairCodes.axs_jpy},
            {"flr_jpy", PairCodes.flr_jpy},
            {"sand_jpy", PairCodes.sand_jpy},
        };

    public ObservableCollection<PairViewModel> Pairs { get; } = [
        new PairViewModel(PairCodes.btc_jpy, 24, "{0:#,0}", "C", 100M, 1000M),
        new PairViewModel(PairCodes.xrp_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.eth_jpy, 24, "{0:#,0}", "C", 100M, 1000M),
        /*
        new PairViewModel(PairCodes.ltc_jpy, 24, "{0:#,0.0}", "C1", 100M, 1000M),
        new PairViewModel(PairCodes.mona_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 1M),
        new PairViewModel(PairCodes.bcc_jpy, 24, "{0:#,0}", "C", 10M, 100M),
        new PairViewModel(PairCodes.xlm_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.qtum_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.bat_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.omg_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.xym_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.link_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.mkr_jpy, 24, "{0:#,0}", "C", 100M, 1000M),
        new PairViewModel(PairCodes.boba_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.enj_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.matic_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.dot_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.doge_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.astr_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.ada_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.avax_jpy, 24, "{0:#,0.000}", "C3", 100M, 1000M),
        new PairViewModel(PairCodes.axs_jpy, 24, "{0:#,0.000}", "C3", 100M, 1000M),
        new PairViewModel(PairCodes.flr_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.sand_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        */
    ];

    private PairViewModel? _selectedPair;
    public PairViewModel? SelectedPair
    {
        get => _selectedPair;
        set
        {
            if (_selectedPair == value)
            {
                return;
            }

            _selectedPair = value;

            if (_selectedPair != null)
            {
                foreach (var hoge in Pairs)
                {
                    if (hoge.PairCode == _selectedPair.PairCode)
                    {
                        hoge.IsSelectedActive = true;
                    }
                    else
                    {
                        hoge.IsSelectedActive = false;
                    }
                }
            }

            OnPropertyChanged();
        }
    }

    #endregion

    #endregion

    #region == Public Methods ==
    public async Task InitializeAndStartAsync()
    {
        await _tickerService.StartAsync();
    }

    public async Task SetSelectedPairFromCodeAsync(PairCodes pairCode)
    {
        if (_selectedPair?.PairCode == pairCode)
        {
            return;
        }
        else
        {
            Debug.WriteLine($"SetSelectedPairFromCodeAsync() called for {pairCode} at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        }

        var pairVM = Pairs.FirstOrDefault(x => x.PairCode == pairCode);

        // little hack.
        //_selectedPair = pairVM;

        if (pairVM is not null)
        {
            foreach (var hoge in Pairs)
            {
                if (hoge.PairCode == pairVM.PairCode)
                {
                    hoge.IsSelectedActive = true;

                    // TODO: This is a temporary hack to enable the selected pair. In the future, we should implement a more robust solution to manage the state of pairs.
                    hoge.IsEnabled = true;
                }
                else
                {
                    hoge.IsSelectedActive = false;
                }
            }

            await pairVM.InitializeAndLoadAsync();
        }
        else
        {
            Debug.WriteLine($"No PairViewModel found for {pairCode} at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        }

        SelectedPair = pairVM;
        //OnPropertyChanged(nameof(SelectedPair));
    }

    public int UniformGridColumns
    {
        get
        {
            int count = Pairs.Count(p => p.IsEnabled);
            if (count <= 0) return 1;
            if (count == 1) return 1;
            if (count == 2) return 2;
            if (count <= 4) return 2;
            if (count <= 6) return 3;
            if (count <= 9) return 3;
            if (count <= 12) return 4;
            if (count <= 16) return 4;
            if (count <= 20) return 5;
            return 5; // Default to a maximum of 5 columns for more than 20 pairs.
        }
    }

    public async Task SetSelectedAllPairsFromCodeAsync()
    {
        foreach (var pairVM in Pairs)
        {
            pairVM.IsSelectedActive = true;

            // TODO: This is a temporary hack to enable the selected pair. In the future, we should implement a more robust solution to manage the state of pairs.
            pairVM.IsEnabled = true;

            await pairVM.InitializeAndLoadAsync();
        }


    }

    #endregion

    #region == private methods ==

    private async void OnTickerService_TickerUpdated(object? sender, BitbankTickerData data)
    {
        /*
        PairCodes pair = data.TickerPair;
        DateTime timeStamp = DateTimeOffset.FromUnixTimeMilliseconds(data.Timestamp).LocalDateTime;

        // Calculate a quick math metric (e.g., daily percentage change)
        decimal priceChange = data.Last - data.Open;
        decimal percentChange = data.Open != 0 ? (priceChange / data.Open) * 100 : 0;

        // Print out your typed metrics using standard number formatting
        Debug.WriteLine($"=== {pair} NUMERIC DATA [{timeStamp:yyyy-MM-dd HH:mm:ss}] ===");
        Debug.WriteLine($"  Last Price : {data.Last:N0} JPY ({percentChange:+0.00;-0.00;0.00}%)");
        Debug.WriteLine($"  Bid (Buy)  : {data.Buy:N0}   | Ask (Sell) : {data.Sell:N0}");
        Debug.WriteLine($"  24h High   : {data.High:N0}  | 24h Low    : {data.Low:N0}");
        Debug.WriteLine($"  24h Volume : {data.Vol:N4}");
        Debug.WriteLine(new string('=', 45));
        */
        var pairViewModel = Pairs.FirstOrDefault(p => p.PairCode == data.TickerPair);
        if (pairViewModel is not null)
        {
            await pairViewModel.UpdateTickerDataAsync(data);
        }
        else
        {
            Debug.WriteLine($"No PairViewModel found for {data.TickerPair} at {DateTimeOffset.FromUnixTimeMilliseconds(data.Timestamp).LocalDateTime:yyyy-MM-dd HH:mm:ss}");
        }
    }

    #endregion
}