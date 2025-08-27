using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Connect4Game.Models;
using Connect4Game.Services;
using System.Collections.ObjectModel;
using Plugin.BLE.Abstractions.Contracts;

namespace Connect4Game.ViewModels
{
    public partial class MainMenuViewModel : ObservableObject
    {
        private readonly IGameService _gameService;
        private readonly IBluetoothService _bluetoothService;
        
        [ObservableProperty]
        private string hostName = "";
        
        [ObservableProperty]
        private bool isLoading = false;
        
        [ObservableProperty]
        private string statusMessage = "Welcome to Connect 4!";
        
        public ObservableCollection<IDevice> AvailableGames { get; } = new();
        
        public MainMenuViewModel(IGameService gameService, IBluetoothService bluetoothService)
        {
            _gameService = gameService;
            _bluetoothService = bluetoothService;
        }
        
        [RelayCommand]
        private async Task HostGame()
        {
            if (string.IsNullOrWhiteSpace(HostName))
            {
                StatusMessage = "Please enter a host name";
                return;
            }
            
            IsLoading = true;
            StatusMessage = "Starting game...";
            
            try
            {
                var success = await _gameService.CreateGameAsync(HostName);
                if (success)
                {
                    StatusMessage = "Game created! Waiting for players...";
                    await Shell.Current.GoToAsync("//game");
                }
                else
                {
                    StatusMessage = "Failed to create game. Please check Bluetooth permissions.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        [RelayCommand]
        private async Task ScanForGames()
        {
            IsLoading = true;
            StatusMessage = "Scanning for games...";
            AvailableGames.Clear();
            
            try
            {
                var devices = await _bluetoothService.ScanForGamesAsync();
                
                foreach (var device in devices)
                {
                    AvailableGames.Add(device);
                }
                
                StatusMessage = AvailableGames.Count > 0 
                    ? $"Found {AvailableGames.Count} games" 
                    : "No games found. Try local play!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error scanning: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        [RelayCommand]
        private async Task JoinGame(IDevice device)
        {
            IsLoading = true;
            StatusMessage = "Joining game...";
            
            try
            {
                var success = await _gameService.JoinGameAsync(device);
                if (success)
                {
                    StatusMessage = "Joined game successfully!";
                    await Shell.Current.GoToAsync("//game");
                }
                else
                {
                    StatusMessage = "Failed to join game";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        [RelayCommand]
        private async Task StartLocalGame()
        {
            IsLoading = true;
            StatusMessage = "Starting local game...";
            
            try
            {
                var success = await _gameService.CreateGameAsync("Local Player");
                if (success)
                {
                    StatusMessage = "Local game started!";
                    await Shell.Current.GoToAsync("//game");
                }
                else
                {
                    StatusMessage = "Failed to start local game";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}