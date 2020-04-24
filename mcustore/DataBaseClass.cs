﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace mcustore
{
    /// <summary>Класс для работы с БД</summary>
    public class DataBaseClass
    {        
        /// <summary>Строчка подключения к БД</summary>
        private string m_connection_string;

        /// <summary>Создаёт объект по работе с БД</summary>
        /// <param name="connection_string">Строчка подключения к БД</param>
        public DataBaseClass()
        {
            m_connection_string = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\mcustoredatabase.mdf;Integrated Security=True"; // сохраняем строчку подключения к БД
        }

        /// <summary>Проверяет подключение к БД</summary>
        /// <param name="connection_string">Строчка подключения к БД</param>
        public bool IsConnect()
        {
            SqlConnection sqlConnection = new SqlConnection(m_connection_string); // инициализируем соединение с БД
            try
            {
                sqlConnection.Open(); // открываем соединение с БД
            }
            catch // в случае возникновения какого-либо исключения
            {
                return false;
                //MessageBox.Show("Ошибка подключения к базе данных!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            sqlConnection.Close(); // закрываем соединение с БД
            return true;
        }

        /// <summary>Отправляет указанный запрос к БД и заносит полученные данные в указанную таблицу</summary>
        /// <param name="query">Строчка с запросом</param>
        /// <param name="dataGridView">Двумерный массив, строчки которого содержат соответствующие данные таблицы, к которой выполнен запрос (возвращает null в случае возникновения ошибки)</param>
        private List<List<string>> ReadDataToMass(string query)
        {
            List<List<string>> mass = new List<List<string>>();
            SqlConnection sqlConnection = new SqlConnection(m_connection_string); // инициализируем соединение с БД
            try
            {
                sqlConnection.Open(); // открываем соединение с БД

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection); // создание SQL команды с указанным запросом
                SqlDataReader reader = sqlCommand.ExecuteReader(); // выполнение SQL команды

                int row_id = 0; // id текущей строчки
                if (reader.HasRows) // если данные были найдены
                {
                    while (reader.Read()) // пока не будут считаны все строки из выборки
                    {
                        List<string> row = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++) // для каждого столбца таблицы
                        {
                            row.Add(reader.GetValue(i).ToString()); // заносим значение из строчки выборки в таблицу
                        }
                        mass.Add(row);
                        row_id++; // увеличиваем счётчик
                    }
                }

                reader.Close();
            }
            catch
            {
                return null;
            }
            sqlConnection.Close(); // закрываем соединение с БД
            return mass;
        }

        /// <summary>Выбирает данные из таблицы Orders о заказах, созданных с первой по вторую указанные даты (включительно)</summary>
        /// <param name="date_from">Дата "от" (включительно)</param>
        /// <param name="date_to">Дата "до" (включительно)</param>
        /// <returns>Двумерный список данных для таблицы в порядке: ID заказа - Название компании - Микроконтроллеры - Общая цена - Дата заказа</returns>
        public List<List<string>> SelectOrdersData(string date_from = "2000-12-31", string date_to = "2100-12-31")
        {
            // ID заказа - Название компании - Микроконтроллеры - Общая цена - Дата заказа

            string sql = "SELECT Order_id, Company_name, Datetime FROM Orders WHERE (Datetime >= '" + date_from + "') AND (Datetime <= '" + date_to + "');";
            List<List<string>> orders_list = ReadDataToMass(sql);

            List<List<string>> result = new List<List<string>>();
            int all_price = 0;

            for (int i = 0; i < orders_list.Count; i++) // для каждого заказа
            {
                result[i].Add(orders_list[i][0]); // id заказа 
                result[i].Add(orders_list[i][1]); // название компании
                string microcontrollers_info = "";

                int order_id = Convert.ToInt32(orders_list[i][0]);
                string sql2 = "SELECT Microcontrollers.Microcontroller_name, order_microcontroller.Quantity, Microcontrollers.Price FROM Microcontrollers, order_microcontroller WHERE (Microcontrollers.Microcontroller_id = order_microcontroller.Microcontroller_id) AND (order_microcontroller.Order_id = " + order_id + ");";
                List<List<string>> microcontroller_info_list = ReadDataToMass(sql2);
                for (int i2 = 0; i2 < microcontroller_info_list.Count; i2++) {
                    if (i2 != 0) {
                        microcontrollers_info += ", ";
                    }
                    microcontrollers_info += microcontroller_info_list[i2][0] + " (" + microcontroller_info_list[i2][1] + " шт.)";
                    int price = Convert.ToInt32(microcontroller_info_list[i2][2]) * Convert.ToInt32(microcontroller_info_list[i2][1]); // количество микроконтроллеров умножаем на цену
                    all_price += price;
                }
                result[i].Add(microcontrollers_info); // информация о микроконтроллерах и их количестве
                result[i].Add(all_price.ToString()); // общая стоимость
            }

            return result;
        }

        /// <summary>Выбирает данные из таблицы Microcontrollers о микроконтроллерах с названием, содержащим передаваемую строчку</summary>
        /// <param name="name_contains">Строчка, которую должен содержать микроконтроллер</param>
        /// <returns>Двумерный список данных для таблицы в порядке: Название микроконтроллера - Количество - Цена за штуку
        public List<List<string>> SelectMicrocontrollersData(string name_contains = "")
        {
            // Название микроконтроллера - Количество - Цена за штуку

            string sql = "SELECT Microcontroller_name, Quantity, Price FROM Microcontrollers WHERE (Microcontroller_name LIKE N'%" + name_contains + "%');";

            return ReadDataToMass(sql);
        }

        /// <summary>Выполняет запрос без возврата значения и без изменения чего-либо на форме</summary>
        /// <param name="query">Запрос</param>
        /// <returns>1 - в случае успешного запроса, 0 - в случае, если запрос не удалось выполнить, -1 - в случае ошибки</returns>
        private int GoQuery(string query)
        {
            SqlConnection sqlConnection = new SqlConnection(m_connection_string); // инициализируем соединение с БД
            try
            {
                sqlConnection.Open(); // открываем соединение с БД

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection); // создание SQL команды с указанным запросом
                SqlDataReader reader = sqlCommand.ExecuteReader(); // выполнение SQL команды
                if (reader.HasRows)
                {
                    reader.Close();
                    sqlConnection.Close(); // закрываем соединение с БД
                    return 1;
                }
                else
                {
                    reader.Close();
                    sqlConnection.Close(); // закрываем соединение с БД
                    return 0;
                }
            }
            catch
            {
                sqlConnection.Close(); // закрываем соединение с БД
                return -1;
            }
        }

    }
}
