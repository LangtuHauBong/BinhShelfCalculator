using DB = Autodesk.Revit.DB;
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
    public class ShelfCalculateWindow : Window
    {
        private readonly DB.Document _doc;
        private readonly DB.Element _element;
        private readonly ShelfInstanceInfo _shelfInfo;
        private readonly ShelfLibraryService _service;
        private readonly LibraryData _data;

        private ComboBox _shelfProfileCombo;
        private ComboBox _itemCombo;
        private CheckBox _useRecommendedClearHeightCheck;
        private TextBox _manualClearHeightBox;
        private TextBlock _recommendedText;
        private TextBox _resultBox;
        private ShelfCalculationResult _lastResult;

        public ShelfCalculateWindow(DB.Document doc, DB.Element element, ShelfInstanceInfo shelfInfo, ShelfLibraryService service)
        {
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));
            _element = element ?? throw new ArgumentNullException(nameof(element));
            _shelfInfo = shelfInfo ?? throw new ArgumentNullException(nameof(shelfInfo));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _data = _service.Load();

            Title = "Binh Shelf Calculator - Calculate";
            Width = 980;
            Height = 720;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Background = Brushes.White;

            Content = BuildContent();
            LoadComboData();
            UpdateRecommendedClearHeightText();
        }

        private UIElement BuildContent()
        {
            DockPanel root = new DockPanel();

            Border header = new Border
            {
                Height = 64,
                Background = BlueTheme.Blue,
                Child = BlueTheme.CreateHeader("Calculate Shelf - Tính sức chứa giá/kệ")
            };
            DockPanel.SetDock(header, Dock.Top);
            root.Children.Add(header);

            Grid main = new Grid { Margin = new Thickness(14) };
            main.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(380) });
            main.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            Border leftCard = new Border
            {
                Background = BlueTheme.LightBlue,
                BorderBrush = BlueTheme.BorderBlue,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(14),
                Margin = new Thickness(0, 0, 12, 0),
                Child = BuildControlPanel()
            };
            Grid.SetColumn(leftCard, 0);
            main.Children.Add(leftCard);

            Border rightCard = new Border
            {
                Background = Brushes.White,
                BorderBrush = BlueTheme.BorderBlue,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(10),
                Child = BuildResultPanel()
            };
            Grid.SetColumn(rightCard, 1);
            main.Children.Add(rightCard);

            root.Children.Add(main);
            return root;
        }

        private UIElement BuildControlPanel()
        {
            StackPanel panel = new StackPanel();

            TextBlock selectedTitle = new TextBlock
            {
                Text = "Đối tượng kệ đã chọn",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = BlueTheme.DarkBlue,
                Margin = new Thickness(0, 0, 0, 8)
            };
            panel.Children.Add(selectedTitle);

            TextBlock selectedInfo = new TextBlock
            {
                Text = BuildSelectedInfoText(),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 14),
                Foreground = Brushes.Black
            };
            panel.Children.Add(selectedInfo);

            panel.Children.Add(BlueTheme.CreateLabel("Chọn cấu hình kệ"));
            _shelfProfileCombo = new ComboBox
            {
                MinHeight = 32,
                Margin = new Thickness(0, 0, 0, 10)
            };
            _shelfProfileCombo.SelectionChanged += (s, e) => UpdateRecommendedClearHeightText();
            panel.Children.Add(_shelfProfileCombo);

            panel.Children.Add(BlueTheme.CreateLabel("Chọn vật dụng"));
            _itemCombo = new ComboBox
            {
                MinHeight = 32,
                Margin = new Thickness(0, 0, 0, 10)
            };
            _itemCombo.SelectionChanged += (s, e) => UpdateRecommendedClearHeightText();
            panel.Children.Add(_itemCombo);

            _useRecommendedClearHeightCheck = new CheckBox
            {
                Content = new TextBlock
                {
                    Text = "Tự đề xuất khoảng cách giữa 2 tấm ngăn theo chiều cao vật dụng",
                    TextWrapping = TextWrapping.Wrap
                },
                IsChecked = true,
                Margin = new Thickness(0, 8, 0, 8),
                Foreground = BlueTheme.DarkBlue
            };
            _useRecommendedClearHeightCheck.Checked += (s, e) => UpdateManualClearHeightState();
            _useRecommendedClearHeightCheck.Unchecked += (s, e) => UpdateManualClearHeightState();
            panel.Children.Add(_useRecommendedClearHeightCheck);

            _recommendedText = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brushes.DimGray,
                Margin = new Thickness(0, 0, 0, 10)
            };
            panel.Children.Add(_recommendedText);

            panel.Children.Add(BlueTheme.CreateLabel("Khoảng cách thông thủy nhập tay (mm)"));
            _manualClearHeightBox = new TextBox
            {
                MinHeight = 30,
                Padding = new Thickness(6, 4, 6, 4),
                Margin = new Thickness(0, 0, 0, 14),
                BorderBrush = BlueTheme.BorderBlue,
                BorderThickness = new Thickness(1)
            };
            panel.Children.Add(_manualClearHeightBox);

            Button calculateButton = BlueTheme.CreatePrimaryButton("Tính sức chứa");
            calculateButton.Click += (s, e) => Calculate();
            panel.Children.Add(calculateButton);

            Button createTextButton = BlueTheme.CreateSecondaryButton("Tạo TextNote trên view hiện tại");
            createTextButton.Click += (s, e) => CreateTextNote();
            panel.Children.Add(createTextButton);

            Button copyButton = BlueTheme.CreateSecondaryButton("Copy kết quả");
            copyButton.Click += (s, e) => CopyResult();
            panel.Children.Add(copyButton);

            TextBlock note = new TextBlock
            {
                Text = "Luật cứng: add-in chỉ lấy kích thước kệ từ 3 tham số DÀI / RỘNG / CAO của đối tượng đang chọn. 'Tầng kệ' là lớp/khoang bên trong giá kệ, không phải Level Revit.",
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brushes.DimGray,
                Margin = new Thickness(0, 16, 0, 0)
            };
            panel.Children.Add(note);

            return panel;
        }

        private UIElement BuildResultPanel()
        {
            DockPanel panel = new DockPanel();

            TextBlock title = new TextBlock
            {
                Text = "Kết quả",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = BlueTheme.DarkBlue,
                Margin = new Thickness(0, 0, 0, 8)
            };
            DockPanel.SetDock(title, Dock.Top);
            panel.Children.Add(title);

            _resultBox = new TextBox
            {
                IsReadOnly = true,
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 13,
                Padding = new Thickness(10),
                BorderBrush = BlueTheme.BorderBlue,
                BorderThickness = new Thickness(1),
                Text = "Bấm 'Tính sức chứa' để xem kết quả."
            };
            panel.Children.Add(_resultBox);

            return panel;
        }

        private string BuildSelectedInfoText()
        {
            return "Family: " + (_shelfInfo.FamilyName ?? "") + Environment.NewLine
                + "Type: " + (_shelfInfo.TypeName ?? "") + Environment.NewLine
                + "DÀI: " + _shelfInfo.LengthMm.ToString("0") + " mm" + Environment.NewLine
                + "RỘNG: " + _shelfInfo.WidthMm.ToString("0") + " mm" + Environment.NewLine
                + "CAO: " + _shelfInfo.HeightMm.ToString("0") + " mm";
        }

        private void LoadComboData()
        {
            _shelfProfileCombo.ItemsSource = _data.ShelfProfiles.OrderBy(x => x.Name).ToList();
            _itemCombo.ItemsSource = _data.ItemBoxTypes.OrderBy(x => x.Name).ToList();

            if (_shelfProfileCombo.Items.Count > 0) _shelfProfileCombo.SelectedIndex = 0;
            if (_itemCombo.Items.Count > 0) _itemCombo.SelectedIndex = 0;

            ShelfProfile profile = _shelfProfileCombo.SelectedItem as ShelfProfile;
            if (profile != null)
            {
                _manualClearHeightBox.Text = profile.ClearHeightBetweenShelvesMm.ToString("0.##");
            }

            UpdateManualClearHeightState();
        }

        private void UpdateManualClearHeightState()
        {
            bool useRecommended = _useRecommendedClearHeightCheck != null && _useRecommendedClearHeightCheck.IsChecked == true;
            if (_manualClearHeightBox != null)
            {
                _manualClearHeightBox.IsEnabled = !useRecommended;
                _manualClearHeightBox.Background = useRecommended ? Brushes.Gainsboro : Brushes.White;
            }

            UpdateRecommendedClearHeightText();
        }

        private void UpdateRecommendedClearHeightText()
        {
            if (_recommendedText == null) return;

            ShelfProfile profile = _shelfProfileCombo == null ? null : _shelfProfileCombo.SelectedItem as ShelfProfile;
            ItemBoxType item = _itemCombo == null ? null : _itemCombo.SelectedItem as ItemBoxType;

            if (profile == null || item == null)
            {
                _recommendedText.Text = "Chưa đủ dữ liệu để đề xuất khoảng cách tầng kệ.";
                return;
            }

            double recommended = item.HeightMm + profile.HandlingClearanceMm;
            _recommendedText.Text = "Đề xuất: " + item.HeightMm.ToString("0") + " mm chiều cao vật dụng + "
                + profile.HandlingClearanceMm.ToString("0") + " mm khoảng thao tác = "
                + recommended.ToString("0") + " mm.";
        }

        private void Calculate()
        {
            try
            {
                ShelfProfile profile = _shelfProfileCombo.SelectedItem as ShelfProfile;
                ItemBoxType item = _itemCombo.SelectedItem as ItemBoxType;

                if (profile == null) throw new Exception("Chưa chọn cấu hình kệ.");
                if (item == null) throw new Exception("Chưa chọn vật dụng.");

                bool useRecommended = _useRecommendedClearHeightCheck.IsChecked == true;
                double manualClearHeight = useRecommended
                    ? profile.ClearHeightBetweenShelvesMm
                    : NumberParser.ToDouble(_manualClearHeightBox.Text, "Khoảng cách thông thủy nhập tay");

                _lastResult = ShelfCapacityEngine.Calculate(
                    _shelfInfo,
                    profile,
                    item,
                    useRecommended,
                    manualClearHeight);

                _resultBox.Text = _lastResult.ToReportText();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi tính toán");
            }
        }

        private void CreateTextNote()
        {
            try
            {
                if (_lastResult == null)
                {
                    Calculate();
                }

                if (_lastResult == null) return;

                ShelfTextNoteWriter.CreateTextNoteNearElement(
                    _doc,
                    _doc.ActiveView,
                    _element,
                    BuildShortTextNote(_lastResult));

                MessageBox.Show("Đã tạo TextNote trên view hiện tại.", "Calculate Shelf");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Không tạo được TextNote");
            }
        }

        private string BuildShortTextNote(ShelfCalculationResult result)
        {
            return "KỆ: " + result.Shelf.TypeName + Environment.NewLine
                + result.Shelf.SizeText + Environment.NewLine
                + "Vật dụng: " + result.Item.Name + Environment.NewLine
                + "Sức chứa: " + result.TotalQuantityAfterSafety + " cái" + Environment.NewLine
                + result.ShelfTierCount + " tầng kệ | " + result.BoxesPerTier + " cái/tầng | "
                + result.Item.QuantityPerBox + " cái/box";
        }

        private void CopyResult()
        {
            if (string.IsNullOrWhiteSpace(_resultBox.Text)) return;
            Clipboard.SetText(_resultBox.Text);
            MessageBox.Show("Đã copy kết quả.", "Calculate Shelf");
        }
    }
}
