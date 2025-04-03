using System;

namespace Account
{
    public class Alphabank : BankAccount // Счёт в альфа-банке
    {
        public Alphabank(int id, int number, decimal balance, DateTime dateOfOpeningOfBankAccount) : base(id, number, balance, dateOfOpeningOfBankAccount) { } // Конструктор

        public override string ToString()
        {
            return "Альфа-банк";
        }

        public override bool AddDeposit(int value, float persent, DateTime closeDate, DateTime birthdayDate) // Добавление вклада (только если аккаунт пользователя создан больше трёх лет назад)
        {
            int age;

            if ((DateTime.Now.Month >= DateOfOpeningOfBankAccount.Month) && (DateTime.Now.Day >= DateOfOpeningOfBankAccount.Day))
                age = DateTime.Now.Year - DateOfOpeningOfBankAccount.Year;
            else
                age = DateTime.Now.Year - DateOfOpeningOfBankAccount.Year - 1;

            if (age >= 3)
            {
                AddDeposit(value, persent, closeDate, 1);
                return true;
            }

            return false;
        }

        public override int TransferSum(int value)  // Сумма для перевода на другой счёт
        {
            return value + 30;
        }
    }
}
