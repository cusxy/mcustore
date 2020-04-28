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
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }
        bool login = false;
        /// <summary>
        /// Определяет позиции кнопок и других элементов формы
        /// </summary>
        private void LocationElement()
        {
            button1.Location = new Point(0, this.ClientSize.Height - button1.Height);
            button2.Location = new Point(this.ClientSize.Width - button2.Width, this.ClientSize.Height - button2.Height);
            if(login)
            {
                label1.Location = new Point(this.ClientSize.Width / 4, this.ClientSize.Height / 2);
                label2.Location = new Point(this.ClientSize.Width / 4, this.ClientSize.Height / 2 + 60);
                label3.Location = new Point(this.ClientSize.Width / 4, this.ClientSize.Height / 2-60);
                label4.Location = new Point(this.ClientSize.Width / 4, this.ClientSize.Height / 2 + 120);
                textBox1.Width = textBox2.Width =textBox3.Width=textBox4.Width =label1.Width*2;
                textBox3.Location = new Point(this.ClientSize.Width / 2, this.ClientSize.Height / 2 - 60);
                textBox4.Location = new Point(this.ClientSize.Width / 2, this.ClientSize.Height / 2 + 120);
                label3.Visible = textBox3.Visible = textBox4.Visible=label4.Visible= true;
            }
            else
            {
                label3.Visible = textBox3.Visible = textBox4.Visible = label4.Visible = false;
                textBox1.Width = textBox2.Width = label1.Width * 3;
                label1.Location = new Point(this.ClientSize.Width / 3, this.ClientSize.Height / 2);
                label2.Location = new Point(this.ClientSize.Width / 3, this.ClientSize.Height / 2 + 60);
            }
            textBox1.Location = new Point(this.ClientSize.Width / 2, this.ClientSize.Height / 2);
            textBox2.Location = new Point(this.ClientSize.Width / 2, this.ClientSize.Height / 2 + 60);
        }
        /// <summary>
        /// Цвет при наведение на кнопку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonLigthMove(object sender, MouseEventArgs e)
        {
            Button t = sender as Button;
            t.BackColor = Color.FromArgb(255, 255, 255);
            t.ForeColor = Color.FromArgb(0, 0, 128);
        }
        /// <summary>
        /// Цвет при ухождение с кнопки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonLigthLeave(object sender, EventArgs e)
        {
            Button t = sender as Button;
            t.BackColor = Color.FromArgb(0, 0, 128);
            t.ForeColor = Color.FromArgb(255, 255, 255);
        }
        /// <summary>
        /// Установка размера формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Load(object sender, EventArgs e)
        {
            this.Width = 1200;
            this.Height = 600;
            LocationElement();
        }
        /// <summary>
        /// Выполняет проверку кода активации
        /// </summary>
        /// <param name="pas"></param>
        /// <returns></returns>
        private bool ActivationCodeCheck(string pas)
        {
            if (DataBaseClass.GetMD5FromString(pas) == "b12be09df7b5175ca791f3bcad45f513")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Отправляет запрос на создание учетной записи
        /// </summary>
        /// <returns></returns>
        private bool CreateAccountRequestRequest()
        {
            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "")
            {
                string user_name = textBox3.Text;
                string loginname = textBox1.Text;
                string password = textBox2.Text;
                if (DataBaseClass.AddNewManager(loginname, user_name, password) == 1 && ActivationCodeCheck(textBox4.Text))
                {
                    MessageBox.Show("Регистрация успешна!", "Регистрация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                else
                {
                    MessageBox.Show("Придумайте новый логин, такой уже есть!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox4.Clear();
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Не все поля заполнены!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        /// <summary>
        /// Отправляет запрос на проверку логина и пароля
        /// </summary>
        /// <returns></returns>
        private bool AccountLogin()
        {
            if(textBox1.Text!="" && textBox2.Text!="")
            {
                string loginname = textBox1.Text;
                string password = textBox2.Text;
                if (DataBaseClass.CheckPassword(loginname, password) == 1)
                {
                    MessageBox.Show("Вход выполнен!", "Вход", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                else
                {
                    MessageBox.Show("Неверный пароль или логин!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Clear();
                    textBox2.Clear();
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Не все поля заполнены!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            
            if(login)
            {
                if(CreateAccountRequestRequest())
                {
                    Work_Window form = new Work_Window();
                    form.Owner = this;
                    this.Hide();
                    form.ShowDialog();
                    this.Close();
                }
            }
            else
            {
                if (AccountLogin())
                {
                    Work_Window form = new Work_Window();
                    form.Owner = this;
                    this.Hide();
                    form.ShowDialog();
                    this.Close();
                }            }
        }
        /// <summary>
        /// Меняет названия кнопок и отображение полей, в завимости вход это или регистрация
        /// </summary>
        private void Checkingtheloginformmode()
        {
            if(login)
            {
                button1.Text = "Отмена";
                button2.Text = "Создать и войти";
                label1.Text = "Ваш логин";
                label2.Text = "Ваш пароль";
                LocationElement();
            }
            else
            {
                button1.Text = "Регистрация";
                button2.Text = "Войти";
                label1.Text = "Логин";
                label2.Text = "Пароль";
                LocationElement();
            }
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            if(button1.Text =="Отмена")
            {
                login = false;
                textBox1.Clear();
                textBox2.Clear();
                textBox4.Clear();
                textBox3.Clear();
            }
            else
            {
                login = true;
                textBox1.Clear();
                textBox2.Clear();
            }
            Checkingtheloginformmode();
        }
    }
}
