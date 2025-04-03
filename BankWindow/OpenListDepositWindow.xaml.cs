using System.Windows;

namespace BankWindow
{
    /// <summary>
    /// Interaction logic for OpenListDepositWindow.xaml
    /// </summary>
    public partial class OpenListDepositWindow : Window
    {
        public OpenListDepositWindow(Account.Account account, int numberOfBank)
        {
            InitializeComponent();

            foreach(Account.BankAccount temp in account.GetABankAccountList()) // Проход по всем банковским счетам
            {
                if (temp.Number == numberOfBank) // Если номер совпадает с банком, с которым взаимодействует пользователь
                {
                    foreach (Account.Deposit deposit in temp.GetDeposits()) // Проход по всем вкладам
                    {
                        depositsBox.Items.Add(deposit.ToString());
                    }
                    break;
                }
            }

            if (depositsBox.Items.Count == 0) 
                depositsBox.Items.Add("Нет открытых вкладов для этого банковского аккаунта!");
        }
    }
}
