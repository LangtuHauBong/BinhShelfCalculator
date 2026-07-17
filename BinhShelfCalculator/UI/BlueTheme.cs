using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BinhShelfCalculator.UI
{
    public static class BlueTheme
    {
        public static readonly Brush Blue = new SolidColorBrush(Color.FromRgb(18, 97, 178));
        public static readonly Brush DarkBlue = new SolidColorBrush(Color.FromRgb(10, 54, 112));
        public static readonly Brush LightBlue = new SolidColorBrush(Color.FromRgb(232, 243, 255));
        public static readonly Brush BorderBlue = new SolidColorBrush(Color.FromRgb(126, 184, 236));
        public static readonly Brush White = Brushes.White;

        public static Button CreatePrimaryButton(string text)
        {
            Button button = new Button
            {
                Content = text,
                MinHeight = 34,
                Margin = new Thickness(4),
                Padding = new Thickness(14, 6, 14, 6),
                Background = Blue,
                Foreground = White,
                BorderBrush = DarkBlue,
                BorderThickness = new Thickness(1),
                FontWeight = FontWeights.SemiBold
            };
            return button;
        }

        public static Button CreateSecondaryButton(string text)
        {
            Button button = new Button
            {
                Content = text,
                MinHeight = 34,
                Margin = new Thickness(4),
                Padding = new Thickness(14, 6, 14, 6),
                Background = LightBlue,
                Foreground = DarkBlue,
                BorderBrush = BorderBlue,
                BorderThickness = new Thickness(1),
                FontWeight = FontWeights.SemiBold
            };
            return button;
        }

        public static TextBlock CreateHeader(string text)
        {
            return new TextBlock
            {
                Text = text,
                Foreground = White,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(14, 0, 0, 0)
            };
        }

        public static Label CreateLabel(string text)
        {
            return new Label
            {
                Content = text,
                FontWeight = FontWeights.SemiBold,
                Foreground = DarkBlue,
                Margin = new Thickness(0, 2, 0, 0)
            };
        }
    }
}
