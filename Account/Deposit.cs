using System;

namespace Account
{
    public class Deposit // Информация о вкладе
    {
        private readonly int Value; // Значение вклада
        private readonly float Percent; // Процент вклада
        private readonly DateTime CloseDate; // Дата закрытия вклада

        public Deposit(int value, float percent, DateTime closeDate) // Конструктор
        {
            Value = value;
            Percent = percent;
            CloseDate = closeDate;
        }

        public override string ToString() // Строчное представление вклада
        {
            return $"Сумма вклада: {Value}, процент: {Percent * 100}, дата закрытия вклада: {CloseDate}, прибыль: {Value * Percent}"; // Указывается только чистая прибыль
        }
    }
}
