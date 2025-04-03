using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using Account;
using System.Collections.Generic;
using System.Globalization;

namespace BankWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Account.Account account; // Аккаунт, в который залогиниться пользователь
        private readonly AccountsFromTheFile existAccounts = new AccountsFromTheFile(); // Данные об уже созданных аккаунтах
        private int numberOfBank = 0; // Номер счёта для того, чтобы понять, на какой вкладке мы находимся (1 - сбербанк, 2 - альфа-банк, 3 - почта-банк)
        private BankAccount chosenAccount = null; // Аккаунт, который выбрал пользователь в момент переключения между банковскими счетами

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GoToRegistation(object sender, RoutedEventArgs e) // Перейти к регистрации аккаунта
        {
            mainCanvas.Visibility = Visibility.Hidden;
            registrationCanvas.Visibility = Visibility.Visible;
        }

        private void LoginAndPasswordRegistation(object sender, RoutedEventArgs e) // Регистрация аккаунта (ввод логина и пароля)
        {
            if (!choosingAnAvatar.IsEnabled)
                choosingAnAvatar.IsEnabled = true;

            string log = registrationLogin.Text.Replace(" ", ""), pass = registrationPassword.Password.Replace(" ", "");

            if (log.Length != registrationLogin.Text.Length || pass.Length != registrationPassword.Password.Length || log == "" || pass == "")
            {
                registrationLogin.Text = registrationPassword.Password = "";
                MessageBox.Show("В логине или пароли недопустимо наличие пробелов. Повторите попытку!");
            }
            else
            {
                if (!existAccounts.RegistrateAccount(registrationLogin.Text, registrationPassword.Password))
                {
                    registrationLogin.Text = registrationPassword.Password = "";
                    MessageBox.Show("Аккаунт с таким логином уже зарегистрирован. Повторите попытку!");
                }
                else
                {
                    registrationCanvas.Visibility = Visibility.Hidden;
                    personalDataCanvas.Visibility = Visibility.Visible;
                }
            }
        }

        private void RegistationOfPersonalData(object sender, RoutedEventArgs e) // Регистрация аккаунта (ввод персональных данных и выбор аватарки пользователя)
        {
            if (surname.Text != "" && name.Text != "" && patronymic.Text != "" && calendar.Text != "")
            {
                int id = existAccounts.GetAnAccountId(registrationLogin.Text);
                if (id != 0) // Если первый этап регистрации был пройден успешно
                {
                    try
                    {
                        DateTime openDate = DateTime.ParseExact(existAccounts.GetAnAccountCreationDate(id), "dd.MM.yyyy HH:mm:ss", new CultureInfo("ru-RU"));

                        string path = @"..\..\data\accountDetail.txt"; // Путь до файла с данными обо всех аккаунтах
                        File.AppendAllText(path, $"\n{registrationLogin.Text + " " + registrationPassword.Password + " " + id + " " + openDate}"); // Добавляем информацию в файл с данными обо всех аккаунтах

                        path = $@"..\..\data\{id}\{id}.txt"; // Путь до файла с данными о текущем аккаунте
                        Directory.CreateDirectory($@"..\..\data\{id}");
                        File.AppendAllText(path, $"{openDate}\n"); // Добавляем дату регистрации аккаунта
                        File.AppendAllText(path, $"{surname.Text} {name.Text} {patronymic.Text} {calendar.Text}\n"); // Добавляем информацию о пользователе

                        path = $@"..\..\data\{id}\{id}.png"; // Путь до аватарки пользователя
                        if (!File.Exists(path)) // Если аватарку не выбрали, то добавляем свою картинку
                            File.Copy(@"..\..\data\Picture.png", path);

                        surname.Text = name.Text = patronymic.Text = calendar.Text = "";
                        registrationLogin.Text = registrationPassword.Password = "";
                        personalDataCanvas.Visibility = Visibility.Collapsed;
                        authenticationCanvas.Visibility = Visibility.Visible;
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("Не удалось записать данные в файн, или он уже существует!");
                    }
                }
                else
                {
                    MessageBox.Show("Файл с данными повреждён! Повторите регистрацию!");
                }
            }
            else
            {
                MessageBox.Show("Все поля должны быть заполнены. Повторите попытку!");
            }
        }

        private void ChoosingAUsersAvatar(object sender, RoutedEventArgs e) // Выбор аватарки для пользователя
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "PNG Files (*.png)|*.png"
            };

            bool? result = dlg.ShowDialog();

            int id = existAccounts.GetAnAccountId(registrationLogin.Text);

            string line = $@"..\..\data\{id}\{id}.png";
            try
            {
                Directory.CreateDirectory($@"..\..\data\{id}");

                if (result == true)
                {
                    File.Copy(dlg.FileName, line);
                    choosingAnAvatar.IsEnabled = false;
                }
                else
                {

                    File.Copy(@"..\..\data\Picture.png", line);
                }
            }
            catch(IOException)
            {
                MessageBox.Show("Файл с аватаркой уже существует, или обнаружен некорректный путь до аватарки!");
            }
        }

        private void LogIn(object sender, RoutedEventArgs e) // Вход в аккаунт
        {
            account = existAccounts.LoginAccount(authenticationLogin.Text, authenticationPassword.Password); 

            if (account != null) // Если логин и пароль верные
            {
                accountList.Items.Clear();
                foreach (BankAccount acc in account.GetABankAccountList()) // Добавляем счета пользователя
                {
                    if (accountList.Items.Count < 3)
                        accountList.Items.Add(acc.ToString());
                }

                Id.Content = $"Id аккаунта: {account.Id}";
                authenticationCanvas.Visibility = Visibility.Hidden;
                accountMainCanvas.Visibility = Visibility.Visible;
                MainImage.Source = new BitmapImage(new Uri(new FileInfo($@"..\..\data\{account.Id}\{account.Id}.png").FullName, UriKind.Absolute));
            }
            else
            {
                authenticationLogin.Text = authenticationPassword.Password = "";
                MessageBox.Show("Некорректные данные для входа, или файл с данными повреждён!");
            }
        }

        private void AddNewBankAccount(object sender, RoutedEventArgs e) // Добавление нового банковского счёта
        {
            if (account.GetABankAccountList().Count < 3)
            {
                AddBankAccountWindow addBankAccountWindow = new AddBankAccountWindow(account);
                if (addBankAccountWindow.ShowDialog() == true)
                {
                    string temp = addBankAccountWindow.AddBankAccount();
                    if (temp != null)
                    {
                        accountList.Items.Add(temp);
                    }
                }
            }
            else
            {
                MessageBox.Show("Вы открыли счета во всех доступных банках!");
            }
        }

        private void ViewUserData(object sender, RoutedEventArgs e) // Посмотреть данные пользователя
        {
            personal.Content = $"Фамилия имя отчество:\n{account.Person.PersonalDataToString()}";
            birthdayDate.Content = $"Дата рождения:\n{account.Person.GetABirthdayDate():d}";
            OpenAccountDate.Content = $"Дата открытия аккаунта:\n{account.AccountCreationDate:d}";

            if (PersonalData.Visibility == Visibility.Visible)
            {
                PersonalData.Visibility = Visibility.Hidden;
                WatchData.Content = "Отобразить данные пользователя";
            }
            else
            {
                PersonalData.Visibility = Visibility.Visible;
                WatchData.Content = "Закрыть данные пользователя";
            }
        }

        private void SelectingAnAccountToView(object sender, RoutedEventArgs e) // Выбор банковского счёта для просмотра
        {
            var selectedItem = accountList.SelectedItem;
            if (selectedItem != null)
            {
                accountMainCanvas.Visibility = Visibility.Collapsed;
                bankAccountCanvas.Visibility = Visibility.Visible;

                string temp = selectedItem.ToString().Split(' ')[0];
                bankImage.Tag = temp;

                switch (temp)
                {
                    case "Сбербанк":
                        bankImage.Source = new BitmapImage(new Uri(new FileInfo(@"..\..\..\Account\ImageBank\Sberbank.jpg").FullName, UriKind.Absolute));
                        balance.Content = ((Sberbank)account.GetABankAccountByNumber(1)).GetBalance();
                        numberOfBank = 1;
                        break;
                    case "Альфа-банк":
                        bankImage.Source = new BitmapImage(new Uri(new FileInfo(@"..\..\..\Account\ImageBank\Alphabank.jpg").FullName, UriKind.Absolute));
                        balance.Content = ((Alphabank)account.GetABankAccountByNumber(2)).GetBalance();
                        numberOfBank = 2;
                        break;
                    case "Почта-банк":
                        bankImage.Source = new BitmapImage(new Uri(new FileInfo(@"..\..\..\Account\ImageBank\Pochtabank.jpg").FullName, UriKind.Absolute));
                        balance.Content = ((Pochtabank)account.GetABankAccountByNumber(3)).GetBalance();
                        numberOfBank = 3;
                        break;
                    default:
                        break;
                }
            }
        }

        public void Switcher(ref BankAccount chosenAccount) // Определение, с каким конкретно банком взаимодействует пользователь
        {
            switch (numberOfBank)
            {
                case 1:
                    chosenAccount = (Sberbank)account.GetABankAccountByNumber(numberOfBank);
                    break;
                case 2:
                    chosenAccount = (Alphabank)account.GetABankAccountByNumber(numberOfBank);
                    break;
                case 3:
                    chosenAccount = (Pochtabank)account.GetABankAccountByNumber(numberOfBank);
                    break;
                default:
                    break;
            }
        }

        public void ChangeDataInFile(string newBalance, int code) // Перезапись данных в файл (code нужен для того, чтобы метод корректно работал)
        {
            string path = $@"..\..\data\{account.Id}\{account.Id}.txt";

            List<string> lines = new List<string>(); // Хранение данных из файла

            try
            {
                using (StreamReader reader = new StreamReader(path)) // Чтение данных из файла
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }

                for (int i = 2; i < lines.Count; i++) // Замена в строке значения на новое (обновление баланса)
                {
                    string[] words = lines[i].Split(' ');
                    if (words[0].Equals("s") && code == 1)
                    {
                        words[1] = newBalance;
                    }
                    else if (words[0].Equals("a") && code == 2)
                    {
                        words[1] = newBalance;
                    }
                    else if (words[0].Equals("p") && code == 3)
                    {
                        words[1] = newBalance;
                    }

                    lines[i] = string.Join(" ", words);
                }

                File.WriteAllLines(path, lines);
            }
            catch(IOException)
            {
                MessageBox.Show("Файл с данными об аккаунте отсутствует!");
            }
        }

        private void AddBalance(object sender, RoutedEventArgs e) // Добавление баланса на счёт
        {
            AddOrSubBalanceWindow addOrSubBalanceWindow = new AddOrSubBalanceWindow("Введите сумму для пополнения счёта");

            if (addOrSubBalanceWindow.ShowDialog() == true)
            {
                if (addOrSubBalanceWindow.AnInputValue() > 0)
                {
                    Switcher(ref chosenAccount); // Определение, с каким банком взаимодействует пользователь
                    chosenAccount.AddBalance(addOrSubBalanceWindow.AnInputValue()); // Добавление введённого значения на счёт
                    balance.Content = chosenAccount.GetBalance(); // Обновление баланса на метке
                    ChangeDataInFile($"{chosenAccount.GetBalance()}", numberOfBank); // Изменение данных в файле
                }
                else
                {
                    MessageBox.Show("Введённое значение должно быть натуральным число (и не должно быть больше, чем 7,9228 x 10^28)!");
                }
            }
        }

        private void SubstructBalance(object sender, RoutedEventArgs e) // Снятие баланса со счёта
        {
            AddOrSubBalanceWindow addOrSubBalanceWindow = new AddOrSubBalanceWindow("Введите сумму для снятия со счёта");

            if (addOrSubBalanceWindow.ShowDialog() == true)
            {
                if (addOrSubBalanceWindow.AnInputValue() > 0)
                {
                    if (addOrSubBalanceWindow.AnInputValue() <= decimal.Parse(balance.Content.ToString()))
                    { 
                        Switcher(ref chosenAccount); // Определение, с каким банком взаимодействует пользователь
                        chosenAccount.SubstructBalance(addOrSubBalanceWindow.AnInputValue()); // Вычитание введённого значения со счёта
                        balance.Content = chosenAccount.GetBalance(); // Обновление баланса на метке
                        ChangeDataInFile($"{chosenAccount.GetBalance()}", numberOfBank); // Изменение данных в файле
                    }
                    else
                    {
                        MessageBox.Show("На Вашем счету недостаточно средств для снятия!");
                    }
                }
                else
                {
                    MessageBox.Show("Введённое значение должно быть натуральным числом (и не должно быть больше, чем 7,9228 x 10^28)!");
                }
            }
        }

        private void TransferBetweenBankAccounts(object sender, RoutedEventArgs e) // Перевод между счетами
        {
            TransferBetweenBankAccountWindow transferOnBankAccount = new TransferBetweenBankAccountWindow(account, numberOfBank);

            if (transferOnBankAccount.ShowDialog() == true)
            {
                if (transferOnBankAccount.Transfer(numberOfBank))
                {
                    Switcher(ref chosenAccount); // Определение, с каким банком взаимодействует пользователь
                    balance.Content = chosenAccount.GetBalance(); // Обновление баланса на метке
                    ChangeDataInFile($"{chosenAccount.GetBalance()}", numberOfBank); // Изменение данных в файле текущего банка
                    ChangeDataInFile($"{account.GetABankAccountByNumber(transferOnBankAccount.numberToTransfer).GetBalance()}", transferOnBankAccount.numberToTransfer); // Изменение данных в файле банка, в который переводились деньги
                }
                else
                {
                    MessageBox.Show("Перевод не выполнен!");
                }
            }
        }

        private void AddDeposit(object sender, RoutedEventArgs e) // Добавление вклада к банковскому счёту
        {
            AddDepositWindow addDepositWindow = new AddDepositWindow();

            if (addDepositWindow.ShowDialog() == true)
            {
                try
                {
                    if (addDepositWindow.Deposit() != null)
                    {
                        string[] lines = addDepositWindow.Deposit().Split(' ');
                        int value = int.Parse(lines[0]);
                        float percent = float.Parse(lines[1], new CultureInfo("ru-RU"));
                        DateTime closeData = DateTime.ParseExact(lines[2] + " " + lines[3], "dd.MM.yyyy HH:mm:ss", new CultureInfo("ru-RU"));

                        if (lines.Length == 4 && value <= decimal.Parse(balance.Content.ToString()))
                        {
                            Switcher(ref chosenAccount); // Определение, с каким банком взаимодействует пользователь
                            if (chosenAccount.AddDeposit(value, percent, closeData, account.Person.GetABirthdayDate())) // Добавление вклада
                            {
                                chosenAccount.SubstructBalance(value); // Вычитание введённого значения со счёта  
                                balance.Content = chosenAccount.GetBalance(); // Обновление баланса на метке
                                ChangeDataInFile($"{chosenAccount.GetBalance()}", numberOfBank); // Изменение данных в файле
                            }
                            else
                            {
                                MessageBox.Show("Вы не достигли восемнадцатилетий, либо Вашему аккаунту меньше трёх лет!");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Недостаточно средств на счету!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Введите корректные данные!");
                    }
                }
                catch (Exception ex)
                {
                    if (ex is FormatException || ex is ArgumentNullException)
                        MessageBox.Show("Введите корректные данные!");
                    else
                        MessageBox.Show("Введённое значение слишком большое!");
                }
            }
        }

        private void OpenListOfDeposits(object sender, RoutedEventArgs e) // Открыть список депозитов для данного счёта
        {
            OpenListDepositWindow openListDepositWindow = new OpenListDepositWindow(account, numberOfBank);
            openListDepositWindow.ShowDialog();     
        }

        private void GoToAccountMain(object sender, RoutedEventArgs e) // Перейти к странице аккаунта
        {
            bankAccountCanvas.Visibility = Visibility.Collapsed;
            accountMainCanvas.Visibility = Visibility.Visible;
        }

        private void LogOut(object sender, RoutedEventArgs e) // Выйти из аккаунта
        {
            authenticationLogin.Text = authenticationPassword.Password = "";
            authenticationCanvas.Visibility = Visibility.Visible;
            accountMainCanvas.Visibility = Visibility.Hidden;
            if (PersonalData.Visibility == Visibility.Visible)
                PersonalData.Visibility = Visibility.Hidden;
      
        }

        private void OpenAutentificationCanvas(object sender, RoutedEventArgs e) // Перейти ко входу в аккаунт
        {
            mainCanvas.Visibility = Visibility.Hidden;
            authenticationCanvas.Visibility = Visibility.Visible;
        }

        private void RegistrationFromInputPersonalData(object sender, RoutedEventArgs e) // Перейти к регистрации аккаунта
        {
            name.Text = surname.Text = patronymic.Text = calendar.Text = "";
            personalDataCanvas.Visibility = Visibility.Collapsed;
            registrationCanvas.Visibility = Visibility.Visible;

            if(existAccounts.DeleteAccount(existAccounts.GetAnAccountId(registrationLogin.Text)))
                MessageBox.Show("Повторите регистрацию снова!");
        }

        private void GoToMainCanvas(object sender, RoutedEventArgs e) // Вернуться в главное окно из меню регистрации/аутентификации
        {
            if (authenticationCanvas.Visibility == Visibility.Visible)
            {
                authenticationLogin.Text = authenticationPassword.Password = "";
                authenticationCanvas.Visibility = Visibility.Hidden;
            }
            else
            {
                registrationLogin.Text = registrationPassword.Password = "";
                registrationCanvas.Visibility = Visibility.Hidden;
            }

            mainCanvas.Visibility = Visibility.Visible;
        }
    }
}