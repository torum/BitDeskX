using BitDesk.Models;
using System;
using System.Threading.Tasks;

namespace BitDesk.Services.Contracts
{
    internal interface IBitbankStreamService
    {
        event EventHandler<BitbankTickerData>? TickerUpdated;

        Task StartAsync();
        Task StopAsync();
    }
}