using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TennisTracker
{
    public class Player
    {
        public string Name { get; set; }
        public int TotalPoints { get; private set; }
        public int GamePoints { get; private set; }
        public int Games {  get; private set; }
        public int Sets { get; private set; }
        public int Aces { get; private set; }
        public int DoubleFaults { get; private set; }
        public int ForcedErrors { get; private set; }
        public int UnforcedErrors { get; private set; }



        public Player(string name)
        {
            TotalPoints = 0;
            Games = 0;
            Sets = 0;
            Games = 0;
            GamePoints = 0;
            Name = name;
        }

        public void WinPoint(PointType p)
        {
            this.GamePoints++;
            this.TotalPoints++;
            if (p == PointType.ACE) this.Aces++;
        }

        public void WinPointDeuce(PointType p)
        {
            this.TotalPoints++;
            if (p == PointType.ACE) this.Aces++;
        }

        public void LosePoint(PointType p)
        {
            switch (p)
            {
                case PointType.DFAULT:
                    this.DoubleFaults++; break;
                case PointType.FERROR:
                    this.ForcedErrors++; break;
                case PointType.UFERROR:
                    this.UnforcedErrors++; break;
            }

        }

        public void LosePointDeuce(PointType p)
        {
            LosePoint(p);
            GamePoints--;
        }

        public void WinGame()
        {
            this.Games++;
        }

        public void WinSet()
        {
            this.Sets++;
            ResetGames();
        }

        public void ResetGamePoints()
        {
            this.GamePoints = 0;
        }

        public void ResetGames()
        {
            this.Games = 0;
        }


    }
}
