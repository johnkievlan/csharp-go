using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CGoban.Game;
using System.Media;

namespace CGoban
{
    public partial class Board : Form
    {
        // private members
        private Game.Game _game;
        private int _hDiff; private int _wDiff;
        private Board.BoardSize _boardSize;
        private bool _shouldClose = false;
        private bool _gameScoring = false;

        // public members
        public enum BoardSize { Nine, Thirteen, Nineteen }

        public Board()
        {
            InitializeSettings();
            InitializeComponent();

            this._hDiff = this.Height - this.ClientRectangle.Height + this.MainMenuStrip.Height + this.statusStrip.Height;
            this._wDiff = this.Width - this.ClientRectangle.Width;

            this.SizeChanged += Board_SizeChanged;
            this.ResizeEnd += Board_Resize;
            this.Load += Board_Load;
            
            this.startNewGame();
        }

        void Board_SizeChanged(object sender, EventArgs e)
        {
            // just set all spaces to invisible, because new spaces will be draw
            foreach (Space space in this.Game.Spaces)
            {
                space.Visible = false;
            }
        }

        void Board_Load(object sender, EventArgs e)
        {
            if (this._shouldClose) this.Close();
        }

        private void InitializeSettings()
        {
            // initialize form settings
            if (Properties.Settings.Default.LastWidth == 0)
                Properties.Settings.Default.LastWidth = this.Width;
            else
                this.Width = Properties.Settings.Default.LastWidth;

            if (Properties.Settings.Default.LastHeight == 0) 
                Properties.Settings.Default.LastHeight = this.Height;
            else
                this.Height = Properties.Settings.Default.LastHeight;

            Properties.Settings.Default.Save();
        }

        public Game.Game Game
        {
            get { return this._game; }
            set
            {
                this._game = value;

                this.RefreshGame();
            }
        }

        public void RefreshGame()
        {
            for (int i = this.Controls.Count - 1; i >= 0; i--)
            {
                Control ctl = this.Controls[i];

                if (ctl.GetType() == typeof(Space))
                {
                    this.Controls.Remove(ctl);
                }
            }

            foreach (Space space in this._game.Spaces)
            {
                space.Visible = false;
                this.Controls.Add(space);
            }

            this.DrawBoard();
        }

        public Board.BoardSize GameSize { get { return this._boardSize; } }

        public Rectangle BoardRectangle
        {
            get { return new Rectangle(0, this.mainMenuStrip.Height, this.Width - this._wDiff, this.Height - this._hDiff); }
        }

        void Board_Resize(object sender, EventArgs e)
        {
            this.DrawBoard();
        }

        public void DrawBoard()
        {
            int lastWidth = Properties.Settings.Default.LastWidth;
            int lastHeight = Properties.Settings.Default.LastHeight;

            this.adjustSize();

            if (this.Width != lastWidth)    // width resized; base new height on width
            {
                this.Height = (this.Width - this._wDiff) + this._hDiff;
            }
            else   // base new width on height
            {
                this.Width = (this.Height - this._hDiff) + this._wDiff;
            }

            Properties.Settings.Default.LastWidth = this.Width;
            Properties.Settings.Default.LastHeight = this.Height;
            Properties.Settings.Default.Save();

            foreach (Space space in this.Game.Spaces)
            {
                this.setSpaceDimensions(space);
                space.Visible = true;
            }

            if (this.Game.CanRedo) this.redoToolStripMenuItem.Enabled = true; else this.redoToolStripMenuItem.Enabled = false;

            this.Focus();
        }

        public void SetStatus()
        {
            this.setStatus(false);
        }

        public void SetScoringStatus()
        {
            this.setStatus(true);
        }

        private void setStatus(bool scoring)
        {
            this._gameScoring = scoring;

            if (this.Game != null)
            {
                if (scoring) this.statusLabelScore.Text = "W: " + this.Game.GetScore(Space.Color.White).ToString() + ", B: " + this.Game.GetScore(Space.Color.Black).ToString();
                else this.statusLabelScore.Text = "";

                this.statusLabelCaptures.Text = "Cap(W): " + this.Game.GetCaptures(Space.Color.White).ToString() + ", Cap(B): " + this.Game.GetCaptures(Space.Color.Black).ToString();

                if (scoring) this.statusLabelComment.Text = "Click stones to add/remove.";
                else this.statusLabelComment.Text = this.Game.NextMove == Space.Color.White ? "White's Move" : "Black's Move";

                this.passToolStripMenuItem.Visible = !scoring;
                this.undoToolStripMenuItem.Visible = !scoring;
                this.redoToolStripMenuItem.Visible = !scoring;
                this.resumeToolStripMenuItem.Visible = scoring;
                this.resetToolStripMenuItem.Visible = scoring;
            }
        }

        protected virtual void _DrawBoard(Object sender, PaintEventArgs args)
        {
            Rectangle clientRect = this.ClientRectangle;
            Size size = this.Size;

            int hDiff = size.Height - clientRect.Height + this.MainMenuStrip.Height + this.statusStrip.Height;
            int wDiff = size.Width - clientRect.Width;

            Size newSize = new Size(wDiff + 589, hDiff + 589);
            this.Size = newSize;

            int zeroTop = this.MainMenuStrip.Height;

            using (SolidBrush bg = new SolidBrush(Color.Moccasin))
            {
                args.Graphics.FillRectangle(bg, 0, 0 + zeroTop, 589, 589 + zeroTop);
            }

            int lineCounter = 1;
            using (Pen linePen = new Pen(Color.Black))
            {
                for (int pos = 16; pos < 589; pos += 31)
                {
                    args.Graphics.DrawLine(linePen, new Point(16, pos + zeroTop), new Point(574, pos + zeroTop));
                    args.Graphics.DrawLine(linePen, new Point(pos, 16 + zeroTop), new Point(pos, 574 + zeroTop));

                    if (lineCounter == 4 || lineCounter == 10 || lineCounter == 16)
                    {
                        using (Brush hcBrush = new SolidBrush(linePen.Color))
                        {
                            for (int hcLine = 4; hcLine <= 16; hcLine += 6)
                            {
                                Point hcPos = new Point(pos, 16 + 31 * (hcLine - 1) + zeroTop);
                                int hcRad = 5;

                                args.Graphics.FillEllipse(hcBrush, hcPos.X - hcRad, hcPos.Y - hcRad, hcRad * 2, hcRad * 2);
                            }
                        }
                    }

                    lineCounter++;
                }
            }
        }

        private void startNewGame()
        {
            NewGame dlg1 = new NewGame();
            dlg1.ShowDialog();

            if (dlg1.GameType == 1)
            {
                GameSetup dlg2 = new GameSetup();

                if (dlg2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this._boardSize = dlg2.BoardSize;
                    this.Game = new LocalGame(this, dlg2.BoardSize, dlg2.Handicaps, dlg2.Rules);
                    this.Game.StoneChanged += Game_StoneChanged;
                }
                else
                {
                    if (this._game == null)
                    {
                        this._shouldClose = true;
                    }
                }
            }
            else if (dlg1.GameType == 2)
            {
            }
            else
            {
                if (this._game == null)
                {
                    this._shouldClose = true;
                }
            }

            this.SetStatus();
        }

        void Game_StoneChanged(object sender, StoneChangedEventArgs e)
        {
            if (e.Color != Space.Color.None && !this.Game.ScoringMode)
            {
                Stream audioStream = Properties.Resources.stoneclick;

                SoundPlayer player = new SoundPlayer(audioStream);
                player.Play();
            }
        }

        private void adjustSize()
        {
            this.Width = this.getAdjustedSize(this.Width - this._wDiff) + this._wDiff;
            this.Height = this.getAdjustedSize(this.Height - this._hDiff) + this._hDiff;
        }

        private int getAdjustedSize(int oldSize)
        {
            int adjustedSize = ((int)Math.Round(((decimal)oldSize - (decimal)this.Game.BoardSize) / (decimal)this.Game.BoardSize) + 1) * this.Game.BoardSize;
            if ((adjustedSize / this.Game.BoardSize) < 17) adjustedSize = 17 * this.Game.BoardSize;
            return adjustedSize;
        }

        private void setSpaceDimensions(Space space)
        {
            space.Top = this.BoardRectangle.Top + (space.Y - 1) * (this.BoardRectangle.Height / this.Game.BoardSize);
            space.Left = this.BoardRectangle.Left + (space.X - 1) * (this.BoardRectangle.Width / this.Game.BoardSize);
            space.Width = this.BoardRectangle.Width / this.Game.BoardSize;
            space.Height = this.BoardRectangle.Height / this.Game.BoardSize;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.startNewGame();
        }

        private void passToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Game.Pass();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Game.Undo()) this.RefreshGame();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Game.Redo()) this.RefreshGame();
        }

        internal void EnterScoringMode()
        {
            SetScoringStatus();
        }

        internal void ExitScoringMode()
        {
            SetStatus();
        }

        private void resumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Game.ResumeGame();
            this.RefreshGame();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Game.ResetScoringMode();
            this.RefreshGame();
        }
    }
}
