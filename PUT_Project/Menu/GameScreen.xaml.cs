using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PUT_Project.Logic;
using System.Windows.Threading;

namespace PUT_Project.Menu
{
    /// <summary>
    /// Interaction logic for GameScreen.xaml
    /// </summary>
    public partial class GameScreen : UserControl, ISwitchable
    {
        private Paragraph paragraph;

        public string GamestateText { get { return "Game State:/n" + GameManager.Instance.GameState; } }

        public GameScreen()
        {
            InitializeComponent();
            GameManager.Instance.GameStartEvent += OnGameStart;
            GameManager.Instance.GameOverEvent += OnGameEnd;
            GameManager.Instance.GameStateChanged += UpdateStatusBox;
           // GameManager.Instance.GameLogic.OnExecuteDistantMove += RenderDistantInput;
            EndgameLabel.Visibility = Visibility.Hidden;

            paragraph = new Paragraph();
            RichTextBoxChat.Document = new FlowDocument(paragraph);
        }

        #region ISwitchable Members

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }
        #endregion

        void OnGameStart()
        {
            

            if (GameManager.Instance.NetworkRole == NETWORKROLE.SERVER)
            {
                if (GameManager.Instance.GameServer != null)
                {
                    GameManager.Instance.GameServer.DataArrivedEvent += MessageArrived;
                    PlayerIPLabel.Content = GameManager.Instance.GameServer.IpAddresIP4;
                }

                this.paragraph.Inlines.Add(new Bold(new Run("SERVER MSG: Server started, IP: " + GameManager.Instance.GameServer.IpAddresIP4))
                {
                    Foreground = Brushes.Yellow,
                    Background = Brushes.DarkGray,
                });
                //this.paragraph.Inlines.Add(p.Message);
                this.paragraph.Inlines.Add(new LineBreak());
            }

            if (GameManager.Instance.NetworkRole == NETWORKROLE.CLIENT)
            {
                if (GameManager.Instance.GameClient != null)
                {
                    GameManager.Instance.GameClient.DataArrivedEvent += MessageArrived;
                    PlayerIPLabel.Content = GameManager.Instance.GameClient.IpAddressV4;
                }
            }


        }

        void OnGameEnd()
        {
            EndgameLabel.Visibility = Visibility.Visible;
        }

        #region Input Handlers

        private void GameFieldButton1_Click(object sender, RoutedEventArgs e)
        {
            if (GameManager.Instance.GameLogic.RequestMove(0, 0))
            {
                GameFieldButton1.Content = GameManager.Instance.Symbol;
            }
        }

        private void GameFieldButton2_Click(object sender, RoutedEventArgs e)
        {
            if (GameManager.Instance.GameLogic.RequestMove(0, 1))
            {
                GameFieldButton2.Content = GameManager.Instance.Symbol;
            }
        }

        private void GameFieldButton3_Click(object sender, RoutedEventArgs e)
        {
            if (GameManager.Instance.GameLogic.RequestMove(0, 2))
            {
                GameFieldButton3.Content = GameManager.Instance.Symbol;
            }
        }

        private void GameFieldButton4_Click(object sender, RoutedEventArgs e)
        {
            if (GameManager.Instance.GameLogic.RequestMove(1, 0))
            {
                GameFieldButton4.Content = GameManager.Instance.Symbol;
            }
        }

        private void GameFieldButton5_Click(object sender, RoutedEventArgs e)
        {
            if (GameManager.Instance.GameLogic.RequestMove(1, 1))
            {
                GameFieldButton5.Content = GameManager.Instance.Symbol;
            }
        }

        private void GameFieldButton6_Click(object sender, RoutedEventArgs e)
        {
            if (GameManager.Instance.GameLogic.RequestMove(1, 2))
            {
                GameFieldButton6.Content = GameManager.Instance.Symbol;
            }
        }

        private void GameFieldButton7_Click(object sender, RoutedEventArgs e)
        {
            if (GameManager.Instance.GameLogic.RequestMove(2, 0))
            {
                GameFieldButton7.Content = GameManager.Instance.Symbol;
            }
        }

        private void GameFieldButton8_Click(object sender, RoutedEventArgs e)
        {
            if (GameManager.Instance.GameLogic.RequestMove(2, 1))
            {
                GameFieldButton8.Content = GameManager.Instance.Symbol;
            }
        }

        private void GameFieldButton9_Click(object sender, RoutedEventArgs e)
        {
            if (GameManager.Instance.GameLogic.RequestMove(2, 2))
            {
                GameFieldButton9.Content = GameManager.Instance.Symbol;
            }
        }

        private void TextBoxEnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

                this.paragraph.Inlines.Add(new Bold(new Run(GameManager.Instance.Name + ": "))
                {
                    Foreground = Brushes.DeepSkyBlue,
                    Background = Brushes.LightGray
                });
                this.paragraph.Inlines.Add(EnterTextChatTextbox.Text);
                this.paragraph.Inlines.Add(new LineBreak());

                RichTextBoxChat.ScrollToEnd();

                if (GameManager.Instance.NetworkRole == NETWORKROLE.SERVER)
                {
                    var packet = new Packet();
                    packet.DataID = DATAIDENTIFIER.CHATMSG;
                    packet.Name = GameManager.Instance.Name;
                    packet.Message = EnterTextChatTextbox.Text;
                    GameManager.Instance.GameServer.Send(packet);
                }
                else if (GameManager.Instance.NetworkRole == NETWORKROLE.CLIENT)
                {
                    var packet = new Packet();
                    packet.DataID = DATAIDENTIFIER.CHATMSG;
                    packet.Name = GameManager.Instance.Name;
                    packet.Message = EnterTextChatTextbox.Text;
                    GameManager.Instance.GameClient.Send(packet);
                }

                EnterTextChatTextbox.Text = "";
            }
        }

        private void EnterTextTexboxOnFocus(object sender, RoutedEventArgs e)
        {
            EnterTextChatTextbox.Text = "";
        }

        private void EnterTextTexboxOnFocusLost(object sender, RoutedEventArgs e)
        {
            if (EnterTextChatTextbox.Text == "")
            {
                EnterTextChatTextbox.Text = "Enter Message";
            }
        }

        public void UpdateStatusBox()
        {
            GameStateLabel.Content = "Game State: " + GameManager.Instance.GameState;
        }

        public void RenderDistantInput(int fieldNum)
        {
            switch (fieldNum)
            {
                case 0:
                    GameFieldButton1.Content = GameManager.Instance.DistantSymbol;
                    break;
                case 1:
                    GameFieldButton2.Content = GameManager.Instance.DistantSymbol;
                    break;
                case 2:
                    GameFieldButton3.Content = GameManager.Instance.DistantSymbol;
                    break;
                case 3:
                    GameFieldButton4.Content = GameManager.Instance.DistantSymbol;
                    break;
                case 4:
                    GameFieldButton5.Content = GameManager.Instance.DistantSymbol;
                    break;
                case 5:
                    GameFieldButton6.Content = GameManager.Instance.DistantSymbol;
                    break;
                case 6:
                    GameFieldButton7.Content = GameManager.Instance.DistantSymbol;
                    break;
                case 7:
                    GameFieldButton8.Content = GameManager.Instance.DistantSymbol;
                    break;
                case 8:
                    GameFieldButton9.Content = GameManager.Instance.DistantSymbol;
                    break;
                default:
                    break;
            }
        }
        #endregion


        #region Message handling

        private void InformAboutMove(int fieldNum)
        {
            string stringFieldnum = fieldNum.ToString();

            Packet p = new Packet();
            p.DataID = DATAIDENTIFIER.GAME_MOVE;
            p.GameState = GAMESTATE.LOCAL_PLAYER_MOVE;
            p.Message = stringFieldnum;

            GameManager.Instance.Send(p);
        }

        private void MessageArrived(Packet p)
        {
            App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                this.HandleNewMessage(p);
            }));
        }

        private void HandleNewMessage(Packet p)
        {
            if (p.DataID == DATAIDENTIFIER.INITIAL_CONNECTION_MSG)
            {
                if (GameManager.Instance.GameState == GAMESTATE.SYNC_WAIT)
                {
                    OponentLabel.Content = "Oponent: " + p.Name;
                    OponentIPLabel.Content = p.Message;

                    GameManager.Instance.GameState = GAMESTATE.GAME_START;

                    if (GameManager.Instance.NetworkRole == NETWORKROLE.SERVER)
                    {
                        this.paragraph.Inlines.Add(new Bold(new Run("USER JOINED: " + p.Name))
                        {
                            Foreground = Brushes.Black,
                            Background = Brushes.DarkGray,
                        });
                        this.paragraph.Inlines.Add(new LineBreak());

                        GameManager.Instance.StartGamePlay();
                    }
                    else 
                    {
                        this.paragraph.Inlines.Add(new Bold(new Run("Joined to: " + p.Name))
                        {
                            Foreground = Brushes.Black,
                            Background = Brushes.DarkGray,
                        });
                        this.paragraph.Inlines.Add(new LineBreak());
                    }
                }
            }

            if (p.DataID == DATAIDENTIFIER.STARTING_SETUP)
            {
                GameManager.Instance.GameState = p.GameState;

                if (p.GameState == GAMESTATE.LOCAL_PLAYER_MOVE)
                {
                    GameManager.Instance.Symbol = 'X';
                    GameManager.Instance.DistantSymbol = 'O';
                }
                else
                {
                    GameManager.Instance.Symbol = 'O';
                    GameManager.Instance.DistantSymbol = 'X';
                }
            }

            if (p.DataID == DATAIDENTIFIER.GAME_MOVE)
            {
                RenderDistantInput(Convert.ToInt32(p.Message));
                GameManager.Instance.GameLogic.ExecuteDistantMove(p.Message);
                GameManager.Instance.GameState = p.GameState;
            }

            if (p.DataID == DATAIDENTIFIER.CHATMSG)
            {
                WriteToChat(p);
            }
        }

        private void WriteToChat(Packet p)
        {
            this.paragraph.Inlines.Add(new Bold(new Run(p.Name + ": "))
            {
                Foreground = Brushes.DarkRed,
                Background = Brushes.LightGray
            });
            this.paragraph.Inlines.Add(p.Message);
            this.paragraph.Inlines.Add(new LineBreak());

            this.RichTextBoxChat.ScrollToEnd();
        }

        #endregion
    }
}
