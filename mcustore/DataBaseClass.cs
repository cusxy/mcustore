using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace mcustore
{
    /// <summary>Класс для работы с БД</summary>
    abstract public class DataBaseClass
    {        
        /// <summary>Строчка подключения к БД</summary>
        private static readonly string m_connection_string = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\mcustoredatabase.mdf;Integrated Security=True";

        /// <summary>Проверяет подключение к БД</summary>
        /// <param name="connection_string">Строчка подключения к БД</param>
        public static bool IsConnect()
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
        private static List<List<string>> ReadDataToMass(string query)
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
        /// <returns>Двумерный список данных для таблицы в порядке: ID заказа - Название компании - Микроконтроллеры - Общая цена - Дата заказа. Возвращает null в случае возникновения ошибки.</returns>
        public static List<List<string>> SelectOrdersData(string date_from = "2000-12-31", string date_to = "2100-12-31")
        {
            // ID заказа - Название компании - Микроконтроллеры - Общая цена - Дата заказа

            string sql = "SELECT Order_id, Company_name, Datetime FROM Orders WHERE (Datetime >= '" + date_from + "') AND (Datetime <= '" + date_to + "');";
            List<List<string>> orders_list = ReadDataToMass(sql);
            if (orders_list == null) return null; // в случае ошибки

            List<List<string>> result = new List<List<string>>();
            int all_price = 0;

            for (int i = 0; i < orders_list.Count; i++) // для каждого заказа
            {
                result[i] = new List<string>();
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
        /// <returns>Двумерный список данных для таблицы в порядке: Название микроконтроллера - Количество - Цена за штуку. Возвращает null в случае возникновения ошибки.
        public static List<List<string>> SelectMicrocontrollersData(string name_contains = "")
        {
            // Название микроконтроллера - Количество - Цена за штуку

            string sql = "SELECT Microcontroller_name, Quantity, Price FROM Microcontrollers WHERE (Microcontroller_name LIKE N'%" + name_contains + "%');";

            return ReadDataToMass(sql);
        }

        /// <summary>Выполняет запрос без возврата значения и без изменения чего-либо на форме</summary>
        /// <param name="query">Запрос</param>
        /// <returns>1 - в случае успешного запроса, 0 - в случае, если запрос не удалось выполнить, -1 - в случае ошибки</returns>
        private static int GoQuery(string query)
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

        /// <summary>Создаём новый микроконтроллер</summary>
        /// <param name="name">Название микроконтроллера</param>
        /// <param name="quantity">Количество на складе</param>
        /// <param name="price">Цена</param>
        /// <returns>1 - в случае успешного запроса, 0 - в случае, если запрос не удалось выполнить, -1 - в случае ошибки</returns>
        public static int CreateNewMicrocontroller(string name, int quantity, int price)
        {
            string sql = "INSERT INTO Microcontrollers (Microcontroller_name, Quantity, Price) VALUES (N'" + name + "', " + quantity + ", " + price + ")";
            return GoQuery(sql);
        }

        /// <summary>Добавляет указанное количество выбранного микроконтроллера на склад</summary>
        /// <param name="name">Название микроконтроллера (должно быть в БД)</param>
        /// <param name="plus">Количество, которое прибавить</param>
        /// <returns>1 - в случае успешного добавления, 0 - в случае, если запрос не удалось выполнить, -1 - в случае ошибки</returns>
        public static int PlusQuantity(string name, int plus)
        {
            string sql = "SELECT Quantity FROM Microcontrollers WHERE Microcontroller_name = N'" + name + "';";
            List<List<string>> info = ReadDataToMass(sql);
            if (info == null) return -1; // в случае ошибки

            int total_quantity = Convert.ToInt32(info[0][0]);
            total_quantity += plus;

            string sql2 = "UPDATE Microcontrollers SET Quantity = " + total_quantity + ";";
            return GoQuery(sql2);
        }

        /// <summary>Изменяет информацию об указанном микроконтроллере</summary>
        /// <param name="name">Название микроконтроллера</param>
        /// <param name="new_name">Новое название</param>
        /// <param name="new_quantity">Новое количество на складе</param>
        /// <param name="new_price">Новая цена за штуку</param>
        /// <returns>1 - в случае успешного изменения, 0 - в случае, если запрос не удалось выполнить, -1 - в случае ошибки</returns>
        public static int EditMicrocontroller(string name, string new_name, int new_quantity, int new_price)
        {
            string sql = "SELECT Quantity FROM Microcontrollers WHERE Microcontroller_name = N'" + name + "';";
            List<List<string>> info = ReadDataToMass(sql);
            if (info == null) return -1; // в случае ошибки

            string sql2 = "UPDATE Microcontrollers SET Microcontroller_name = N'" + new_name + "', Quantity = " + new_quantity + ", Price = " + new_price + ";";
            return GoQuery(sql2);
        }

        /// <summary>Удаляет микроконтроллер</summary>
        /// <param name="name">Название микроконтроллера</param>
        /// <returns>1 - в случае успешного удаления, 0 - в случае, если запрос не удалось выполнить (микроконтроллер не найден или др. причина), -1 - в случае ошибки</returns>
        public static int DeleteMicrocontroller(string name)
        {
            string sql = "SELECT * FROM Microcontrollers WHERE Microcontroller_name = N'" + name + "';";
            List<List<string>> info = ReadDataToMass(sql);
            if (info == null) return -1; // в случае ошибки
            if (info.Count == 0) return 0; // в случае отсутствия такого микроконтроллера

            string sql2 = "DELETE FROM Microcontrollers WHERE Microcontroller_name = N'" + name + "';";
            return GoQuery(sql2);
        }

        /// <summary>Создаём новый заказ</summary>
        /// <param name="company_name">Название компании</param>
        /// <param name="microcontrollers_names">Одномерный список названий заказываемых микроконтроллеров</param>
        /// <param name="microcontrollers_quantities">Одномерный список соответствующего количества заказываемых микроконтроллеров</param>
        /// <returns>1 - в случае успешного создания, 0 - в случае, если запрос не удалось выполнить, -1 - в случае ошибки</returns>
        public static int CreateNewOrder(string company_name, List<string> microcontrollers_names, List<int> microcontrollers_quantities)
        {
            string sql = "INSERT INTO Orders (Company_name, Datetime) VALUES (N'" + company_name + "', CONVERT (date, GETDATE()));";
            int result = GoQuery(sql);
            if (result != 1) return result;

            string sql2 = "SELECT Order_id FROM Orders WHERE Company_name = N'" + company_name + "';";
            List<List<string>> info = ReadDataToMass(sql2);
            if (info == null) return -1; // в случае ошибки
            if (info.Count == 0) return 0; // в случае отсутствия такого микроконтроллера
            int order_id = Convert.ToInt32(info[0][0]);

            for (int i = 0; i < microcontrollers_names.Count; i++) {
                string sql3 = "SELECT Microcontroller_id FROM Microcontrollers WHERE Microcontroller_name = N'" + microcontrollers_names[i] + "';";
                List<List<string>> info_mc_id = ReadDataToMass(sql3);
                if (info_mc_id == null) return -1; // в случае ошибки
                if (info_mc_id.Count == 0) return 0; // в случае отсутствия такого микроконтроллера

                int microcontroller_id = Convert.ToInt32(info_mc_id[0][0]);

                string sql4 = "INSERT INTO order_microcontroller (Order_id, Microcontroller_id, Quantity) VALUES (" + order_id + ", " + microcontroller_id + ", " + microcontrollers_quantities[i] + ");";
                result = GoQuery(sql4);
                if (result != 1) return result;
            }

            return 1;
        }

        /// <summary>Возвращает одномерный список, содержащий названия всех созданных микроконтроллеров в таблице Microcontrollers</summary>
        /// <returns>Одномерный список</returns>
        public static List<string> GetAllMicrocontrollersNames() {
            string sql = "SELECT Microcontroller_name FROM Microcontrollers";
            List<List<string>> info = ReadDataToMass(sql);
            if (info == null) return null; // в случае ошибки

            List<string> result = new List<string>();
            for (int i = 0; i < info.Count; i++) {
                result.Add(info[i][0]);
            }

            return result;
        }
    }
}
