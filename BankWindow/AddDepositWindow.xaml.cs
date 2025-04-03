using System;
using System.Globalization;
using System.Windows;

namespace BankWindow
{
    /// <summary>
    /// Interaction logic for AddDepositWindow.xaml
    /// </summary>
    public partial class AddDepositWindow : Window
    {
        public AddDepositWindow()
        {
            InitializeComponent();
        }

        private void Ok(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public string Deposit()
        {
            try
            {
                int Value = int.Parse(value.Text); // Значение вклада
                float percent = (float)(float.Parse(perсent.Text, new CultureInfo("ru-RU")) * 0.01); // Процент для вклада

                if (percent > 0 && percent <= 100 && Value > 0)
                    return $"{Value} {percent} {DateTime.Now.AddDays((int)(percent * 100 * 30))}"; // Строка вида: сумма процент дата закрытия

                return null;
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}
