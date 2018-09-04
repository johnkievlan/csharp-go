using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGoban.RuleSets;

namespace CGoban.Game
{
    public delegate void OnStoneChangedEventHandler(object sender, StoneChangedEventArgs e);

    public abstract class Game
    {
        protected SpaceCollection _spaces;
        protected IRuleSet _rules;
        protected Board _board;
        protected Space.Color _nextMove;
        protected bool _lastPassed = false;
        protected List<SpaceCollection> _groups = new List<SpaceCollection>();

        public List<BasicGame> History = new List<BasicGame>();

        private int _historyIndex = -1;
        private int _capturesB = 0;
        private int _capturesW = 0;
        private bool _scoringMode = false;
        private List<Space> _whiteTerritory = new List<Space>();
        private List<Space> _blackTerritory = new List<Space>();

        public event OnStoneChangedEventHandler StoneChanged;
        protected virtual void OnStoneChanged(StoneChangedEventArgs e)
        {
            if (this.StoneChanged != null) this.StoneChanged(this, e);
        }

        public Game()
        {
        }

        protected void setupGame(Board board, Board.BoardSize size, int handicaps, IRuleSet rules)
        {
            this._board = board;
            this._rules = rules;
            this._spaces = new SpaceCollection(this, size);
            int[,] hcList = Rules.GetHandicaps(board.Game, handicaps);
            for (int x = 1; x <= this._spaces.Size; x++)
            {
                for (int y = 1; y <= this._spaces.Size; y++) if (hcList[x - 1, y - 1] == 1) this.AddStone(x, y, Space.Color.Black);
            }
            this._spaces.SpaceClick += _spaces_SpaceClick;

            if (handicaps > 1) this.NextMove = Space.Color.White;
            else this.NextMove = Space.Color.Black;

            this.History.Add((BasicGame)this.Copy());
            this._historyIndex = 0;
        }

        private void removeFromGroup(Space space)
        {
            SpaceCollection group = space.GetGroup();
            if (group.RemoveAt(space.X, space.Y))
            {
                if (group.Count == 0)
                {
                    if (this._groups.Contains(group)) this._groups.Remove(group);
                }
            }
        }

        private void addToGroup(Space space)
        {
            SpaceCollection newGroup = new SpaceCollection();

            newGroup.AddAt(space.X, space.Y);
            this._groups.Add(newGroup);
            space.SetGroup(newGroup);

            // TODO: Modify: get each connected space and add the groups for all connected spaces together
            List<Space> connected = GetConnectedSpaces(space.X, space.Y);
            foreach (Space cSpace in connected)
            {
                if (cSpace.GetGroup() != space.GetGroup())
                {
                    SpaceCollection cGroup = cSpace.GetGroup();
                    for (int i = cGroup.Count - 1; i >= 0; i--)
                    {
                        Space groupSpace = cGroup.GetByIndex(i);
                        Space boardGroupSpace = Spaces.GetSpace(groupSpace.X, groupSpace.Y);
                        space.GetGroup().AddAt(boardGroupSpace.X, boardGroupSpace.Y);
                        boardGroupSpace.SetGroup(space.GetGroup());
                    }
                    this._groups.Remove(cGroup);
                }
            }
        }

        void _spaces_SpaceClick(object sender, EventArgs e)
        {
            Space space = (Space)sender;

            if (this._scoringMode)
            {
                Space origSpace = this.History[this.History.Count - 1].GetSpace(space.X, space.Y);

                if (space.SpaceColor == Space.Color.None)
                {
                    if (origSpace.SpaceColor != Space.Color.None) space.SpaceColor = origSpace.SpaceColor;

                    if (origSpace.SpaceColor == Space.Color.Black) this._capturesW -= 1;
                    if (origSpace.SpaceColor == Space.Color.White) this._capturesB -= 1;
                }
                else
                {
                    space.SpaceColor = Space.Color.None;

                    if (origSpace.SpaceColor == Space.Color.Black) this._capturesW += 1;
                    if (origSpace.SpaceColor == Space.Color.White) this._capturesB += 1;
                }

                this.checkTerritory();
            }
            else this.PlaceStone(space.X, space.Y);
        }

        public Space GetSpace(int x, int y)
        {
            return this._spaces.GetSpace(x, y);
        }

        public void PlaceStone(int x, int y)
        {
            if (_rules.IsLegal(_board, x, y, this.NextMove))
            {
                this.AddStone(x, y, this.NextMove);

                if (this.NextMove == Space.Color.Black) this._capturesB += this.RemoveDeadStones(x, y);
                if (this.NextMove == Space.Color.White) this._capturesW += this.RemoveDeadStones(x, y);

                while (this._historyIndex < this.History.Count - 1)
                {
                    this.History.Remove(this.History[this.History.Count - 1]);
                }
                if (this.NextMove == Space.Color.Black) this.NextMove = Space.Color.White; else this.NextMove = Space.Color.Black;

                _lastPassed = false;

                this.History.Add((BasicGame)this.Copy());
                this._historyIndex = this.History.Count - 1;
            }
        }

        public int GetCaptures(Space.Color color)
        {
            if (color == Space.Color.White) return this._capturesW;
            else return this._capturesB;
        }

        public int GetScore(Space.Color color)
        {
            if (color == Space.Color.Black) return this._blackTerritory.Count + this._capturesB;
            else return this._whiteTerritory.Count + this._capturesW;
        }

        public Space AddStone(int x, int y, Space.Color spaceColor)
        {
            Space addSpace = this._spaces.GetSpace(x, y);

            if (addSpace.SpaceColor == Space.Color.None)
            {
                addSpace.SpaceColor = spaceColor;
                this.addToGroup(addSpace); 

                StoneChangedEventArgs e = new StoneChangedEventArgs();
                e.Color = addSpace.SpaceColor;
                e.X = addSpace.X;
                e.Y = addSpace.Y;
                this.OnStoneChanged(e);

                return addSpace;
            }
            return null;
        }

        public bool RemoveStone(int x, int y)
        {
            Space remSpace = this._spaces.GetSpace(x, y);

            if (remSpace.SpaceColor != Space.Color.None)
            {
                remSpace.SpaceColor = Space.Color.None;
                this.removeFromGroup(remSpace);

                StoneChangedEventArgs e = new StoneChangedEventArgs();
                e.Color = remSpace.SpaceColor;
                e.X = remSpace.X;
                e.Y = remSpace.Y;
                this.OnStoneChanged(e);

                return true;
            }
            return false;
        }

        public void Pass()
        {
            if (_lastPassed)
            {
                if (this.NextMove == Space.Color.Black) this.NextMove = Space.Color.White; else this.NextMove = Space.Color.Black;
                ScoreGame();
            }
            else
            {
                while (this._historyIndex < this.History.Count - 1)
                {
                    this.History.Remove(this.History[this.History.Count - 1]);
                }
                if (this.NextMove == Space.Color.Black) this.NextMove = Space.Color.White; else this.NextMove = Space.Color.Black;

                _lastPassed = true;

                this.History.Add((BasicGame)this.Copy());
                this._historyIndex = this.History.Count - 1;
            }
        }

        public int RemoveDeadStones(int exceptionX = 0, int exceptionY = 0)
        {
            bool exception = false;
            int deadStones = 0;

            for (int i = this._groups.Count - 1; i >= 0; i--)
            {
                SpaceCollection grp = this._groups[i];

                if (this.GetGroupLiberties(grp) == 0)
                {
                    foreach (Space space in grp)
                    {
                        if (space.X == exceptionX && space.Y == exceptionY) exception = true;
                    }

                    if (!exception)
                    {
                        deadStones += grp.Count;
                        while (grp.Count > 0) this.RemoveStone(grp.GetByIndex(0).X, grp.GetByIndex(0).Y);
                    }
                    exception = false;
                }
            }

            return deadStones;
        }

        public void ScoreGame()
        {
            this.History.Add((BasicGame)this.Copy());
            this._scoringMode = true;

            this.checkTerritory();

            this._board.EnterScoringMode();
        }

        public void ResetScoringMode()
        {
            if (this._scoringMode)
            {
                this._scoringMode = false;
                this.loadHistory(this.History.Count - 1);
                this.ScoreGame();
            }
        }

        private void checkTerritory()
        {
            this._whiteTerritory.Clear();
            this._blackTerritory.Clear();
            List<Space> none = new List<Space>();

            foreach (Space space in this.Spaces)
            {
                if (space.SpaceColor == Space.Color.None && !_whiteTerritory.Contains(space) && !_blackTerritory.Contains(space) && !none.Contains(space))
                {
                    List<Space> group = this.GetEmptyGroup(space.X, space.Y);

                    Space.Color territory = this.GetGroupTerritory(group);

                    if (territory == Space.Color.None)
                    {
                        foreach (Space tSpace in group)
                        {
                            none.Add(tSpace);
                            tSpace.SetTerritory(territory);
                        }
                    }
                    else
                    {
                        foreach (Space tSpace in group)
                        {
                            if (territory == Space.Color.White) _whiteTerritory.Add(tSpace);
                            if (territory == Space.Color.Black) _blackTerritory.Add(tSpace);

                            tSpace.SetTerritory(territory);
                        }
                    }
                }
            }

            this._board.SetScoringStatus();
       }

        public void ResumeGame()
        {
            this._scoringMode = false;
            this.loadHistory(this.History.Count - 3);
            this.History.Remove(this.History[this.History.Count - 2]); this.History.Remove(this.History[this.History.Count - 1]);
            this._lastPassed = false;
            this._board.ExitScoringMode();
        }

        public SpaceCollection Spaces { get { return this._spaces; } }
        public int BoardSize { get { return this._spaces.Size; } }
        public IRuleSet Rules { get { return this._rules; } }
        public Board Board { get { return this._board; } }
        public int HistoryIndex { get { return this._historyIndex; } }
        public bool ScoringMode { get { return this._scoringMode; } }

        public Space.Color NextMove
        {
            get
            {
                return this._nextMove;
            }
            set
            {
                this._nextMove = value;

                if (this._nextMove == Space.Color.Black)
                {
                    this._board.SetStatus();
                }
                else
                {
                    this._board.SetStatus();
                }
            }
        }

        public bool CanRedo
        {
            get
            {
                return (this._historyIndex < this.History.Count - 1);
            }
        }

        public SpaceCollection GetSpaceLibertyList(int x, int y)
        {
            if (Spaces.GetSpace(x, y).SpaceColor == Space.Color.None) return null;

            SpaceCollection liberties = new SpaceCollection();

            int libertyX = x - 1;
            int libertyY = y;
            if (libertyX > 0 && Spaces.GetSpace(libertyX, libertyY).SpaceColor == Space.Color.None) liberties.AddAt(libertyX, libertyY);

            libertyX = x + 1;
            if (libertyX <= this.BoardSize && Spaces.GetSpace(libertyX, libertyY).SpaceColor == Space.Color.None) liberties.AddAt(libertyX, libertyY);

            libertyX = x;
            libertyY = y - 1;
            if (libertyY > 0 && Spaces.GetSpace(libertyX, libertyY).SpaceColor == Space.Color.None) liberties.AddAt(libertyX, libertyY);

            libertyY = y + 1;
            if (libertyY <= this.BoardSize && Spaces.GetSpace(libertyX, libertyY).SpaceColor == Space.Color.None) liberties.AddAt(libertyX, libertyY);

            return liberties;
        }

        public int GetSpaceLiberties(int x, int y)
        {
            if (Spaces.GetSpace(x, y).SpaceColor == Space.Color.None) return -1;

            int liberties = 0;

            int libertyX = x - 1;
            int libertyY = y;
            if (libertyX > 0 && Spaces.GetSpace(libertyX, libertyY).SpaceColor == Space.Color.None) liberties++;

            libertyX = x + 1;
            if (libertyX <= this.BoardSize && Spaces.GetSpace(libertyX, libertyY).SpaceColor == Space.Color.None) liberties++;

            libertyX = x;
            libertyY = y - 1;
            if (libertyY > 0 && Spaces.GetSpace(libertyX, libertyY).SpaceColor == Space.Color.None) liberties++;

            libertyY = y + 1;
            if (libertyY <= this.BoardSize && Spaces.GetSpace(libertyX, libertyY).SpaceColor == Space.Color.None) liberties++;

            return liberties;
        }

        public int GetSpaceConnections(int x, int y)
        {
            if (Spaces.GetSpace(x, y).SpaceColor == Space.Color.None) return -1;

            int connections = 0;

            int connectionX = x - 1;
            int connectionY = y;
            if (connectionX > 0 && Spaces.GetSpace(connectionX, connectionY).SpaceColor == Spaces.GetSpace(x, y).SpaceColor) connections++;

            connectionX = x + 1;
            if (connectionX <= this.BoardSize && Spaces.GetSpace(connectionX, connectionY).SpaceColor == Spaces.GetSpace(x, y).SpaceColor) connections++;

            connectionX = x;
            connectionY = y - 1;
            if (connectionY > 0 && Spaces.GetSpace(connectionX, connectionY).SpaceColor == Spaces.GetSpace(x, y).SpaceColor) connections++;

            connectionY = y + 1;
            if (connectionY <= this.BoardSize && Spaces.GetSpace(connectionX, connectionY).SpaceColor == Spaces.GetSpace(x, y).SpaceColor) connections++;

            return connections;
        }

        public List<Space> GetEmptyGroup(int x, int y)
        {
            Space space = this.Spaces.GetSpace(x, y);
            if (space.SpaceColor == Space.Color.None)
            {
                List<Space> emptyGroup = new List<Space>();

                this.addToEmptyGroup(x, y, emptyGroup);

                return emptyGroup;
            }
            return null;
        }

        public Space.Color GetGroupTerritory(List<Space> group)
        {
            Space.Color territory = Space.Color.None;

            foreach (Space space in group)
            {
                for (int cX = space.X - 1; cX <= space.X + 1; cX++) for (int cY = space.Y - 1; cY <= space.Y + 1; cY++)
                    {
                        if ((cX == space.X && cY != space.Y) || (cX != space.X && cY == space.Y))
                        {
                            if (cX > 0 && cX <= this.BoardSize && cY > 0 && cY <= this.BoardSize)
                            {
                                Space cSpace = this.Spaces.GetSpace(cX, cY);

                                if (cSpace.SpaceColor != Space.Color.None)
                                {
                                    if (territory == Space.Color.None) territory = cSpace.SpaceColor;
                                    if (cSpace.SpaceColor != territory) return Space.Color.None;
                                }
                            }
                        }
                    }
            }

            return territory;
        }

        private void addToEmptyGroup(int x, int y, List<Space> group)
        {
            Space space = this.Spaces.GetSpace(x, y);
            group.Add(space);

            List<Space> connected = this.GetConnectedSpaces(x, y);

            foreach (Space cSpace in connected)
            {
                if (!group.Contains(cSpace))
                {
                    this.addToEmptyGroup(cSpace.X, cSpace.Y, group);
                }
            }
        }

        public List<Space> GetConnectedSpaces(int x, int y)
        {
            List<Space> spaces = new List<Space>();

            int connectionX = x - 1;
            int connectionY = y;
            if (connectionX > 0 && Spaces.GetSpace(connectionX, connectionY).SpaceColor == Spaces.GetSpace(x, y).SpaceColor) spaces.Add(Spaces.GetSpace(connectionX, connectionY));

            connectionX = x + 1;
            if (connectionX <= this.BoardSize && Spaces.GetSpace(connectionX, connectionY).SpaceColor == Spaces.GetSpace(x, y).SpaceColor) spaces.Add(Spaces.GetSpace(connectionX, connectionY));

            connectionX = x;
            connectionY = y - 1;
            if (connectionY > 0 && Spaces.GetSpace(connectionX, connectionY).SpaceColor == Spaces.GetSpace(x, y).SpaceColor) spaces.Add(Spaces.GetSpace(connectionX, connectionY));

            connectionY = y + 1;
            if (connectionY <= this.BoardSize && Spaces.GetSpace(connectionX, connectionY).SpaceColor == Spaces.GetSpace(x, y).SpaceColor) spaces.Add(Spaces.GetSpace(connectionX, connectionY));

            return spaces;
        }

        public int GetGroupLiberties(int x, int y)
        {
            return GetGroupLiberties(this.Spaces.GetSpace(x, y).GetGroup());
        }

        public int GetGroupLiberties(SpaceCollection group)
        {
            SpaceCollection liberties = new SpaceCollection();

            foreach (Space space in group)
            {
                SpaceCollection spaceLiberties = GetSpaceLibertyList(space.X, space.Y);

                foreach (Space liberty in spaceLiberties) liberties.AddAt(liberty.X, liberty.Y);
            }
            
            return liberties.Count;
        }

        public Game Copy()
        {
            Game copy = new BasicGame();
            copy._nextMove = this._nextMove;
            copy._lastPassed = this._lastPassed;

            copy._spaces = new SpaceCollection();
            copySpaces(this._spaces, copy._spaces);
            copy._spaces.Size = (int)Math.Sqrt(this._spaces.Count);

            copy._groups = new List<SpaceCollection>();
            foreach (SpaceCollection grp in this._groups)
            {
                SpaceCollection copyGrp = new SpaceCollection();
                copySpaces(grp, copyGrp, copy._spaces);
                copy._groups.Add(copyGrp);
            }

            copy._capturesB = this._capturesB;
            copy._capturesW = this._capturesW;

            return copy;
        }

        public bool Undo()
        {
            return this.loadHistory(this.HistoryIndex - 1);
        }

        public bool Redo()
        {
            return this.loadHistory(this.HistoryIndex + 1);
        }

        private bool loadHistory(int index)
        {
            if (index >= 0 && index < this.History.Count && !this._scoringMode)
            {
                BasicGame historyGame = this.History[index];

                this._lastPassed = historyGame._lastPassed;

                this._spaces = new SpaceCollection(this, this.BoardSize);
                this._groups.Clear();
                foreach (Space space in historyGame.Spaces)
                {
                    Space gameSpace = this.Spaces.GetSpace(space.X, space.Y);
                    if (space.SpaceColor != Space.Color.None)
                    {
                        gameSpace.SpaceColor = space.SpaceColor;
                        this.addToGroup(gameSpace);
                    }
                }
                this._spaces.SpaceClick += _spaces_SpaceClick;

                this._capturesB = historyGame._capturesB;
                this._capturesW = historyGame._capturesW;

                this._historyIndex = index;

                this.NextMove = historyGame._nextMove;

                return true;
            }
            else return false;
        }

        private void copySpaces(SpaceCollection src, SpaceCollection dest, SpaceCollection groupSpaces = null)
        {
            foreach (Space space in src)
            {
                dest.AddAt(space.X, space.Y);
                Space copySpace = dest.GetSpace(space.X, space.Y);
                copySpace.SpaceColor = space.SpaceColor;
                if (groupSpaces != null) groupSpaces.GetSpace(space.X, space.Y).SetGroup(dest);
            }
        }
    }

    public class LocalGame : Game
    {
        public LocalGame(Board board, Board.BoardSize size, int handicaps, IRuleSet rules)
        {
            this.setupGame(board, size, handicaps, rules);
        }
    }

    public class LANGame : Game
    {
        public LANGame(Board board, Board.BoardSize size, int handicaps, IRuleSet rules)
        {
            this.setupGame(board, size, handicaps, rules);
        }
    }

    public class BasicGame : Game
    {
        public BasicGame()
        {
        }
    }

    public class StoneChangedEventArgs : EventArgs
    {
        public Space.Color Color = Space.Color.None;
        public int X = -1;
        public int Y = -1;
    }
}
