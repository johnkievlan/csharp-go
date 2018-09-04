using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGoban.Game
{
    public class SpaceCollection : System.Collections.CollectionBase
    {
        // private members
        private int _size;
        private Game _game;

        // events
        public event SpaceClickEventHandler SpaceClick;
        protected virtual void OnSpaceClick(object sender, EventArgs e)
        {
            if (SpaceClick != null) SpaceClick(sender, e);
        }
        public event SpaceColorChangedEventHandler SpaceColorChanged;
        protected virtual void OnSpaceColorChanged(object sender, SpaceColorEventArgs e)
        {
            if (SpaceColorChanged != null) SpaceColorChanged(sender, e);
        }

        public SpaceCollection()
        {
        }

        public SpaceCollection(Game game, Board.BoardSize size)
        {
            switch (size)
            {
                case Board.BoardSize.Nine:
                    {
                        this.setupCollection(game, 9);
                        break;
                    }
                case Board.BoardSize.Thirteen:
                    {
                        this.setupCollection(game, 13);
                        break;
                    }
                default:
                    {
                        this.setupCollection(game, 19);
                        break;
                    }
            }
        }

        public SpaceCollection(Game game, int size)
        {
            this.setupCollection(game, size);
        }

        private void setupCollection(Game game, int size)
        {
            this._size = size;
            this._game = game;

            for (int i = 0; i < Math.Pow(this._size, 2); i++)
            {
                this.Add();
            }
        }

        public void Add()
        {
            int x = (this.List.Count % this._size) + 1;
            int y = (int)Math.Ceiling((decimal)(this.List.Count + 1) / (decimal)this._size);

            Space space = new Space(x, y);
            if (x == 1) space.LeftEdge = true;
            if (x == this._size) space.RightEdge = true;
            if (y == 1) space.TopEdge = true;
            if (y == this._size) space.BottomEdge = true;
            this.List.Add(space);

            space.SpaceClick += space_SpaceClick;
            space.ColorChanged += space_ColorChanged;

            if (this._size == 19)
            {
                if (x == 4 || x == 10 || x == 16)
                {
                    if (y == 4 || y == 10 || y == 16) space.Handicap = true;
                }
            }

            if (this._size == 13)
            {
                if (x == 4 || x == 10)
                {
                    if (y == 4 || y == 10) space.Handicap = true;
                }
            }

            if (this._size == 9)
            {
                if (x == 4 || x == 6)
                {
                    if (y == 4 || y == 6) space.Handicap = true;
                }
            }
        }

        void space_ColorChanged(object sender, SpaceColorEventArgs e)
        {
            this.OnSpaceColorChanged(sender, e);
        }

        public bool AddAt(int x, int y)
        {
            if (this.GetSpace(x, y) == null)
            {
                Space space = new Space(x, y);
                this.List.Add(space);
                return true;
            }

            return false;
        }

        public bool RemoveAt(int x, int y)
        {
            if (this.GetSpace(x, y) != null)
            {
                this.List.Remove(this.GetSpace(x, y));
                return true;
            }

            return false;
        }

        void space_SpaceClick(object sender, EventArgs e)
        {
            this.OnSpaceClick(sender, e);
        }

        public Space GetSpace(int x, int y)
        {
            for (var i = 0; i < this.List.Count; i++)
            {
                Space space = (Space)this.List[i];
                if (space.X == x && space.Y == y) return space;
            }

            return null;
        }

        public Space GetByIndex(int index)
        {
            if (index < this.List.Count) return (Space)this.List[index];
            return null;
        }

        public int Size { get { return this._size; } set { this._size = value; } }
    }
}
