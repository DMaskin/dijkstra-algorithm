using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dijkstra_s_Algorithm
{
    public partial class FormHello : Form
    {
        byte orientation = 2;
        public FormHello()
        {
            InitializeComponent();
        }

        private void FormHello_Load(object sender, EventArgs e)
        {
            this.richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
            this.richTextBox1.Text = "Вас приветствует мастер визуализации графов. Пожалуйста, выберите режим работы с графом: ";
        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            this.richTextBox1.Select(0, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            orientation = 1;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            orientation = 0;
            this.Close();
        }

        public byte Orientation
        {           
            get
            {
                return orientation;
            }
        }

        private void FormHello_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                if (orientation == 2)
                    e.Cancel = true;
        }
    }
}
