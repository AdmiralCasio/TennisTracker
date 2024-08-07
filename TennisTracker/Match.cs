namespace TennisTracker
{
    public class Match
    {
        public Player Player1 { get; }
        public Player Player2 { get; }

        private int Games;
        public int Sets { get; private set; }
        public int CurrentSet { get; private set; }

        public bool Deuce { get; private set; } = false;

        public int Service { get; private set; } = 0;

        public bool NewGame { get; private set; } = true;
        public bool NewSet { get; private set; } = true;
        public bool SetWon { get; private set; } = false;
        public bool MatchWon { get; private set; } = false;

        public Player? Winner { get; private set; }

        public List<(int, int)> GameScores { get; private set; } = new();

        public Match(Player player1, Player player2, int games, int sets, int service = 0)
        {
            this.Player1 = player1;
            this.Player2 = player2;
            this.Games = games;
            this.Sets = sets;
            this.Service = service;
        }

        public void Point(Player player, PointType p)
        {
            NewPoint();

            if (Deuce)
            {
                Loss(player, player == Player1 ? Player2 : Player1, p);
            }
            else
            {
                player.WinPoint(p);
                IsDeuce();
            }

            CheckGameWin(player, player == Player1 ? Player2 : Player1);
        }

        public void Loss(Player playerWin, Player playerLose, PointType p)
        {
            NewPoint();
            if (!Deuce)
            {
                playerWin.WinPoint(p);
                playerLose.LosePoint(p);
                IsDeuce();
                CheckGameWin(playerWin, playerLose);
            }
            else
            {
                if (playerWin.GamePoints == 4)
                {
                    playerWin.WinPoint(p);
                    playerLose.LosePoint(p);
                    GameWin(playerWin, playerLose);
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
            
        }

        public void ChangeService()
        {
            Service = (Service + 1) % 2;
        }

        public void GameWin(Player playerWin, Player playerLose)
        {
            playerWin.WinGame();
            ResetGamePoints();
            NewGame = true;
            Deuce = false;
            CheckSetWin(playerWin);
            CheckMatchWin();
            if (MatchWon) Winner = playerWin;
            ChangeService();
        }

        private void CheckGameWin(Player playerWin, Player playerLose)
        {
            if (!Deuce && playerWin.GamePoints == 4)
            {
                GameWin(playerWin, playerLose);
            }
        }

        private void CheckMatchWin()
        {
            if (Player1.Sets == Sets || Player2.Sets == Sets)
            {
                MatchWon = true;
            }
        }

        private void CheckSetWin(Player playerWin)
        {
            if (playerWin.Games == Games)
            {
                GameScores.Add((Player1.Games, Player2.Games));
                playerWin.WinSet();
                if (Sets > playerWin.Sets)
                {
                    NewSet = true;
                }
                SetWon = true;
            }
        }

        private void IsDeuce()
        {
            if (!Deuce && Player1.GamePoints == 3 && Player2.GamePoints == 3)
            {
                Deuce = true;
            }
        }

        private void ResetGamePoints()
        {
            Player1.ResetGamePoints();
            Player2.ResetGamePoints();
        }

        private void NewPoint()
        {
            if (NewGame) NewGame = false;
            if (NewSet)
            {
                SetWon = false;
                NewSet = false;
            }
        }

        



    }
}
