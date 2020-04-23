using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mcustore
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            password_tb.UseSystemPasswordChar = true;
            user_tb.Select();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            bool is_password_right = false; // верный ли пароль
            if (user_tb.Text=="admin") // если вход через учётную запись бухгалтера
            {
                    is_password_right = true;
            }
            else if (user_tb.Text == "user") // если вход через учётную запись администратора
            {
                if (password_tb.Text == "123456789") // если введённый пароль - верный
                {
                    is_password_right = true;
                }
            }
            else if(user_tb.Text == "developer")
            {
                if (password_tb.Text == "987654321") // если введённый пароль - верный
                {
                    is_password_right = true;
                }
            }

            if (is_password_right) // если введённый пароль - верный
            {
                this.Close();//закрываем текущую форму
                Work_Window WorkForm = new Work_Window(); // создаём главную форму
                WorkForm.Owner = this; // устанавливаем владельца главной формы - эту форму
                //WorkForm.IsAdmin = radioButton_administrator.Checked; // отправляем в главную форму информацию о том, под какой учётной записью выполнен вход
                
                Hide(); // скрываем текущую форму
                WorkForm.Show(); // показываем главную форму
            }
            else // если введённый пароль - неверный
            {
                //textBox_password.Clear(); // очищаем поле для ввода пароля
                MessageBox.Show("Неверный пароль или логин!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error); // показываем сообщение об ошибке
            }
        }
    }
}
