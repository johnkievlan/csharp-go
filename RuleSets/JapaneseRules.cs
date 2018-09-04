using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGoban.Game;

namespace CGoban.RuleSets
{
    class JapaneseRules : RuleSet
    {
        public override string GetName()
        {
            return "Japanese";
        }

        public override int GetScore(Board board, Space.Color color)
        {
            throw new NotImplementedException();
        }

        public override bool IsLegal(Board board, int x, int y, Space.Color color)
        {
            bool isLegal = base.IsLegal(board, x, y, color);

            if (isLegal)
            {
                return true;
            }
            else return false;
        }

        public override int[,] GetHandicaps(CGoban.Game.Game game, int count)
        {
            int[,] handicaps = new int[19, 19];

            if (count >= 2)
            {
                handicaps[15, 3] = 1;
                handicaps[3, 15] = 1;
            }
            if (count >= 3) handicaps[15, 15] = 1;
            if (count >= 4) handicaps[3, 3] = 1;
            if (count >= 5) handicaps[9, 9] = 1;
            if (count >= 6)
            {
                handicaps[3, 9] = 1;
                handicaps[15, 9] = 1;
            }
            if (count >= 8)
            {
                handicaps[9, 3] = 1;
                handicaps[9, 15] = 1;
            }
            if (count == 6 || count == 8) handicaps[9, 9] = 0;

            return handicaps;
        }
    }
}
