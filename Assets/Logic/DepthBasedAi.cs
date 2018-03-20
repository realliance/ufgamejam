using System.Collections.Generic;

struct BoardPosition {
    public uint column;
    public uint row;

    public BoardPosition(int column, int row) {
        this.column = (uint)column;
        this.row = (uint)row;
    }
}

struct Node {
    uint?[,] board;
    // 1000 -> AI win
    // -1000 -> Opponent win
    // 100(numbner of {winLength - 1} runs) -> All others (capped to abs600)
    int score;
}

public class DepthBasedAi : AiLogic {
    public override uint GetMove(uint?[,] board, uint playerNumber, int playerCount, uint winLength) {
        
    }

    bool CheckWin(uint column, uint row, uint player, uint winLength, uint?[,] board) {
        int length = board.GetLength(0);
        int height = board.GetLength(1);
        if(column + winLength <= length) {
            for(uint i = 1; i < winLength; i++) {
                if(board[column + i, row] != player) {
                    break;
                } else if(i == winLength - 1) {
                    return true;
                }
            }
        }
        if((length - column - 1) + winLength <= length) {
            for(uint i = 1; i < winLength; i++) {
                if(board[column - i, row] != player) {
                    break;
                } else if(i == winLength - 1) {
                    return true;
                }
            }
        }
        if(row + winLength <= height) {
            for(uint i = 1; i < winLength; i++) {
                if(board[column, row + i] != player) {
                    break;
                } else if(i == winLength - 1) {
                    return true;
                }
            }
        }
        if((height - row - 1) + winLength <= height) {
            for(uint i = 1; i < winLength; i++) {
                if(board[column, row - i] != player) {
                    break;
                } else if(i == winLength - 1) {
                    return true;
                }
            }
        }
        if(column + winLength <= length && row + winLength <= height) {
            for(uint i = 1; i < winLength; i++) {
                if(board[column + i, row + i] != player) {
                    break;
                } else if(i == winLength - 1) {
                    return true;
                }
            }
        }
        if((length - column - 1) + winLength <= length && row + winLength <= height) {
            for(uint i = 1; i < winLength; i++) {
                if(board[column - i, row + i] != player) {
                    break;
                } else if(i == winLength - 1) {
                    return true;
                }
            }
        }
        if(column + winLength <= length && (height - row - 1) + winLength <= height) {
            for(uint i = 1; i < winLength; i++) {
                if(board[column + i, row - i] != player) {
                    break;
                } else if(i == winLength - 1) {
                    return true;
                }
            }
        }
        if((length - column - 1) + winLength <= length && (height - row - 1) + winLength <= height) {
            for(uint i = 1; i < winLength; i++) {
                if(board[column - i, row - i] != player) {
                    break;
                } else if(i == winLength - 1) {
                    return true;
                }
            }
        }
        return false;
    }

    // Definitely not partially copy-pasted from unity answers
    static BoardPosition[] CoordinatesOf<T>(this T[,] matrix, T value) {
        int w = matrix.GetLength(0);
        int h = matrix.GetLength(1);
        List<BoardPosition> b = new List<BoardPosition>();

        for(int x = 0; x < w; ++x) {
            for(int y = 0; y < h; ++y) {
                if(matrix[x, y].Equals(value)) {
                    b.Add(new BoardPosition(x, y));
                }
            }
        }

        return b.ToArray();
    }

    int EvaluatePosition(uint?[,] board, uint playerNumber, uint winLength) {
        uint otherPlayer = (playerNumber == 1) ? (uint)0 : (uint)1;
        BoardPosition[] ours = CoordinatesOf<uint?>(board, playerNumber);
        BoardPosition[] others = CoordinatesOf<uint?>(board, otherPlayer);

        for(int i = 0; i < ours.Length; i++) {
            BoardPosition p = ours[i];
            if(CheckWin(p.column, p.row, playerNumber, winLength, board)) {
                return 1000;
            }
        }
        for(int i = 0; i < others.Length; i++) {
            BoardPosition p = others[i];
            if(CheckWin(p.column, p.row, otherPlayer, winLength, board)) {
                return -1000;
            }
        }

        int score = 0;
        BoardPosition[] spaces = CoordinatesOf<uint?>(board, null);

        for(int i = 0; i < spaces.Length; i++) {
            BoardPosition p = spaces[i];
            if(CheckWin(p.column, p.row, playerNumber, winLength, board)) {
                score += 100;
            }
            if(CheckWin(p.column, p.row, otherPlayer, winLength, board)) {
                score -= 100;
            }
        }

        if(score > 500) {
            score = 500;
        } else if(score < -500) {
            score = -500;
        }
        return score;
    }

    uint 
}
