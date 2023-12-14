using ContactManagerPro.Models;
using NetworkProgramming.Models;
using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace ContactManagerPro
{
    public partial class EmailForm : Window
    {
        private NetworkConfig? networkConfig;


        public EmailForm()
        {
            InitializeComponent();
            FillContactComboBox();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Tag is NetworkConfig config)
            {
                networkConfig = config;
            }
            else
            {
                MessageBox.Show("Configuration error");
                Close();
            }
        }
        private void FillContactComboBox()
        {
            using (var context = new ContactDbContext())
            {
                // Отримуємо список контактів з бази даних
                List<Contact> contacts = context.Contacts.ToList();

                // Встановлюємо комбобокс як джерело даних список контактів
                contactComboBox.ItemsSource = contacts;

                // Встановлюємо, яке поле обрати для відображення у комбо боксі (в данному випадку - Name)
                contactComboBox.DisplayMemberPath = "Name";
            }
        }

        private void SendMessageButtonClick(object sender, RoutedEventArgs e)
        {
            if (networkConfig is null) return;

            try
            {
                Socket clientSocket = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);// з'єднань за допомогою протоколу TCP/IP.

                clientSocket.Connect(
                    networkConfig.EndPoint);

                Models.ClientRequest request = new()
                {
                    Command = "CREATE",
                    Data = $"{((Contact)contactComboBox.SelectedItem)?.Name}: {messageTextBox.Text}"
                };

                clientSocket.Send(
                    networkConfig.Encoding.GetBytes(
                        JsonSerializer.Serialize(request)
                ));

                byte[] buffer = new byte[2048];
                String str = "";
                int n;
                do
                {
                    n = clientSocket.Receive(buffer);
                    str += networkConfig.Encoding.GetString(buffer, 0, n);
                } while (clientSocket.Available > 0);
                Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{str}\n";

                    messageListBox.Items.Add(newMessage);
                });

                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => 
                {
                    var newMessage = ex.Message + "\noffline\n";
                    messageListBox.Items.Add(newMessage);
                });
            }
        }


    }
}


    
