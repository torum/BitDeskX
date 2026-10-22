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

internal record IsEnabledState
{
    public bool IsEnabled { get; set; }
    public PairCodes PairCode { get; set; }
}

internal partial class MainViewModel : ObservableObject
{
    private readonly bool _isWebSocketOn = true;
    private readonly bool _isLiveChartsOn = true;

    private readonly IBitbankStreamService _bitbankStreamService;
    private readonly IConfigurationService _configurationService;

    public event EventHandler<IsEnabledState>? IsEnabledChanged;

    public MainViewModel()
    {
        // This constructor is for design-time data context in XAML preview. It should not be used in production code.

        // Sutisfiy the non-null for design-time purposes.
        _bitbankStreamService = App.GetService<IBitbankStreamService>();
        _configurationService = App.GetService<IConfigurationService>();
    }

    public MainViewModel(IBitbankStreamService bitbankStreamService, IConfigurationService configurationService)
    {
        _bitbankStreamService = bitbankStreamService;
        _configurationService = configurationService;

        var config = _configurationService.CurrentConfiguration;
        IsSaveLog = config.IsSaveLog;
        IsNavigationViewMenuOpen = config.IsNavigationViewMenuOpen;

        IsOnBtcJpy = config.IsOnBtcJpy;
        IsOnEthJpy = config.IsOnEthJpy;
        IsOnXrpJpy = config.IsOnXrpJpy;

        IsEnabledChanged += OnIsEnabledChanged;

        _bitbankStreamService.TickerUpdated += OnTickerService_TickerUpdated;

        // For developing purposes, we can set this to true to avoid starting the ticker service and charts loading.
        _isWebSocketOn = true;
        _isLiveChartsOn = true;

#if DEBUG
        IsSaveLog = true;

#else
        IsSaveLog = false;
#endif
    }


    #region == Properties ==

    public bool IsSaveLog { get; }

    #region == View related ==

    public bool IsNavigationViewMenuOpen { get; set; } = true;

    public int UniformGridColumns
    {
        get
        {
            int count = EnabledPairs.Count;//Pairs.Count(p => p.IsEnabled);
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

    #endregion

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

    public ObservableCollection<PairViewModel> EnabledPairs => new(Pairs.Where(x => x.IsEnabled));

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

    #region == IsOn ==

    private bool _isOnBtcJpy = true;
    public bool IsOnBtcJpy
    {
        get => _isOnBtcJpy;
        set
        {
            if (_isOnBtcJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.btc_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnBtcJpy = value;

            OnPropertyChanged(nameof(IsOnBtcJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnXrpJpy = true;
    public bool IsOnXrpJpy
    {
        get => _isOnXrpJpy;
        set
        {
            if (_isOnXrpJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.xrp_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnXrpJpy = value;

            OnPropertyChanged(nameof(IsOnXrpJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnEthJpy = true;
    public bool IsOnEthJpy
    {
        get => _isOnEthJpy;
        set
        {
            if (_isOnEthJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.eth_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnEthJpy = value;

            OnPropertyChanged(nameof(IsOnEthJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnLtcJpy = true;
    public bool IsOnLtcJpy
    {
        get => _isOnLtcJpy;
        set
        {
            if (_isOnLtcJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.ltc_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnLtcJpy = value;
            OnPropertyChanged(nameof(IsOnLtcJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnBccJpy = true;
    public bool IsOnBccJpy
    {
        get => _isOnBccJpy;
        set
        {
            if (_isOnBccJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.bcc_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnBccJpy = value;
            OnPropertyChanged(nameof(IsOnBccJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnMonaJpy = true;
    public bool IsOnMonaJpy
    {
        get => _isOnMonaJpy;
        set
        {
            if (_isOnMonaJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.mona_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnMonaJpy = value;
            OnPropertyChanged(nameof(IsOnMonaJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnXlmJpy = true;
    public bool IsOnXlmJpy
    {
        get => _isOnXlmJpy;
        set
        {
            if (_isOnXlmJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.xlm_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnXlmJpy = value;
            OnPropertyChanged(nameof(IsOnXlmJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnQtumJpy = true;
    public bool IsOnQtumJpy
    {
        get => _isOnQtumJpy;
        set
        {
            if (_isOnQtumJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.qtum_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnQtumJpy = value;
            OnPropertyChanged(nameof(IsOnQtumJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnBatJpy = true;
    public bool IsOnBatJpy
    {
        get => _isOnBatJpy;
        set
        {
            if (_isOnBatJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.bat_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnBatJpy = value;
            OnPropertyChanged(nameof(IsOnBatJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnOmgJpy = true;
    public bool IsOnOmgJpy
    {
        get => _isOnOmgJpy;
        set
        {
            if (_isOnOmgJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.omg_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnOmgJpy = value;
            OnPropertyChanged(nameof(IsOnOmgJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnXymJpy = true;
    public bool IsOnXymJpy
    {
        get => _isOnXymJpy;
        set
        {
            if (_isOnXymJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.xym_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnXymJpy = value;
            OnPropertyChanged(nameof(IsOnXymJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnLinkJpy = true;
    public bool IsOnLinkJpy
    {
        get => _isOnLinkJpy;
        set
        {
            if (_isOnLinkJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.link_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnLinkJpy = value;
            OnPropertyChanged(nameof(IsOnLinkJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnMkrJpy = true;
    public bool IsOnMkrJpy
    {
        get => _isOnMkrJpy;
        set
        {
            if (_isOnMkrJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.mkr_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnMkrJpy = value;
            OnPropertyChanged(nameof(IsOnMkrJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnBobaJpy = true;
    public bool IsOnBobaJpy
    {
        get => _isOnBobaJpy;
        set
        {
            if (_isOnBobaJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.boba_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnBobaJpy = value;
            OnPropertyChanged(nameof(IsOnBobaJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnEnjJpy = true;
    public bool IsOnEnjJpy
    {
        get => _isOnEnjJpy;
        set
        {
            if (_isOnEnjJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.enj_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnEnjJpy = value;
            OnPropertyChanged(nameof(IsOnEnjJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnMaticJpy = true;
    public bool IsOnMaticJpy
    {
        get => _isOnMaticJpy;
        set
        {
            if (_isOnMaticJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.matic_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnMaticJpy = value;
            OnPropertyChanged(nameof(IsOnMaticJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnDotJpy = true;
    public bool IsOnDotJpy
    {
        get => _isOnDotJpy;
        set
        {
            if (_isOnDotJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.dot_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnDotJpy = value;
            OnPropertyChanged(nameof(IsOnDotJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnDogeJpy = true;
    public bool IsOnDogeJpy
    {
        get => _isOnDogeJpy;
        set
        {
            if (_isOnDogeJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.doge_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnDogeJpy = value;
            OnPropertyChanged(nameof(IsOnDogeJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnAstrJpy = true;
    public bool IsOnAstrJpy
    {
        get => _isOnAstrJpy;
        set
        {
            if (_isOnAstrJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.astr_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnAstrJpy = value;
            OnPropertyChanged(nameof(IsOnAstrJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnAdaJpy = true;
    public bool IsOnAdaJpy
    {
        get => _isOnAdaJpy;
        set
        {
            if (_isOnAdaJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.ada_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnAdaJpy = value;
            OnPropertyChanged(nameof(IsOnAdaJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnAvaxJpy = true;
    public bool IsOnAvaxJpy
    {
        get => _isOnAvaxJpy;
        set
        {
            if (_isOnAvaxJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.avax_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnAvaxJpy = value;
            OnPropertyChanged(nameof(IsOnAvaxJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnAxsJpy = true;
    public bool IsOnAxsJpy
    {
        get => _isOnAxsJpy;
        set
        {
            if (_isOnAxsJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.axs_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnAxsJpy = value;
            OnPropertyChanged(nameof(IsOnAxsJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnFlrJpy = true;
    public bool IsOnFlrJpy
    {
        get => _isOnFlrJpy;
        set
        {
            if (_isOnFlrJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.flr_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnFlrJpy = value;
            OnPropertyChanged(nameof(IsOnFlrJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    private bool _isOnSandJpy = true;
    public bool IsOnSandJpy
    {
        get => _isOnSandJpy;
        set
        {
            if (_isOnSandJpy == value)
            {
                return;
            }

            var hoge = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.sand_jpy);
            if (hoge is not null)
            {
                hoge.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = hoge.PairCode });
            }

            _isOnSandJpy = value;
            OnPropertyChanged(nameof(IsOnSandJpy));
            OnPropertyChanged(nameof(EnabledPairs)); // Notify that EnabledPairs has changed
            OnPropertyChanged(nameof(UniformGridColumns)); // Notify that UniformGridColumns has changed
        }
    }

    #endregion

    #region == LTP ==

    private string _ltpBtcJpy = "...";
    public string LtpBtcJpy
    {
        get => _ltpBtcJpy;
        set
        {
            if (_ltpBtcJpy == value)
            {
                return;
            }

            _ltpBtcJpy = value;
            OnPropertyChanged(nameof(LtpBtcJpy));
        }
    }

    private string _ltpXrpJpy = "...";
    public string LtpXrpJpy
    {
        get => _ltpXrpJpy;
        set
        {
            if (_ltpXrpJpy == value)
            {
                return;
            }

            _ltpXrpJpy = value;
            OnPropertyChanged(nameof(LtpXrpJpy));
        }
    }

    private string _ltpEthJpy = "...";
    public string LtpEthJpy
    {
        get => _ltpEthJpy;
        set
        {
            if (_ltpEthJpy == value)
            {
                return;
            }

            _ltpEthJpy = value;
            OnPropertyChanged(nameof(LtpEthJpy));
        }
    }

    private string _ltpLtcJpy = "";
    public string LtpLtcJpy
    {
        get => _ltpLtcJpy;
        set
        {
            if (_ltpLtcJpy == value)
            {
                return;
            }

            _ltpLtcJpy = value;
            OnPropertyChanged(nameof(LtpLtcJpy));
        }
    }

    private string _ltpMonaJpy = "";
    public string LtpMonaJpy
    {
        get => _ltpMonaJpy;
        set
        {
            if (_ltpMonaJpy == value)
            {
                return;
            }

            _ltpMonaJpy = value;
            OnPropertyChanged(nameof(LtpMonaJpy));
        }
    }

    private string _ltpBccJpy = "";
    public string LtpBccJpy
    {
        get => _ltpBccJpy;
        set
        {
            if (_ltpBccJpy == value)
            {
                return;
            }

            _ltpBccJpy = value;
            OnPropertyChanged(nameof(LtpBccJpy));
        }
    }

    private string _ltpXlmJpy = "";
    public string LtpXlmJpy
    {
        get => _ltpXlmJpy;
        set
        {
            if (_ltpXlmJpy == value)
            {
                return;
            }

            _ltpXlmJpy = value;
            OnPropertyChanged(nameof(LtpXlmJpy));
        }
    }

    private string _ltpQtumJpy = "";
    public string LtpQtumJpy
    {
        get => _ltpQtumJpy;
        set
        {
            if (_ltpQtumJpy == value)
            {
                return;
            }

            _ltpQtumJpy = value;
            OnPropertyChanged(nameof(LtpQtumJpy));
        }
    }

    private string _ltpBatJpy = "";
    public string LtpBatJpy
    {
        get => _ltpBatJpy;
        set
        {
            if (_ltpBatJpy == value)
            {
                return;
            }

            _ltpBatJpy = value;
            OnPropertyChanged(nameof(LtpBatJpy));
        }
    }

    private string _ltpOmgJpy = "";
    public string LtpOmgJpy
    {
        get => _ltpOmgJpy;
        set
        {
            if (_ltpOmgJpy == value)
            {
                return;
            }

            _ltpOmgJpy = value;
            OnPropertyChanged(nameof(LtpOmgJpy));
        }
    }

    private string _ltpXymJpy = "";
    public string LtpXymJpy
    {
        get => _ltpXymJpy;
        set
        {
            if (_ltpXymJpy == value)
            {
                return;
            }

            _ltpXymJpy = value;
            OnPropertyChanged(nameof(LtpXymJpy));
        }
    }

    private string _ltpLinkJpy = "";
    public string LtpLinkJpy
    {
        get => _ltpLinkJpy;
        set
        {
            if (_ltpLinkJpy == value)
            {
                return;
            }

            _ltpLinkJpy = value;
            OnPropertyChanged(nameof(LtpLinkJpy));
        }
    }

    private string _ltpMkrJpy = "";
    public string LtpMkrJpy
    {
        get => _ltpMkrJpy;
        set
        {
            if (_ltpMkrJpy == value)
            {
                return;
            }

            _ltpMkrJpy = value;
            OnPropertyChanged(nameof(LtpMkrJpy));
        }
    }

    private string _ltpBobaJpy = "";
    public string LtpBobaJpy
    {
        get => _ltpBobaJpy;
        set
        {
            if (_ltpBobaJpy == value)
            {
                return;
            }

            _ltpBobaJpy = value;
            OnPropertyChanged(nameof(LtpBobaJpy));
        }
    }

    private string _ltpEnjJpy = "";
    public string LtpEnjJpy
    {
        get => _ltpEnjJpy;
        set
        {
            if (_ltpEnjJpy == value)
            {
                return;
            }

            _ltpEnjJpy = value;
            OnPropertyChanged(nameof(LtpEnjJpy));
        }
    }

    private string _ltpMaticJpy = "";
    public string LtpMaticJpy
    {
        get => _ltpMaticJpy;
        set
        {
            if (_ltpMaticJpy == value)
            {
                return;
            }

            _ltpMaticJpy = value;
            OnPropertyChanged(nameof(LtpMaticJpy));
        }
    }

    private string _ltpDotJpy = "";
    public string LtpDotJpy
    {
        get => _ltpDotJpy;
        set
        {
            if (_ltpDotJpy == value)
            {
                return;
            }

            _ltpDotJpy = value;
            OnPropertyChanged(nameof(LtpDotJpy));
        }
    }

    private string _ltpDogeJpy = "";
    public string LtpDogeJpy
    {
        get => _ltpDogeJpy;
        set
        {
            if (_ltpDogeJpy == value)
            {
                return;
            }

            _ltpDogeJpy = value;
            OnPropertyChanged(nameof(LtpDogeJpy));
        }
    }

    private string _ltpAstrJpy = "";
    public string LtpAstrJpy
    {
        get => _ltpAstrJpy;
        set
        {
            if (_ltpAstrJpy == value)
            {
                return;
            }

            _ltpAstrJpy = value;
            OnPropertyChanged(nameof(LtpAstrJpy));
        }
    }

    private string _ltpAdaJpy = "";
    public string LtpAdaJpy
    {
        get => _ltpAdaJpy;
        set
        {
            if (_ltpAdaJpy == value)
            {
                return;
            }

            _ltpAdaJpy = value;
            OnPropertyChanged(nameof(LtpAdaJpy));
        }
    }

    private string _ltpAvaxJpy = "";
    public string LtpAvaxJpy
    {
        get => _ltpAvaxJpy;
        set
        {
            if (_ltpAvaxJpy == value)
            {
                return;
            }

            _ltpAvaxJpy = value;
            OnPropertyChanged(nameof(LtpAvaxJpy));
        }
    }

    private string _ltpAxsJpy = "";
    public string LtpAxsJpy
    {
        get => _ltpAxsJpy;
        set
        {
            if (_ltpAxsJpy == value)
            {
                return;
            }

            _ltpAxsJpy = value;
            OnPropertyChanged(nameof(LtpAxsJpy));
        }
    }

    private string _ltpFlrJpy = "";
    public string LtpFlrJpy
    {
        get => _ltpFlrJpy;
        set
        {
            if (_ltpFlrJpy == value)
            {
                return;
            }

            _ltpFlrJpy = value;
            OnPropertyChanged(nameof(LtpFlrJpy));
        }
    }

    private string _ltpSandJpy = "";
    public string LtpSandJpy
    {
        get => _ltpSandJpy;
        set
        {
            if (_ltpSandJpy == value)
            {
                return;
            }

            _ltpSandJpy = value;
            OnPropertyChanged(nameof(LtpSandJpy));
        }
    }


    #endregion

    #endregion

    #region == Public Methods ==

    public async Task InitializeAndStartAsync()
    {
        _bitbankStreamService.Initialize(Pairs.Where(x => x.IsEnabled).Select(x => x.PairCode).ToList());

        if (_isWebSocketOn)
            await _bitbankStreamService.StartAsync();
    }

    public async Task SetSelectedPairFromCodeAsync(PairCodes pairCode)
    {
        if (_selectedPair?.PairCode == pairCode)
        {
            return;
        }
        else
        {
            //Debug.WriteLine($"SetSelectedPairFromCodeAsync() called for {pairCode} at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        }

        var pairVM = Pairs.FirstOrDefault(x => x.PairCode == pairCode);

        if (pairVM?.IsEnabled == false)
        {
            return;
        }

        if (pairVM is not null)
        {
            foreach (var hoge in Pairs)
            {
                if (hoge.PairCode == pairVM.PairCode)
                {
                    hoge.IsSelectedActive = true;
                }
                else
                {
                    hoge.IsSelectedActive = false;
                }
            }
            
            if (_isLiveChartsOn)
                await pairVM.InitializeAndLoadAsync();
        }
        else
        {
            Debug.WriteLine($"No PairViewModel found for {pairCode} at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        }

        SelectedPair = pairVM;
    }

    public async Task SetSelectedAllPairsFromCodeAsync()
    {
        foreach (var pairVM in EnabledPairs)
        {
            pairVM.IsSelectedActive = true;

            if (_isLiveChartsOn)
                await pairVM.InitializeAndLoadAsync();
        }
    }

    public void UpdateCurrentSettings()
    {
        var config = _configurationService.CurrentConfiguration;

        config.IsSaveLog = IsSaveLog;

        config.IsNavigationViewMenuOpen = IsNavigationViewMenuOpen;

        config.IsOnBtcJpy = IsOnBtcJpy;
        config.IsOnEthJpy = IsOnEthJpy;
        config.IsOnXrpJpy = IsOnXrpJpy;
    }

    #endregion

    #region == private methods ==

    private async void OnIsEnabledChanged(object? sender, IsEnabledState e)
    {
        if (e.IsEnabled)
        {
            await _bitbankStreamService.JoinRoomAsync(e.PairCode);
        }
        else
        {
            await _bitbankStreamService.LeaveRoomAsync(e.PairCode);
        }
    }

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
            if (pairViewModel.IsEnabled == false)
            {
                return;
            }

            await pairViewModel.UpdateTickerDataAsync(data);



            switch (data.TickerPair)
            {
                case PairCodes.btc_jpy:
                    LtpBtcJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.xrp_jpy:
                    LtpXrpJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.eth_jpy:
                    LtpEthJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.ltc_jpy:
                    LtpLtcJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.mona_jpy:
                    LtpMonaJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.bcc_jpy:
                    LtpBccJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.xlm_jpy:
                    LtpXlmJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.qtum_jpy:
                    LtpQtumJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.bat_jpy:
                    LtpBatJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.omg_jpy:
                    LtpOmgJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.xym_jpy:
                    LtpXymJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.link_jpy:
                    LtpLinkJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.mkr_jpy:
                    LtpMkrJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.boba_jpy:
                    LtpBobaJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.enj_jpy:
                    LtpEnjJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.matic_jpy:
                    LtpMaticJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.dot_jpy:
                    LtpDotJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.doge_jpy:
                    LtpDogeJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.astr_jpy:
                    LtpAstrJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.ada_jpy:
                    LtpAdaJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.avax_jpy:
                    LtpAvaxJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.axs_jpy:
                    LtpAxsJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.flr_jpy:
                    LtpFlrJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                case PairCodes.sand_jpy:
                    LtpSandJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        else
        {
            Debug.WriteLine($"No PairViewModel found for {data.TickerPair} at {DateTimeOffset.FromUnixTimeMilliseconds(data.Timestamp).LocalDateTime:yyyy-MM-dd HH:mm:ss}");
        }
    }

    #endregion

}