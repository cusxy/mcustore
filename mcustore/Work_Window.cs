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

        private void Work_Window_Load(object sender, EventArgs e)
        {
            dataGridView1.ColumnCount = 5;
            dataGridView1.Columns[0].Name = "Номер заказа";
            dataGridView1.Columns[1].Name = "Компания";
            dataGridView1.Columns[2].Name = "Модели микроконтроллеров";
            dataGridView1.Columns[3].Name = "Количество (шт.)";
            dataGridView1.Columns[4].Name = "Дата";

            dataGridView2.ColumnCount = 3;
            dataGridView2.Columns[0].Name = "Модель микроконтроллера";
            dataGridView2.Columns[1].Name = "Количество";
            dataGridView2.Columns[2].Name = "Цена за шт.";
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
