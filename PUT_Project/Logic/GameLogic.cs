using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PUT_Project.Logic
{
    class GameLogic
    {
        private int _numberOfSteps;
        private FIELDSTATE[,] _gameField = new FIELDSTATE[3, 3];

        public delegate void RenderDistantMoveEventHandler(int fieldnum);
        //public event RenderDistantMoveEventHandler OnExecuteDistantMove;

        public FIELDSTATE[,] GameFieldState
        {
            get
            {
                return _gameField;
            }
        }

        public void ExecuteDistantMove(string num)
        {
            int fieldNum = Convert.ToInt32(num);
            _gameField[fieldNum / 3, fieldNum % 3] = FIELDSTATE.PLAYER_DISTANT;
        }

        public bool RequestMove(int gamefieldRow, int gamefieldColumn)
        {
            if (GameManager.Instance.GameState == GAMESTATE.LOCAL_PLAYER_MOVE)
            {
                if (_gameField[gamefieldRow,gamefieldColumn] == FIELDSTATE.EMPTY)
                {
                    _gameField[gamefieldRow, gamefieldColumn] = FIELDSTATE.PLAYER_LOCAL;

                    if (((gamefieldRow * 3 + gamefieldColumn + 1) % 2) == 0)
                    {
                        if (CheckGamefield(false))
                        {
                            GameManager.Instance.EndGame();
                            return true;
                        }
                    }
                    else
                    {
                        if (CheckGamefield(true))
                        {
                            GameManager.Instance.EndGame();
                            return true;
                        }
                    }

                    GameManager.Instance.GameState = GAMESTATE.DISTANT_PLAYER_MOVE;

                    Packet p = new Packet();
                    p.DataID = DATAIDENTIFIER.GAME_MOVE;
                    p.GameState = GAMESTATE.LOCAL_PLAYER_MOVE;
                    p.Message = (gamefieldRow * 3 + gamefieldColumn).ToString();
                    GameManager.Instance.Send(p);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        bool CheckGamefield(bool checkDiagonaly)
        {
            if (!checkDiagonaly)
            {
                for(int i = 0; i < 3; i++)
                {
                    if ((_gameField[i, 0] == FIELDSTATE.PLAYER_LOCAL) && (_gameField[i, 1] == FIELDSTATE.PLAYER_LOCAL) && (_gameField[i, 2] == FIELDSTATE.PLAYER_LOCAL))
                    {
                        return true;
                    }
                    if ((_gameField[0, i] == FIELDSTATE.PLAYER_LOCAL) && (_gameField[1, i] == FIELDSTATE.PLAYER_LOCAL) && (_gameField[2, i] == FIELDSTATE.PLAYER_LOCAL))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                if(((_gameField[0, 0] == FIELDSTATE.PLAYER_LOCAL) && (_gameField[1, 1] == FIELDSTATE.PLAYER_LOCAL) && (_gameField[2, 2] == FIELDSTATE.PLAYER_LOCAL)) ||
                    (_gameField[0, 2] == FIELDSTATE.PLAYER_LOCAL) && (_gameField[1, 1] == FIELDSTATE.PLAYER_LOCAL) && (_gameField[2, 0] == FIELDSTATE.PLAYER_LOCAL))
                {
                    return true;
                }
                else
                {
                    return CheckGamefield(false);
                }
            }
        }
    }

    public enum FIELDSTATE
    {
        EMPTY,
        PLAYER_LOCAL,
        PLAYER_DISTANT
    }
}
