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
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace FileMoose
{
    public partial class MainWindow : Window
    {
        private TcpListener listener;
        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        public void Start()
        {
            listener = new TcpListener(IPAddress.Any, 21);
            listener.Start();
            listener.BeginAcceptTcpClient(HandleAcceptTcpClient, listener);
        }

        public void Stop()
        {

        }

        private void HandleAcceptTcpClient(IAsyncResult result)
        {
            TcpClient client = listener.EndAcceptTcpClient(result);
            listener.BeginAcceptTcpClient(HandleAcceptTcpClient, listener);

            Connection connection = new Connection(client);

            ThreadPool.QueueUserWorkItem(connection.HandleClient, client);
        }

    }
}
