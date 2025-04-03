using System;

namespace Account
{
    public class Pochtabank : BankAccount // Счёт в почта-банке
    {
        public Pochtabank(int id, int number, decimal balance, DateTime dateOfOpeningOfBankAccount) : base(id, number, balance, dateOfOpeningOfBankAccount) { } // Конструктор

        public override string ToString()
        {
            return "Почта-банк";
        }

        public override bool AddDeposit(int value, float persent, DateTime closeDate, DateTime birthdayDate) // Добавление вклада (только если пользователю больше 18 лет)
        {
            int age;

            if ((DateTime.Now.Month >= birthdayDate.Month) && (DateTime.Now.Day >= birthdayDate.Day))
                age = DateTime.Now.Year - birthdayDate.Year;
            else
                age = DateTime.Now.Year - birthdayDate.Year - 1;

            if (age >= 18)
            {
                AddDeposit(value, persent, closeDate, 1);
                return true;
            }

            return false;
        }

        public override int TransferSum(int value) // Сумма для перевода на другой счёт
        {
            return value;
        }
    }
}
