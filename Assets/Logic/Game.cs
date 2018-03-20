public enum GameType { TwoPlayer, SinglePlayer, SinglePlayerMoveSecond }
public enum PlayerType { Human, AI }
public enum GameState { InProgress, Finished }
public enum EndState { Victory, Tie }

public delegate void MoveCallback(uint column, uint row, uint player);
public delegate void HumanMoveCallback(uint column, uint row, uint player);
public delegate void AiMoveCallback(uint column, uint row, uint player);
public delegate void GameVictoryCallback(uint winningPlayer);
public delegate void GameTieCallback();
public delegate void GameFinishedCallback(EndState endState, uint? winningPlayer);
public delegate void HumanTurnStartCallback(uint player);

public struct Callbacks {
    public MoveCallback moveCallback;
    public HumanMoveCallback humanMoveCallback;
    public AiMoveCallback aiMoveCallback;
    public GameVictoryCallback gameVictoryCallback;
    public GameTieCallback gameTieCallback;
    public GameFinishedCallback gameFinishedCallback;
    public HumanTurnStartCallback humanTurnStartCallback;
}

public class AiLogic {
    virtual public uint GetMove(uint?[,] board, uint playerNumber, int playerCount, uint winLength) {
        throw new System.Exception("Tried to use base AiLogic class!");
    }
}

public class Game {
    PlayerType[] players;
    GameState gameState;
    AiLogic ai;
    uint length;
    uint height;
    uint?[,] board;
    uint currentPlayer;
    uint winLength;
    Callbacks callbacks;

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
        if(callbacks.gameFinishedCallback != null) {
            callbacks.gameFinishedCallback(EndState.Victory, currentPlayer);
        }
        if(callbacks.gameVictoryCallback != null) {
            callbacks.gameVictoryCallback(currentPlayer);
        }
    }

    void ProcessTie() {
        gameState = GameState.Finished;
        if(callbacks.gameFinishedCallback != null) {
            callbacks.gameFinishedCallback(EndState.Tie, null);
        }
        if(callbacks.gameTieCallback != null) {
            callbacks.gameTieCallback();
        }
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
            if(board[i, height - 1] == null) {
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
        if(currentPlayer == players.Length - 1) {
            currentPlayer = 0;
        } else {
            currentPlayer++;
        }
        while(players[currentPlayer] != PlayerType.Human) {
            uint aiColumn = ai.GetMove(board, currentPlayer, players.Length, winLength);
            uint aiRow = DropPeg(aiColumn);
            if(callbacks.aiMoveCallback != null) {
                callbacks.aiMoveCallback(aiColumn, aiRow, currentPlayer);
            }
            if(callbacks.moveCallback != null) {
                callbacks.moveCallback(aiColumn, aiRow, currentPlayer);
            }
            if(CheckWin(aiColumn, aiRow, currentPlayer)) {
                ProcessWin(currentPlayer);
                return;
            } else if(CheckTie()) {
                ProcessTie();
                return;
            } else {
                if(currentPlayer == players.Length - 1) {
                    currentPlayer = 0;
                } else {
                    currentPlayer++;
                }
            }
        }
        if(callbacks.humanTurnStartCallback != null) {
            callbacks.humanTurnStartCallback(currentPlayer);
        }
    }

    void Setup(AiLogic ai, Callbacks callbacks, uint length, uint height, uint winLength, PlayerType[] players) {
        if(winLength > length && winLength > height) {
            throw new System.Exception("Win length is greater than board length and height!");
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
        this.callbacks = callbacks;
        if(callbacks.humanTurnStartCallback != null) {
            callbacks.humanTurnStartCallback(currentPlayer);
        }
    }

    public Game(AiLogic ai, uint length, uint height, uint winLength, PlayerType[] players) {
        Setup(ai, new Callbacks(), length, height, winLength, players);
    }

    public Game(AiLogic ai, uint length, uint height, uint winLength, GameType gameType) {
        if(gameType == GameType.TwoPlayer) {
            Setup(ai, new Callbacks(), length, height, winLength, new PlayerType[] { PlayerType.Human, PlayerType.Human });
        } else if(gameType == GameType.SinglePlayer) {
            Setup(ai, new Callbacks(), length, height, winLength, new PlayerType[] { PlayerType.Human, PlayerType.AI });
        } else if(gameType == GameType.SinglePlayerMoveSecond) {
            Setup(ai, new Callbacks(), length, height, winLength, new PlayerType[] { PlayerType.AI, PlayerType.Human });
        }
        throw new System.Exception("Fell threw all gameType checks (This should be impossible)!");
    }

    public Game(AiLogic ai, GameType gameType) {
        if(gameType == GameType.TwoPlayer) {
            Setup(ai, new Callbacks(), 7, 6, 4, new PlayerType[] { PlayerType.Human, PlayerType.Human });
        } else if(gameType == GameType.SinglePlayer) {
            Setup(ai, new Callbacks(), 7, 6, 4, new PlayerType[] { PlayerType.Human, PlayerType.AI });
        } else if(gameType == GameType.SinglePlayerMoveSecond) {
            Setup(ai, new Callbacks(), 7, 6, 4, new PlayerType[] { PlayerType.AI, PlayerType.Human });
        }
        throw new System.Exception("Fell threw all gameType checks (This should be impossible)!");
    }

    public Game(AiLogic ai, Callbacks callbacks, uint length, uint height, uint winLength, PlayerType[] players) {
        Setup(ai, callbacks, length, height, winLength, players);
    }

    public Game(AiLogic ai, Callbacks callbacks, uint length, uint height, uint winLength, GameType gameType) {
        if(gameType == GameType.TwoPlayer) {
            Setup(ai, callbacks, length, height, winLength, new PlayerType[] { PlayerType.Human, PlayerType.Human });
        } else if(gameType == GameType.SinglePlayer) {
            Setup(ai, callbacks, length, height, winLength, new PlayerType[] { PlayerType.Human, PlayerType.AI });
        } else if(gameType == GameType.SinglePlayerMoveSecond) {
            Setup(ai, callbacks, length, height, winLength, new PlayerType[] { PlayerType.AI, PlayerType.Human });
        }
        throw new System.Exception("Fell threw all gameType checks (This should be impossible)!");
    }

    public Game(AiLogic ai, Callbacks callbacks, GameType gameType) {
        if(gameType == GameType.TwoPlayer) {
            Setup(ai, callbacks, 7, 6, 4, new PlayerType[] { PlayerType.Human, PlayerType.Human });
        } else if(gameType == GameType.SinglePlayer) {
            Setup(ai, callbacks, 7, 6, 4, new PlayerType[] { PlayerType.Human, PlayerType.AI });
        } else if(gameType == GameType.SinglePlayerMoveSecond) {
            Setup(ai, callbacks, 7, 6, 4, new PlayerType[] { PlayerType.AI, PlayerType.Human });
        }
        throw new System.Exception("Fell threw all gameType checks (This should be impossible)!");
    }

    public void SetCallbacks(Callbacks callbacks) {
        this.callbacks = callbacks;
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
        if(callbacks.humanMoveCallback != null) {
            callbacks.humanMoveCallback(column, newPegRow, currentPlayer);
        }
        if(callbacks.moveCallback != null) {
            callbacks.moveCallback(column, newPegRow, currentPlayer);
        }
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
