using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGoban.Game;

namespace CGoban.RuleSets
{
    public interface IRuleSet
    {
        string GetName();
        int GetScore(Board board, Space.Color color);
        int[,] GetHandicaps(Game.Game game, int count);
        bool IsLegal(Board board, int x, int y, Space.Color color);
    }
}
