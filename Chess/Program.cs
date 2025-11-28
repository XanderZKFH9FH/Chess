using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    // === ABSTRACT PIECE CLASS ===
    abstract class Piece
    {
        public bool IsWhite { get; set; }
        public string Symbol { get; set; }
        public bool HasMoved { get; set; } = false;

        // Added 'isAttackingCheck' to prevent recursion bugs
        public abstract List<(int, int)> GetValidMoves(int x, int y, Piece[,] board, (int x, int y)? enPassantTarget = null, bool isAttackingCheck = false);
    }

    // === KING CLASS ===
    class King : Piece
    {
        public King(bool isWhite) { IsWhite = isWhite; Symbol = isWhite ? "WK" : "BK"; }

        public override List<(int, int)> GetValidMoves(int x, int y, Piece[,] board, (int x, int y)? enPassantTarget = null, bool isAttackingCheck = false)
        {
            List<(int, int)> moves = new List<(int, int)>();
            int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

            // 1. Normal King Moves
            for (int i = 0; i < 8; i++)
            {
                int nx = x + dx[i], ny = y + dy[i];
                if (nx >= 0 && nx < 8 && ny >= 0 && ny < 8)
                    if (board[nx, ny] == null || board[nx, ny].IsWhite != IsWhite)
                        moves.Add((nx, ny));
            }

            // 2. Castling Logic
            // Only check castling if not checking for attacks (prevents infinite loop)
            if (!isAttackingCheck && !HasMoved && !Program.IsKingInCheck(board, IsWhite))
            {
                int rank = IsWhite ? 0 : 7;

                // Queenside Castling
                if (board[rank, 0] is Rook queensideRook && !queensideRook.HasMoved &&
                    board[rank, 1] == null && board[rank, 2] == null && board[rank, 3] == null)
                {
                    if (!Program.IsSquareAttacked(rank, 3, board, !IsWhite) &&
                        !Program.IsSquareAttacked(rank, 2, board, !IsWhite))
                    {
                        moves.Add((rank, 2));
                    }
                }

                // Kingside Castling
                if (board[rank, 7] is Rook kingsideRook && !kingsideRook.HasMoved &&
                    board[rank, 5] == null && board[rank, 6] == null)
                {
                    if (!Program.IsSquareAttacked(rank, 5, board, !IsWhite) &&
                        !Program.IsSquareAttacked(rank, 6, board, !IsWhite))
                    {
                        moves.Add((rank, 6));
                    }
                }
            }

            return moves;
        }
    }

    // === QUEEN CLASS ===
    class Queen : Piece
    {
        public Queen(bool isWhite) { IsWhite = isWhite; Symbol = isWhite ? "WQ" : "BQ"; }

        public override List<(int, int)> GetValidMoves(int x, int y, Piece[,] board, (int x, int y)? enPassantTarget = null, bool isAttackingCheck = false)
        {
            List<(int, int)> moves = new List<(int, int)>();
            int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

            foreach (var dir in dx.Zip(dy, (a, b) => (a, b)))
            {
                int nx = x + dir.a, ny = y + dir.b;
                while (nx >= 0 && nx < 8 && ny >= 0 && ny < 8)
                {
                    if (board[nx, ny] == null) moves.Add((nx, ny));
                    else { if (board[nx, ny].IsWhite != IsWhite) moves.Add((nx, ny)); break; }
                    nx += dir.a; ny += dir.b;
                }
            }
            return moves;
        }
    }

    // === ROOK CLASS ===
    class Rook : Piece
    {
        public Rook(bool isWhite) { IsWhite = isWhite; Symbol = isWhite ? "WR" : "BR"; }

        public override List<(int, int)> GetValidMoves(int x, int y, Piece[,] board, (int x, int y)? enPassantTarget = null, bool isAttackingCheck = false)
        {
            List<(int, int)> moves = new List<(int, int)>();
            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int nx = x + dx[i], ny = y + dy[i];
                while (nx >= 0 && nx < 8 && ny >= 0 && ny < 8)
                {
                    if (board[nx, ny] == null) moves.Add((nx, ny));
                    else { if (board[nx, ny].IsWhite != IsWhite) moves.Add((nx, ny)); break; }
                    nx += dx[i]; ny += dy[i];
                }
            }
            return moves;
        }
    }

    // === BISHOP CLASS ===
    class Bishop : Piece
    {
        public Bishop(bool isWhite) { IsWhite = isWhite; Symbol = isWhite ? "WB" : "BB"; }

        public override List<(int, int)> GetValidMoves(int x, int y, Piece[,] board, (int x, int y)? enPassantTarget = null, bool isAttackingCheck = false)
        {
            List<(int, int)> moves = new List<(int, int)>();
            int[] dx = { -1, -1, 1, 1 };
            int[] dy = { -1, 1, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int nx = x + dx[i], ny = y + dy[i];
                while (nx >= 0 && nx < 8 && ny >= 0 && ny < 8)
                {
                    if (board[nx, ny] == null) moves.Add((nx, ny));
                    else { if (board[nx, ny].IsWhite != IsWhite) moves.Add((nx, ny)); break; }
                    nx += dx[i]; ny += dy[i];
                }
            }
            return moves;
        }
    }

    // === KNIGHT CLASS ===
    class Knight : Piece
    {
        public Knight(bool isWhite) { IsWhite = isWhite; Symbol = isWhite ? "WN" : "BN"; }

        public override List<(int, int)> GetValidMoves(int x, int y, Piece[,] board, (int x, int y)? enPassantTarget = null, bool isAttackingCheck = false)
        {
            List<(int, int)> moves = new List<(int, int)>();
            int[] dx = { -2, -2, -1, -1, 1, 1, 2, 2 };
            int[] dy = { -1, 1, -2, 2, -2, 2, -1, 1 };

            for (int i = 0; i < 8; i++)
            {
                int nx = x + dx[i], ny = y + dy[i];
                if (nx >= 0 && nx < 8 && ny >= 0 && ny < 8)
                    if (board[nx, ny] == null || board[nx, ny].IsWhite != IsWhite)
                        moves.Add((nx, ny));
            }
            return moves;
        }
    }

    // === PAWN CLASS ===
    class Pawn : Piece
    {
        public Pawn(bool isWhite) { IsWhite = isWhite; Symbol = isWhite ? "WP" : "BP"; }

        public override List<(int, int)> GetValidMoves(int x, int y, Piece[,] board, (int x, int y)? enPassantTarget = null, bool isAttackingCheck = false)
        {
            List<(int, int)> moves = new List<(int, int)>();
            int dir = IsWhite ? 1 : -1;
            int startRow = IsWhite ? 1 : 6;

            // 1. Single Step Forward (Skipped if only checking attacks)
            if (!isAttackingCheck && x + dir >= 0 && x + dir < 8 && board[x + dir, y] == null)
                moves.Add((x + dir, y));

            // 2. Double Step Forward (Skipped if only checking attacks)
            if (!isAttackingCheck && x == startRow && board[x + dir, y] == null && board[x + 2 * dir, y] == null)
                moves.Add((x + 2 * dir, y));

            // 3. Captures
            // Left diagonal target
            if (y - 1 >= 0)
            {
                int nx = x + dir;
                int ny = y - 1;
                if (nx >= 0 && nx < 8) // Check bounds
                {
                    Piece target = board[nx, ny];
                    // If checking attacks, add the square. If moving, must be an enemy.
                    if (isAttackingCheck || (target != null && target.IsWhite != IsWhite))
                        moves.Add((nx, ny));
                }
            }
            // Right diagonal target
            if (y + 1 < 8)
            {
                int nx = x + dir;
                int ny = y + 1;
                if (nx >= 0 && nx < 8) // Check bounds
                {
                    Piece target = board[nx, ny];
                    if (isAttackingCheck || (target != null && target.IsWhite != IsWhite))
                        moves.Add((nx, ny));
                }
            }

            // 4. En passant (Skipped if checking attacks)
            if (!isAttackingCheck && enPassantTarget.HasValue)
            {
                var ep = enPassantTarget.Value;
                if (x + dir == ep.Item1 && (y - 1 == ep.Item2 || y + 1 == ep.Item2))
                    moves.Add(ep);
            }

            return moves;
        }
    }

    // === PROGRAM / GAME LOGIC CLASS ===
    class Program
    {
        static void Main(string[] args)
        {
            Piece[,] board = InitializeBoard();
            bool whiteTurn = true;
            (int x, int y)? enPassantTarget = null;

            while (true)
            {
                Console.Clear();
                DisplayBoard(board);
                Console.WriteLine(IsKingInCheck(board, whiteTurn) ? "\n*** CHECK! ***" : "");
                Console.WriteLine(whiteTurn ? "\nWhite's move" : "\nBlack's move");

                // --- CHECKMATE / STALEMATE LOGIC ---
                if (!HasLegalMoves(board, whiteTurn, enPassantTarget))
                {
                    if (IsKingInCheck(board, whiteTurn))
                    {
                        Console.WriteLine($"\n*** CHECKMATE! {(!whiteTurn ? "White" : "Black")} Wins! ***");
                    }
                    else
                    {
                        Console.WriteLine("\n*** STALEMATE! The game is a draw. ***");
                    }
                    Console.Write("Press Enter to exit...");
                    Console.ReadLine();
                    break;
                }
                // ------------------------------------

                Console.Write("Enter move (e.g., e2 e4): ");
                string input = Console.ReadLine();
                string[] parts = input.Split(' ');
                if (parts.Length != 2) { Console.WriteLine("Invalid format. Press Enter."); Console.ReadLine(); continue; }

                var from = ConvertCoordinates(parts[0]);
                var to = ConvertCoordinates(parts[1]);
                if (from == null || to == null) { Console.WriteLine("Invalid coordinates. Press Enter."); Console.ReadLine(); continue; }

                Piece selected = board[from.Value.x, from.Value.y];
                if (selected == null) { Console.WriteLine("No piece at source. Press Enter."); Console.ReadLine(); continue; }
                if (selected.IsWhite != whiteTurn) { Console.WriteLine("Not your turn. Press Enter."); Console.ReadLine(); continue; }

                // Get moves for the player
                List<(int, int)> validMoves = selected.GetValidMoves(from.Value.x, from.Value.y, board, enPassantTarget);
                if (!validMoves.Contains((to.Value.x, to.Value.y))) { Console.WriteLine("Illegal move. Press Enter."); Console.ReadLine(); continue; }

                // --- LEGALITY CHECK: King must not be in check after the move ---
                Piece[,] nextBoard = CopyBoard(board);
                Piece movingPiece = nextBoard[from.Value.x, from.Value.y];

                // 1. Simulate En passant capture removal first for the legality test
                if (movingPiece is Pawn && enPassantTarget.HasValue && (to.Value.x, to.Value.y) == enPassantTarget.Value)
                {
                    // **LOGIC FIX**: Captured pawn is on the *start* rank and *end* file
                    nextBoard[from.Value.x, to.Value.y] = null;
                }
                // 2. Move piece on test board
                nextBoard[to.Value.x, to.Value.y] = movingPiece;
                nextBoard[from.Value.x, from.Value.y] = null;

                if (IsKingInCheck(nextBoard, whiteTurn))
                {
                    Console.WriteLine("Move leaves King in check. Press Enter.");
                    Console.ReadLine();
                    continue;
                }
                // ----------------------------------------------------------------

                // --- EXECUTE CASTLING MOVE (on the actual board) ---
                if (selected is King && Math.Abs(to.Value.y - from.Value.y) == 2)
                {
                    int rank = from.Value.x;
                    int rookY = (to.Value.y == 6) ? 7 : 0;
                    int newRookY = (to.Value.y == 6) ? 5 : 3;

                    Piece rook = board[rank, rookY];

                    board[rank, newRookY] = rook;
                    board[rank, rookY] = null;
                    if (rook != null) rook.HasMoved = true;
                }
                // -----------------------------

                // Set HasMoved flag on the piece on the actual board
                selected.HasMoved = true;

                // En passant capture (pawn removal on the actual board)
                if (selected is Pawn && enPassantTarget.HasValue && (to.Value.x, to.Value.y) == enPassantTarget.Value)
                {
                    // **LOGIC FIX**: Captured pawn is on the *start* rank and *end* file
                    board[from.Value.x, to.Value.y] = null; // Remove the captured pawn
                }

                // Set/Clear enPassantTarget for the next turn
                if (selected is Pawn && Math.Abs(to.Value.x - from.Value.x) == 2)
                    enPassantTarget = (from.Value.x + (to.Value.x - from.Value.x) / 2, from.Value.y);
                else
                    enPassantTarget = null;

                // Move piece (on the actual board)
                board[to.Value.x, to.Value.y] = selected;
                board[from.Value.x, from.Value.y] = null;

                // Pawn promotion
                if (selected is Pawn && (to.Value.x == 7 || to.Value.x == 0))
                {
                    Console.Write("Promote pawn to (Q/R/B/N): ");
                    char choice = char.ToUpper(Console.ReadKey().KeyChar);
                    Console.WriteLine();
                    Piece newPiece;
                    switch (choice)
                    {
                        case 'R': newPiece = new Rook(selected.IsWhite); break;
                        case 'B': newPiece = new Bishop(selected.IsWhite); break;
                        case 'N': newPiece = new Knight(selected.IsWhite); break;
                        case 'Q':
                        default: newPiece = new Queen(selected.IsWhite); break;
                    }
                    newPiece.HasMoved = true;
                    board[to.Value.x, to.Value.y] = newPiece;
                }

                whiteTurn = !whiteTurn;
            }
        }

        // --- HELPER METHODS ---

        static Piece[,] CopyBoard(Piece[,] board)
        {
            Piece[,] newBoard = new Piece[8, 8];
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    newBoard[i, j] = board[i, j];
            return newBoard;
        }

        // Checks if a given square is attacked by the specified color
        public static bool IsSquareAttacked(int x, int y, Piece[,] board, bool byWhite)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Piece piece = board[i, j];
                    if (piece != null && piece.IsWhite == byWhite)
                    {
                        // Call GetValidMoves with isAttackingCheck: true
                        List<(int, int)> attackingMoves = piece.GetValidMoves(i, j, board, null, isAttackingCheck: true);

                        if (attackingMoves.Contains((x, y)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        // Checks if the King of the specified color is currently in check
        public static bool IsKingInCheck(Piece[,] board, bool isWhiteKing)
        {
            int kingX = -1, kingY = -1;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] is King king && king.IsWhite == isWhiteKing)
                    {
                        kingX = i;
                        kingY = j;
                        break;
                    }
                }
            }

            if (kingX == -1) return false;

            return IsSquareAttacked(kingX, kingY, board, !isWhiteKing);
        }

        // **METHOD WITH ALL FIXES**
        // Checks if the specified color has any legal move
        static bool HasLegalMoves(Piece[,] board, bool isWhiteTurn, (int x, int y)? enPassantTarget)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Piece piece = board[i, j];
                    if (piece != null && piece.IsWhite == isWhiteTurn)
                    {
                        // Get moves for the player
                        List<(int, int)> potentialMoves = piece.GetValidMoves(i, j, board, enPassantTarget);

                        foreach (var move in potentialMoves)
                        {
                            Piece[,] testBoard = CopyBoard(board);
                            Piece movingPiece = testBoard[i, j];

                            // **COMPILER FIX + LOGIC FIX HERE**
                            if (movingPiece is Pawn && enPassantTarget.HasValue && move == enPassantTarget.Value)
                            {
                                // The captured pawn is on the *starting rank* (i) and the *destination file* (move.Item2)
                                testBoard[i, move.Item2] = null;
                            }

                            // **COMPILER FIX HERE**: Use .Item1 and .Item2
                            testBoard[move.Item1, move.Item2] = movingPiece;
                            testBoard[i, j] = null;

                            if (!IsKingInCheck(testBoard, isWhiteTurn))
                            {
                                return true; // Found a legal move!
                            }
                        }
                    }
                }
            }
            return false;
        }


        // Board Initialization (remains the same)
        static Piece[,] InitializeBoard()
        {
            Piece[,] board = new Piece[8, 8];
            board[0, 0] = new Rook(true); board[0, 1] = new Knight(true); board[0, 2] = new Bishop(true);
            board[0, 3] = new Queen(true); board[0, 4] = new King(true); board[0, 5] = new Bishop(true);
            board[0, 6] = new Knight(true); board[0, 7] = new Rook(true);
            for (int i = 0; i < 8; i++) board[1, i] = new Pawn(true);

            board[7, 0] = new Rook(false); board[7, 1] = new Knight(false); board[7, 2] = new Bishop(false);
            board[7, 3] = new Queen(false); board[7, 4] = new King(false); board[7, 5] = new Bishop(false);
            board[7, 6] = new Knight(false); board[7, 7] = new Rook(false);
            for (int i = 0; i < 8; i++) board[6, i] = new Pawn(false);

            return board;
        }

        // Display Board (remains the same)
        static void DisplayBoard(Piece[,] board)
        {
            Console.WriteLine("  a   b   c   d   e   f   g   h");
            for (int i = 7; i >= 0; i--)
            {
                Console.Write((i + 1) + " ");
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] == null)
                        Console.Write(".   ");
                    else
                        Console.Write(board[i, j].Symbol.PadRight(4));
                }
                Console.WriteLine();
            }
        }

        // Coordinate Conversion (remains the same)
        static (int x, int y)? ConvertCoordinates(string coord)
        {
            if (coord.Length != 2) return null;
            int y = coord[0] - 'a';
            int x = coord[1] - '1';
            if (x < 0 || x > 7 || y < 0 || y > 7) return null;
            return (x, y);
        }
    }
}