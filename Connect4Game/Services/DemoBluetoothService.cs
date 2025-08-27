using Plugin.BLE.Abstractions.Contracts;

namespace Connect4Game.Services
{
    /// <summary>
    /// Demo Bluetooth service for testing without actual Bluetooth functionality
    /// </summary>
    public class DemoBluetoothService : IBluetoothService
    {
        public event EventHandler<string> MessageReceived;
        public event EventHandler<string> PlayerConnected;
        public event EventHandler<string> PlayerDisconnected;
        public event EventHandler<bool> HostingStateChanged;
        
        public bool IsConnected { get; private set; }
        public bool IsHosting { get; private set; }
        
        public async Task<bool> StartHostingAsync(string hostName)
        {
            await Task.Delay(1000); // Simulate startup time
            IsHosting = true;
            HostingStateChanged?.Invoke(this, true);
            
            // Simulate a player connecting after a delay
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000);
                PlayerConnected?.Invoke(this, "Demo Player");
            });
            
            return true;
        }
        
        public async Task StopHostingAsync()
        {
            await Task.Delay(500);
            IsHosting = false;
            HostingStateChanged?.Invoke(this, false);
        }
        
        public async Task<List<IDevice>> ScanForGamesAsync()
        {
            await Task.Delay(2000); // Simulate scanning time
            
            // Return empty list for demo - in real implementation this would return actual devices
            return new List<IDevice>();
        }
        
        public async Task<bool> ConnectToGameAsync(IDevice device)
        {
            await Task.Delay(1000);
            IsConnected = true;
            return true;
        }
        
        public async Task DisconnectAsync()
        {
            await Task.Delay(500);
            IsConnected = false;
        }
        
        public async Task SendMessageAsync(string message)
        {
            await Task.Delay(100);
            // In demo mode, we don't actually send messages
        }
    }
}