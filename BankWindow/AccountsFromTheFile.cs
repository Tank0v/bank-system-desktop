using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;

namespace BankWindow
{
    public class DataFromFile // Данные об аккаунтах
    {
        public string Login;
        public string Password;
        public int Id;
        public DateTime OpenDate;
    }

    public class AccountsFromTheFile // Класс для взаимодействия с уже созданными аккаунтами
    {
        private readonly List<DataFromFile> fileDataList = new List<DataFromFile>(); // Список данных об уже созданных аккаунтах
        private readonly HashSet<int> usersIds = new HashSet<int>(); // Хранение используемых id аккаунтов

        public AccountsFromTheFile()
        {
            CreatingAListOfDataFromAFile();
        }

        public void CreatingAListOfDataFromAFile() // Заполнение списка уже созданных аккаунтов данными из файла
        {
            string path = @"..\..\data\accountDetail.txt";
            string errorPath = @"..\..\data\errors.txt";

            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path, Encoding.GetEncoding("UTF-8")))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        try
                        {
                            string[] parts = line.Split(' ');
                            string login = parts[0], password = parts[1];
                            int id = int.Parse(parts[2]);
                            DateTime openDate = DateTime.ParseExact(parts[3] + " " + parts[4], "dd.MM.yyyy HH:mm:ss", new CultureInfo("ru-RU"));

                            if (!fileDataList.Any(s => s.Login == login || s.Id == id)) // Если такой аккаунт ещё не зарегистрирован
                            {
                                fileDataList.Add(new DataFromFile { Login = login, Password = password, Id = id, OpenDate = openDate });
                                usersIds.Add(id);
                            }
                        }
                        catch (FormatException)
                        {
                            using (StreamWriter writer = new StreamWriter(errorPath, true, Encoding.GetEncoding("UTF-8")))
                            {
                                writer.WriteLine($"Ошибка в строке: {line}");
                            }

                        }
                        catch (IndexOutOfRangeException)
                        {
                            using (StreamWriter writer = new StreamWriter(errorPath, true, Encoding.GetEncoding("UTF-8")))
                            {
                                if (line.Length == 0)
                                    writer.WriteLine("Пустая строка в файле");
                                else
                                    writer.WriteLine($"Недостаточное количество данных в строке: {line}");
                            }
                        }
                    }
                }
            }
        }

        public int TakeAUniqueId() // Поиск уникального Id
        {
            int newId;
            do
            {
                Random random = new Random();
                newId = random.Next(1, 1000);
            }
            while (usersIds.Contains(newId));
            usersIds.Add(newId);

            return newId;
        }

        public bool RegistrateAccount(string login, string password) // Регистрация аккаунта
        {
            foreach (DataFromFile account in fileDataList)
            {
                if (login == account.Login)
                    return false;
            }

            fileDataList.Add(new DataFromFile { Login = login, Password = password, Id = TakeAUniqueId(), OpenDate = DateTime.Now });

            return true;
        }

        public Account.Account LoginAccount(string login, string password) // Авторизация 
        {
            Account.Account account;

            int id = GetAnAccountId(login);
            string path = $@"..\..\data\{id}\{id}.txt";
            string errorPath = @"..\..\data\errors.txt";

            if (File.Exists(path))
            {
                List<string> strings = new List<string>(); // Хранение данных из файла

                try
                {
                    using (StreamReader reader = new StreamReader(path, Encoding.GetEncoding("UTF-8")))
                    {
                        string s = reader.ReadLine();
                        DateTime date = DateTime.ParseExact(s, "dd.MM.yyyy HH:mm:ss", new CultureInfo("ru-RU"));
                        string[] data = reader.ReadLine().Split(' ');
                        string surname = data[0], name = data[1], patronymic = data[2];
                        DateTime birthdayDate = DateTime.ParseExact(data[3], "dd.MM.yyyy", new CultureInfo("ru-RU"));

                        account = new Account.Account(login, password, id, date, name, surname, patronymic, birthdayDate);

                        while ((s = reader.ReadLine()) != null)
                        {
                            strings.Add(s);
                        }
                    }

                    foreach (string temp in strings) // Добавляем данные о счетах пользователя и его вкладах
                    {
                        string[] parts = temp.Split(' ');
                        switch (parts[0])
                        {
                            case "s":
                                account.AddBankAccount(1, decimal.Parse(parts[1]), DateTime.ParseExact(parts[2] + " " + parts[3], "dd.MM.yyyy HH:mm:ss", new CultureInfo("ru-RU")));
                                break;
                            case "a":
                                account.AddBankAccount(2, decimal.Parse(parts[1]), DateTime.ParseExact(parts[2] + " " + parts[3], "dd.MM.yyyy HH:mm:ss", new CultureInfo("ru-RU")));
                                break;
                            case "p":
                                account.AddBankAccount(3, decimal.Parse(parts[1]), DateTime.ParseExact(parts[2] + " " + parts[3], "dd.MM.yyyy HH:mm:ss", new CultureInfo("ru-RU")));
                                break;
                            case "s_d":
                                account.GetABankAccountByNumber(1).AddDeposit(int.Parse(parts[1]), float.Parse(parts[2], new CultureInfo("ru-RU")), DateTime.ParseExact(parts[3] + " " + parts[4], "dd.MM.yyyy HH:mm:ss", new CultureInfo("ru-RU")), 0);
                                break;
                            case "a_d":
                                account.GetABankAccountByNumber(2).AddDeposit(int.Parse(parts[1]), float.Parse(parts[2], new CultureInfo("ru-RU")), DateTime.ParseExact(parts[3] + " " + parts[4], "dd.MM.yyyy HH:mm:ss", new CultureInfo("ru-RU")), 0);
                                break;
                            case "p_d":
                                account.GetABankAccountByNumber(3).AddDeposit(int.Parse(parts[1]), float.Parse(parts[2], new CultureInfo("ru-RU")), DateTime.ParseExact(parts[3] + " " + parts[4], "dd.MM.yyyy HH:mm:ss", new CultureInfo("ru-RU")), 0);
                                break;
                            default:
                                break;
                        }
                    }
                    return account;
                }
                catch (Exception ex)
                {
                    if (ex is IndexOutOfRangeException || ex is FormatException)
                    {
                        using (StreamWriter writer = new StreamWriter(errorPath, true, Encoding.GetEncoding("UTF-8")))
                        {
                            writer.WriteLine($"Повреждённые данные в файле аккаунта с id {id}");
                            return null;
                        }
                    }
                }
            }
            return null;
        }

        public int GetAnAccountId(string login) // Получение id аккаунта
        {
            foreach (DataFromFile account in fileDataList)
            {
                if (account.Login == login)
                    return account.Id;
            }
            return 0;
        }

        public string GetAnAccountCreationDate(int id) // Получение даты создания аккаунта
        {
            foreach (DataFromFile account in fileDataList)
            {
                if (account.Id == id)
                    return account.OpenDate.ToString();
            }
            return null;
        }

        public bool DeleteAccount(int id) // Удаление аккаунта в случае, если регистрация не была пройдена полностью
        {
            foreach (DataFromFile account in fileDataList)
            {
                if (account.Id == id)
                {
                    fileDataList.Remove(account);
                    usersIds.Remove(account.Id);
                    return true;
                }
            }
            return false;
        }
    }
}
