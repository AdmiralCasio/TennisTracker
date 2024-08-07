using System.Diagnostics;

namespace TennisTracker;

public partial class NewMatch : ContentPage
{
	private Player _player1 = new Player("Player 1");
	private Player _player2 = new Player("Player 2");
	private int _service = 0;

	private int[] _setSetterValues = { 1, 3, 5 };
	private int[] _gameSetterValues = { 1, 2, 3, 4, 5, 6 };
	private int _setSetterIndex = 1;
	private int _gameSetterIndex = 5;

	private int SetSetterIndex
	{
		get
		{
			return _setSetterIndex;
		}

		set
		{
			if (value < _setSetterValues.Length && value >= 0)
			{
				_setSetterIndex = value;
			}
		}
	}

    private int GameSetterIndex
    {
        get
        {
            return _gameSetterIndex;
        }

        set
        {
            if (value < _gameSetterValues.Length && value >= 0)
            {
                _gameSetterIndex = value;
            }
        }
    }

    public NewMatch()
	{
		InitializeComponent();
		P1NameEntry.Text = _player1.Name;
		P2NameEntry.Text = _player2.Name;
		ChangeSets(SetSetterIndex);
		ChangeGames(GameSetterIndex);
	}

	void OnServerSelected(object o, EventArgs e)
	{
		var button = (ImageButton)o;

		if (button == P1ServeSwitch)
		{
			if (_service != 0)
			{

                P1ServeSwitch.Source = "ball.png";
                P2ServeSwitch.Source = "";
                _service = 0;
            }
			
        }
        else
        {
			if (_service != 1)
			{
				P1ServeSwitch.Source = "";
				P2ServeSwitch.Source = "ball.png";
				_service = 1;
			}
		}
    }

	void OnChangeSetterValue(object o, EventArgs e)
	{
		var pressed = (Button)o;

		if (pressed.Text == "+")
		{
			if (pressed.Parent == SetSetter)
			{
				SetSetterIndex++;
				ChangeSets(SetSetterIndex);
			}

			else if (pressed.Parent == GameSetter)
			{
				GameSetterIndex++;
				ChangeGames(GameSetterIndex);
			}
		}
		else
		{
            if (pressed.Parent == SetSetter)
            {
				SetSetterIndex--;
				ChangeSets(SetSetterIndex);
            }

            else if (pressed.Parent == GameSetter)
            {
				GameSetterIndex--;
				ChangeGames(GameSetterIndex);
            }
        }
	}

	void ChangeSets(int val)
	{
        ((Label)SetSetter.Children[1]).Text = _setSetterValues[val].ToString();
    }

	void ChangeGames(int val)
	{
        ((Label)GameSetter.Children[1]).Text = _gameSetterValues[val].ToString();
    }

	public async void GoToMain(object o, EventArgs e)
	{
        var navigationParameter = new ShellNavigationQueryParameters
		{
			{ "match", new Match(_player1, _player2, _gameSetterValues[GameSetterIndex], _setSetterValues[SetSetterIndex], _service) }
		};
        await Shell.Current.GoToAsync("//MainPage", navigationParameter);
    }
}