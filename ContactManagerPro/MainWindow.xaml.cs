using ContactManagerPro.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NetworkProgramming.Models;


namespace ContactManagerPro
{
    public partial class MainWindow : Window
    {
        private readonly ContactManager contactManager;
        public MainWindow()
        {
            InitializeComponent();
            contactManager = new ContactManager();
            
            UpdateContactList();
        }
  
        

        private void SendEmailButtonClick(object sender, RoutedEventArgs e)
        {
            var emailForm = new EmailForm();
            emailForm.Tag = new NetworkConfig
            {
               Ip = "127.0.0.1",
                Port = 8080,
                Encoding = Encoding.UTF8    
            };
            emailForm.Show();
        }



        /* private void AddContactButtonClick(object sender, RoutedEventArgs e)
         {
             string name = nameTextBox.Text;
             string email = emailTextBox.Text;

             // Викликаємо метод додавання контакту AddContactAsync
             contactManager.AddContact(name, email);

             // Після додавання оновлюємо список контактів
             UpdateContactList();
         }*/
        private async void EditButtonClick(object sender, RoutedEventArgs e)
        {
            // Отримуємо вибраний контакт з ListBox
            Contact selectedContact = (Contact)contactListBox.SelectedItem;

            // Перевіряємо, чи вибраний контакт не є нульовим
            if (selectedContact != null)
            {
                // Отримуємо нові значення для редагування
                string newName = nameTextBox.Text;
                string newEmail = emailTextBox.Text;

                // Викликаємо асинхронний метод редагування контакту
                await contactManager.UpdateContactAsync(selectedContact.Id, newName, newEmail);

                // Після редагування оновлюємо список контактів
                UpdateContactList();
            }
        }


        private async void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            // Отримуємо вибраний контакт з ListBox
            Contact selectedContact = (Contact)contactListBox.SelectedItem;

            // Перевіряємо, чи вибраний контакт не є нульовим
            if (selectedContact != null)
            {
                // Викликаємо асинхронний метод видалення контакту
                await contactManager.DeleteContactAsync(selectedContact.Id);

                // Після видалення оновлюємо список контактів
                UpdateContactList();
            }
        }

        private async void AddContactButtonClick(object sender, RoutedEventArgs e)
        {
            string name = nameTextBox.Text;
            string email = emailTextBox.Text;

            // Викликаємо асинхронний метод додавання контакту AddContactAsync
            await contactManager.AddContactAsync(name, email);

            // Після додавання оновлюємо список контактів
            UpdateContactList();
        }
        private void UpdateContactList()
        {
            using (var context = new ContactDbContext())
            {
                
                List<Contact> contacts = context.Contacts.ToList();

                // Очищаємо ListBox
                contactListBox.Items.Clear();

                // Додаємо нові контакти в ListBox
                foreach (var contact in contacts)
                {
                    contactListBox.Items.Add(contact);
                }
            }
        }

        private void ServerButtonClick(object sender, RoutedEventArgs e)
        {
            var serverWindow = new ServerWindow();
            serverWindow.Tag = new NetworkConfig
            {
                Ip = "127.0.0.1",
                Port = 8080,
                Encoding = Encoding.UTF8    
            };
            serverWindow.Show();
        }
    }
}
