using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PNGBuddy
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly MainWindow _MainWindow;

        public SettingsWindow(MainWindow mainWindow, ObservableCollection<string> inputs)
        {
            _MainWindow = mainWindow;
            InitializeComponent();
            InputBox.ItemsSource = inputs;
            var mic = inputs.Where(i => i == Settings.Default.MIC_NAME).FirstOrDefault();

            if (mic != null)
            {
                InputBox.SelectedItem = mic;
            }

            if (Settings.Default.BACKGROUND_COLOR == Colors.Transparent)
            {
                TransparentButton.IsChecked = true;
            } else
            {
                CustomColor.IsChecked = true;
            }

            IdlePicture.Text = Settings.Default.IDLE_PICTURE;
            SpeakingPicture.Text = Settings.Default.CHAT_PICTURE;
            BlinkingPicture.Text = Settings.Default.BLINK_PICTURE;
            BlinkSpeakPicture.Text = Settings.Default.BLINK_SPEAK_PICTURE;

        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnMinButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void IdleSelectFile(object sender, RoutedEventArgs e)
        {
            var file = SelectFile(sender, e);

            if (file != null)
            {
                IdlePicture.Text = file;
                Settings.Default.IDLE_PICTURE = file;
            }
        }

        private void BlinkSelectFile(object sender, RoutedEventArgs e)
        {
            var file = SelectFile(sender, e);

            if (file != null)
            {
                BlinkingPicture.Text = file;
                Settings.Default.BLINK_PICTURE = file;
            }
        }

        private void SpeakSelectFile(object sender, RoutedEventArgs e)
        {
            var file = SelectFile(sender, e);

            if (file != null)
            {
                SpeakingPicture.Text = file;
                Settings.Default.CHAT_PICTURE = file;
            }
        }

        private void BlinkSpeakSelectFile(object sender, RoutedEventArgs e)
        {
            var file = SelectFile(sender, e);

            if (file != null)
            {
                BlinkSpeakPicture.Text = file;
                Settings.Default.BLINK_SPEAK_PICTURE = file;
            }
        }

        private static string SelectFile(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new()
            {
                // Set filter for file extension and default file extension 
                DefaultExt = ".png",
                Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif"
            };


            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                return dlg.FileName;
            }

            return string.Empty;
        }

        private void NumericInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextAllowed(e.Text);
        }

        private static readonly Regex _regex = new("[^0-9-.]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return _regex.IsMatch(text);
        }

        private void InputBox_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (InputBox.SelectedItem == null) return;

            Settings.Default.MIC_NAME = InputBox.SelectedItem.ToString();
        }

        private async void GrayScaleCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            await _MainWindow.Dispatcher.InvokeAsync(() =>
            {
                if (_MainWindow.Muted)
                {
                    _MainWindow.Ani.Visibility = Visibility.Hidden;
                    _MainWindow.IdleGrayScale.Visibility = Visibility.Visible;
                }
            });
        }

        private async void GrayScaleCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            await _MainWindow.Dispatcher.InvokeAsync(() =>
            {
                _MainWindow.Ani.Visibility = Visibility.Visible;
                _MainWindow.IdleGrayScale.Visibility = Visibility.Hidden;
            });
        }

        private static readonly Regex _regex2 = new("[^0-9]+"); //regex that matches disallowed text

        private void BlinkDuration_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex2.IsMatch(e.Text);
        }

        private void BlinkInterval_TextInput(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(BlinkInterval.Text)) return;

            Settings.Default.BLINK_INTERVAL = int.Parse(BlinkInterval.Text);
            _MainWindow.BlinkTimer.Interval = TimeSpan.FromMilliseconds(int.Parse(BlinkInterval.Text));
        }

        private void BlinkDuration_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(BlinkDuration.Text)) return;

            Settings.Default.BLINK_DURATION = int.Parse(BlinkDuration.Text);
        }

        private void BlinkingCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.SHOULD_BLINK = (bool)BlinkingCheckBox.IsChecked!;
        }

        private void BlinkSpeakCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.SHOULD_BLINK_SPEAK = (bool)BlinkSpeakCheckBox.IsChecked!;
        }

        private void TransparentButton_Checked(object sender, RoutedEventArgs e)
        {
            _MainWindow.Background = Brushes.Transparent;
        }

        private void CustomColor_Checked(object sender, RoutedEventArgs e)
        {
            if (ColorPicker == null) return;

            var color = ColorPicker.Color;
            _MainWindow.Background = new SolidColorBrush(color);
        }

        private void TextBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender == null) return;

            Settings.Default.WEBSOCKET_PORT = WSPort.Text;
        }

        private void WSPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Settings.Default.WEBSOCKET_PASSWORD = WSPassword.Password;
        }

        private void WSPassword_Loaded(object sender, RoutedEventArgs e)
        {
            WSPassword.Password = Settings.Default.WEBSOCKET_PASSWORD;
        }
    }
}
