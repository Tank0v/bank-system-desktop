using System;
using System.Windows;

namespace BankWindow
{
    /// <summary>
    /// Interaction logic for TransferOnBankAccount.xaml
    /// </summary>
    public partial class TransferBetweenBankAccountWindow : Window
    {
        public Account.Account account;

        public int numberToTransfer { get; set; } // Номер банка, куда переводились деньги

        public TransferBetweenBankAccountWindow(Account.Account account, int numberOfBank)
        {
            InitializeComponent();

            this.account = account;
            foreach (Account.BankAccount account1 in account.GetABankAccountList()) // Добавляем в список открытые счета
            {
                if (account1.Number != numberOfBank)
                    accountBox.Items.Add(account1.ToString());
            }
        }

        private void Ok(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public bool Transfer(int numberOfBank) // Перевод между счетами
        {
            bool mark = false;

            try
            {
                object selectedItem = accountBox.SelectedItem;
                int number = int.Parse(value.Text);

                if (selectedItem != null)
                {
                    switch (selectedItem.ToString())
                    {
                        case "Сбербанк":
                            mark = account.Transfer(numberOfBank, 1, number);
                            numberToTransfer = 1;
                            break;
                        case "Альфа-банк":
                            mark = account.Transfer(numberOfBank, 2, number);
                            numberToTransfer = 2;
                            break;
                        case "Почта-банк":
                            mark = account.Transfer(numberOfBank, 3, number);
                            numberToTransfer = 3;
                            break;
                    } 
                }
                return mark;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
