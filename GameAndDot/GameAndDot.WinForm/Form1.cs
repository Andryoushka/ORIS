using GameAndDot.Shared.Models;
using System.Net.Sockets;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using GameAndDot.Shared.Enums;
using System.Text.Json;
using System.Net.Http;
using XProtocol.shared;
using XProtocol.Serializator;
using XProtocol;
using XProtocol.Message;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Windows.Forms;
using GameAndDot.WinForm.Models;
using System.Text.Unicode;
using System.Text;

namespace GameAndDot.WinForm;

public partial class Form1 : Form
{
    readonly string host = "127.0.0.1";
    readonly int port = 4910; // 8888
    private string userName = "anonimus";
    private static int _handshakeMagic;
    private Random _random = new Random();


    // <_ Old Realization _>
    /*private readonly TcpClient _tcpClient;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;*/

    private readonly XClient _client;

    private Bitmap gameSurface;
    private Graphics gfx;
    public Form1()
    {
        InitializeComponent();

        // <!using?>
        /*_tcpClient = new TcpClient();
        _tcpClient.Connect(host, port); //подключение клиента
        _reader = new StreamReader(_tcpClient.GetStream());
        _writer = new StreamWriter(_tcpClient.GetStream());*/
        _client = new XClient();
        _client.OnPacketRecieve += OnPacketRecieve;
        _client.Connect(host, port);

        gameSurface = new Bitmap(panel1.Width, panel1.Height);
        gfx = Graphics.FromImage(gameSurface);
        gfx.Clear(Color.White); // фон

        // Перерисовываем панель из битмапа при изменении размера или Paint
        panel1.Paint += (s, e) => e.Graphics.DrawImage(gameSurface, 0, 0);

        panel1.BackColor = Color.White;
        //panel1.Dock = DockStyle.Fill;
        panel1.BorderStyle = BorderStyle.FixedSingle;
        panel1.Visible = false;

        usernameLbl.Visible = false;
        colorLbl.Visible = false;
        label2.Visible = false;
        label5.Visible = false;
        listBox1.Visible = false;
    }

    private async void button1_Click(object sender, EventArgs e)
    {
        userName = textBox1.Text;

        label1.Visible = false;
        textBox1.Visible = false;
        button1.Visible = false;

        usernameLbl.Text = userName;
        usernameLbl.Visible = true;

        panel1.Visible = true;
        colorLbl.Visible = true;
        label2.Visible = true;
        label5.Visible = true;
        listBox1.Visible = true;

        try
        {
            //if (_writer is null || _reader is null) return;
            // запускаем новый поток для получения данных
            //Task.Run(() => ReceiveMessageAsync());

            var message = new XEventMessage
            {
                //Type = EventType.PlayerConnected,
                PlayerName = userName,
            };

            var packet = XPacketConverter.Serialize(XPacketType.PlayerConnected, message)
                .ToPacket();
            _client.QueuePacketSend(packet); // <- Отправка

            /*var json = JsonSerializer.Serialize(message);
            // запускаем ввод сообщений
            await SendMessageAsync(json);*/
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        //_writer?.Close();
        //_reader?.Close();
    }

    // отправка сообщений
    async Task SendMessageAsync(string message)
    {
        // сначала отправляем имя
        //await _writer.WriteLineAsync(message);
        //await _writer.FlushAsync();
        _handshakeMagic = _random.Next();

        _client.QueuePacketSend(
                XPacketConverter.Serialize(
                    XPacketType.Handshake,
                    new XPacketHandshake
                    {
                        MagicHandshakeNumber = _handshakeMagic
                    })
                    .ToPacket());
    }

    // получение сообщений
    /*async Task ReceiveMessageAsync()
    {
        while (true)
        {
            try
            {
                // получаем имя пользователя
                string json = await _reader.ReadLineAsync();
                var message = JsonSerializer.Deserialize<EventMessage>(json);

                switch (message.Type)
                {
                    case Shared.Enums.EventType.PlayerConnected:

                        Invoke(() =>
                        {
                            listBox1.Items.Clear();
                            foreach (var playerName in message.Players)
                            {
                                listBox1.Items.Add(playerName);
                            }
                        });
                        break;

                    case Shared.Enums.EventType.PlayerDisconnected:
                        Invoke(() =>
                        {
                            if (listBox1.Items.Contains(message.PlayerName))
                                listBox1.Items.Remove(message.PlayerName);
                        });
                        break;

                    case Shared.Enums.EventType.PointPlased:
                        Invoke(() =>
                        {
                            // Рисуем чужую точку (например, синюю)
                            var pointColor = message.PlayerName == userName
                                ? Color.Red
                                : Color.Blue;

                            DrawPoint(message.X, message.Y, pointColor);
                        });
                        break;

                    default:
                        break;
                }
            }
            catch
            {
                break;
            }
        }
    }
*/

    // Выход пользователя (disconnected)
    private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {

        try
        {
            //if (_writer is null || _reader is null) return;
            // запускаем новый поток для получения данных
            //Task.Run(() => ReceiveMessageAsync());

            var message = new EventMessage
            {
                Type = EventType.PlayerDisconnected,
                PlayerName = userName,
            };

            var json = JsonSerializer.Serialize(message);

            // запускаем ввод сообщений
            await SendMessageAsync(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return;
    }

    private async void panel1_MouseClick(object sender, MouseEventArgs e)
    {
        if (string.IsNullOrEmpty(userName) || userName == "anonimus") return;

        var message = new EventMessage
        {
            Type = EventType.PointPlased,
            PlayerName = userName,
            X = e.X,   // <- координата по клику
            Y = e.Y,
            // Можно добавить цвет, если нужно
            Color = colorLbl.ForeColor.Name // например, если colorLbl хранит выбранный цвет
        };

        var m = new XEventMessage()
        {
            Color = colorLbl.ForeColor.Name,
            X = e.X,   // <- координата по клику
            Y = e.Y,
            PlayerName = userName,
        };
        var packet = XPacketConverter.Serialize(XPacketType.PointPlased ,m).ToPacket();
        _client.QueuePacketSend(packet);

        //var json = JsonSerializer.Serialize(message);
        //await SendMessageAsync(json);

        // Локально тоже рисуем точку (для мгновенного отклика)
        DrawPoint(e.X, e.Y, Color.Red); // например, красная — своя точка
    }

    private void DrawPoint(int x, int y, Color color)
    {
        using var brush = new SolidBrush(color);
        gfx.FillEllipse(brush, x - 5, y - 5, 10, 10); // круг радиусом 5
        panel1.Invalidate(); // запросить перерисовку
    }

    private void OnPacketRecieve(byte[] packet)
    {
        var parsed = XPacket.Parse(packet);

        if (parsed != null)
        {
            ProcessIncomingPacket(parsed);
        }
    }

    private void ProcessIncomingPacket(XPacket packet)
    {
        var type = XPacketTypeManager.GetTypeFromPacket(packet);

        var message = XPacketConverter.Deserialize<XEventMessage>(packet);

        switch (type)
        {
            case XPacketType.Handshake:
                ProcessHandshake(packet);
                break;
            case XPacketType.Unknown:
                break;
            case XPacketType.PlayerConnected:
                Invoke(() =>
                {
                    listBox1.Items.Clear();
                    foreach (var playerName in message.PlayersString.Split('/'))
                    {
                        listBox1.Items.Add(playerName);
                    }
                });
                break;
            case XPacketType.PlayerDisconnected:
                Invoke(() =>
                {
                    if (listBox1.Items.Contains(message.PlayerName))
                        listBox1.Items.Remove(message.PlayerName);
                });
                break;
            case XPacketType.PointPlased:
                Invoke(() =>
                {
                    // Рисуем чужую точку (например, синюю)
                    var pointColor = message.PlayerName == userName
                        ? Color.Red
                    : Color.Blue;

                    DrawPoint(message.X, message.Y, pointColor);
                });
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void ProcessHandshake(XPacket packet)
    {
        var handshake = XPacketConverter.Deserialize<XPacketHandshake>(packet);

        if (_handshakeMagic - handshake.MagicHandshakeNumber == 15)
        {
            Console.WriteLine("Handshake successful!");
        }
    }
}
