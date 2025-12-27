using System.ComponentModel;

namespace ZombieCats.Controls;

// класс Иконки Игрока
public partial class PlayerView : ContentView, INotifyPropertyChanged
{
	public PlayerView()
	{
		BindingContext = this;
		InitializeComponent();
	}

    public event PropertyChangedEventHandler PropertyChanged;

    private string _name = "Player"; // имя игрока
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

	private Brush _color = Brush.Grey; // цвет рамки
	private bool _isDead = false; // умер?

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
		// меняем цвет рамки, если игрок не умер
		if (_isDead)
			return;

        PlayerBorderColor = brush;
    }

	public void Dead()
	{
		// констатируем смэрть
        _isDead = true;
    }
}