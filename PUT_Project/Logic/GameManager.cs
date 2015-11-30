using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PUT_Project.Logic
{
    class GameManager
    {
        public delegate void GameEventHandler();
        public event GameEventHandler GameStartEvent;
        public event GameEventHandler GameOverEvent;
        public event GameEventHandler GameStateChanged;

        public GameServer GameServer;
        public GameClient GameClient;
        public GameLogic GameLogic;

        public string Name;
        public char Symbol;
        public char DistantSymbol;

        private static GameManager _instance;
        private GAMESTATE _gameState;

        public GAMESTATE GameState
        {
            get { return _gameState; }
            set 
            {
                _gameState = value;
                if (GameStateChanged != null)
                    GameStateChanged();
            }
        }
        public NETWORKROLE NetworkRole = NETWORKROLE.UNDEFINED;

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameManager();
                }
                return _instance;
            }
        }

        private GameManager() 
        {
            GameState = GAMESTATE.GAME_ON_LOGIN_SCREEN;
            GameLogic = new GameLogic();
        }

        public void StartGame()
        {
            if (GameStartEvent != null)
            {
                GameStartEvent();
            }
        }

        public void StartGamePlay()
        {
            Random rnd = new Random();

            if (rnd.Next(0, 2) > 0)
            {
                GameState = GAMESTATE.DISTANT_PLAYER_MOVE;
                Packet p = new Packet();
                p.DataID = DATAIDENTIFIER.STARTING_SETUP;
                p.GameState = GAMESTATE.LOCAL_PLAYER_MOVE;
                GameServer.Send(p);
                Symbol = 'O';
                DistantSymbol = 'X';
            }
            else
            {
                GameState =  GAMESTATE.LOCAL_PLAYER_MOVE;
                Packet p = new Packet();
                p.DataID = DATAIDENTIFIER.STARTING_SETUP;
                p.GameState = GAMESTATE.DISTANT_PLAYER_MOVE;
                GameServer.Send(p);
                Symbol = 'X';
                DistantSymbol = 'O';
            }
        }

        public void EndGame()
        {
            if (GameOverEvent != null)
            {
                GameState = GAMESTATE.GAME_OVER;
                if (NetworkRole == NETWORKROLE.SERVER)
                {
                    Random random = new Random();
                    if (random.Next(0, 2) > 0)
                    {
                        GameState = GAMESTATE.LOCAL_PLAYER_MOVE;
                    }
                    else
                    {
                        GameState = GAMESTATE.DISTANT_PLAYER_MOVE;
                    }
                }
                GameOverEvent();
            }
        }

        public void Send(Packet p)
        {
            if (NetworkRole == NETWORKROLE.SERVER) GameServer.Send(p);
            else GameClient.Send(p);
        }

    }

    public enum GAMESTATE
    {
        GAME_ON_LOGIN_SCREEN,
        GAME_START,
        GAME_OVER,
        SYNC_WAIT,
        LOCAL_PLAYER_MOVE,
        DISTANT_PLAYER_MOVE,
        UNDEFINED
    }

    public enum NETWORKROLE
    {
        UNDEFINED,
        SERVER,
        CLIENT
    }
}
