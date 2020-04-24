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
        //private int[] ItemCheck1()
        //{
        //    int i = 0;
        //    foreach (int indexChecked in checkedListBox1.CheckedIndices)
        //    {
        //        i++;
        //    }
        //    int[] mass = new int[i];
        //    i = 0;
        //    foreach (object itemChecked in checkedListBox1.CheckedItems)
        //    {
        //        mass[i] = Convert.ToInt32(itemChecked);
        //        i++;
        //    }
        //    return mass;
        //}
        int[] array;
        string[] mass;
        private string[] ItemCheck2()
        {
            if(mass==null && array==null)
            {
                int i = 0;
                foreach (int indexChecked in checkedListBox1.CheckedIndices)
                {
                    i++;
                }
                mass = new string[i];
                array = new int[i];
                i = 0;
                foreach (object itemChecked in checkedListBox1.CheckedItems)
                {
                    array[i] = 0;
                    mass[i] = itemChecked.ToString();
                    i++;
                }
            }
            else
            {
                int i = 0;
                foreach (int indexChecked in checkedListBox1.CheckedIndices)
                {
                    i++;
                }
                string[] mas = mass;
                int[] arr = array;
                mass = new string[i];
                array = new int[i];
                i = 0;
                foreach (object itemChecked in checkedListBox1.CheckedItems)
                {
                    array[i] = 0;
                    mass[i] = itemChecked.ToString();
                    i++;
                }
                if (array.Count() > arr.Count())
                {
                    for(int i1=0;i1< array.Count();i1++)
                    {
                        try
                        {
                            array[i1] = arr[i1];
                        }
                        catch
                        {
                            array[i1] = 0;
                        }
                    }
                }
                else
                {
                    for (int i1 = 0; i1 < mass.Count(); i1++)
                    {
                        try
                        {
                            if (mass[i1]!=mas[i1])
                            {
                               if(i1!=mas.Count()&& arr[i1]!=0)
                                {
                                    array[i1] = arr[i1 + 1];
                                }
                            }
                            else
                            {
                                array[i1] = arr[i1];
                            }
                        }
                        catch
                        {
                            array[i1] = 0;
                        }
                    }
                }
            }
            return mass;
        }

        private void CheckedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            dataGridView3.Rows.Clear();
            mass = ItemCheck2();
            for (int i = 0; i < mass.Count(); i++)
            {
                dataGridView3.Rows.Add();
                dataGridView3.Rows[i].Cells[0].Value = mass[i];
                dataGridView3.Rows[i].Cells[1].Value = array[i];
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
            if(e.ColumnIndex == 0)
            {
                dataGridView3.Rows[e.RowIndex].Cells[0].Value = mass[e.RowIndex];
            }
            if(e.ColumnIndex==1)
            {
                try
                {
                    array[e.RowIndex] = Convert.ToInt32(dataGridView3.Rows[e.RowIndex].Cells[1].Value);
                    for (int i = 0; i < dataOrder2.Count; i++)
                    {
                        if (dataGridView3.Rows[e.RowIndex].Cells[0].Value == dataOrder2[i][0])
                        {
                            dataGridView3.Rows[e.RowIndex].Cells[2].Value = array[e.RowIndex] * Convert.ToDouble(dataOrder2[i][2]);
                            break;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Буквы нельзя вводить!");
                    dataGridView3.Rows[e.RowIndex].Cells[1].Value = array[e.RowIndex];
                }
                price();
            }
            if(e.ColumnIndex == 2)
            {
                try
                {
                    Convert.ToInt32(dataGridView3.Rows[e.RowIndex].Cells[2].Value);
                }
                catch
                {
                    for (int i = 0; i < dataOrder2.Count; i++)
                    {
                        if (dataGridView3.Rows[e.RowIndex].Cells[0].Value == dataOrder2[i][0])
                        {
                            dataGridView3.Rows[e.RowIndex].Cells[2].Value = array[e.RowIndex] * Convert.ToDouble(dataOrder2[i][2]);
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
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            DataBaseClass.CreateNewOrder(textBox1.Text, mass.ToList(), array.ToList());
            Work_Window_Load(sender, e);
        }
    }
}
