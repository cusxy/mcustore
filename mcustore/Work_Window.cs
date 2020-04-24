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
        List<List<string>> dataOrder;
        List<List<string>> dataOrder2;
        private void Work_Window_Load(object sender, EventArgs e)
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
            dataGridView2.Columns[2].Name = "Цена за шт.";
            if(DataBaseClass.IsConnect())
            {
                MessageBox.Show("Соединение c БД установлено.", "Соединение!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dataOrder = DataBaseClass.SelectOrdersData();
                for(int i = 0;i<dataOrder.Count;i++)
                {
                    dataGridView1.Rows.Add();
                    for (int j=0;j<dataOrder[i].Count;j++)
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
                }
            }
            else
            {
                MessageBox.Show("Ошибка подключения к базе данных!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Work_Window_Resize(object sender, EventArgs e)
        {
            tabControl1.Width = this.Width;
            tabControl1.Height = this.Height;
            dataGridView1.Width = dataGridView2.Width = tabControl1.Width - 20;
            dataGridView1.Height = dataGridView2.Height = tabControl1.Height / 3;
            groupBox1.Width = groupBox2.Width = tabControl1.Width / 2 - 16;
            groupBox1.Location = new Point(0, tabControl1.Height / 3);
            groupBox2.Location = new Point(groupBox1.Width+5, tabControl1.Height / 3);
            groupBox1.Height = groupBox2.Height = tabControl1.Height - tabControl1.Height/3;
            
        }

        private void checkListBox()
        {

        }
    }
}
