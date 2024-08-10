using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Web;

namespace TennisTracker
{
    public partial class MainPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
    {

        private SolidColorBrush markerWhiteColor;
        private Match _currentMatch;
        
        private Match CurrentMatch
        {
            get => _currentMatch;
            set
            {
                _currentMatch = value;
                NewMatch(_currentMatch);
            }
        }

        private Dictionary<string, PointType> PointTypeMappings = new()
        {
            {  "D. Fault", PointType.DFAULT },
            { "F. Error", PointType.FERROR },
            { "Unf. Error", PointType.UFERROR },
            { "Ace", PointType.ACE },
            { "Winner", PointType.WINNER }
        };
        private Dictionary<int, string> PointMappings = new()
        {
            { 0, "0" },
            { 1, "15" },
            { 2, "30" },
            { 3, "40" },
            { 4, "AD" }
        };
        
        private ScoreDisplay[]? setScores;

        private Stopwatch Stopwatch = new();

        
        public MainPage()
        {
            InitializeComponent();
            markerWhiteColor = new SolidColorBrush(Color.FromArgb("#FCFCFC"));
            P1ServeMarker.Stroke = SolidColorBrush.Green;
            if (CurrentMatch == null) CurrentMatch = new Match(new Player("Player 1"), new Player("Player 2"), 6, 3);
        }

        public void NewMatch(Match newMatch)
        {
            Stopwatch.Reset();
            if (Application.Current != null) Application.Current.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        GameTimer.Text = FormatElapsedTime(Stopwatch.Elapsed);
                    });
                });
                return Stopwatch.IsRunning;
            });

            Debug.WriteLine(CurrentMatch.Service);
            P1ScorerName.Text = newMatch.Player1.Name;
            P2ScorerName.Text = newMatch.Player2.Name;
            P2Name.Text = newMatch.Player2.Name;
            P1Name.Text = newMatch.Player1.Name;
            GameTitle.Text = $"{newMatch.Player1.Name} vs {newMatch.Player2.Name}";
            double maxWidth = Math.Max(newMatch.Player1.Name.Length, newMatch.Player2.Name.Length);
            P1Name.WidthRequest = Math.Max(maxWidth * 7.25 + 20, 100);
            P2Name.WidthRequest = Math.Max(maxWidth * 7.25 + 20, 100);
            P1ServeMarker.Stroke = newMatch.Service == 0 ? Colors.Green : Colors.White;
            P2ServeMarker.Stroke = newMatch.Service == 1 ? Colors.Green : Colors.White;
            P1ServiceControls.Opacity = newMatch.Service == 0 ? 1 : 0;
            P2ServiceControls.Opacity = newMatch.Service == 1 ? 1 : 0;
            P1Set1.FontAttributes = FontAttributes.None;
            P2Set1.FontAttributes = FontAttributes.None;
            UpdateScoreDisplay((P1Set1, P2Set1), newMatch.Player1, newMatch.Player2);
            UpdateScoreDisplay((P1Score, P2Score), newMatch.Player1, newMatch.Player2);
            if (setScores != null)
            {
                for (int i = 1; i < setScores.Length; i++)
                {
                    P1ScoreCard.Remove(setScores[i].Border1);
                    P2ScoreCard.Remove(setScores[i].Border2);
                }
            }
            setScores = new ScoreDisplay[newMatch.Sets];
            setScores[0] = new ScoreDisplay((Border)P1Set1.Parent, (Border)P2Set1.Parent, P1Set1, P2Set1);
            for (int i = 1; i < setScores.Length; i++)
            {
                var P1Border = new Border();
                P1Border.StrokeThickness = 2;
                P1Border.StrokeLineJoin = Microsoft.Maui.Controls.Shapes.PenLineJoin.Round;
                P1Border.StrokeLineCap = Microsoft.Maui.Controls.Shapes.PenLineCap.Round;
                P1Border.Stroke = new SolidColorBrush(Color.FromArgb("#FFC0C0C0"));
                P1Border.MinimumHeightRequest = 30;
                P1Border.MinimumWidthRequest = 30;
                P1Border.BackgroundColor = Color.FromArgb("#FFC0C0C0");

                var P2Border = new Border();
                P2Border.StrokeThickness = 2;
                P2Border.StrokeLineJoin = Microsoft.Maui.Controls.Shapes.PenLineJoin.Round;
                P2Border.StrokeLineCap = Microsoft.Maui.Controls.Shapes.PenLineCap.Round;
                P2Border.Stroke = new SolidColorBrush(Color.FromArgb("#FFC0C0C0"));
                P2Border.MinimumHeightRequest = 30;
                P2Border.MinimumWidthRequest = 30;
                P2Border.BackgroundColor = Color.FromArgb("#FFC0C0C0");


                var P1Label = new Label();
                var P2Label = new Label();

                P1Label.Padding = 2;
                P1Label.HorizontalOptions = LayoutOptions.Center;
                P1Label.VerticalOptions = LayoutOptions.Center;
                P1Label.TextColor = Colors.Black;
                P1Label.Text = "0";

                P2Label.Padding = 2;
                P2Label.HorizontalOptions = LayoutOptions.Center;
                P2Label.VerticalOptions = LayoutOptions.Center;
                P2Label.TextColor = Colors.Black;
                P2Label.Text = "0";

                setScores[i] = new ScoreDisplay(P1Border, P2Border, P1Label, P2Label);
                
            }
            ForceLayout();

        }

        public void ChangeServer()
        {
            if (CurrentMatch.Service == 1)
            {
                P1ServeMarker.Stroke = markerWhiteColor;
                P2ServeMarker.Stroke = SolidColorBrush.Green;
            }
            else
            {
                P1ServeMarker.Stroke = SolidColorBrush.Green;
                P2ServeMarker.Stroke = markerWhiteColor;
            }
        }

        public void Point(object o, EventArgs e)
        {
            
            if (!CurrentMatch.MatchWon)
            {
                StartTimer();
       
                Button button = (Button)o;

                if (((HorizontalStackLayout)button.Parent).Opacity == 0) return;

                if (button.Parent.Parent == P1Scorer)
                {
                    CurrentMatch.Point(CurrentMatch.Player1, PointTypeMappings[button.Text]);
                    UpdateScoreDisplay((P1Score, P2Score), CurrentMatch.Player1, CurrentMatch.Player2);
                }
                else
                {
                    CurrentMatch.Point(CurrentMatch.Player2, PointTypeMappings[button.Text]);
                    UpdateScoreDisplay((P2Score, P1Score), CurrentMatch.Player2, CurrentMatch.Player1);
                }

                if (CurrentMatch.SetWon)
                {
                    UpdateGameScoreDisplay(setScores[GetTotalSets() - 1], CurrentMatch.GameScores[GetTotalSets() - 1].Item1, CurrentMatch.GameScores[GetTotalSets() - 1].Item2);

                    if (button.Parent.Parent == P1Scorer)
                    {
                        setScores[GetTotalSets() - 1].Label1.FontAttributes = FontAttributes.Bold;
                    }
                    else
                    {
                        setScores[GetTotalSets() - 1].Label2.FontAttributes = FontAttributes.Bold;
                    }
                }

                CheckNewGame();

                CheckNewSet();

                CheckMatchWon();

                if (P1Fault.Text == "D. Fault" || P2Fault.Text == "D. Fault")
                {
                    ResetFault();
                }
                
            }

        }

        public void Loss(object o, EventArgs e)
        {

            if (!CurrentMatch.MatchWon)
            {
                StartTimer();

                Button button = (Button)o;

                if (((HorizontalStackLayout)button.Parent).Opacity == 0) return;

                if (button.Parent.Parent == P1Scorer)
                {
                    CurrentMatch.Loss(CurrentMatch.Player2, CurrentMatch.Player1, PointTypeMappings[button.Text]);
                    UpdateScoreDisplay((P2Score, P1Score), CurrentMatch.Player2, CurrentMatch.Player1);
                }
                else
                {
                    CurrentMatch.Loss(CurrentMatch.Player1, CurrentMatch.Player2, PointTypeMappings[button.Text]);
                    UpdateScoreDisplay((P1Score, P2Score), CurrentMatch.Player1, CurrentMatch.Player2);
                }

                if (CurrentMatch.SetWon)
                {
                    UpdateGameScoreDisplay(setScores[GetTotalSets() - 1], CurrentMatch.GameScores[GetTotalSets() - 1].Item1, CurrentMatch.GameScores[GetTotalSets() - 1].Item2);

                    if (button.Parent.Parent == P2Scorer)
                    {
                        setScores[GetTotalSets() - 1].Label1.FontAttributes = FontAttributes.Bold;
                    }
                    else
                    {
                        setScores[GetTotalSets() - 1].Label2.FontAttributes = FontAttributes.Bold;
                    }
                }

                CheckNewGame();

                CheckNewSet();

                CheckMatchWon();

                ResetFault();

            }
        }

        public void Fault(object o, EventArgs e)
        {
            Button button = (Button)o;

            if (((HorizontalStackLayout)button.Parent).Opacity == 0) return;

            if (button.Text == "Fault") button.Text = "D. Fault";
            else { Loss(o, e); button.Text = "Fault"; }
        }

        private void UpdateScoreDisplay((Label, Label) scoreDisplay, Player player, Player otherPlayer)
        {
            if (_currentMatch.matchState.GetType() == typeof(TieBreakState))
            {
                scoreDisplay.Item1.Text = player.GamePoints.ToString();
                scoreDisplay.Item2.Text = otherPlayer.GamePoints.ToString();
            }
            else
            {
                scoreDisplay.Item1.Text = PointMappings[player.GamePoints];

                if (player.GamePoints == 4)
                {
                    scoreDisplay.Item2.Text = "";
                }
                else
                {
                    scoreDisplay.Item2.Text = PointMappings[otherPlayer.GamePoints];
                }
            }
        }

        private void UpdateGameScoreDisplay(ScoreDisplay scoreDisplay, int player1Score, int player2Score)
        {
            if (CurrentMatch.NewGame)
            {
                ChangeServer();
            }

            scoreDisplay.Label1.Text = player1Score.ToString();
            scoreDisplay.Label2.Text = player2Score.ToString();

            
        }

        private void UpdateScorers()
        {
            if (CurrentMatch.NewGame)
            {
                P1ServiceControls.Opacity = CurrentMatch.Service == 0 ? 1 : 0;
                P2ServiceControls.Opacity = CurrentMatch.Service == 1 ? 1 : 0;
            }
        }

        private int GetTotalSets()
        {
            return CurrentMatch.Player1.Sets + CurrentMatch.Player2.Sets;
        }

        private void CheckMatchWon()
        {
            if (CurrentMatch.MatchWon)
            {
                if (CurrentMatch.Winner != null)
                {
                    GameTitle.Text = $"{CurrentMatch.Winner.Name} def. {(CurrentMatch.Winner == CurrentMatch.Player1 ? CurrentMatch.Player2 : CurrentMatch.Player1).Name}";
                }
                StopTimer();
            }
        }

        private void CheckNewGame()
        {
            if (CurrentMatch.NewGame)
            {
                StopTimer();
                UpdateGameScoreDisplay(setScores[GetTotalSets()], CurrentMatch.Player1.Games, CurrentMatch.Player2.Games);
                UpdateScorers();
            }
        }

        private void CheckNewSet()
        {
            if (CurrentMatch.NewSet)
            {
                P1ScoreCard.Children.Insert(2 + GetTotalSets(), setScores[GetTotalSets()].Border1);
                P2ScoreCard.Children.Insert(2 + GetTotalSets(), setScores[GetTotalSets()].Border2);
                setScores[GetTotalSets()].ShowDisplay();
            }
        }

        private string FormatElapsedTime(TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"hh\:mm\:ss");
        }

        private void StartTimer()
        {
            if (!Stopwatch.IsRunning) Stopwatch.Start();
        }

        private void StopTimer()
        {
            if (Stopwatch.IsRunning) Stopwatch.Stop();
        }

        private void ResetFault()
        {
            if (P1Fault.Text == "D. Fault")
            {
                P1Fault.Text = "Fault";
            }
            if (P2Fault.Text == "D. Fault")
            {
               P2Fault.Text = "Fault";
            }
        }
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            CurrentMatch = (Match)query["match"];
        }
    }

}
