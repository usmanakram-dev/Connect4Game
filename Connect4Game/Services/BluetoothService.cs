using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using System.Text;
using System.Text.Json;
using Connect4Game.Models;

namespace Connect4Game.Services
{
    public interface IBluetoothService
    {
        event EventHandler<string> MessageReceived;
        event EventHandler<string> PlayerConnected;
        event EventHandler<string> PlayerDisconnected;
        event EventHandler<bool> HostingStateChanged;
        
        Task<bool> StartHostingAsync(string hostName);
        Task StopHostingAsync();
        Task<List<IDevice>> ScanForGamesAsync();
        Task<bool> ConnectToGameAsync(IDevice device);
        Task DisconnectAsync();
        Task SendMessageAsync(string message);
        bool IsConnected { get; }
        bool IsHosting { get; }
    }

    public class BluetoothService : IBluetoothService
    {
        private IBluetoothLE _ble;
        private IAdapter _adapter;
        private IService _gameService;
        private ICharacteristic _messageCharacteristic;
        private List<IDevice> _connectedDevices = new();
        private IDevice _connectedDevice;
        
        public event EventHandler<string> MessageReceived;
        public event EventHandler<string> PlayerConnected;
        public event EventHandler<string> PlayerDisconnected;
        public event EventHandler<bool> HostingStateChanged;
        
        public bool IsConnected { get; private set; }
        public bool IsHosting { get; private set; }
        
        private const string GameServiceId = "12345678-1234-1234-1234-123456789abc";
        private const string MessageCharacteristicId = "87654321-4321-4321-4321-cba987654321";
        
        public BluetoothService()
        {
            _ble = CrossBluetoothLE.Current;
            _adapter = _ble.Adapter;
        }
        
        public async Task<bool> StartHostingAsync(string hostName)
        {
            try
            {
                var state = _ble.State;
                if (state != BluetoothState.On)
                {
                    return false;
                }
                
                _adapter.DeviceConnected += OnDeviceConnected;
                _adapter.DeviceDisconnected += OnDeviceDisconnected;
                
                IsHosting = true;
                HostingStateChanged?.Invoke(this, true);
                
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error starting hosting: {ex.Message}");
                return false;
            }
        }
        
        public async Task StopHostingAsync()
        {
            try
            {
                _adapter.DeviceConnected -= OnDeviceConnected;
                _adapter.DeviceDisconnected -= OnDeviceDisconnected;
                
                foreach (var device in _connectedDevices.ToList())
                {
                    await _adapter.DisconnectDeviceAsync(device);
                }
                _connectedDevices.Clear();
                
                IsHosting = false;
                HostingStateChanged?.Invoke(this, false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error stopping hosting: {ex.Message}");
            }
        }
        
        public async Task<List<IDevice>> ScanForGamesAsync()
        {
            var gameDevices = new List<IDevice>();
            
            try
            {
                _adapter.DeviceAdvertised += (sender, args) =>
                {
                    var device = args.Device;
                    if (device.Name?.StartsWith("Connect4-") == true)
                    {
                        gameDevices.Add(device);
                    }
                };
                
                await _adapter.StartScanningForDevicesAsync();
                await Task.Delay(10000); // Scan for 10 seconds
                await _adapter.StopScanningForDevicesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error scanning for games: {ex.Message}");
            }
            
            return gameDevices;
        }
        
        public async Task<bool> ConnectToGameAsync(IDevice device)
        {
            try
            {
                await _adapter.ConnectToDeviceAsync(device);
                _connectedDevice = device;
                
                var services = await device.GetServicesAsync();
                _gameService = services.FirstOrDefault(s => s.Id.ToString() == GameServiceId);
                
                if (_gameService != null)
                {
                    var characteristics = await _gameService.GetCharacteristicsAsync();
                    _messageCharacteristic = characteristics.FirstOrDefault(c => c.Id.ToString() == MessageCharacteristicId);
                    
                    if (_messageCharacteristic != null)
                    {
                        _messageCharacteristic.ValueUpdated += OnCharacteristicValueUpdated;
                        await _messageCharacteristic.StartUpdatesAsync();
                        
                        IsConnected = true;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error connecting to game: {ex.Message}");
            }
            
            return false;
        }
        
        public async Task DisconnectAsync()
        {
            try
            {
                if (_messageCharacteristic != null)
                {
                    await _messageCharacteristic.StopUpdatesAsync();
                    _messageCharacteristic.ValueUpdated -= OnCharacteristicValueUpdated;
                }
                
                if (_connectedDevice != null)
                {
                    await _adapter.DisconnectDeviceAsync(_connectedDevice);
                }
                
                IsConnected = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error disconnecting: {ex.Message}");
            }
        }
        
        public async Task SendMessageAsync(string message)
        {
            try
            {
                if (_messageCharacteristic != null && IsConnected)
                {
                    var data = Encoding.UTF8.GetBytes(message);
                    await _messageCharacteristic.WriteAsync(data);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sending message: {ex.Message}");
            }
        }
        
        private void OnDeviceConnected(object sender, DeviceEventArgs e)
        {
            _connectedDevices.Add(e.Device);
            PlayerConnected?.Invoke(this, e.Device.Name ?? "Unknown Player");
        }
        
        private void OnDeviceDisconnected(object sender, DeviceEventArgs e)
        {
            _connectedDevices.Remove(e.Device);
            PlayerDisconnected?.Invoke(this, e.Device.Name ?? "Unknown Player");
        }
        
        private void OnCharacteristicValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Characteristic.Value);
            MessageReceived?.Invoke(this, message);
        }
    }
}