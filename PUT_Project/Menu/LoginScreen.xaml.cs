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

namespace PUT_Project.Menu
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : UserControl, ISwitchable
    {
        public LoginScreen()
        {
            InitializeComponent();
        }

        private void OnStartGamePressed(object sender, RoutedEventArgs e)
        {
            GameManager.Instance.Name = NameTextBox.Text;
            GameManager.Instance.GameState = GAMESTATE.SYNC_WAIT;
            Switcher.Switch(new GameScreen());
            GameManager.Instance.GameClient = new GameClient();
            GameManager.Instance.NetworkRole = NETWORKROLE.CLIENT;
            GameManager.Instance.GameClient.StartClient(IPTextBox.Text, PortTextBox.Text);
            GameManager.Instance.StartGame();  
        }

        #region ISwitchable Related

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }
        #endregion

        private void OnStartServerClicked(object sender, RoutedEventArgs e)
        {
            GameManager.Instance.Name = NameTextBox.Text;
            GameManager.Instance.GameState = GAMESTATE.SYNC_WAIT;
            Switcher.Switch(new GameScreen());
            GameManager.Instance.GameServer = new GameServer();
            GameManager.Instance.NetworkRole = NETWORKROLE.SERVER;
            GameManager.Instance.GameServer.StartServer(PortTextBox.Text);
            GameManager.Instance.StartGame();
            
        }

        private void IpTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (IPTextBox.Text == "")
            {
                IPTextBox.Text = "Enter IP";
            }
        }

        private void PortTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (PortTextBox.Text == "")
            {
                PortTextBox.Text = "Enter Port";
            }
        }

        private void NickTextBoxLostFocus(object sender, RoutedEventArgs e)
        {

            if (NameTextBox.Text == "")
            {
                NameTextBox.Text = "Enter Name";
            }
        }

        private void NickTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if(NameTextBox.Text == "Enter Name")
                NameTextBox.Text = "";
        }

        private void IpTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (IPTextBox.Text == "Enter IP")
                IPTextBox.Text = "";
        }

        private void PortTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (PortTextBox.Text == "Enter Port")
                PortTextBox.Text = "";
        }
    }
}
