using System;

namespace Account
{
    public class Person // Информация о пользователе
    {
        private readonly string name; // Имя
        private readonly string surname; // Фамилия
        private readonly string patronymic; // Отчество
        private readonly DateTime birthdayDate; // Дата рождения

        public Person(string name, string surname, string patronymic, DateTime birthdayDate) // Конструктор
        {
            this.name = name;
            this.surname = surname;
            this.patronymic = patronymic;
            this.birthdayDate = birthdayDate;
        }

        public string PersonalDataToString() // Строчное представление персональных данных
        {
            return $"{name} {surname} {patronymic}";
        }

        public DateTime GetABirthdayDate() // Получение даты рождения пользователя
        {
            return birthdayDate;
        }
    }
}
