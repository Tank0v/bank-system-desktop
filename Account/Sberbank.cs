using System;

namespace Account
{
    public class Sberbank : BankAccount // Счёт в сбербанке
    {
        public Sberbank(int id, int number, decimal balance, DateTime dateOfOpeningOfBankAccount) : base(id, number, balance, dateOfOpeningOfBankAccount) { } // Конструктор

        public override string ToString()
        {
            return "Сбербанк";
        }

        public override bool AddDeposit(int value, float persent, DateTime closeDate, DateTime birthdayDate) // Добавление вклада
        {
            AddDeposit(value, persent, closeDate, 1);
            return true;
        }

        public override int TransferSum(int value) // Сумма для перевода на другой счёт
        {
            return value + ((int)(0.01 * value)) + 30;
        }
    }
}
