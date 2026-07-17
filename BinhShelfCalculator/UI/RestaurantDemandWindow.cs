using BinhShelfCalculator.Engine;
using BinhShelfCalculator.Memory;
using BinhShelfCalculator.Models;
using BinhShelfCalculator.Utils;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BinhShelfCalculator.UI
{
    public class RestaurantDemandWindow : Window
    {
        private readonly ShelfLibraryService _service;
        private readonly LibraryData _data;

        private TextBox _tableCountBox;
        private TextBox _guestsPerTableBox;
        private ComboBox _itemCombo;
        private TextBox _quantityPerGuestBox;
        private TextBox _resultBox;

        public RestaurantDemandWindow(ShelfLibraryService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _data = _service.Load();

            Title = "Binh Shelf Calculator - Restaurant Demand";
            Width = 760;
            Height = 610;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Background = Brushes.White;
            Content = BuildContent();

            LoadItems();
        }

        private UIElement BuildContent()
        {
            DockPanel root = new DockPanel();

            Border header = new Border
            {
                Height = 64,
                Background = BlueTheme.Blue,
                Child = BlueTheme.CreateHeader("Restaurant Demand - Tính số lượng vật dụng")
            };
            DockPanel.SetDock(header, Dock.Top);
            root.Children.Add(header);

            ScrollViewer scroll = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            StackPanel panel = new StackPanel
            {
                Margin = new Thickness(18)
            };

            panel.Children.Add(CreateSectionTitle("Quy mô phục vụ"));
            panel.Children.Add(BlueTheme.CreateLabel("Số bàn"));
            _tableCountBox = CreateTextBox("35");
            panel.Children.Add(_tableCountBox);

            panel.Children.Add(BlueTheme.CreateLabel("Số khách / bàn"));
            _guestsPerTableBox = CreateTextBox("4");
            panel.Children.Add(_guestsPerTableBox);

            panel.Children.Add(CreateSectionTitle("Vật dụng"));
            panel.Children.Add(BlueTheme.CreateLabel("Chọn vật dụng"));
            _itemCombo = new ComboBox
            {
                MinHeight = 32,
                Margin = new Thickness(0, 0, 0, 10)
            };
            _itemCombo.SelectionChanged += (s, e) => LoadSelectedItem();
            panel.Children.Add(_itemCombo);

            panel.Children.Add(BlueTheme.CreateLabel("Số lượng trên 1 khách (lấy từ Library)"));
            _quantityPerGuestBox = CreateTextBox("1");
            _quantityPerGuestBox.IsReadOnly = true;
            _quantityPerGuestBox.Background = Brushes.Gainsboro;
            panel.Children.Add(_quantityPerGuestBox);

            Button calculateButton = BlueTheme.CreatePrimaryButton("Tính số lượng vật dụng");
            calculateButton.Click += (s, e) => CalculateDemand();
            panel.Children.Add(calculateButton);

            panel.Children.Add(CreateSectionTitle("Kết quả"));
            _resultBox = new TextBox
            {
                IsReadOnly = true,
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 13,
                Height = 180,
                Padding = new Thickness(10),
                BorderBrush = BlueTheme.BorderBlue,
                BorderThickness = new Thickness(1),
                Text = "Nhập số bàn, số khách / bàn và chọn vật dụng."
            };
            panel.Children.Add(_resultBox);

            TextBlock note = new TextBlock
            {
                Text = "Chức năng này chỉ tính nhu cầu nhà hàng và không yêu cầu chọn giá/kệ. Số lượng trên 1 khách được đọc trực tiếp từ Library.",
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brushes.DimGray,
                Margin = new Thickness(0, 12, 0, 0)
            };
            panel.Children.Add(note);

            scroll.Content = panel;
            root.Children.Add(scroll);
            return root;
        }

        private TextBlock CreateSectionTitle(string text)
        {
            return new TextBlock
            {
                Text = text,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = BlueTheme.DarkBlue,
                Margin = new Thickness(0, 10, 0, 8)
            };
        }

        private TextBox CreateTextBox(string value)
        {
            return new TextBox
            {
                Text = value,
                MinHeight = 30,
                Margin = new Thickness(0, 0, 0, 8),
                Padding = new Thickness(6, 4, 6, 4),
                BorderBrush = BlueTheme.BorderBlue,
                BorderThickness = new Thickness(1)
            };
        }

        private void LoadItems()
        {
            _itemCombo.ItemsSource = _data.ItemBoxTypes.OrderBy(x => x.Name).ToList();
            _itemCombo.SelectedItem = _itemCombo.Items.Cast<object>().FirstOrDefault();
        }

        private void LoadSelectedItem()
        {
            ItemBoxType item = _itemCombo.SelectedItem as ItemBoxType;
            if (item == null)
            {
                _quantityPerGuestBox.Text = string.Empty;
                return;
            }

            _quantityPerGuestBox.Text = item.QuantityPerGuest.ToString("0.###");
        }

        private void CalculateDemand()
        {
            try
            {
                int tableCount = NumberParser.ToInt(_tableCountBox.Text, "Số bàn");
                int guestsPerTable = NumberParser.ToInt(_guestsPerTableBox.Text, "Số khách / bàn");
                ItemBoxType item = GetSelectedItem();

                RestaurantDemandResult result = RestaurantDemandEngine.Calculate(tableCount, guestsPerTable, item);

                _resultBox.Text =
                    "Số bàn: " + result.TableCount + Environment.NewLine +
                    "Khách / bàn: " + result.GuestsPerTable + Environment.NewLine +
                    "Tổng khách: " + result.TotalGuests + Environment.NewLine +
                    "Vật dụng: " + result.Item.Name + Environment.NewLine +
                    "Số lượng / khách: " + result.Item.QuantityPerGuest.ToString("0.###") + Environment.NewLine +
                    "Nhu cầu: " + result.RequiredQuantity;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi tính toán");
            }
        }

        private ItemBoxType GetSelectedItem()
        {
            ItemBoxType item = _itemCombo.SelectedItem as ItemBoxType;
            if (item == null)
            {
                throw new Exception("Chưa chọn vật dụng.");
            }

            if (item.QuantityPerGuest <= 0)
            {
                throw new Exception("Vật dụng chưa có Số lượng trên 1 khách hợp lệ trong Library.");
            }

            return item;
        }
    }
}
