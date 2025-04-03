using System;
using System.Windows;

namespace BankWindow
{
    /// <summary>
    /// Interaction logic for AddOrSubBalanceWindow.xaml
    /// </summary>
    public partial class AddOrSubBalanceWindow : Window
    {
        public AddOrSubBalanceWindow(string tip)
        {
            InitializeComponent();
            hint.Content = tip;
        }

        private void Ok(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public decimal AnInputValue()
        {
            try
            {
                return decimal.Parse(value.Text);
            }
            catch (FormatException)
            {
                return 0;
            }
            catch(OverflowException)
            {
                return 0;
            }
        }
    }
}