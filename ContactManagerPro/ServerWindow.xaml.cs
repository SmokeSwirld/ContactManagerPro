using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Windows;
using ContactManagerPro.Models; 
using NetworkProgramming.Models;

namespace ContactManagerPro
{
    public partial class ServerWindow : Window
    {
        private NetworkConfig? networkConfig;
        private Socket? listenSocket;
        private Thread? listenThread;
        
        public ServerWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Tag is NetworkConfig config)
            {
                networkConfig = config;
                listenSocket = new Socket(
                    System.Net.Sockets.AddressFamily.InterNetwork,
                    System.Net.Sockets.SocketType.Stream,
                    System.Net.Sockets.ProtocolType.Tcp
                );
                listenThread = new Thread(StartServer);
                listenThread.Start();
            }
            else
            {
                MessageBox.Show("Configuration error");
                Close();
            }
        }

        private void StartServer()
        {
            Models.ServerResponse request1 = new()
            {
                Status = "online",
                Data1 = "no message"
            };

            if (listenSocket is null || networkConfig is null) return;

            try
            {
                listenSocket.Bind(networkConfig.EndPoint);
                listenSocket.Listen(10);

                Dispatcher.Invoke(() => { Log.Text += "Сервер запущений\n"; });

                byte[] buffer = new byte[2048];
                String str;
                int n;
                while (true)
                {
                    Socket requestSocket = listenSocket.Accept();
                    str = "";
                    do
                    {
                        n = requestSocket.Receive(buffer);
                        str += networkConfig.Encoding?.GetString(buffer, 0, n);

                    } while (requestSocket.Available > 0);

                    var request = JsonSerializer.Deserialize<ClientRequest>(str);
                    String response;
                    switch (request?.Command)
                    {
                        case "CREATE":
                            response = "Нове повідомлення: " + request.Data;
                            break;
                        default:
                            response = "Команда не розпізнана";
                            break;
                    }

                    buffer = networkConfig.Encoding.GetBytes(response);
                    requestSocket.Send(buffer);
                    requestSocket.Send(networkConfig.Encoding.GetBytes(JsonSerializer
                        .Serialize(request1)));
                    requestSocket.Shutdown(SocketShutdown.Both);
                    requestSocket.Close();
                    Dispatcher.Invoke(() => { Log.Text += response + "\n"; });
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => { Log.Text += ex.Message + "\nСервер зупинено\n"; });
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (listenThread != null)
            {
                listenSocket?.Close();
            }
        }
    }
}

