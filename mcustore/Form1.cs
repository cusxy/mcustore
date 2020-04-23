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


        private void Form1_Load(object sender, EventArgs e)
        {
            password_tb.UseSystemPasswordChar = true;
            user_tb.Select();
        }
        /// <summary>
        /// цвет при наведение на кнопку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonLigthMove(object sender, MouseEventArgs e)
        {
            Button t = sender as Button;
            t.BackColor = Color.FromArgb(255, 255, 255);
        }

        /// <summary>
        /// Цвет при ухождение с кнопки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonLigthLeave(object sender, EventArgs e)
        {
            Button t = sender as Button;
            t.BackColor = Color.FromArgb(37, 107, 219);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if(registr)
            {
                if(textBox1.Text == "AAAA-AAAA")
                {
                    Registr_User();
                }
                else
                {
                    textBox1.Clear();
                    MessageBox.Show("Неверный код активации!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error); // показываем сообщение об ошибке
                }
            }
            else
            {
                bool is_password_right = false; // верный ли пароль
                if (user_tb.Text == "admin") // если вход через учётную запись бухгалтера
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
                else if (user_tb.Text == "developer")
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
                    password_tb.Clear(); // очищаем поле для ввода пароля
                    MessageBox.Show("Неверный пароль или логин!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error); // показываем сообщение об ошибке
                }
            }
        }
        bool registr = false;
        private void Button2_Click(object sender, EventArgs e)
        {
            if(button2.Text == "Регистрация")
            {
                button1.Text = "Зарегистрировать";
                button2.Text = "Отмена";
                registr = true;
                label1.Text = "РЕГИСТРАЦИЯ";
                label4.Visible = textBox1.Visible = true;
            }
            else
            {
                button1.Text = "Войти";
                button2.Text = "Регистрация";
                registr = false;
                label1.Text = "ДОБРО ПОЖАЛОВАТЬ";
                label4.Visible = textBox1.Visible = false;
            }
        }

        private void Label1_Resize(object sender, EventArgs e)
        {
            label1.Location = new Point(this.Width / 2 - label1.Width / 2, 9);
        }

        private void Registr_User()
        {

        }
    }
}
