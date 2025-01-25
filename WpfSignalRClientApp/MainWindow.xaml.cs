using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.AspNetCore.SignalR.Client;

namespace WpfSignalRClientApp;

public record struct MyColor(byte R, byte G, byte B);
public partial class MainWindow : Window
{
    private readonly HubConnection _hub;
    private readonly MyColor _userColor;
    public MainWindow()
    {
        InitializeComponent();
        
        var random = new Random();
            
        _userColor = new MyColor(
            (byte)random.Next(256), 
            (byte)random.Next(256), 
            (byte)random.Next(256));
            
        while (_userColor is { R: > 240, G: > 240, B: > 240 })
        {
            _userColor.R = (byte)random.Next(255);
        }
            
        _hub = new HubConnectionBuilder().WithUrl("https://localhost:7244/chat").Build();

        _hub.On<string, string, MyColor>("Receive", (user, message, color) =>
        {
            Dispatcher.Invoke(() =>
            {
                var textColor = Color.FromRgb(color.R, color.G, color.B);
                var item = new ListBoxItem
                {
                    Content = $"{user}: {message}",
                    Foreground = new SolidColorBrush(textColor)

                };
                ChatRoom.Items.Add(item);
            });
        });

        _hub.On<string>("Notify", (message) =>
        {
            Dispatcher.Invoke(() => ChatRoom.Items.Add(message));
        });
    }

    private async void btnSend_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await _hub.InvokeAsync("SendMessage",User.Text, Message.Text, _userColor);
            Message.Text = "";
        }
        catch(Exception ex)
        {
            ChatRoom.Items.Add(ex.Message);
        }
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            await _hub.StartAsync();
            BtnSend.IsEnabled = true;
        }
        catch(Exception ex)
        {
            ChatRoom.Items.Add(ex.Message);
        }
    }
}