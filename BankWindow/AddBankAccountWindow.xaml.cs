using System;
using System.IO;
using System.Windows;

namespace BankWindow
{
    /// <summary>
    /// Interaction logic for AddBankAccountWindow.xaml
    /// </summary>
    public partial class AddBankAccountWindow : Window
    {
        private readonly Account.Account account;

        public AddBankAccountWindow(Account.Account account)
        {
            InitializeComponent();

            this.account = account;

            if (account.GetANameOfNotOpenBankAccounts().Length != 0) // Добавление в список неоткрытых счетов
            {
                foreach (string s in account.GetANameOfNotOpenBankAccounts())
                {
                    accountBox.Items.Add(s);
                }
            }
        }

        private void Ok(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public string AddBankAccount() // Добавление счёта в банке
        {
            object selectedItem = accountBox.SelectedItem; // Выбранный в списке аккаунт
            if (selectedItem != null)
            {
                int id = account.Id;
                string path = $@"..\..\data\{id}\{id}.txt";

                accountBox.Items.Remove(selectedItem.ToString()); // Удаление выбранного счёта из списка
                switch (selectedItem.ToString())
                {
                    case "Сбербанк":
                        account.AddBankAccount(1, 0, DateTime.Now);
                        File.AppendAllText(path, $"s {0} {DateTime.Now}\n");
                        break;
                    case "Альфа-банк":
                        account.AddBankAccount(2, 0, DateTime.Now);
                        File.AppendAllText(path, $"a {0} {DateTime.Now}\n");
                        break;
                    case "Почта-банк":
                        account.AddBankAccount(3, 0, DateTime.Now);
                        File.AppendAllText(path, $"p {0} {DateTime.Now}\n");
                        break;
                }
                return selectedItem.ToString();
            }
            return null;
        }
    }
}
