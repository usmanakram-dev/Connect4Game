using Connect4Game.Models;
using System.Text.Json;
using Plugin.BLE.Abstractions.Contracts;

namespace Connect4Game.Services
{
    public interface IGameService
    {
        event EventHandler<GameMove> MoveMade;
        event EventHandler<GameState> GameStateChanged;
        event EventHandler<PlayerType?> GameEnded;
        
        GameSession CurrentSession { get; }
        bool IsHost { get; }
        PlayerType LocalPlayer { get; }
        
        Task<bool> CreateGameAsync(string hostName);
        Task<bool> JoinGameAsync(IDevice device);
        Task<bool> MakeMoveAsync(int column);
        Task LeaveGameAsync();
        void ResetGame();
    }

    public class GameService : IGameService
    {
        private readonly IBluetoothService _bluetoothService;
        
        public event EventHandler<GameMove> MoveMade;
        public event EventHandler<GameState> GameStateChanged;
        public event EventHandler<PlayerType?> GameEnded;
        
        public GameSession CurrentSession { get; private set; } = new();
        public bool IsHost { get; private set; }
        public PlayerType LocalPlayer { get; private set; } = PlayerType.Player1;
        
        public GameService(IBluetoothService bluetoothService)
        {
            _bluetoothService = bluetoothService;
            _bluetoothService.MessageReceived += OnMessageReceived;
            _bluetoothService.PlayerConnected += OnPlayerConnected;
            _bluetoothService.PlayerDisconnected += OnPlayerDisconnected;
        }
        
        public async Task<bool> CreateGameAsync(string hostName)
        {
            var success = await _bluetoothService.StartHostingAsync(hostName);
            if (success)
            {
                IsHost = true;
                LocalPlayer = PlayerType.Player1;
                CurrentSession = new GameSession
                {
                    HostName = hostName,
                    State = GameState.WaitingForPlayers
                };
                CurrentSession.Players.Add(hostName);
                
                GameStateChanged?.Invoke(this, CurrentSession.State);
            }
            return success;
        }
        
        public async Task<bool> JoinGameAsync(IDevice device)
        {
            var success = await _bluetoothService.ConnectToGameAsync(device);
            if (success)
            {
                IsHost = false;
                LocalPlayer = PlayerType.Player2;
                
                // Send join request
                var joinMessage = new
                {
                    Type = "JoinGame",
                    PlayerId = "Player2",
                    Timestamp = DateTime.Now
                };
                
                await _bluetoothService.SendMessageAsync(JsonSerializer.Serialize(joinMessage));
            }
            return success;
        }
        
        public async Task<bool> MakeMoveAsync(int column)
        {
            if (CurrentSession.State != GameState.InProgress ||
                CurrentSession.CurrentPlayer != LocalPlayer ||
                CurrentSession.Board.IsColumnFull(column))
            {
                return false;
            }
            
            // Make the move locally first
            var moveSuccess = CurrentSession.Board.DropPiece(column, LocalPlayer);
            if (!moveSuccess) return false;
            
            var move = new GameMove
            {
                Column = column,
                Player = LocalPlayer,
                Timestamp = DateTime.Now
            };
            
            // Check for win or draw
            if (CurrentSession.Board.CheckWin(LocalPlayer))
            {
                CurrentSession.State = GameState.PlayerWon;
                CurrentSession.Winner = LocalPlayer;
                GameEnded?.Invoke(this, LocalPlayer);
            }
            else if (CurrentSession.Board.IsBoardFull())
            {
                CurrentSession.State = GameState.GameOver;
                GameEnded?.Invoke(this, null);
            }
            else
            {
                // Switch turns
                CurrentSession.CurrentPlayer = CurrentSession.CurrentPlayer == PlayerType.Player1 
                    ? PlayerType.Player2 
                    : PlayerType.Player1;
            }
            
            // Send move to other players
            var moveMessage = new
            {
                Type = "Move",
                Move = move,
                GameState = CurrentSession.State,
                CurrentPlayer = CurrentSession.CurrentPlayer,
                Winner = CurrentSession.Winner
            };
            
            await _bluetoothService.SendMessageAsync(JsonSerializer.Serialize(moveMessage));
            
            MoveMade?.Invoke(this, move);
            GameStateChanged?.Invoke(this, CurrentSession.State);
            
            return true;
        }
        
        public async Task LeaveGameAsync()
        {
            if (IsHost)
            {
                await _bluetoothService.StopHostingAsync();
            }
            else
            {
                await _bluetoothService.DisconnectAsync();
            }
            
            CurrentSession = new GameSession();
            IsHost = false;
        }
        
        public void ResetGame()
        {
            CurrentSession.Board.Reset();
            CurrentSession.CurrentPlayer = PlayerType.Player1;
            CurrentSession.State = GameState.InProgress;
            CurrentSession.Winner = null;
            
            GameStateChanged?.Invoke(this, CurrentSession.State);
        }
        
        private void OnMessageReceived(object sender, string message)
        {
            try
            {
                using var document = JsonDocument.Parse(message);
                var root = document.RootElement;
                
                if (root.TryGetProperty("Type", out var typeProperty))
                {
                    var messageType = typeProperty.GetString();
                    
                    switch (messageType)
                    {
                        case "JoinGame":
                            if (IsHost && CurrentSession.Players.Count < 2)
                            {
                                CurrentSession.Players.Add("Player2");
                                CurrentSession.State = GameState.InProgress;
                                GameStateChanged?.Invoke(this, CurrentSession.State);
                            }
                            break;
                            
                        case "Move":
                            if (root.TryGetProperty("Move", out var moveProperty))
                            {
                                var move = JsonSerializer.Deserialize<GameMove>(moveProperty.GetRawText());
                                if (move != null)
                                {
                                    // Apply the move from the other player
                                    CurrentSession.Board.DropPiece(move.Column, move.Player);
                                    
                                    if (root.TryGetProperty("GameState", out var gameStateProperty))
                                    {
                                        CurrentSession.State = (GameState)gameStateProperty.GetInt32();
                                    }
                                    
                                    if (root.TryGetProperty("CurrentPlayer", out var currentPlayerProperty))
                                    {
                                        CurrentSession.CurrentPlayer = (PlayerType)currentPlayerProperty.GetInt32();
                                    }
                                    
                                    MoveMade?.Invoke(this, move);
                                    GameStateChanged?.Invoke(this, CurrentSession.State);
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error processing message: {ex.Message}");
            }
        }
        
        private void OnPlayerConnected(object sender, string playerName)
        {
            if (IsHost && CurrentSession.Players.Count < 2)
            {
                CurrentSession.Players.Add(playerName);
                if (CurrentSession.Players.Count == 2)
                {
                    CurrentSession.State = GameState.InProgress;
                    GameStateChanged?.Invoke(this, CurrentSession.State);
                }
            }
        }
        
        private void OnPlayerDisconnected(object sender, string playerName)
        {
            CurrentSession.Players.Remove(playerName);
            if (CurrentSession.Players.Count < 2 && CurrentSession.State == GameState.InProgress)
            {
                CurrentSession.State = GameState.WaitingForPlayers;
                GameStateChanged?.Invoke(this, CurrentSession.State);
            }
        }
    }
}