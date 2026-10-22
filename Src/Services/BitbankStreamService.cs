using BitDesk.ViewModels;
using SocketIOClient;
using SocketIOClient.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using BitDesk.Models;
using BitDesk.Services.Contracts;

namespace BitDesk.Services;

internal class BitbankStreamService : IBitbankStreamService
{
    private SocketIOClient.SocketIO? _client;
    //private bool _isRunning;
    private bool _isConnected;
    public event EventHandler<BitbankTickerData>? TickerUpdated;

    public BitbankStreamService()
    {
    }

    public async Task StartAsync()
    {
        // Bitbank Socket.IO endpoint (Engine.IO v4)
        var uri = new Uri("wss://stream.bitbank.cc");
        var options = new SocketIOOptions
        {
            EIO = EngineIO.V4,
            Transport = TransportProtocol.WebSocket
        };

        _client = new SocketIOClient.SocketIO(uri, options);

        _client.OnConnected += OnClientConnected;

        _client.OnDisconnected += (sender, e) =>
        {
            Debug.WriteLine($"--> Disconnected: {e}");
        };

        _client.On("message", response =>
        {
            var element = response.GetValue<System.Text.Json.JsonElement?>(0);
            if (!element.HasValue) return Task.CompletedTask;

            // Prints the raw JSON string for debugging purposes
            //string rawJsonString = element.Value.GetRawText();
            //Debug.WriteLine($"[RAW STREAM DATA]: {rawJsonString}");

            var root = response.GetValue<BitbankTickerRoot>(0);

            // Ensure data nodes exist before reading values
            if (root?.Message?.Data != null)
            {
                /*
                var data = root.Message.Data;
                string pair = (root.RoomName ?? "unknown").Replace("ticker_", "").ToUpper();
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

                string pair = (root.RoomName ?? "unknown").Replace("ticker_", "");

                if (Enum.TryParse(pair, true, out PairCodes pairCode))
                {
                    var data = root.Message.Data with { TickerPair = pairCode };
                    OnTickerUpdated(data);
                }
                else
                {
                    Debug.WriteLine($"--> Unknown pair code received: {pair}");
                }
            }

            return Task.CompletedTask;
        });

        await _client.ConnectAsync();
    }

    private async void OnClientConnected(object? sender, EventArgs e)
    {
        if (_client == null) return;
        _isConnected = true;

        Debug.WriteLine("--> Successfully connected to Bitbank Stream! Subscribing now...");

        var rooms = new[] { "ticker_btc_jpy", "ticker_eth_jpy", "ticker_xrp_jpy" };

        foreach (var room in rooms)
        {
            var roomArray = new[] { room };
            await _client.EmitAsync("join-room", roomArray);
        }
    }

    public async Task StopAsync()
    {
        _client?.OnConnected -= OnClientConnected;

        if (_client is not null && _isConnected == true)
        {
            try
            {
                if (_client.Connected)
                {
                    Debug.WriteLine("--> Disconnecting from Bitbank Stream...");
                    await _client.DisconnectAsync(); // 1. Signal graceful disconnect to server
                }
            }
            catch (System.Net.WebSockets.WebSocketException)
            {
                /* Swallow any concurrent connection drops */
            }
            _client.Dispose();
        }
    }

    protected virtual void OnTickerUpdated(BitbankTickerData data)
    {
        TickerUpdated?.Invoke(this, data);
    }
}
