using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGoban.RuleSets
{
    abstract class RuleSet : IRuleSet
    {
        public abstract string GetName();

        public abstract int[,] GetHandicaps(Game.Game game, int count);

        public virtual bool IsLegal(Board board, int x, int y, Game.Space.Color color)
        {
            if (board.Game.Spaces.GetSpace(x, y).SpaceColor != Game.Space.Color.None) return false;

            // make a copy to test this move
            Game.Game game = board.Game.Copy();

            // add the new stone; make sure there's not already a stone there
            Game.Space newStone = game.AddStone(x, y, color);
            if (newStone == null) return false;

            // remove dead stones and make sure this group isn't dead
            game.RemoveDeadStones(x, y);
            if (game.GetGroupLiberties(newStone.GetGroup()) == 0) return false;

            // check for ko
            if (board.Game.HistoryIndex >= 1)
            {
                bool ko = true;
                foreach (Game.Space space in board.Game.History[board.Game.HistoryIndex - 1].Spaces)
                {
                    if (space.SpaceColor != game.Spaces.GetSpace(space.X, space.Y).SpaceColor) ko = false;
                }
                if (ko) return false;
            }

            return true;
        }

        public virtual int GetScore(Board board, Game.Space.Color color)
        {
            throw new NotImplementedException();
        }
    }
}
