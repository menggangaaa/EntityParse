using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EntityParse
{
    public partial class BulidSql : Form
    {
        public BulidSql()
        {
            InitializeComponent();
        }

        protected override bool ShowWithoutActivation//重写显示时是否激活窗体
        {
            get
            {
                //return base.ShowWithoutActivation;
                return true;
            }
        }

        private void BulidSql_Load(object sender, EventArgs e)
        {

        }

        private void BulidSql_MouseLeave(object sender, EventArgs e)
        {
            Close();
        }
    }
}
