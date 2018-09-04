using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CGoban.RuleSets;

namespace CGoban
{
    public partial class GameSetup : Form
    {
        public GameSetup()
        {
            InitializeComponent();

            cboRuleSet.SelectedIndex = 0;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK; this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel; this.Close();
        }

        public Board.BoardSize BoardSize
        {
            get
            {
                if (this.rad9x9.Checked) return Board.BoardSize.Nine;
                if (this.rad13x13.Checked) return Board.BoardSize.Thirteen;
                return Board.BoardSize.Nineteen;
            }
        }

        public IRuleSet Rules
        {
            get
            {
                if (this.cboRuleSet.Text == "Chinese") return new RuleSets.ChineseRules();
                if (this.cboRuleSet.Text == "Korean") return new RuleSets.KoreanRules();
                return new RuleSets.JapaneseRules();
            }
        }

        public int Handicaps { get { return (int)this.numHandicaps.Value; } }

        private void rad13x13_CheckedChanged(object sender, EventArgs e)
        {
            if (rad13x13.Checked)
            {
                // lblHandicaps.Enabled = false;
                // numHandicaps.Enabled = false;
                numHandicaps.Maximum = 4;
            } 
        }

        private void rad9x9_CheckedChanged(object sender, EventArgs e)
        {
            if (rad9x9.Checked)
            {
                //lblHandicaps.Enabled = false;
                //numHandicaps.Enabled = false;
                numHandicaps.Maximum = 4;
            }
        }

        private void rad19x19_CheckedChanged(object sender, EventArgs e)
        {
            if (rad19x19.Checked)
            {
                lblHandicaps.Enabled = true;
                numHandicaps.Enabled = true;
                numHandicaps.Maximum = 9;
            }
        }
    }
}
