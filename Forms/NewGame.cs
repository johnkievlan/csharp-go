using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CGoban
{
    public partial class NewGame : Form
    {
        private int _gameType = 0;

        public NewGame()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _gameType = 1;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _gameType = 2;
            this.Close();
        }

        public int GameType { get { return _gameType; } }
    }
}
