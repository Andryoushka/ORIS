using System.ComponentModel;

namespace ZombieCats.Controls;

public partial class PlayerView : ContentView, INotifyPropertyChanged
{
	public PlayerView()
	{
		BindingContext = this;
		InitializeComponent();
	}

    public event PropertyChangedEventHandler PropertyChanged;

    private string _name = "Player";
	public string PlayerName
	{
		get
		{
			return _name;
		}

		set
		{
			_name = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PlayerName"));
        }
	}

	private Brush _color = Brush.Grey;
	private bool _isDead = false;

    public Brush PlayerBorderColor
	{
		get { return _color; }
		set
		{
			_color = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PlayerBorderColor"));
        }
    }

	public void ChangeColor(Brush brush)
	{
		if (_isDead)
			return;

        PlayerBorderColor = brush;
    }

	public void Dead()
	{
        _isDead = true;
    }
}