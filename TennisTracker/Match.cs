namespace TennisTracker
{
    public class Match
    {
        public Player Player1 { get; }
        public Player Player2 { get; }

        private int Games;
        public int Sets { get; private set; }
        public int CurrentSet { get; private set; }

        public int Service { get; private set; } = 0;

        public bool NewGame { get; private set; } = true;
        public bool NewSet { get; private set; } = true;
        public bool SetWon { get; private set; } = false;
        public bool MatchWon { get; private set; } = false;

        public Player? Winner { get; private set; }

        public List<(int, int)> GameScores { get; private set; } = new();

        public IMatchState matchState { get; private set; }

        public Match(Player player1, Player player2, int games, int sets, int service = 0)
        {
            this.Player1 = player1;
            this.Player2 = player2;
            this.Games = games;
            this.Sets = sets;
            this.Service = service;
            this.matchState = new NormalState(this);
        }

        public void Point(Player player, PointType p)
        {
            NewPoint();
            matchState.Point(player, p);
            matchState.CheckGameWin(player, player == Player1 ? Player2 : Player1);
        }

        public void Loss(Player playerWin, Player playerLose, PointType p)
        {
            NewPoint();
            matchState.Loss(playerWin, playerLose, p);
            matchState.CheckGameWin(playerWin, playerLose);
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
            CheckSetWin(playerWin, playerLose);
            CheckMatchWin();
            if (MatchWon) Winner = playerWin;
            ChangeService();
        }

        public void CheckGameWin(Player playerWin, Player playerLose)
        {
            matchState.CheckGameWin(playerWin, playerLose);
        }

        private void CheckMatchWin()
        {
            if (Player1.Sets == Sets || Player2.Sets == Sets)
            {
                MatchWon = true;
                NewGame = false;
            }
        }

        private void CheckSetWin(Player playerWin, Player playerLose)
        {
            if (matchState.GetType() == typeof(TieBreakState))
            {
                GameScores.Add((Player1.Games, Player2.Games));
                playerWin.WinSet();
                playerLose.ResetGames();
                if (Sets > playerWin.Sets)
                {
                    NewSet = true;
                }
                SetWon = true;
                ChangeState(new NormalState(this));
            }

            if (playerWin.Games == Games && playerLose.Games == Games)
            {
                ChangeState(new TieBreakState(this));
            }
            
            if (playerWin.Games >= Games && playerWin.Games - playerLose.Games >= 2)
            {
                GameScores.Add((Player1.Games, Player2.Games));
                playerWin.WinSet();
                playerLose.ResetGames();
                if (Sets > playerWin.Sets)
                {
                    NewSet = true;
                }
                SetWon = true;
            }
        }

        public void ChangeState(IMatchState matchState)
        {
            this.matchState = matchState;
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
