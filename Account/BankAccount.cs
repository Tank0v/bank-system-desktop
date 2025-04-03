using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace Account
{
    public abstract class BankAccount // Банковский аккаунт
    {
        public int Id { get; } // Уникальный идентификатор аккаунта
        public int Number { get; } // Номер банковского счёта (1 - сбербанк, 2 - альфа-банк, 3 - почта-банк)
        private decimal Balance; // Баланс банковского счёта
        public DateTime DateOfOpeningOfBankAccount { get; } // Дата открытия банковского счёта
        protected List<Deposit> deposits = new List<Deposit>(); // Список вкладов для банковского счёта

        public BankAccount(int id, int number, decimal balance, DateTime dateOfOpeningOfBankAccount) // Констуктор
        {
            Id = id;
            Number = number;
            Balance = balance;
            DateOfOpeningOfBankAccount = dateOfOpeningOfBankAccount;
        }

        public abstract bool AddDeposit(int sum, float persent, DateTime closeDate, DateTime birthdayDate); // Добавление вклада к банковскому счёту (проверка условий в конкретных банках)

        public abstract int TransferSum(int sum); // Сумма для перевода на другой счёт

        public decimal GetBalance() // Получение баланса
        {
            return Balance;
        }

        public void AddBalance(decimal amount) // Добавление баланса на счёт
        {
            Balance += amount;
        }

        public void SubstructBalance(decimal amount) // Снятие баланса со счёта
        {
            Balance -= amount;
        }

        public string GetCode(int number) // Получение кода по номеру (для заполнения файла)
        {
            switch(number)
            {
                case 1:
                    return "s_d";
                case 2:
                    return "a_d";
                case 3:
                    return "p_d";
                default:
                    return null;
            }
        }

        public List<Deposit> GetDeposits() // Получение списка вкладов
        {
            return deposits;
        }

        public void AddDeposit(int value, float percent, DateTime depositClosingDate, int tip) // Добавление вклада к банковскому счёту
        {
            int result = DateTime.Compare(depositClosingDate, DateTime.Now); // Сравнение даты закрытия счёта и текущей даты
            string code = GetCode(Number);

            if (result <= 0) // Дата закрытия вклада уже прошла
            {
                ChangeDataInFile($"{code} {value} {percent.ToString(new CultureInfo("ru-RU"))} {depositClosingDate}"); // Удаление информации о вкладе из файла
                Balance += Math.Round((decimal)percent * value, 3); // Добавление на баланс полученного значения с вклада
            }
            else
            {
                deposits.Add(new Deposit(value, percent, depositClosingDate)); // Добавление вклада к банковскому счёту

                if (tip != 0) // Добавление данных через диалог
                {
                    string path = $@"..\..\data\{Id}\{Id}.txt";
                    using (StreamWriter writetext = new StreamWriter(path, true)) // Добавление информации о вкладе в файл данного пользователя
                    {
                        writetext.WriteLine($"{code} {value} {percent} {depositClosingDate}");
                    }
                }
            }
        }

        public void ChangeDataInFile(string smth) // Перезаписывание данных в файл
        {
            string path = $@"..\..\data\{Id}\{Id}.txt";

            List<string> lines = new List<string>(); // Список для хранения строк в файле

            using (StreamReader reader = new StreamReader(path)) // Чтение данных
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Equals(smth))
                {
                    lines[i] = "";
                    break;
                }
            }

            File.WriteAllLines(path, lines);
        }
    }
}
