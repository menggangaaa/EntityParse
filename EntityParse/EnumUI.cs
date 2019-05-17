using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EntityParse
{
    public partial class EnumUI : Form
    {
        public EnumUI()
        {
            InitializeComponent();
        }

        public EnumUI(ArrayList list)
        {
            InitializeComponent();
            foreach (ArrayList listNode in list)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = listNode[0];
                dataGridView1.Rows[index].Cells[1].Value = listNode[1];
                dataGridView1.Rows[index].Cells[2].Value = listNode[2];
            }
        }

        private void EnumUI_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
        }

        public DialogResult ShowDialog(ArrayList list)
        {
            dataGridView1.Rows.Clear();
            foreach (ArrayList listNode in list)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = listNode[0];
                dataGridView1.Rows[index].Cells[1].Value = listNode[1];
                dataGridView1.Rows[index].Cells[2].Value = listNode[2];
            }

            return ShowDialog();
        }

        private void EnumUI_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
