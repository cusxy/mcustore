using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

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

            string sql = "SELECT Order_id, Company_name, Datetime FROM Orders WHERE (Datetime >= '" + date_from + "') AND (Datetime <= '" + date_to + " 23:59:59');";
            List<List<string>> orders_list = ReadDataToMass(sql);
            if (orders_list == null) return null; // в случае ошибки

            List<List<string>> result = new List<List<string>>();

            for (int i = 0; i < orders_list.Count; i++) // для каждого заказа
            {
                double all_price = 0;
                result.Add(new List<string>());
                result[i].Add(orders_list[i][0]); // id заказа 
                result[i].Add(orders_list[i][1]); // название компании
                string microcontrollers_info = "";

                int order_id = Convert.ToInt32(orders_list[i][0]);
                string sql2 = "SELECT Microcontrollers.Microcontroller_name, order_microcontroller.Quantity, Microcontrollers.Price FROM Microcontrollers, order_microcontroller WHERE (Microcontrollers.Microcontroller_id = order_microcontroller.Microcontroller_id) AND (order_microcontroller.Order_id = " + order_id + ");";
                List<List<string>> microcontroller_info_list = ReadDataToMass(sql2);
                double price = 0;
                for (int i2 = 0; i2 < microcontroller_info_list.Count; i2++) {
                    if (i2 != 0) {
                        microcontrollers_info += "\n";
                    }
                    microcontrollers_info += microcontroller_info_list[i2][0] + " (" + microcontroller_info_list[i2][1] + " шт.)";
                    price += Convert.ToDouble(microcontroller_info_list[i2][2]) * Convert.ToInt32(microcontroller_info_list[i2][1]); // количество микроконтроллеров умножаем на цену
                }
                all_price += price;
                result[i].Add(microcontrollers_info); // информация о микроконтроллерах и их количестве
                result[i].Add(all_price.ToString()); // общая стоимость
                result[i].Add(orders_list[i][2]); // название компании
            }

            return result;
        }

        /// <summary>Выбирает данные из таблицы Microcontrollers о микроконтроллерах с названием, содержащим передаваемую строчку</summary>
        /// <param name="name_contains">Строчка, которую должен содержать микроконтроллер</param>
        /// <returns>Двумерный список данных для таблицы в порядке: Название микроконтроллера - Количество - Цена за штуку. Возвращает null в случае возникновения ошибки.</returns>
        public static List<List<string>> SelectMicrocontrollersData(string name_contains = "")
        {
            string sql = "SELECT * FROM dbo.GetMicrocontrollers(N'" + name_contains + "');";
            return ReadDataToMass(sql);
        }

        /// <summary>Выполняет запрос без возврата значения и без изменения чего-либо на форме</summary>
        /// <param name="query">Запрос</param>
        /// <returns>1 - в случае успешного запроса, 0 - в случае, если запрос был отклонён БД, -1 - в случае ошибки</returns>
        private static int GoQuery(string query)
        {
            SqlConnection sqlConnection = new SqlConnection(m_connection_string); // инициализируем соединение с БД
            try
            {
                sqlConnection.Open(); // открываем соединение с БД

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection); // создание SQL команды с указанным запросом
                SqlDataReader reader = sqlCommand.ExecuteReader(); // выполнение SQL команды
                if (reader.RecordsAffected != 0)
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

        /// <summary>Создаёт новый микроконтроллер</summary>
        /// <param name="name">Название микроконтроллера</param>
        /// <param name="quantity">Количество на складе</param>
        /// <param name="price">Цена</param>
        /// <returns>1 - в случае успешного запроса, 0 - в случае, если запрос не удалось выполнить, -1 - в случае ошибки</returns>
        public static int CreateNewMicrocontroller(string name, int quantity, double price)
        {
            string sql = "EXECUTE AddNewMicrocontroller N'" + name + "', " + quantity + ", " + price.ToString(System.Globalization.CultureInfo.InvariantCulture) + ";"; // выполнение хранимой процедуры AddNewMicrocontroller
            return GoQuery(sql);
        }

        /// <summary>Возвращает ID микроконтроллера по его названию</summary>
        /// <param name="name">Название микроконтроллера</param>
        /// <returns>ID микроконтроллера (возвращает -1, если возникла ошибка, возвращает -2, если указанный МК не найден)</returns>
        private static int GetMicrocontrollerIdFromName(string name) {
            string sql = "SELECT Microcontroller_id FROM Microcontrollers WHERE Microcontroller_name = N'" + name + "';";
            List<List<string>> info = ReadDataToMass(sql);
            if (info.Count == 0) return -2; // если МК не найден 
            if (info == null) return -1; // в случае ошибки
            return Convert.ToInt32(info[0][0]);
        }

        /// <summary>Изменяет информацию об указанном микроконтроллере</summary>
        /// <param name="name">Название микроконтроллера</param>
        /// <param name="new_name">Новое название</param>
        /// <param name="new_quantity">Новое количество на складе</param>
        /// <param name="new_price">Новая цена за штуку</param>
        /// <returns>1 - в случае успешного изменения, 0 - в случае, если запрос не удалось выполнить, -1 - в случае ошибки, -2 - если МК не найден</returns>
        public static int EditMicrocontroller(string name, string new_name, int new_quantity, double new_price)
        {
            int mc_id = GetMicrocontrollerIdFromName(name);
            if (mc_id < 0) return mc_id; // в случае ошибки или отсутствия такого микроконтроллера

            string sql = "UPDATE Microcontrollers SET Microcontroller_name = N'" + new_name + "', Quantity = " + new_quantity + ", Price = " + new_price.ToString(System.Globalization.CultureInfo.InvariantCulture) + " WHERE Microcontroller_id = " + mc_id + ";";
            return GoQuery(sql);
        }

        /// <summary>Создаёт новый заказ</summary>
        /// <param name="company_name">Название компании</param>
        /// <param name="microcontrollers_names">Одномерный список названий заказываемых микроконтроллеров</param>
        /// <param name="microcontrollers_quantities">Одномерный список соответствующего количества заказываемых микроконтроллеров</param>
        /// <returns>1 - в случае успешного создания, 0 - в случае, если запрос отклонён БД, -1 - в случае ошибки</returns>
        public static int CreateNewOrder(string company_name, List<string> microcontrollers_names, List<int> microcontrollers_quantities, string manager_login)
        {
            string sql = "EXECUTE AddNewOrder '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', N'" + company_name + "', " + manager_login + ";"; // выполнение хранимой процедуры AddNewOrder
            int result = GoQuery(sql);
            if (result != 1) return result; // если возникла ошибка, либо запрос был отклонён БД

            string sql2 = "SELECT Order_id FROM Orders WHERE Company_name = N'" + company_name + "';";
            List<List<string>> info = ReadDataToMass(sql2);
            if (info == null) return -1; // в случае ошибки
            if (info.Count == 0) return 0; // в случае отсутствия такого микроконтроллера
            int order_id = Convert.ToInt32(info[info.Count - 1][0]);

            for (int i = 0; i < microcontrollers_names.Count; i++) {
                if (microcontrollers_names[i] != null)
                {
                    int microcontroller_id = GetMicrocontrollerIdFromName(microcontrollers_names[i]);

                    string sql4 = "EXECUTE AddMicrocontrollerToOrder " + order_id + ", " + microcontroller_id + ", " + microcontrollers_quantities[i].ToString(System.Globalization.CultureInfo.InvariantCulture) + ";"; // выполнение хранимой процедуры AddMicrocontrollerToOrder
                    result = GoQuery(sql4);
                    if (result != 1) return result;
                }
            }

            return 1;
        }

        /// <summary>Вызов SQL-функции, возвращающей значение типа int</summary>
        /// <param name="query">Запрос с вызовом функции</param>
        /// <returns>Возвращаемое значение функции</returns>
        private static int GetScalarIntVariableFromSQLFunction(string query)
        {
            SqlConnection sqlConnection = new SqlConnection(m_connection_string); // инициализируем соединение с БД
            try
            {
                sqlConnection.Open(); // открываем соединение с БД

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection); // создание SQL команды с указанным запросом

                SqlCommand cmd = new SqlCommand(query, sqlConnection);
                int variable = (int)cmd.ExecuteScalar();
                sqlConnection.Close(); // закрываем соединение с БД
                return variable;
            }
            catch
            {
                sqlConnection.Close(); // закрываем соединение с БД
                return -1;
            }
        }

        /// <summary>Вызов SQL-функции, возвращающей значение типа string</summary>
        /// <param name="query">Запрос с вызовом функции</param>
        /// <returns>Возвращаемое значение функции</returns>
        private static string GetScalarStringVariableFromSQLFunction(string query)
        {
            SqlConnection sqlConnection = new SqlConnection(m_connection_string); // инициализируем соединение с БД
            try
            {
                sqlConnection.Open(); // открываем соединение с БД

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection); // создание SQL команды с указанным запросом

                SqlCommand cmd = new SqlCommand(query, sqlConnection);
                string variable = (string)cmd.ExecuteScalar();
                sqlConnection.Close(); // закрываем соединение с БД
                return variable;
            }
            catch
            {
                sqlConnection.Close(); // закрываем соединение с БД
                return "";
            }
        }

        /// <summary>Возвращает строчку, зашифрованную через MD5</summary>
        /// <param name="text">Исходная строчка</param>
        /// <returns>Зашифрованная строчка (всегда 32 символа)</returns>
        public static string GetMD5FromString(string text)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(text));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            string result = sBuilder.ToString(); // зашифрованная строчка при помощи MD5
            return result;
        }

        /// <summary>Возвращает ID менеджера по его логину</summary>
        /// <param name="login">Логин менеджера</param>
        /// <returns>ID менеджера в БД (-1 в случае, если логин не найден)</returns>
        private static int GetManagerIdByLogin(string login)
        {
            string sql = "SELECT * FROM dbo.GetManagerIdByLogin(N'" + login + "');";
            return GetScalarIntVariableFromSQLFunction(sql);
        }

        /// <summary>Добавляет нового менеджера</summary>
        /// <param name="login">Логин менеджера</param>
        /// <param name="name">Имяменеджера </param>
        /// <param name="password">Пароль (незашифрованный) менеджера</param>
        /// <returns>1 - если запись успешно добавлена, 0 - если логин уже существует, -1 - в случае ошибки</returns>
        public static int AddNewManager(string login, string name, string password)
        {
            string sql = "EXECUTE dbo.AddNewManager(N'" + name + "', N'" + login + "', N'" + GetMD5FromString(password) + "');";
            return GoQuery(sql);
        }

        /// <summary>Проверка пароля менеджера по логину</summary>
        /// <param name="login">Логин менеджера</param>
        /// <param name="password">Пароль (незашифрованный) менеджера</param>
        /// <returns>1 - если пароль верен, 0 - если неверен, -1 - в случае ошибки</returns>
        public static int CheckPassword(string login, string password)
        {
            // поиск записи в бд
            string sql = "SELECT * FROM dbo.GetManagerIdByLogin(N'" + login + "');";
            int result = GoQuery(sql);
            if (result != 1) return result; // если запись не найдена или возникла ошибка

            // если запись найдена
            string sql2 = "SELECT * FROM dbo.GetManagerPasswordMD5ByLogin(N'" + login + "');";
            string password_md5_in_base = GetScalarStringVariableFromSQLFunction(sql2);
            string password_md5 = GetMD5FromString(password);
            if (password_md5_in_base == password_md5) return 1;
            else return 0;
        }
    }
}
