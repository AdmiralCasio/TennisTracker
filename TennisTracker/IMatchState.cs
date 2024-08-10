using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TennisTracker
{
    public interface IMatchState
    {
        public void Point(Player player, PointType p);

        public void Loss(Player playerWin, Player playerLose, PointType p);

        public void CheckGameWin(Player playerWin, Player playerLose);

    }

    internal class NormalState : IMatchState
    {

        private Match _match;

        public NormalState(Match match)
        {
            this._match = match;
        }

        public void CheckGameWin(Player playerWin, Player playerLose)
        {
            if (playerWin.GamePoints == 4)
            {
                _match.GameWin(playerWin, playerLose);
            }
        }

        public void Loss(Player playerWin, Player playerLose, PointType p)
        {
            playerWin.WinPoint(p);
            playerLose.LosePoint(p);
            IsDeuce();
            _match.CheckGameWin(playerWin, playerLose);
        }

        public void Point(Player player, PointType p)
        {
            player.WinPoint(p);
            IsDeuce();
        }

        private void IsDeuce()
        {
            if (_match.Player1.GamePoints == 3 && _match.Player2.GamePoints == 3)
            {
                _match.ChangeState(new DeuceState(_match));
            }
        }
    }

    internal class DeuceState : IMatchState
    {
        private Match _match;

        public DeuceState(Match match)
        {
            this._match = match;
        }

        public void CheckGameWin(Player playerWin, Player playerLose)
        {
            if (playerWin.GamePoints == 5)
            {
                _match.ChangeState(new NormalState(_match));
                _match.GameWin(playerWin, playerLose);
            }
        }

        public void Loss(Player playerWin, Player playerLose, PointType p)
        {
            if (playerWin.GamePoints == 4)
            {
                playerWin.WinPoint(p);
                playerLose.LosePoint(p);
                CheckGameWin(playerWin, playerLose);
            }
            else if (playerWin.GamePoints == 3 && playerLose.GamePoints == 3)
            {
                playerWin.WinPoint(p);
            }
            else if (playerWin.GamePoints == 3 && playerLose.GamePoints == 4)
            {
                playerLose.LosePointDeuce(p);
                playerWin.WinPointDeuce(p);
            }
        }

        public void Point(Player player, PointType p)
        {
            Loss(player, player == _match.Player1 ? _match.Player2 : _match.Player1, p);
        }
    }

    internal class TieBreakState : IMatchState
    {
        private Match _match;

        public TieBreakState(Match match)
        {
            this._match = match;
        }

        public void CheckGameWin(Player playerWin, Player playerLose)
        {
            if (playerWin.GamePoints >= 7 && playerWin.GamePoints - playerLose.GamePoints >= 2)
            {
                _match.GameWin(playerWin, playerLose);
            }
        }

        public void Loss(Player playerWin, Player playerLose, PointType p)
        {
            playerWin.WinPoint(p);
            playerLose.LosePoint(p);
        }

        public void Point(Player player, PointType p)
        {
            player.WinPoint(p);
        }
    }

}
