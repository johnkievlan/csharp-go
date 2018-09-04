using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CGoban.Game
{

    // event delegates
    public delegate void SpaceClickEventHandler(object sender, EventArgs e);
    public delegate void SpaceColorChangedEventHandler(object sender, SpaceColorEventArgs e);

    public partial class Space : UserControl
    {
        // private members
        private int _x; private int _y;
        private Space.Color _color = Space.Color.None;
        private bool _topEdge = false; private bool _botEdge = false; private bool _leftEdge = false; private bool _rightEdge = false;
        private bool _handicap = false;
        private int _center;
        private SpaceCollection _group;
        private Space.Color _territory = Color.None;

        // public members
        public enum Color { None, White, Black }

        // events
        public event SpaceClickEventHandler SpaceClick;
        protected virtual void OnSpaceClick(EventArgs e)
        {
            if (SpaceClick != null) SpaceClick(this, e);
        }
        public event SpaceColorChangedEventHandler ColorChanged;
        protected virtual void OnColorChanged(SpaceColorEventArgs e)
        {
            if (ColorChanged != null) ColorChanged(this, e);
        }

        public Space(int x, int y)
        {
            this.X = x;
            this.Y = y;

            this.Click += Space_Click;
            this.Paint += Space_Paint;

            InitializeComponent();
        }

        void Space_Paint(object sender, PaintEventArgs e)
        {
            this.paintSpace();
        }

        private void paintSpace()
        {
            using (Graphics g = this.CreateGraphics())
            {
                g.FillRectangle(new SolidBrush(System.Drawing.Color.Moccasin), 0, 0, this.Width, this.Height);

                this._center = (this.Width - 1) / 2;
                
                if (this.TopEdge)
                    g.DrawLine(new Pen(System.Drawing.Color.Black, (float)1), _center, _center, _center, this.Height);
                else if (this.BottomEdge)
                    g.DrawLine(new Pen(System.Drawing.Color.Black, (float)1), _center, 0, _center, _center);
                else
                    g.DrawLine(new Pen(System.Drawing.Color.Black, (float)1), _center, 0, _center, this.Height);

                if (this.LeftEdge)
                    g.DrawLine(new Pen(System.Drawing.Color.Black, (float)1), _center, _center, this.Width, _center);
                else if (this.RightEdge)
                    g.DrawLine(new Pen(System.Drawing.Color.Black, (float)1), 0, _center, _center, _center);
                else
                    g.DrawLine(new Pen(System.Drawing.Color.Black, (float)1), 0, _center, this.Width, _center);

                if (this.Handicap) g.FillEllipse(new SolidBrush(System.Drawing.Color.Black), _center - 4, _center - 4, 8, 8);

                if (this.SpaceColor == Color.Black)
                    this.drawBlackStone(g);
                else if (this.SpaceColor == Color.White)
                    this.drawWhiteStone(g);

                if (this._territory == Color.Black) this.drawBlackStone(g, true);
                if (this._territory == Color.White) this.drawWhiteStone(g, true);
            }
        }

        void Space_Click(object sender, EventArgs e)
        {
            OnSpaceClick(EventArgs.Empty);
        }

        public void SetGroup(SpaceCollection group)
        {
            this._group = group;
        }

        public SpaceCollection GetGroup()
        {
            return this._group;
        }

        public void SetTerritory(Space.Color territory)
        {
            if (this._territory != territory)
            {
                this._territory = territory;

                this.paintSpace();
            }
        }

        public Space.Color SpaceColor
        {
            get
            {
                return this._color;
            }
            set
            {
                if (this._color != value)
                {
                    this._color = value;

                    if (this._color != Color.None) this._territory = Color.None;

                    SpaceColorEventArgs args = new SpaceColorEventArgs();
                    args.Color = value;
                    args.X = this.X;
                    args.Y = this.Y;
                    this.OnColorChanged(args);

                    this.Refresh();
                }
            }
        }

        public int X { get { return this._x; } set { this._x = value; } }
        public int Y { get { return this._y; } set { this._y = value; } }

        public bool TopEdge { get { return this._topEdge; } set { this._topEdge = value; } }
        public bool BottomEdge { get { return this._botEdge; } set { this._botEdge = value; } }
        public bool LeftEdge { get { return this._leftEdge; } set { this._leftEdge = value; } }
        public bool RightEdge { get { return this._rightEdge; } set { this._rightEdge = value; } }

        public bool Handicap { get { return this._handicap; } set { this._handicap = value; } }

        private void drawBlackStone(Graphics g, bool smallStone = false)
        {
            this.drawStone(g, 20, 200, 70, smallStone);
        }

        private void drawWhiteStone(Graphics g, bool smallStone = false)
        {
            this.drawStone(g, 150, 250, 170, smallStone);
        }

        private void drawStone(Graphics g, int startColor, int endColor, int borderColor, bool smallStone)
        {
            float width = (float)this.Width * .75F;
            if (smallStone) width = width / 2;
            float offset = 0;
            int color;

            for (float rad = width / 2; rad > 0; rad -= .25F)
            {
                color = startColor + (int)(((width - rad * 2F) / width) * ((float)endColor - (float)startColor));
                using (Pen pen = new Pen(System.Drawing.Color.FromArgb(color, color, color)))
                {
                    g.DrawEllipse(pen, _center - offset - rad, _center - offset - rad, rad * 2, rad * 2);
                }

                offset += .1F;
            }

            using (Pen borderPen = new Pen(System.Drawing.Color.FromArgb(borderColor, borderColor, borderColor)))
            {
                g.DrawEllipse(borderPen, _center - (width / 2), _center - (width / 2), width, width);
            }
        }
    }

    public class SpaceColorEventArgs : EventArgs
    {
        public Space.Color Color;
        public int X;
        public int Y;
    }
}
