using AutoUpdaterDotNET;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualBasic;
using PNGBuddy.Messages;
using PNGBuddy.Messages.Hello;
using PNGBuddy.Types;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PNGBuddy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClientWebSocket? Socket;

        public ObservableCollection<string> Inputs = new();

        public DispatcherTimer BlinkTimer;

        public bool Speaking { get; set; } = false;
        public bool Blinking { get; set; } = false;
        public bool Muted { get; set; } = false;

        public ShakeBehavior ShakeBehavior = new();

        private SettingsWindow? SettingsWindow { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            BlinkTimer = new() { Interval = TimeSpan.FromMilliseconds(Settings.Default.BLINK_INTERVAL) };

            BlinkTimer.Tick += async (o, e) =>
            {
                if (Settings.Default.SHOULD_BLINK == false) return;

                if (Blinking || Muted) return;
                Blinking = true;
                await Task.Delay(Settings.Default.BLINK_DURATION);
                Blinking = false;
            };

            BlinkTimer.Start();

            Initialize();
        }

        private async void Initialize()
        {
            Socket = new();

            try
            {
                await Socket.ConnectAsync(new Uri($"ws://localhost:{Settings.Default.WEBSOCKET_PORT}"), CancellationToken.None);
                byte[] data = new byte[1024];

                await Task.Run(async () =>
                {
                    while (true)
                    {
                        WebSocketReceiveResult result;
                        string json = "";
                        do
                        {
                            result = await Socket.ReceiveAsync(new ArraySegment<byte>(data), CancellationToken.None);
                            json += Encoding.UTF8.GetString(data, 0, result.Count);
                            // Trace.WriteLine("Received: " + result.MessageType + ", " + result.Count + ", " + result.EndOfMessage);
                        } while (!result.EndOfMessage);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            // Trace.WriteLine("Websocket Closed");
                            Socket = null;
                            break;
                        }
                        else
                        {
                            await HandleMessage(JsonSerializer.Deserialize<WebSocketMessage>(json)!);
                        }
                    }
                });

                Socket = null;
                await Task.Delay(5000).ContinueWith((e) => { Initialize(); });
            }
            catch
            {
                Socket = null;
                await Task.Delay(5000).ContinueWith((e) => { Initialize(); });
            }
        }

        private async Task HandleMessage(WebSocketMessage? message)
        {
            switch (message?.OpCode)
            {
                // Hello!
                case 0:
                    var hello = JsonSerializer.Deserialize<Hello>(message.Data?.ToString()!);
                    if (hello?.Authentication != null)
                    {
                        var salt = hello?.Authentication.Salt;
                        var challenge = hello?.Authentication.Challenge;

                        var secret = HashEncode(Settings.Default.WEBSOCKET_PASSWORD + salt);
                        var auth = HashEncode(secret + challenge);

                        var authMessage = new { op = 1, d = new { rpcVersion = hello?.RpcVersion, authentication = auth, eventSubscriptions = 1 << 16 | 1 << 3 } };

                        await Socket!.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(authMessage)), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    else
                    {
                        var authMessage = new { op = 1, d = new { rpcVersion = hello?.RpcVersion, eventSubscriptions = 1 << 16 | 1 << 3 } };

                        await Socket!.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(authMessage)), WebSocketMessageType.Text, true, CancellationToken.None);
                    }

                    break;

                // Identified
                case 2:
                    await Identified();
                    break;
                case 5:
                    var ev = JsonSerializer.Deserialize<Event>(message.Data?.ToString()!);

                    switch (ev?.Type)
                    {
                        case "InputVolumeMeters":
                            {
                                var inputs = JsonSerializer.Deserialize<Input>(ev?.Data?.ToString()!);
                                var mic = inputs?.Inputs?.SingleOrDefault(i => i.Name == Settings.Default.MIC_NAME);
                                if (mic != null && !Muted)
                                {
                                    await Dispatcher.InvokeAsync(() =>
                                    {
                                        var currentVolume = 20f * Math.Log10(mic.InputLevelsMul![0][2]);
                                        if (currentVolume > Settings.Default.MIC_THRESHOLD)
                                        {
                                            Speaking = true;
                                            /*if (currentVolume > -20)
                                            {
                                                Chat.Visibility = Visibility.Hidden;
                                                ShakePicture.Visibility = Visibility.Visible;
                                            } else
                                            {*/
                                            ShakePicture.Visibility = Visibility.Hidden;

                                            if (Blinking)
                                            {
                                                BlinkSpeakPicture.Visibility = Visibility.Visible;
                                                Chat.Visibility = Visibility.Hidden;
                                            }
                                            else
                                            {
                                                Chat.Visibility = Visibility.Visible;
                                                BlinkSpeakPicture.Visibility = Visibility.Hidden;
                                            }
                                            //}
                                            Idle.Visibility = Visibility.Hidden;
                                        }
                                        else
                                        {
                                            Speaking = false;
                                            ShakePicture.Visibility = Visibility.Hidden;
                                            Chat.Visibility = Visibility.Hidden;

                                            if (Blinking)
                                            {
                                                BlinkSpeakPicture.Visibility = Visibility.Hidden;
                                                BlinkPicture.Visibility = Visibility.Visible;
                                            }
                                            else
                                            {
                                                BlinkSpeakPicture.Visibility = Visibility.Hidden;
                                                BlinkPicture.Visibility = Visibility.Hidden;
                                            }
                                            Idle.Visibility = Visibility.Visible;
                                        }
                                    });
                                }
                            }
                            break;
                        case "InputMuteStateChanged":
                            {
                                var input = JsonSerializer.Deserialize<InputMuteStateChanged>(ev.Data?.ToString()!);

                                if (Settings.Default.MIC_NAME != null && Settings.Default.MIC_NAME == input?.InputName)
                                {
                                    Muted = (bool)input.InputMuted!;

                                    if (!Settings.Default.MUTED_GRAYSCALE) return;

                                    await Dispatcher.InvokeAsync(() =>
                                    {
                                        if (Muted)
                                        {
                                            Idle.Visibility = Visibility.Hidden;
                                            IdleGrayScale.Visibility = Visibility.Visible;
                                        }
                                        else
                                        {
                                            Idle.Visibility = Visibility.Visible;
                                            IdleGrayScale.Visibility = Visibility.Hidden;
                                        }
                                    });
                                }
                            }
                            break;
                        case "InputCreated":
                            {
                                var input = JsonSerializer.Deserialize<InputCreated>(ev.Data?.ToString()!);
                                if (input?.InputKind == "wasapi_input_capture")
                                {
                                    await Dispatcher.InvokeAsync(() =>
                                    {
                                        Inputs.Add(input?.InputName!);
                                    });
                                }
                            }
                            break;
                        case "InputRemoved":
                            {
                                var input = JsonSerializer.Deserialize<InputRemoved>(ev?.Data?.ToString()!);
                                if (Inputs.Contains(input?.InputName!))
                                {
                                    await Dispatcher.InvokeAsync(() =>
                                    {
                                        Inputs.Remove(input?.InputName!);
                                    });
                                }
                            }
                            break;
                        case "InputNameChanged":
                            {
                                var input = JsonSerializer.Deserialize<InputNameChanged>(ev.Data?.ToString()!);
                                if (Inputs.Contains(input?.OldInputName!))
                                {
                                    await Dispatcher.InvokeAsync(() =>
                                    {
                                        Inputs.Remove(input?.OldInputName!);
                                        Inputs.Add(input?.InputName!);
                                    });
                                }
                            }
                            break;
                    }
                    break;

                // Request Response
                case 7:
                    var res = JsonSerializer.Deserialize<Response>(message.Data?.ToString()!);

                    switch (res?.Type)
                    {
                        case "GetInputList":
                            var inputs = JsonSerializer.Deserialize<Input>(res.Data?.ToString()!);
                            var audio = inputs?.Inputs?.Where(i => i.Kind == "wasapi_input_capture").ToList();

                            await Dispatcher.InvokeAsync(() =>
                            {
                                for (var i = 0; i < audio?.Count; i++) Inputs.Add(audio[i].Name!);
                            });

                            break;
                    }

                    break;

            }
        }

        private async Task Identified()
        {
            var inputMessage = new { op = 6, d = new { requestType = "GetInputList", requestId = "Idle" } };

            await Socket!.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(inputMessage)), WebSocketMessageType.Text, true, CancellationToken.None);

        }

        internal static string HashEncode(string input)
        {
            using var sha256 = SHA256.Create();

            byte[] textBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = sha256.ComputeHash(textBytes);

            return Convert.ToBase64String(hash);
        }

        internal class WebSocketMessage
        {
            [JsonPropertyName("op")]
            public int OpCode { get; set; }

            [JsonPropertyName("d")]
            public object? Data { get; set; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsWindow == null)
            {
                SettingsWindow = new(this, Inputs);
                SettingsWindow.Closed += (o, e) =>
                {
                    SettingsWindow = null;
                };

                SettingsWindow.Show();
            }
            else
            {
                SettingsWindow.Activate();
            }
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnMinButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Idle_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (Idle.Source == null) return;

            var bitmapImage = new BitmapImage(new Uri(Settings.Default.IDLE_PICTURE));
            // Convert it to Gray
            IdleGrayScale.Source = new FormatConvertedBitmap(bitmapImage, PixelFormats.Gray32Float, null, 0);
            // Create Opacity Mask for greyscale image as FormatConvertedBitmap does not keep transparency info
            IdleGrayScale.OpacityMask = new ImageBrush(bitmapImage);
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize();
        }
    }
}
