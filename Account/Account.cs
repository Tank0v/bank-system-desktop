using System;
using System.Collections.Generic;
using System.Linq;

namespace Account
{
    public class Account // Аккаунт пользователя
    {
        public string Login { get; } // Логин
        private readonly string Password; // Пароль
        public int Id { get; } // Уникальный идентификатор аккаунта
        public DateTime AccountCreationDate { get; } // Дата создания аккаунта
        public Person Person { get; } // Персональные данные пользователя
        private readonly bool[] OpenBankAccounts = new bool[3]; // Открытые банковские счета
        private readonly List<BankAccount> BankAccountList = new List<BankAccount>(); // Список банковских счетов

        public Account(string login, string pass, int id, DateTime accountCreationDate, string name, string surname, string patronymic, DateTime birthdayDate) // Конструктор
        {
            Login = login;
            Password = pass;
            Id = id;
            AccountCreationDate = accountCreationDate;
            Person = new Person(name, surname, patronymic, birthdayDate);
        }

        public void AddBankAccount(int number, decimal balance, DateTime accountOpeningDate) // Добавление банковских счетов
        {
            switch (number)
            {
                case 1:
                    if (!OpenBankAccounts[number - 1]) // Добавление счёта в сбербанке
                    {
                        BankAccountList.Add(new Sberbank(Id, number, balance, accountOpeningDate));
                        OpenBankAccounts[number - 1] = true;
                    }
                    break;
                case 2:
                    if (!OpenBankAccounts[number - 1]) // Добавление счёта в альфа-банке
                    {
                        BankAccountList.Add(new Alphabank(Id, number, balance, accountOpeningDate));
                        OpenBankAccounts[number - 1] = true;
                    }
                    break;
                case 3:
                    if (!OpenBankAccounts[number - 1]) // Добавление счёта в почта-банке
                    {
                        BankAccountList.Add(new Pochtabank(Id, number, balance, accountOpeningDate));
                        OpenBankAccounts[number - 1] = true;
                    }
                    break;
                default:
                    break;
            }
        }

        public string[] GetANameOfNotOpenBankAccounts() // Названия банков, в которых счета ещё не открыты
        {
            int len = OpenBankAccounts.Count(s => !s);
            string[] strings = new string[len];

            int index = 0;
            if (!OpenBankAccounts[0])
            {
                strings[index] = "Сбербанк";
                index++;
            }
            if (!OpenBankAccounts[1])
            {
                strings[index] = "Альфа-банк";
                index++;
            }
            if (!OpenBankAccounts[2])
            {
                strings[index] = "Почта-банк";
                index++;
            }

            return strings;
        }

        public List<BankAccount> GetABankAccountList() // Возвращает список открытых банковских счетов
        {
            return BankAccountList;
        }

        public BankAccount GetABankAccountByNumber(int number) // Возвращает конкретный банковский счёт по номеру
        {
            foreach (BankAccount temp in BankAccountList)
            {
                if (temp.Number == number)
                    return temp;
            }
            return null;
        }

        public bool Transfer(int numberFromWhere, int numberWhere, int value) // Перевод между счетами
        {
            bool hint = false; // Метка для проверки того, что на счету отправителя достаточно средств на счёте

            if (numberFromWhere != numberWhere)
            {
                foreach (BankAccount temp in GetABankAccountList()) // Снятие значения со счёта отправителя
                {
                    if (temp.Number == numberFromWhere)
                    {
                        if (temp.GetBalance() >= temp.TransferSum(value))
                        {
                            temp.SubstructBalance(temp.TransferSum(value));
                            hint = true;
                        }
                        else
                            return false;
                    }
                }

                foreach(BankAccount temp in GetABankAccountList()) // Добавление значения на счёт получателя
                {
                    if (temp.Number == numberWhere && hint)
                    {
                        temp.AddBalance(value);
                        return true;
                    }
                }
            }
            return false;
        }

        public string AccountIdToString() // Строчное представление id аккаунта
        {
            return "Номер счёта: " + Id;
        }
    }
}
