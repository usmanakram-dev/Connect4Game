using Connect4Game.Models;
using Plugin.BLE.Abstractions.Contracts;

namespace Connect4Game.Services
{
    /// <summary>
    /// Demo service for local multiplayer testing without Bluetooth
    /// </summary>
    public class DemoGameService : IGameService
    {
        public event EventHandler<GameMove> MoveMade;
        public event EventHandler<GameState> GameStateChanged;
        public event EventHandler<PlayerType?> GameEnded;
        
        public GameSession CurrentSession { get; private set; } = new();
        public bool IsHost { get; private set; }
        public PlayerType LocalPlayer { get; private set; } = PlayerType.Player1;
        
        public async Task<bool> CreateGameAsync(string hostName)
        {
            await Task.Delay(500); // Simulate async operation
            
            IsHost = true;
            LocalPlayer = PlayerType.Player1;
            CurrentSession = new GameSession
            {
                HostName = hostName,
                State = GameState.InProgress // Start immediately for demo
            };
            CurrentSession.Players.Add(hostName);
            CurrentSession.Players.Add("Demo Player 2"); // Add second player for demo
            
            System.Diagnostics.Debug.WriteLine($"Demo game created. State: {CurrentSession.State}, Current Player: {CurrentSession.CurrentPlayer}");
            
            // Print initial board state
            System.Diagnostics.Debug.WriteLine("Initial board state after game creation:");
            CurrentSession.Board.PrintBoardState();
            
            GameStateChanged?.Invoke(this, CurrentSession.State);
            return true;
        }
        
        public async Task<bool> JoinGameAsync(IDevice device)
        {
            await Task.Delay(500); // Simulate async operation
            
            IsHost = false;
            LocalPlayer = PlayerType.Player2;
            CurrentSession.State = GameState.InProgress;
            
            GameStateChanged?.Invoke(this, CurrentSession.State);
            return true;
        }
        
        public async Task<bool> MakeMoveAsync(int column)
        {
            System.Diagnostics.Debug.WriteLine($"MakeMoveAsync called: Column={column}, State={CurrentSession.State}, CurrentPlayer={CurrentSession.CurrentPlayer}");
            
            // Print board state before move
            System.Diagnostics.Debug.WriteLine("Board state BEFORE move:");
            CurrentSession.Board.PrintBoardState();
            
            if (CurrentSession.State != GameState.InProgress)
            {
                System.Diagnostics.Debug.WriteLine("Game not in progress");
                return false;
            }
                
            if (CurrentSession.Board.IsColumnFull(column))
            {
                System.Diagnostics.Debug.WriteLine($"Column {column} is full");
                return false;
            }
            
            // Make the move with the current player (not restricted to LocalPlayer in demo mode)
            var moveSuccess = CurrentSession.Board.DropPiece(column, CurrentSession.CurrentPlayer);
            if (!moveSuccess)
            {
                System.Diagnostics.Debug.WriteLine("Failed to drop piece");
                return false;
            }
            
            System.Diagnostics.Debug.WriteLine($"Piece dropped successfully in column {column} for player {CurrentSession.CurrentPlayer}");
            
            // Print board state after move
            System.Diagnostics.Debug.WriteLine("Board state AFTER move:");
            CurrentSession.Board.PrintBoardState();
            
            var move = new GameMove
            {
                Column = column,
                Player = CurrentSession.CurrentPlayer,
                Timestamp = DateTime.Now
            };
            
            // Check for win or draw
            if (CurrentSession.Board.CheckWin(CurrentSession.CurrentPlayer))
            {
                CurrentSession.State = GameState.PlayerWon;
                CurrentSession.Winner = CurrentSession.CurrentPlayer;
                System.Diagnostics.Debug.WriteLine($"Player {CurrentSession.CurrentPlayer} won!");
                GameEnded?.Invoke(this, CurrentSession.CurrentPlayer);
            }
            else if (CurrentSession.Board.IsBoardFull())
            {
                CurrentSession.State = GameState.GameOver;
                System.Diagnostics.Debug.WriteLine("Game is a draw!");
                GameEnded?.Invoke(this, null);
            }
            else
            {
                // Switch turns
                var previousPlayer = CurrentSession.CurrentPlayer;
                CurrentSession.CurrentPlayer = CurrentSession.CurrentPlayer == PlayerType.Player1 
                    ? PlayerType.Player2 
                    : PlayerType.Player1;
                System.Diagnostics.Debug.WriteLine($"Turn switched from {previousPlayer} to {CurrentSession.CurrentPlayer}");
            }
            
            MoveMade?.Invoke(this, move);
            GameStateChanged?.Invoke(this, CurrentSession.State);
            
            await Task.Delay(100); // Small delay for smooth UX
            return true;
        }
        
        public async Task LeaveGameAsync()
        {
            await Task.Delay(100);
            CurrentSession = new GameSession();
            IsHost = false;
        }
        
        public void ResetGame()
        {
            CurrentSession.Board.Reset();
            CurrentSession.CurrentPlayer = PlayerType.Player1;
            CurrentSession.State = GameState.InProgress;
            CurrentSession.Winner = null;
            
            System.Diagnostics.Debug.WriteLine("Game reset");
            
            // Print board state after reset
            System.Diagnostics.Debug.WriteLine("Board state after reset:");
            CurrentSession.Board.PrintBoardState();
            
            GameStateChanged?.Invoke(this, CurrentSession.State);
        }
    }
}