using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Connect4Game.Models;
using Connect4Game.Services;
using System.Collections.ObjectModel;

namespace Connect4Game.ViewModels
{
    public partial class GameViewModel : ObservableObject
    {
        private readonly IGameService _gameService;
        
        [ObservableProperty]
        private string currentPlayerText = "";
        
        [ObservableProperty]
        private string gameStatusText = "";
        
        [ObservableProperty]
        private bool isGameActive = false;
        
        [ObservableProperty]
        private bool isMyTurn = false;
        
        [ObservableProperty]
        private string playerOneColor = "#FF4444";
        
        [ObservableProperty]
        private string playerTwoColor = "#4444FF";
        
        [ObservableProperty]
        private string currentPlayerButtonColor = "#e94560";
        
        // Individual cell properties for XAML binding
        public CellViewModel[,] GameBoardCells { get; private set; }
        
        // Individual properties for each cell (6 rows x 7 columns = 42 properties)
        public CellViewModel Cell00 => GameBoardCells[0, 0];
        public CellViewModel Cell01 => GameBoardCells[0, 1];
        public CellViewModel Cell02 => GameBoardCells[0, 2];
        public CellViewModel Cell03 => GameBoardCells[0, 3];
        public CellViewModel Cell04 => GameBoardCells[0, 4];
        public CellViewModel Cell05 => GameBoardCells[0, 5];
        public CellViewModel Cell06 => GameBoardCells[0, 6];
        
        public CellViewModel Cell10 => GameBoardCells[1, 0];
        public CellViewModel Cell11 => GameBoardCells[1, 1];
        public CellViewModel Cell12 => GameBoardCells[1, 2];
        public CellViewModel Cell13 => GameBoardCells[1, 3];
        public CellViewModel Cell14 => GameBoardCells[1, 4];
        public CellViewModel Cell15 => GameBoardCells[1, 5];
        public CellViewModel Cell16 => GameBoardCells[1, 6];
        
        public CellViewModel Cell20 => GameBoardCells[2, 0];
        public CellViewModel Cell21 => GameBoardCells[2, 1];
        public CellViewModel Cell22 => GameBoardCells[2, 2];
        public CellViewModel Cell23 => GameBoardCells[2, 3];
        public CellViewModel Cell24 => GameBoardCells[2, 4];
        public CellViewModel Cell25 => GameBoardCells[2, 5];
        public CellViewModel Cell26 => GameBoardCells[2, 6];
        
        public CellViewModel Cell30 => GameBoardCells[3, 0];
        public CellViewModel Cell31 => GameBoardCells[3, 1];
        public CellViewModel Cell32 => GameBoardCells[3, 2];
        public CellViewModel Cell33 => GameBoardCells[3, 3];
        public CellViewModel Cell34 => GameBoardCells[3, 4];
        public CellViewModel Cell35 => GameBoardCells[3, 5];
        public CellViewModel Cell36 => GameBoardCells[3, 6];
        
        public CellViewModel Cell40 => GameBoardCells[4, 0];
        public CellViewModel Cell41 => GameBoardCells[4, 1];
        public CellViewModel Cell42 => GameBoardCells[4, 2];
        public CellViewModel Cell43 => GameBoardCells[4, 3];
        public CellViewModel Cell44 => GameBoardCells[4, 4];
        public CellViewModel Cell45 => GameBoardCells[4, 5];
        public CellViewModel Cell46 => GameBoardCells[4, 6];
        
        public CellViewModel Cell50 => GameBoardCells[5, 0];
        public CellViewModel Cell51 => GameBoardCells[5, 1];
        public CellViewModel Cell52 => GameBoardCells[5, 2];
        public CellViewModel Cell53 => GameBoardCells[5, 3];
        public CellViewModel Cell54 => GameBoardCells[5, 4];
        public CellViewModel Cell55 => GameBoardCells[5, 5];
        public CellViewModel Cell56 => GameBoardCells[5, 6];
        
        public GameViewModel(IGameService gameService)
        {
            _gameService = gameService;
            
            // Subscribe to game service events
            _gameService.MoveMade += OnMoveMade;
            _gameService.GameStateChanged += OnGameStateChanged;
            _gameService.GameEnded += OnGameEnded;
            
            InitializeBoard();
            UpdateGameStatus();
        }
        
        private void InitializeBoard()
        {
            // Initialize the 2D array
            GameBoardCells = new CellViewModel[Models.GameBoard.Rows, Models.GameBoard.Columns];
            
            for (int row = 0; row < Models.GameBoard.Rows; row++)
            {
                for (int col = 0; col < Models.GameBoard.Columns; col++)
                {
                    GameBoardCells[row, col] = new CellViewModel(row, col);
                }
            }
            
            System.Diagnostics.Debug.WriteLine($"GameBoard initialized with {Models.GameBoard.Rows} rows and {Models.GameBoard.Columns} columns");
            
            // Notify all cell properties
            NotifyAllCellProperties();
        }
        
        private void NotifyAllCellProperties()
        {
            // Notify all individual cell properties
            OnPropertyChanged(nameof(Cell00)); OnPropertyChanged(nameof(Cell01)); OnPropertyChanged(nameof(Cell02)); OnPropertyChanged(nameof(Cell03)); OnPropertyChanged(nameof(Cell04)); OnPropertyChanged(nameof(Cell05)); OnPropertyChanged(nameof(Cell06));
            OnPropertyChanged(nameof(Cell10)); OnPropertyChanged(nameof(Cell11)); OnPropertyChanged(nameof(Cell12)); OnPropertyChanged(nameof(Cell13)); OnPropertyChanged(nameof(Cell14)); OnPropertyChanged(nameof(Cell15)); OnPropertyChanged(nameof(Cell16));
            OnPropertyChanged(nameof(Cell20)); OnPropertyChanged(nameof(Cell21)); OnPropertyChanged(nameof(Cell22)); OnPropertyChanged(nameof(Cell23)); OnPropertyChanged(nameof(Cell24)); OnPropertyChanged(nameof(Cell25)); OnPropertyChanged(nameof(Cell26));
            OnPropertyChanged(nameof(Cell30)); OnPropertyChanged(nameof(Cell31)); OnPropertyChanged(nameof(Cell32)); OnPropertyChanged(nameof(Cell33)); OnPropertyChanged(nameof(Cell34)); OnPropertyChanged(nameof(Cell35)); OnPropertyChanged(nameof(Cell36));
            OnPropertyChanged(nameof(Cell40)); OnPropertyChanged(nameof(Cell41)); OnPropertyChanged(nameof(Cell42)); OnPropertyChanged(nameof(Cell43)); OnPropertyChanged(nameof(Cell44)); OnPropertyChanged(nameof(Cell45)); OnPropertyChanged(nameof(Cell46));
            OnPropertyChanged(nameof(Cell50)); OnPropertyChanged(nameof(Cell51)); OnPropertyChanged(nameof(Cell52)); OnPropertyChanged(nameof(Cell53)); OnPropertyChanged(nameof(Cell54)); OnPropertyChanged(nameof(Cell55)); OnPropertyChanged(nameof(Cell56));
        }
        
        [RelayCommand]
        private async Task DropPiece(string columnParameter)
        {
            System.Diagnostics.Debug.WriteLine($"DropPiece command executed with parameter: {columnParameter}");
            
            if (int.TryParse(columnParameter, out int column) && IsGameActive)
            {
                System.Diagnostics.Debug.WriteLine($"Attempting to drop piece in column {column}");
                
                // In demo mode, allow moves regardless of turn (for local multiplayer)
                // In real multiplayer mode, check IsMyTurn
                bool canMakeMove = _gameService is DemoGameService || IsMyTurn;
                
                System.Diagnostics.Debug.WriteLine($"Can make move: {canMakeMove}, IsGameActive: {IsGameActive}, IsMyTurn: {IsMyTurn}");
                
                if (canMakeMove)
                {
                    var success = await _gameService.MakeMoveAsync(column);
                    System.Diagnostics.Debug.WriteLine($"Move result: {success}");
                    
                    if (success)
                    {
                        // Force immediate board update
                        System.Diagnostics.Debug.WriteLine("Move successful, updating board display...");
                        UpdateBoardDisplay();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Move failed - piece could not be dropped");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Cannot make move - conditions not met");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Invalid move: columnParameter={columnParameter}, IsGameActive={IsGameActive}");
            }
        }
        
        [RelayCommand]
        private async Task LeaveGame()
        {
            await _gameService.LeaveGameAsync();
            await Shell.Current.GoToAsync("//mainmenu");
        }
        
        [RelayCommand]
        private void ResetGame()
        {
            if (_gameService.IsHost)
            {
                _gameService.ResetGame();
                UpdateBoardDisplay(); // Force update after reset
                UpdateGameStatus();
            }
        }
        
        private void OnMoveMade(object sender, GameMove move)
        {
            System.Diagnostics.Debug.WriteLine($"OnMoveMade event received for column {move.Column}, player {move.Player}");
            MainThread.BeginInvokeOnMainThread(() =>
            {
                UpdateBoardDisplay();
                UpdateGameStatus();
            });
        }
        
        private void OnGameStateChanged(object sender, GameState newState)
        {
            System.Diagnostics.Debug.WriteLine($"OnGameStateChanged event received: {newState}");
            MainThread.BeginInvokeOnMainThread(() =>
            {
                UpdateGameStatus();
            });
        }
        
        private void OnGameEnded(object sender, PlayerType? winner)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsGameActive = false;
                IsMyTurn = false;
                
                if (winner.HasValue)
                {
                    var winnerText = _gameService is DemoGameService 
                        ? GetPlayerDisplayName(winner.Value)
                        : (winner == _gameService.LocalPlayer ? "You" : "Opponent");
                    GameStatusText = $"*** {winnerText} Won! ***";
                }
                else
                {
                    GameStatusText = "*** It's a Draw! ***";
                }
                
                // Reset button color when game ends
                CurrentPlayerButtonColor = "#666666";
            });
        }
        
        private string GetPlayerDisplayName(PlayerType player)
        {
            return player switch
            {
                PlayerType.Player1 => "Player 1",
                PlayerType.Player2 => "Player 2",
                _ => "Unknown Player"
            };
        }
        
        private void UpdateBoardDisplay()
        {
            System.Diagnostics.Debug.WriteLine("=== UpdateBoardDisplay START ===");
            
            if (_gameService?.CurrentSession?.Board == null)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: GameService, CurrentSession, or Board is null");
                return;
            }
            
            System.Diagnostics.Debug.WriteLine("Updating board display");
            
            // Update the 2D array cells
            for (int row = 0; row < Models.GameBoard.Rows; row++)
            {
                for (int col = 0; col < Models.GameBoard.Columns; col++)
                {
                    var cellValue = _gameService.CurrentSession.Board.GetCell(row, col);
                    var cellViewModel = GameBoardCells[row, col];
                    
                    if (cellViewModel.Player != cellValue)
                    {
                        System.Diagnostics.Debug.WriteLine($"Updating cell [{row},{col}] from {cellViewModel.Player} to {cellValue}");
                        
                        cellViewModel.Player = cellValue;
                        cellViewModel.BackgroundColor = cellValue switch
                        {
                            PlayerType.Player1 => PlayerOneColor,
                            PlayerType.Player2 => PlayerTwoColor,
                            _ => "#F0F0F0"
                        };
                        
                        System.Diagnostics.Debug.WriteLine($"Cell [{row},{col}] updated: Player={cellViewModel.Player}, Color={cellViewModel.BackgroundColor}, Text={cellViewModel.DisplayText}");
                    }
                }
            }
            
            // Notify all cell properties changed
            NotifyAllCellProperties();
            
            System.Diagnostics.Debug.WriteLine("=== UpdateBoardDisplay END ===");
        }
        
        private void UpdateGameStatus()
        {
            var session = _gameService.CurrentSession;
            
            switch (session.State)
            {
                case GameState.WaitingForPlayers:
                    GameStatusText = _gameService.IsHost 
                        ? "Waiting for players to join..." 
                        : "Looking for host...";
                    IsGameActive = false;
                    IsMyTurn = false;
                    CurrentPlayerButtonColor = "#666666"; // Gray when not active
                    break;
                    
                case GameState.InProgress:
                    IsGameActive = true;
                    
                    // Update button color to match current player
                    CurrentPlayerButtonColor = session.CurrentPlayer switch
                    {
                        PlayerType.Player1 => PlayerOneColor,
                        PlayerType.Player2 => PlayerTwoColor,
                        _ => "#e94560" // Default color
                    };
                    
                    // In demo mode, always allow moves (local multiplayer)
                    // In real multiplayer, check if it's the player's turn
                    if (_gameService is DemoGameService)
                    {
                        IsMyTurn = true; // Allow both players to move in demo mode
                        var currentPlayerName = GetPlayerDisplayName(session.CurrentPlayer);
                        CurrentPlayerText = $"{currentPlayerName}'s Turn";
                        GameStatusText = $"> {CurrentPlayerText}";
                    }
                    else
                    {
                        IsMyTurn = session.CurrentPlayer == _gameService.LocalPlayer;
                        var currentPlayerName = session.CurrentPlayer == _gameService.LocalPlayer ? "Your" : "Opponent's";
                        CurrentPlayerText = $"{currentPlayerName} Turn";
                        var turnSymbol = IsMyTurn ? ">" : "...";
                        GameStatusText = $"{turnSymbol} {CurrentPlayerText}";
                    }
                    break;
                    
                case GameState.GameOver:
                case GameState.PlayerWon:
                    IsGameActive = false;
                    IsMyTurn = false;
                    CurrentPlayerButtonColor = "#666666"; // Gray when game is over
                    break;
            }
            
            System.Diagnostics.Debug.WriteLine($"Game status updated: IsGameActive={IsGameActive}, IsMyTurn={IsMyTurn}, Status={GameStatusText}, ButtonColor={CurrentPlayerButtonColor}");
        }
    }
    
    public partial class CellViewModel : ObservableObject
    {
        public int Row { get; }
        public int Column { get; }
        
        [ObservableProperty]
        private PlayerType player = PlayerType.None;
        
        [ObservableProperty]
        private string backgroundColor = "#F0F0F0";
        
        [ObservableProperty]
        private string displayText = "";
        
        public CellViewModel(int row, int column)
        {
            Row = row;
            Column = column;
        }
        
        partial void OnPlayerChanged(PlayerType value)
        {
            // Use simple, universally supported characters as fallback
            DisplayText = value switch
            {
                PlayerType.Player1 => "O",  // Simple letter O
                PlayerType.Player2 => "O",  // Simple letter O
                _ => ""
            };
            
            System.Diagnostics.Debug.WriteLine($"CellViewModel [{Row},{Column}] Player changed to {value}, DisplayText={DisplayText}");
        }
    }
}