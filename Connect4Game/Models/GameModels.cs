namespace Connect4Game.Models
{
    public enum PlayerType
    {
        None = 0,    // Explicitly set None to 0 so it's the default
        Player1 = 1,
        Player2 = 2
    }

    public enum GameState
    {
        WaitingForPlayers,
        InProgress,
        GameOver,
        PlayerWon
    }

    public class GameMove
    {
        public int Column { get; set; }
        public PlayerType Player { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class GameBoard
    {
        public const int Rows = 6;
        public const int Columns = 7;
        
        private PlayerType[,] _board;
        
        public GameBoard()
        {
            Reset(); // Initialize the board properly
        }
        
        public PlayerType GetCell(int row, int column) => _board[row, column];
        
        public bool DropPiece(int column, PlayerType player)
        {
            if (column < 0 || column >= Columns) return false;
            
            for (int row = Rows - 1; row >= 0; row--)
            {
                if (_board[row, column] == PlayerType.None)
                {
                    _board[row, column] = player;
                    return true;
                }
            }
            return false;
        }
        
        public bool IsColumnFull(int column)
        {
            // Return false for invalid columns (they're not "full", they're invalid)
            if (column < 0 || column >= Columns)
            {
                System.Diagnostics.Debug.WriteLine($"IsColumnFull: Column {column} is invalid (out of bounds)");
                return false;
            }
            
            // A column is full if the top row (row 0) has a piece
            var topCell = _board[0, column];
            var isFull = topCell != PlayerType.None;
            
            System.Diagnostics.Debug.WriteLine($"IsColumnFull: Column {column}, Top cell value: {topCell}, Is full: {isFull}");
            
            return isFull;
        }
        
        public bool CheckWin(PlayerType player)
        {
            // Check horizontal
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col <= Columns - 4; col++)
                {
                    if (CheckLine(row, col, 0, 1, player)) return true;
                }
            }
            
            // Check vertical
            for (int row = 0; row <= Rows - 4; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (CheckLine(row, col, 1, 0, player)) return true;
                }
            }
            
            // Check diagonal (top-left to bottom-right)
            for (int row = 0; row <= Rows - 4; row++)
            {
                for (int col = 0; col <= Columns - 4; col++)
                {
                    if (CheckLine(row, col, 1, 1, player)) return true;
                }
            }
            
            // Check diagonal (top-right to bottom-left)
            for (int row = 0; row <= Rows - 4; row++)
            {
                for (int col = 3; col < Columns; col++)
                {
                    if (CheckLine(row, col, 1, -1, player)) return true;
                }
            }
            
            return false;
        }
        
        private bool CheckLine(int startRow, int startCol, int deltaRow, int deltaCol, PlayerType player)
        {
            for (int i = 0; i < 4; i++)
            {
                int row = startRow + i * deltaRow;
                int col = startCol + i * deltaCol;
                if (_board[row, col] != player) return false;
            }
            return true;
        }
        
        public bool IsBoardFull()
        {
            for (int col = 0; col < Columns; col++)
            {
                if (!IsColumnFull(col)) return false;
            }
            return true;
        }
        
        public void Reset()
        {
            _board = new PlayerType[Rows, Columns];
            
            // Explicitly initialize all cells to PlayerType.None
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    _board[row, col] = PlayerType.None;
                }
            }
            
            System.Diagnostics.Debug.WriteLine("GameBoard reset - all cells explicitly set to PlayerType.None");
            
            // Debug: Print the board state after reset
            PrintBoardState();
        }
        
        /// <summary>
        /// Helper method to check if a column is valid (within bounds)
        /// </summary>
        public bool IsValidColumn(int column)
        {
            return column >= 0 && column < Columns;
        }
        
        /// <summary>
        /// Debug method to print the entire board state
        /// </summary>
        public void PrintBoardState()
        {
            System.Diagnostics.Debug.WriteLine("=== BOARD STATE ===");
            for (int row = 0; row < Rows; row++)
            {
                var rowString = "";
                for (int col = 0; col < Columns; col++)
                {
                    var cell = _board[row, col];
                    var symbol = cell switch
                    {
                        PlayerType.Player1 => "1",
                        PlayerType.Player2 => "2",
                        PlayerType.None => ".",
                        _ => "?"
                    };
                    rowString += symbol + " ";
                }
                System.Diagnostics.Debug.WriteLine($"Row {row}: {rowString}");
            }
            System.Diagnostics.Debug.WriteLine("==================");
        }
    }

    public class GameSession
    {
        public string SessionId { get; set; } = Guid.NewGuid().ToString();
        public string HostName { get; set; } = string.Empty;
        public List<string> Players { get; set; } = new();
        public GameBoard Board { get; set; } = new();
        public PlayerType CurrentPlayer { get; set; } = PlayerType.Player1;
        public GameState State { get; set; } = GameState.WaitingForPlayers;
        public PlayerType? Winner { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}