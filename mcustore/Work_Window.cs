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
    public partial class Work_Window : Form
    {
        public Work_Window()
        {
            InitializeComponent();
        }
        List<List<string>> dataOrder;//массив с данными запроса к первой таблице
        List<List<string>> dataOrder2;//массив с данными запроса к второй таблице
        List<List<string>> dataGrid3;//массив с данными третий таблицы
        /// <summary>
        /// закгрузка формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Work_Window_Load(object sender, EventArgs e)
        {
            datagGridCreate(sender, e);
        }
        /// <summary>
        /// Функция заполнения dataGridView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void datagGridCreate(object sender, EventArgs e)
        {
            Work_Window_Resize(sender, e);
            dataGridView1.ColumnCount = 5;
            dataGridView1.Columns[0].Name = "Номер заказа";
            dataGridView1.Columns[1].Name = "Компания";
            dataGridView1.Columns[2].Name = "Модели микроконтроллеров (шт.)";
            dataGridView1.Columns[3].Name = "Цена ($)";
            dataGridView1.Columns[4].Name = "Дата";

            dataGridView2.ColumnCount = 3;
            dataGridView2.Columns[0].Name = "Модель микроконтроллера";
            dataGridView2.Columns[1].Name = "Количество";
            dataGridView2.Columns[2].Name = "Цена за шт./$";

            dataGridView3.ColumnCount = 3;
            dataGridView3.Columns[0].Name = "Модель";
            dataGridView3.Columns[1].Name = "Количество";
            dataGridView3.Columns[2].Name = "Цена ($)";
            if (DataBaseClass.IsConnect())
            {
                dataOrder = DataBaseClass.SelectOrdersData();
                dataGridView1.Rows.Clear();
                dataGridView2.Rows.Clear();
                comboBox1.Items.Clear();
                comboBox2.Items.Clear();
                for (int i = 0; i < dataOrder.Count; i++)
                {
                    dataGridView1.Rows.Add();
                    for (int j = 0; j < dataOrder[i].Count; j++)
                    {
                        dataGridView1.Rows[i].Cells[j].Value = dataOrder[i][j];
                    }
                }
                dataOrder2 = DataBaseClass.SelectMicrocontrollersData();
                for (int i = 0; i < dataOrder2.Count; i++)
                {
                    dataGridView2.Rows.Add();
                    for (int j = 0; j < dataOrder2[i].Count; j++)
                    {
                        dataGridView2.Rows[i].Cells[j].Value = dataOrder2[i][j];

                    }
                    comboBox1.Items.Add(dataOrder2[i][0]);
                    comboBox2.Items.Add(dataOrder2[i][0]);
                }
                checkedListBox1.Items.Clear();
                checkListBox();
                dataGridFunction();
            }
            else
            {
                MessageBox.Show("Ошибка подключения к базе данных!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// изменение размеров формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Work_Window_Resize(object sender, EventArgs e)
        {
            tabControl1.Width = this.Width;
            tabControl1.Height = this.Height;
            dataGridView1.Width = dataGridView2.Width = tabControl1.Width - 20;
            dataGridView1.Height = dataGridView2.Height = tabControl1.Height / 3;
            groupBox1.Width = groupBox2.Width = tabControl1.Width / 2 - 16;
            groupBox1.Height = groupBox2.Height = tabControl1.Height - tabControl1.Height/3 - 105;
            groupBox1.Location = new Point(2, tabControl1.Height / 3 + 15);
            groupBox2.Location = new Point(groupBox1.Width + 6, tabControl1.Height / 3 + 15);
            int heig_margin_tabpage1 = groupBox1.Height/10;
            int width_margin_tabpage1 = 8;
            int width_padding_tabpage1 = 30;
            textBox1.Width = groupBox1.Width - label1.Width - width_margin_tabpage1- width_padding_tabpage1*2;
            label1.Location = new Point(dataGridView1.Location.X+ width_margin_tabpage1, dataGridView1.Location.Y + heig_margin_tabpage1);
            label1.Location = new Point(dataGridView1.Location.X+ width_margin_tabpage1, dataGridView1.Location.Y + heig_margin_tabpage1);
            textBox1.Location = new Point(dataGridView1.Location.X + width_padding_tabpage1*2 + label1.Width+width_margin_tabpage1, dataGridView1.Location.Y + heig_margin_tabpage1-textBox1.Height/3);
            label2.Location = new Point(label1.Location.X, label1.Location.Y + heig_margin_tabpage1);
            dataGridView3.Height = checkedListBox1.Height;
            checkedListBox1.Width =groupBox1.Width / 3;
            dataGridView3.Width = groupBox1.Width - checkedListBox1.Width  -width_margin_tabpage1 - width_padding_tabpage1 ;
            checkedListBox1.Location = new Point(label1.Location.X, label2.Location.Y + heig_margin_tabpage1);
            dataGridView3.Location = new Point(checkedListBox1.Location.X + width_padding_tabpage1  +  checkedListBox1.Width, checkedListBox1.Location.Y);
            label3.Location = new Point(label1.Location.X, checkedListBox1.Location.Y + heig_margin_tabpage1 + checkedListBox1.Height);
            button1.Location = new Point(label1.Location.X, label3.Location.Y + heig_margin_tabpage1);
            button2.Location = new Point(button1.Location.X + groupBox1.Width - button2.Width - width_margin_tabpage1, label3.Location.Y + heig_margin_tabpage1);

            groupBox3.Width = groupBox4.Width = groupBox5.Width = groupBox5.Width = tabControl1.Width / 2 - 16;
            groupBox3.Height = groupBox4.Height = groupBox5.Height = groupBox5.Height = tabControl1.Height / 4;
            groupBox3.Location = new Point(2, tabControl1.Height / 3 + 15);
            groupBox4.Location = new Point(groupBox3.Width + 6, tabControl1.Height / 3 + 15);
            groupBox5.Location = new Point(2, tabControl1.Height / 3 + tabControl1.Height / 4 + 30);
            label4.Location = label1.Location;
            label5.Location = new Point(checkedListBox1.Location.X, label2.Location.Y + label5.Height/2);
            textBox2.Location = new Point(textBox1.Location.X + label4.Width, textBox1.Location.Y);
            numericUpDown1.Location = new Point(textBox2.Location.X, label2.Location.Y);
            comboBox1.Width =textBox2.Width = textBox1.Width - label4.Width;
            button3.Width = textBox2.Width;
            button3.Location = new Point(textBox2.Location.X, checkedListBox1.Location.Y);
            label6.Location = new Point(checkedListBox1.Location.X, checkedListBox1.Location.Y+label6.Height);
            textBox3.Width = label6.Width;
            textBox3.Height = label6.Height;
            textBox3.Location = new Point(button3.Location.X - textBox3.Width*2 + textBox3.Width/2, label6.Location.Y - label6.Height/3);
            label8.Location = new Point(label5.Location.X,label4.Location.Y);
            label7.Location = new Point(label6.Location.X, label5.Location.Y );
            textBox4.Width = textBox3.Width;
            numericUpDown2.Location = new Point(textBox3.Location.X, textBox2.Location.Y);
            textBox4.Location = new Point(textBox3.Location.X, numericUpDown1.Location.Y);
            button5.Width = button4.Width = comboBox1.Width;
            button4.Location = new Point(comboBox1.Location.X, button3.Location.Y);
            label9.Location = label1.Location;
            comboBox2.Width  = textBox2.Width;
            comboBox2.Location = textBox2.Location;
            button5.Location = new Point(comboBox1.Location.X, button3.Location.Y);
            button7.Location = new Point(checkedListBox1.Location.X, button3.Location.Y);
            label11.Location = label1.Location;
            label12.Location = checkedListBox1.Location;
            dateTimePicker1.Location = textBox1.Location;
            dateTimePicker2.Location = new Point(textBox1.Location.X, dataGridView3.Location.Y);
            dateTimePicker1.Width = dateTimePicker2.Width = textBox1.Width;
            button9.Width = button8.Width = button1.Width;
            button8.Location = button1.Location;
            button9.Location = button2.Location;
        }
        /// <summary>
        /// заполнение checkListBox1 данными из массива
        /// </summary>
        private void checkListBox()
        {
            for (int i = 0; i < dataOrder2.Count; i++)
            {
                 checkedListBox1.Items.Add(dataOrder2[i][0],false);
            }
        }
        /// <summary>
        /// заполнение данными из массива третьей таблицы
        /// </summary>
        private void dataGridFunction()
        {
            dataGrid3 = new List<List<string>>();
            for (int i=0;i< dataOrder2.Count; i++)
            {
                dataGrid3.Add(new List<string>());
                dataGrid3[i].Add("false");
                dataGrid3[i].Add(dataOrder2[i][0]);
                dataGrid3[i].Add("0");
                dataGrid3[i].Add(dataOrder2[i][2]);
            }
        }
        /// <summary>
        /// поиск выделенных элементов в checkListBox
        /// </summary>
        private void ItemCheck2()
        {
           for(int i=0;i<checkedListBox1.Items.Count;i++)
           {
                if(checkedListBox1.GetItemChecked(i) && dataGrid3[i][1]==checkedListBox1.GetItemText(checkedListBox1.Items[i]))
                {
                    dataGrid3[i][0] = "true";
                }
                else
                {
                    dataGrid3[i][0] = "false";
                    dataGrid3[i][1] = dataOrder2[i][0];
                    dataGrid3[i][2]="0";
                }
            }
        }
        
        private void CheckedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ItemCheck2();
            dataGridView3.Rows.Clear();
            int j = 0;
            for (int i = 0; i < dataGrid3.Count; i++)
            {
                if(dataGrid3[i][0]=="true")
                {
                    dataGridView3.Rows.Add();
                    dataGridView3.Rows[j].Cells[0].Value = dataGrid3[i][1];
                    dataGridView3.Rows[j].Cells[1].Value = dataGrid3[i][2];
                    j++;
                }
            }
            price();
        }
        /// <summary>
        /// подсчет общей стоимости заказа
        /// </summary>
        private void price()
        {
            int price = 0;
            for (int i = 0; i < dataGridView3.Rows.Count; i++)
            {
                price += Convert.ToInt32(dataGridView3.Rows[i].Cells[2].Value);
            }
            label3.Text = "Стоимость заказа " + price + "$";
        }
        private void DataGridView3_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                int j = 0;
                for (int i = 0; i < dataOrder2.Count; i++)
                {
                    if(dataGrid3[i][0]=="true")
                    {
                        if(j==e.RowIndex)
                        {
                            dataGridView3.Rows[e.RowIndex].Cells[0].Value = dataGrid3[i][1];
                        }
                        j++;
                    }
                }
            }
            if (e.ColumnIndex == 1)
            {
                int temp = 0;
                try
                {
                    temp = Convert.ToInt32(dataGridView3.Rows[e.RowIndex].Cells[1].Value);
                    for(int i=0;i<dataGrid3.Count;i++)
                    {
                        if(dataGrid3[i][1]== dataGridView3.Rows[e.RowIndex].Cells[0].Value.ToString())
                        {
                            if (temp > Convert.ToInt32(dataOrder2[i][1]))
                            {
                                MessageBox.Show("Такого количество микроконтроллеров модели " + dataGridView3.Rows[e.RowIndex].Cells[0].Value.ToString() + " нет");
                                temp = 0;
                            }
                            else
                            {
                                dataGrid3[i][2] = temp.ToString();
                                dataGridView3.Rows[e.RowIndex].Cells[2].Value = Convert.ToDouble(dataGrid3[i][3]) * Convert.ToInt32(dataGrid3[i][2]);
                            }
                        }
                    }
                    if(temp==0)
                    {
                        for (int i = 0; i < dataGrid3.Count; i++)
                        {
                            if (dataGrid3[i][1] == dataGridView3.Rows[e.RowIndex].Cells[0].Value.ToString())
                            {
                                dataGrid3[i][2] = "1";
                                dataGridView3.Rows[e.RowIndex].Cells[1].Value = 1;
                            }
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Буквы нельзя вводить!");
                    for (int i = 0; i < dataGrid3.Count; i++)
                    {
                        if (dataGrid3[i][1] == dataGridView3.Rows[e.RowIndex].Cells[0].Value.ToString())
                        {
                            dataGridView3.Rows[e.RowIndex].Cells[1].Value = dataGrid3[i][2];
                        }
                    }
                }
                price();
            }
            if (e.ColumnIndex == 2)
            {
                try
                {
                    Convert.ToInt32(dataGridView3.Rows[e.RowIndex].Cells[2].Value);
                    for (int i = 0; i < dataOrder2.Count; i++)
                    {
                        if (dataGridView3.Rows[e.RowIndex].Cells[0].Value == dataOrder2[i][0])
                        {
                            dataGridView3.Rows[e.RowIndex].Cells[2].Value = Convert.ToInt32(dataGrid3[i][2]) * Convert.ToDouble(dataOrder2[i][2]);
                            break;
                        }
                    }
                }
                catch
                {
                    for (int i = 0; i < dataOrder2.Count; i++)
                    {
                        if (dataGridView3.Rows[e.RowIndex].Cells[0].Value == dataOrder2[i][0])
                        {
                            dataGridView3.Rows[e.RowIndex].Cells[2].Value = Convert.ToInt32(dataGrid3[i][2]) * Convert.ToDouble(dataOrder2[i][2]);
                            break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// обработка кликов по разным кнопкам на форме
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
            }
            dataGridView3.Rows.Clear();
            label3.Text = "";
            dataGrid3.Clear();
            dataGridFunction();
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            if(textBox1.Text!="" && label3.Text!="" && label3.Text!="Стоимость заказа 0$")
            {
                string[] mass = new string[dataGrid3.Count];
                int[] array = new int[dataGrid3.Count];
                for (int i = 0; i < dataGrid3.Count; i++)
                {
                    if (dataGrid3[i][0] == "true")
                    {
                        mass[i] = dataGrid3[i][1];
                        array[i] = Convert.ToInt32(dataGrid3[i][2]);
                    }
                }
                DataBaseClass.CreateNewOrder(textBox1.Text, mass.ToList(), array.ToList());
                Work_Window_Load(sender, e);
                Button1_Click(sender, e);
            }
            else
            {
                if(textBox1.Text=="")
                {
                    MessageBox.Show("Укажите предприятие!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Пустой заказ!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void Button3_Click(object sender, EventArgs e)
        {
            if(textBox2.Text!=""&&textBox3.Text!="")
            {
                DataBaseClass.CreateNewMicrocontroller(textBox2.Text, Convert.ToInt32(numericUpDown1.Value), Convert.ToDouble(textBox3.Text));
                Work_Window_Load(sender, e);
                textBox2.Clear();
                textBox3.Clear();
            }
            else
            {
                MessageBox.Show("Не все данные указаны!", "Данные!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "" && textBox4.Text != "")
            {
                DataBaseClass.EditMicrocontroller(comboBox1.Text, comboBox1.Text, Convert.ToInt32(numericUpDown2.Value), Convert.ToDouble(textBox4.Text));
                Work_Window_Load(sender, e);
            }
            else
            {
                MessageBox.Show("Не все данные указаны!", "Данные!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Button5_Click(object sender, EventArgs e)
        {
            if (comboBox2.Text != "" )
            {
                List<List<string>> dataOrder3;
                dataGridView2.ColumnCount = 3;
                dataGridView2.Columns[0].Name = "Модель микроконтроллера";
                dataGridView2.Columns[1].Name = "Количество";
                dataGridView2.Columns[2].Name = "Цена за шт./$";
                dataGridView2.Rows.Clear();
                dataOrder3 = DataBaseClass.SelectMicrocontrollersData(comboBox2.Text);
                for (int i = 0; i < dataOrder3.Count; i++)
                {
                    dataGridView2.Rows.Add();
                    for (int j = 0; j < dataOrder3[i].Count; j++)
                    {
                        dataGridView2.Rows[i].Cells[j].Value = dataOrder3[i][j];
                    }
                }
            }
            else
            {
                MessageBox.Show("Не все данные указаны!", "Данные!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Button7_Click(object sender, EventArgs e)
        {
            Work_Window_Load(sender, e);
        }
        private void Button8_Click(object sender, EventArgs e)
        {
            Work_Window_Load(sender, e);
            dateTimePicker2.Value = DateTime.Now;
        }
        private void Button9_Click(object sender, EventArgs e)
        {
            string data1 = Convert.ToString(dateTimePicker1.Value.Year) + "-" + Convert.ToString(dateTimePicker1.Value.Month) + "-" + Convert.ToString(dateTimePicker1.Value.Day);
            string data2 = Convert.ToString(dateTimePicker2.Value.Year) + "-" + Convert.ToString(dateTimePicker2.Value.Month) + "-" + Convert.ToString(dateTimePicker2.Value.Day);

            List<List<string>> dataOrder3;
            dataGridView1.ColumnCount = 5;
            dataGridView1.Columns[0].Name = "Номер заказа";
            dataGridView1.Columns[1].Name = "Компания";
            dataGridView1.Columns[2].Name = "Модели микроконтроллеров (шт.)";
            dataGridView1.Columns[3].Name = "Цена ($)";
            dataGridView1.Columns[4].Name = "Дата";
            dataGridView1.Rows.Clear();
            dataOrder3 = DataBaseClass.SelectOrdersData(data1, data2);
            for (int i = 0; i < dataOrder3.Count; i++)
            {
                dataGridView1.Rows.Add();
                for (int j = 0; j < dataOrder3[i].Count; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = dataOrder3[i][j];
                }
            }

        }
        /// <summary>
        /// первая закгрузка формы и проверка соединения с БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Work_Window_Shown(object sender, EventArgs e)
        {
            Work_Window_Resize(sender, e);
            if (DataBaseClass.IsConnect())
            {
                MessageBox.Show("Соединение c БД установлено.", "Соединение!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Ошибка подключения к базе данных!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// запрет на ввод не подходящих символов для textBox(2,3)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsNumber(e.KeyChar) || e.KeyChar == 8 || e.KeyChar == ',')
            {
                return;
            }
            e.Handled = true;
        }
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<List<string>> dataOrder3;
            dataOrder3 = DataBaseClass.SelectMicrocontrollersData(comboBox1.Text);
            numericUpDown2.Value = Convert.ToDecimal(dataOrder3[0][1]);
            textBox4.Text = dataOrder3[0][2];
            dataOrder3.Clear();
        }

        
    }
}
