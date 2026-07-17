using Autodesk.Revit.DB;
using BinhShelfCalculator.Engine;
using BinhShelfCalculator.Memory;
using BinhShelfCalculator.Models;
using BinhShelfCalculator.RevitWriter;
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
        private readonly Document _document;
        private readonly Element _shelfElement;
        private readonly ShelfLibraryService _service;
        private readonly LibraryData _data;

        private TextBox _tableCountBox;
        private TextBox _guestsPerTableBox;
        private ComboBox _itemCombo;
        private TextBox _quantityPerGuestBox;
        private TextBox _resultBox;

        private RestaurantDemandResult _lastDemand;

        public RestaurantDemandWindow(Document document, Element shelfElement, ShelfLibraryService service)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));
            _shelfElement = shelfElement ?? throw new ArgumentNullException(nameof(shelfElement));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _data = _service.Load();

            Title = "Binh Shelf Calculator - Restaurant Demand";
            Width = 760;
            Height = 650;
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

            panel.Children.Add(BlueTheme.CreateLabel("Số lượng trên 1 khách"));
            _quantityPerGuestBox = CreateTextBox("1");
            panel.Children.Add(_quantityPerGuestBox);

            Button saveItemButton = BlueTheme.CreateSecondaryButton("Lưu số lượng / khách vào Library");
            saveItemButton.Click += (s, e) => SaveQuantityPerGuest();
            panel.Children.Add(saveItemButton);

            Button calculateButton = BlueTheme.CreatePrimaryButton("Tính số lượng vật dụng");
            calculateButton.Click += (s, e) => CalculateDemand();
            panel.Children.Add(calculateButton);

            Button writeMarkButton = BlueTheme.CreatePrimaryButton("Ghi MARK theo vật dụng đã chọn");
            writeMarkButton.Click += (s, e) => WriteItemMark();
            panel.Children.Add(writeMarkButton);

            panel.Children.Add(CreateSectionTitle("Kết quả"));

            _resultBox = new TextBox
            {
                IsReadOnly = true,
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 13,
                Height = 160,
                Padding = new Thickness(10),
                BorderBrush = BlueTheme.BorderBlue,
                BorderThickness = new Thickness(1),
                Text = "Nhập số bàn, số khách / bàn và chọn vật dụng."
            };
            panel.Children.Add(_resultBox);

            TextBlock note = new TextBlock
            {
                Text = "MARK được tạo theo vật dụng đang chọn. Ví dụ: Kệ đựng cốc: 200 cốc hoặc Kệ đựng bát: 200 bát.",
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
                return;
            }

            _quantityPerGuestBox.Text = item.QuantityPerGuest.ToString("0.###");
        }

        private void SaveQuantityPerGuest()
        {
            try
            {
                ItemBoxType item = GetSelectedItem();
                double quantityPerGuest = NumberParser.ToDouble(_quantityPerGuestBox.Text, "Số lượng trên 1 khách");

                if (quantityPerGuest <= 0)
                {
                    throw new Exception("Số lượng trên 1 khách phải lớn hơn 0.");
                }

                item.QuantityPerGuest = quantityPerGuest;
                _service.Save(_data);
                MessageBox.Show("Đã lưu số lượng trên 1 khách vào Library.", "Restaurant Demand");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi dữ liệu");
            }
        }

        private void CalculateDemand()
        {
            try
            {
                int tableCount = NumberParser.ToInt(_tableCountBox.Text, "Số bàn");
                int guestsPerTable = NumberParser.ToInt(_guestsPerTableBox.Text, "Số khách / bàn");
                ItemBoxType item = GetSelectedItem();
                double quantityPerGuest = NumberParser.ToDouble(_quantityPerGuestBox.Text, "Số lượng trên 1 khách");

                item.QuantityPerGuest = quantityPerGuest;
                _lastDemand = RestaurantDemandEngine.Calculate(tableCount, guestsPerTable, item);

                string markText = ShelfMarkWriter.BuildMarkText(
                    _lastDemand.Item.Name,
                    _lastDemand.RequiredQuantity);

                _resultBox.Text =
                    "Số bàn: " + _lastDemand.TableCount + Environment.NewLine +
                    "Khách / bàn: " + _lastDemand.GuestsPerTable + Environment.NewLine +
                    "Tổng khách: " + _lastDemand.TotalGuests + Environment.NewLine +
                    "Vật dụng: " + _lastDemand.Item.Name + Environment.NewLine +
                    "Số lượng / khách: " + _lastDemand.Item.QuantityPerGuest.ToString("0.###") + Environment.NewLine +
                    "Nhu cầu: " + _lastDemand.RequiredQuantity + Environment.NewLine +
                    "MARK: " + markText;
            }
            catch (Exception ex)
            {
                _lastDemand = null;
                MessageBox.Show(ex.Message, "Lỗi tính toán");
            }
        }

        private void WriteItemMark()
        {
            try
            {
                CalculateDemand();

                if (_lastDemand == null)
                {
                    return;
                }

                string markText = ShelfMarkWriter.BuildMarkText(
                    _lastDemand.Item.Name,
                    _lastDemand.RequiredQuantity);

                ShelfMarkWriter.WriteItemQuantity(
                    _document,
                    _shelfElement,
                    _lastDemand.Item.Name,
                    _lastDemand.RequiredQuantity);

                MessageBox.Show("Đã ghi Mark: " + markText, "Restaurant Demand");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Không ghi được Mark");
            }
        }

        private ItemBoxType GetSelectedItem()
        {
            ItemBoxType item = _itemCombo.SelectedItem as ItemBoxType;
            if (item == null)
            {
                throw new Exception("Chưa chọn vật dụng.");
            }

            return item;
        }
    }
}
