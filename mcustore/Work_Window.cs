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

            dataGridView3.ColumnCount = 3;
            dataGridView3.Columns[0].Name = "Модель";
            dataGridView3.Columns[1].Name = "Количество";
            dataGridView3.Columns[2].Name = "Цена";
            if (DataBaseClass.IsConnect())
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
                checkListBox();
                dataGridFunction();
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
            for (int i = 0; i < dataOrder2.Count; i++)
            {
                 checkedListBox1.Items.Add(dataOrder2[i][0],false);
            }
        }
        List<List<string>> dataGrid3;
        private void dataGridFunction()
        {
            dataGrid3 = new List<List<string>>();
            for (int i=0;i< dataOrder2.Count; i++)
            {
                dataGrid3.Add(new List<string>());
                dataGrid3[i].Add("false");
                dataGrid3[i].Add(dataOrder2[i][0]);
                dataGrid3[i].Add("0");
            }
        }
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
                    dataGrid3[e.RowIndex][2] = dataGridView3.Rows[e.RowIndex].Cells[1].Value.ToString();
                    for (int i = 0; i < dataOrder2.Count; i++)
                    {
                        if (dataGridView3.Rows[e.RowIndex].Cells[0].Value == dataOrder2[i][0])
                        {
                            dataGridView3.Rows[e.RowIndex].Cells[2].Value = Convert.ToInt32(dataGrid3[e.RowIndex][2]) * Convert.ToDouble(dataOrder2[i][2]);
                            break;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Буквы нельзя вводить!");
                    dataGridView3.Rows[e.RowIndex].Cells[1].Value = temp;
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
            string[] mass = new string[dataGrid3.Count];
            int[] array = new int[dataGrid3.Count];
            for(int i=0;i< dataGrid3.Count;i++)
            {
                if(dataGrid3[i][0]=="true")
                {
                    mass[i] = dataGrid3[i][1];
                    array[i] = Convert.ToInt32(dataGrid3[i][2]);
                }
            }
            DataBaseClass.CreateNewOrder(textBox1.Text, mass.ToList(), array.ToList());
        }
    }
}
