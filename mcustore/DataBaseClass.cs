using System;
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
        public bool Connect()
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

        /// <summary>Выполняет запрос без возврата значения и без изменения чего-либо на форме</summary>
        /// <param name="query">Запрос</param>
        /// <returns>1 - в случае успешного запроса, 0 - в случае, если запрос не удалось выполнить, -1 - в случае ошибки</returns>
        public int GoQuery(string query)
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

        /// <summary>Отправляет указанный запрос к БД и заносит полученные данные в указанную таблицу</summary>
        /// <param name="query">Строчка с запросом</param>
        /// <param name="dataGridView">Двумерный массив, первая строчка которого содержит названия столбцов, остальные строчки содержат соответствующие данные (возвращает null в случае возникновения ошибки)</param>
        public List<List<string>> ReadAllToMass(string query)
        {
            List<List<string>> mass = new List<List<string>>();
            SqlConnection sqlConnection = new SqlConnection(m_connection_string); // инициализируем соединение с БД
            try
            {
                sqlConnection.Open(); // открываем соединение с БД

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection); // создание SQL команды с указанным запросом
                SqlDataReader reader = sqlCommand.ExecuteReader(); // выполнение SQL команды
                int columns = reader.FieldCount; // устанавливаем количество столбцов в таблице равное количеству полей в полученной выборке
                mass.Add(new List<string>());
                for (int i = 0; i < columns; i++) // для каждого столбца таблицы
                {
                    mass[0].Add(reader.GetName(i)); // устанавливаем имя столбца таблицы равное имени соответствующего поля в полученной выборке
                }

                int row_id = 0; // id текущей строчки
                if (reader.HasRows) // если данные были найдены
                {
                    while (reader.Read()) // пока не будут считаны все строки из выборки
                    {
                        List<string> row = new List<string>();
                        for (int i = 0; i < columns; i++) // для каждого столбца таблицы
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
                //MessageBox.Show("Ошибка при обработке запроса к базе данных!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            sqlConnection.Close(); // закрываем соединение с БД
            return mass;
        }

        /// <summary>Отправляет указанный запрос к БД и заносит полученные данные в указанную таблицу</summary>
        /// <param name="query">Строчка с запросом</param>
        /// <param name="dataGridView">Двумерный массив, строчки которого содержат соответствующие данные таблицы, к которой выполнен запрос (возвращает null в случае возникновения ошибки)</param>
        public List<List<string>> ReadOnlyDataToMass(string query)
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
                //MessageBox.Show("Ошибка при обработке запроса к базе данных!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            sqlConnection.Close(); // закрываем соединение с БД
            return mass;
        }

        /// <summary>Отправляет указанный запрос к БД и заносит полученные данные в указанную таблицу</summary>
        /// <param name="query">Строчка с запросом</param>
        /// <param name="dataGridView">Одномерный массив, содержащий названия столбцов таблицы, к которой выполнен запрос (возвращает null в случае возникновения ошибки)</param>
        public List<string> ReadColumnsNames(string query)
        {
            List<string> mass = new List<string>();
            SqlConnection sqlConnection = new SqlConnection(m_connection_string); // инициализируем соединение с БД
            try
            {
                sqlConnection.Open(); // открываем соединение с БД

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection); // создание SQL команды с указанным запросом
                SqlDataReader reader = sqlCommand.ExecuteReader(); // выполнение SQL команды
                int columns = reader.FieldCount; // устанавливаем количество столбцов в таблице равное количеству полей в полученной выборке
                for (int i = 0; i < columns; i++) // для каждого столбца таблицы
                {
                    mass.Add(reader.GetName(i)); // устанавливаем имя столбца таблицы равное имени соответствующего поля в полученной выборке
                }

                reader.Close();
            }
            catch
            {
                return null;
                //MessageBox.Show("Ошибка при обработке запроса к базе данных!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            sqlConnection.Close(); // закрываем соединение с БД
            return mass;
        }
    }
}
