using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGoban.Game;

namespace CGoban.RuleSets
{
    class ChineseRules : RuleSet
    {
        public override string GetName()
        {
            throw new NotImplementedException();
        }

        public override int GetScore(Board board, Space.Color color)
        {
            throw new NotImplementedException();
        }

        public override bool IsLegal(Board board, int x, int y, Space.Color color)
        {
            throw new NotImplementedException();
        }

        public override int[,] GetHandicaps(Game.Game game, int count)
        {
            throw new NotImplementedException();
        }
    }
}
