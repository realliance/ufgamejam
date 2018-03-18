public enum GameType { TwoPlayer, SinglePlayer, SinglePlayerMoveSecond }
public enum PlayerType { Human, AI }
public enum GameState { InProgress, Finished }

public class AILogic {
    virtual public uint GetMove(uint?[,] board, uint playerNumber) {
        throw new System.Exception("Tried to use base AILogic class!");
    }
}

public class Game {
    PlayerType[] players;
    GameState gameState;
    AILogic ai;
    uint length;
    uint height;
    uint?[,] board;
    uint currentPlayer;
    uint winLength;

    uint DropPeg(uint column) {
        for(uint i = 0; i < height; i++) {
            if(board[column, i] == null) {
                board[column, i] = currentPlayer;
                return i;
            }
        }
        throw new System.Exception("Tried to drop into a full column!");
    }

    void ProcessWin(uint player) {
        gameState = GameState.Finished;
    }

    void ProcessTie() {
        gameState = GameState.Finished;
    }

    bool CheckWin(uint column, uint row, uint player) {
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

    bool CheckTie() {
        for(uint i = 0; i < length; i++) {
            if(board[i, height - 1] != null) {
                return false;
            }
        }
        return true;
    }

    void ProcessGame(uint column, uint row) {
        if(CheckWin(column, row, currentPlayer)) {
            ProcessWin(currentPlayer);
            return;
        } else if(CheckTie()) {
            ProcessTie();
            return;
        }
        currentPlayer++;
        while(players[currentPlayer] != PlayerType.Human) {
            uint aiColumn = ai.GetMove(board, currentPlayer);
            uint aiRow = DropPeg(aiColumn);
            if(CheckWin(aiColumn, aiRow, currentPlayer)) {
                ProcessWin(currentPlayer);
                return;
            } else if(CheckTie()) {
                ProcessTie();
                return;
            } else {
                currentPlayer++;
            }
        }
    }

    void Setup(AILogic ai, uint length, uint height, uint winLength, PlayerType[] players) {
        if(winLength > length) {
            throw new System.Exception("Win length is greater than board length!");
        } else if(winLength > height) {
            throw new System.Exception("Win length is greater than board height!");
        } else if(winLength < 2) {
            throw new System.Exception("Win length is less than two!");
        } else if(players.Length < 2) {
            throw new System.Exception("There are less than two players!");
        } else if(ai == null) {
            throw new System.Exception("AI is null!");
        } else if(players == null) {
            throw new System.Exception("Players is null!");
        }
        this.ai = ai;
        this.length = length;
        this.height = height;
        this.winLength = winLength;
        this.players = players;
        currentPlayer = 0;
        board = new uint?[length, height];
        gameState = GameState.InProgress;
    }

    public Game(AILogic ai, uint length, uint height, uint winLength, PlayerType[] players) {
        Setup(ai, length, height, winLength, players);
    }

    public Game(AILogic ai, uint length, uint height, uint winLength, GameType gameType) {
        if(gameType == GameType.TwoPlayer) {
            Setup(ai, length, height, winLength, new PlayerType[] { PlayerType.Human, PlayerType.Human });
        } else if(gameType == GameType.SinglePlayer) {
            Setup(ai, length, height, winLength, new PlayerType[] { PlayerType.Human, PlayerType.AI });
        } else if(gameType == GameType.SinglePlayerMoveSecond) {
            Setup(ai, length, height, winLength, new PlayerType[] { PlayerType.AI, PlayerType.Human });
        }
        throw new System.Exception("Fell threw all gameType checks (This should be impossible)!");
    }

    public Game(AILogic ai, GameType gameType) {
        if(gameType == GameType.TwoPlayer) {
            Setup(ai, 7, 6, 4, new PlayerType[] { PlayerType.Human, PlayerType.Human });
        } else if(gameType == GameType.SinglePlayer) {
            Setup(ai, 7, 6, 4, new PlayerType[] { PlayerType.Human, PlayerType.AI });
        } else if(gameType == GameType.SinglePlayerMoveSecond) {
            Setup(ai, 7, 6, 4, new PlayerType[] { PlayerType.AI, PlayerType.Human });
        }
        throw new System.Exception("Fell threw all gameType checks (This should be impossible)!");
    }

    public bool Move(uint column) {
        if(column >= length) {
            return false;
        } else if(board[column, height - 1] != null) {
            return false;
        } else if(gameState != GameState.InProgress) {
            return false;
        }

        uint newPegRow = DropPeg(column);
        ProcessGame(column, newPegRow);
        return true;
    }

    public GameState GetGameState() {
        return gameState;
    }

    public uint GetCurrentPlayer() {
        return currentPlayer;
    }

    public uint?[,] GetBoard() {
        return board;
    }
}
