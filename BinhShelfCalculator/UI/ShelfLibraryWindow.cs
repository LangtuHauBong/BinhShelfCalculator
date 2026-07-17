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
    public class ShelfLibraryWindow : Window
    {
        private readonly ShelfLibraryService _service;
        private LibraryData _data;

        private ListBox _profileList;
        private TextBox _profileNameBox;
        private TextBox _boardThicknessBox;
        private TextBox _clearHeightBox;
        private TextBox _sideClearanceBox;
        private TextBox _frontBackClearanceBox;
        private TextBox _handlingClearanceBox;
        private TextBox _safetyFactorBox;

        private ListBox _itemList;
        private TextBox _itemNameBox;
        private TextBox _itemLengthBox;
        private TextBox _itemWidthBox;
        private TextBox _itemHeightBox;
        private TextBox _quantityPerBoxBox;
        private TextBox _itemNoteBox;

        public ShelfLibraryWindow(ShelfLibraryService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _data = _service.Load();

            EnsureData();

            Title = "Binh Shelf Calculator - Library";
            Width = 920;
            Height = 640;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Background = Brushes.White;

            Content = BuildContent();
            RefreshLists();
        }

        private void EnsureData()
        {
            if (_data == null)
            {
                _data = new LibraryData();
            }

            if (_data.ShelfProfiles == null)
            {
                _data.ShelfProfiles = new System.Collections.Generic.List<ShelfProfile>();
            }

            if (_data.ItemBoxTypes == null)
            {
                _data.ItemBoxTypes = new System.Collections.Generic.List<ItemBoxType>();
            }

            if (_data.ShelfProfiles.Count == 0)
            {
                _data.ShelfProfiles.Add(new ShelfProfile
                {
                    Name = "Kệ cơ bản",
                    BoardThicknessMm = 20,
                    ClearHeightBetweenShelvesMm = 200,
                    SideClearanceMm = 0,
                    FrontBackClearanceMm = 0,
                    HandlingClearanceMm = 80,
                    SafetyFactor = 1.0
                });
            }

            if (_data.ItemBoxTypes.Count == 0)
            {
                _data.ItemBoxTypes.Add(new ItemBoxType
                {
                    Name = "Bát",
                    LengthMm = 130,
                    WidthMm = 130,
                    HeightMm = 70,
                    QuantityPerBox = 1,
                    Note = "Vật dụng mặc định"
                });
            }
        }

        private UIElement BuildContent()
        {
            DockPanel root = new DockPanel();

            Border header = new Border
            {
                Height = 64,
                Background = BlueTheme.Blue,
                Child = BlueTheme.CreateHeader("Shelf Library - Thư viện kệ và vật dụng")
            };
            DockPanel.SetDock(header, Dock.Top);
            root.Children.Add(header);

            TabControl tab = new TabControl
            {
                Margin = new Thickness(12)
            };

            tab.Items.Add(new TabItem
            {
                Header = "Loại giá/kệ",
                Content = BuildShelfProfileTab()
            });

            tab.Items.Add(new TabItem
            {
                Header = "Vật dụng dạng box",
                Content = BuildItemBoxTab()
            });

            root.Children.Add(tab);

            return root;
        }

        private UIElement BuildShelfProfileTab()
        {
            Grid grid = new Grid();

            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(280)
            });

            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });

            _profileList = new ListBox
            {
                Margin = new Thickness(0, 10, 12, 0),
                Background = BlueTheme.LightBlue,
                BorderBrush = BlueTheme.BorderBlue
            };

            _profileList.SelectionChanged += (s, e) => LoadSelectedProfileToForm();

            Grid.SetColumn(_profileList, 0);
            grid.Children.Add(_profileList);

            ScrollViewer scroll = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = BuildShelfProfileForm()
            };

            Grid.SetColumn(scroll, 1);
            grid.Children.Add(scroll);

            return grid;
        }

        private UIElement BuildShelfProfileForm()
        {
            StackPanel panel = new StackPanel
            {
                Margin = new Thickness(8, 10, 8, 8)
            };

            panel.Children.Add(BlueTheme.CreateLabel("Tên loại kệ"));
            _profileNameBox = CreateTextBox();
            panel.Children.Add(_profileNameBox);

            panel.Children.Add(BlueTheme.CreateLabel("Chiều dày tấm ngăn ngang (mm)"));
            _boardThicknessBox = CreateTextBox();
            panel.Children.Add(_boardThicknessBox);

            panel.Children.Add(BlueTheme.CreateLabel("Khoảng cách thông thủy giữa 2 tấm ngăn (mm)"));
            _clearHeightBox = CreateTextBox();
            panel.Children.Add(_clearHeightBox);

            panel.Children.Add(BlueTheme.CreateLabel("Khoảng hở mép trái/phải (mm)"));
            _sideClearanceBox = CreateTextBox();
            panel.Children.Add(_sideClearanceBox);

            panel.Children.Add(BlueTheme.CreateLabel("Khoảng hở mép trước/sau (mm)"));
            _frontBackClearanceBox = CreateTextBox();
            panel.Children.Add(_frontBackClearanceBox);

            panel.Children.Add(BlueTheme.CreateLabel("Khoảng thao tác thêm khi đề xuất khoảng cách tầng kệ (mm)"));
            _handlingClearanceBox = CreateTextBox();
            panel.Children.Add(_handlingClearanceBox);

            panel.Children.Add(BlueTheme.CreateLabel("Hệ số sử dụng thực tế (0.01 đến 1.0)"));
            _safetyFactorBox = CreateTextBox();
            panel.Children.Add(_safetyFactorBox);

            StackPanel buttons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 14, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            Button add = BlueTheme.CreateSecondaryButton("Tạo loại kệ mới");
            add.Click += (s, e) => AddShelfProfile();
            buttons.Children.Add(add);

            Button save = BlueTheme.CreatePrimaryButton("Lưu loại kệ");
            save.Click += (s, e) => SaveSelectedProfile();
            buttons.Children.Add(save);

            Button delete = BlueTheme.CreateSecondaryButton("Xóa");
            delete.Click += (s, e) => DeleteSelectedProfile();
            buttons.Children.Add(delete);

            panel.Children.Add(buttons);

            TextBlock note = new TextBlock
            {
                Text = "Lưu ý: Add-in chỉ đọc kích thước kệ từ tham số DÀI / RỘNG / CAO của đối tượng Revit được chọn. Các số trong phần này là cấu hình do người dùng nhập và lưu lại.",
                Foreground = Brushes.DimGray,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 18, 0, 0)
            };

            panel.Children.Add(note);

            return panel;
        }

        private UIElement BuildItemBoxTab()
        {
            Grid grid = new Grid();

            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(280)
            });

            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });

            _itemList = new ListBox
            {
                Margin = new Thickness(0, 10, 12, 0),
                Background = BlueTheme.LightBlue,
                BorderBrush = BlueTheme.BorderBlue
            };

            _itemList.SelectionChanged += (s, e) => LoadSelectedItemToForm();

            Grid.SetColumn(_itemList, 0);
            grid.Children.Add(_itemList);

            ScrollViewer scroll = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = BuildItemBoxForm()
            };

            Grid.SetColumn(scroll, 1);
            grid.Children.Add(scroll);

            return grid;
        }

        private UIElement BuildItemBoxForm()
        {
            StackPanel panel = new StackPanel
            {
                Margin = new Thickness(8, 10, 8, 8)
            };

            panel.Children.Add(BlueTheme.CreateLabel("Tên vật dụng"));
            _itemNameBox = CreateTextBox();
            panel.Children.Add(_itemNameBox);

            panel.Children.Add(BlueTheme.CreateLabel("Dài vật dụng / box (mm)"));
            _itemLengthBox = CreateTextBox();
            panel.Children.Add(_itemLengthBox);

            panel.Children.Add(BlueTheme.CreateLabel("Rộng vật dụng / box (mm)"));
            _itemWidthBox = CreateTextBox();
            panel.Children.Add(_itemWidthBox);

            panel.Children.Add(BlueTheme.CreateLabel("Cao vật dụng / box (mm)"));
            _itemHeightBox = CreateTextBox();
            panel.Children.Add(_itemHeightBox);

            panel.Children.Add(BlueTheme.CreateLabel("Số lượng quy đổi / 1 box"));
            _quantityPerBoxBox = CreateTextBox();
            panel.Children.Add(_quantityPerBoxBox);

            panel.Children.Add(BlueTheme.CreateLabel("Ghi chú"));
            _itemNoteBox = CreateTextBox();
            _itemNoteBox.Height = 76;
            _itemNoteBox.AcceptsReturn = true;
            _itemNoteBox.TextWrapping = TextWrapping.Wrap;
            panel.Children.Add(_itemNoteBox);

            StackPanel buttons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 14, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            Button add = BlueTheme.CreateSecondaryButton("Tạo vật dụng mới");
            add.Click += (s, e) => AddItemBoxType();
            buttons.Children.Add(add);

            Button save = BlueTheme.CreatePrimaryButton("Lưu vật dụng");
            save.Click += (s, e) => SaveSelectedItem();
            buttons.Children.Add(save);

            Button delete = BlueTheme.CreateSecondaryButton("Xóa");
            delete.Click += (s, e) => DeleteSelectedItem();
            buttons.Children.Add(delete);

            panel.Children.Add(buttons);

            TextBlock note = new TextBlock
            {
                Text = "Mọi vật dụng đều được quy đổi thành box chữ nhật Dài x Rộng x Cao. Nếu muốn tính theo chồng, tạo một vật dụng mới dạng 'Chồng bát 20 cái' và đặt Số lượng quy đổi / 1 box = 20.",
                Foreground = Brushes.DimGray,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 18, 0, 0)
            };

            panel.Children.Add(note);

            return panel;
        }

        private TextBox CreateTextBox()
        {
            return new TextBox
            {
                MinHeight = 30,
                Margin = new Thickness(0, 0, 0, 8),
                Padding = new Thickness(6, 4, 6, 4),
                BorderBrush = BlueTheme.BorderBlue,
                BorderThickness = new Thickness(1)
            };
        }

        private ShelfProfile SelectedProfile
        {
            get { return _profileList.SelectedItem as ShelfProfile; }
        }

        private ItemBoxType SelectedItem
        {
            get { return _itemList.SelectedItem as ItemBoxType; }
        }

        private void RefreshLists(string selectedProfileId = null, string selectedItemId = null)
        {
            if (selectedProfileId == null && SelectedProfile != null)
            {
                selectedProfileId = SelectedProfile.Id;
            }

            if (selectedItemId == null && SelectedItem != null)
            {
                selectedItemId = SelectedItem.Id;
            }

            var profiles = _data.ShelfProfiles
                .OrderBy(x => x.Name)
                .ToList();

            _profileList.ItemsSource = null;
            _profileList.ItemsSource = profiles;

            if (!string.IsNullOrWhiteSpace(selectedProfileId))
            {
                _profileList.SelectedItem = profiles.FirstOrDefault(x => x.Id == selectedProfileId);
            }

            var items = _data.ItemBoxTypes
                .OrderBy(x => x.Name)
                .ToList();

            _itemList.ItemsSource = null;
            _itemList.ItemsSource = items;

            if (!string.IsNullOrWhiteSpace(selectedItemId))
            {
                _itemList.SelectedItem = items.FirstOrDefault(x => x.Id == selectedItemId);
            }
        }

        private void LoadSelectedProfileToForm()
        {
            ShelfProfile profile = SelectedProfile;
            if (profile == null)
            {
                return;
            }

            _profileNameBox.Text = profile.Name;
            _boardThicknessBox.Text = profile.BoardThicknessMm.ToString("0.##");
            _clearHeightBox.Text = profile.ClearHeightBetweenShelvesMm.ToString("0.##");
            _sideClearanceBox.Text = profile.SideClearanceMm.ToString("0.##");
            _frontBackClearanceBox.Text = profile.FrontBackClearanceMm.ToString("0.##");
            _handlingClearanceBox.Text = profile.HandlingClearanceMm.ToString("0.##");
            _safetyFactorBox.Text = profile.SafetyFactor.ToString("0.##");
        }

        private void LoadSelectedItemToForm()
        {
            ItemBoxType item = SelectedItem;
            if (item == null)
            {
                return;
            }

            _itemNameBox.Text = item.Name;
            _itemLengthBox.Text = item.LengthMm.ToString("0.##");
            _itemWidthBox.Text = item.WidthMm.ToString("0.##");
            _itemHeightBox.Text = item.HeightMm.ToString("0.##");
            _quantityPerBoxBox.Text = item.QuantityPerBox.ToString();
            _itemNoteBox.Text = item.Note ?? "";
        }

        private bool IsShelfProfileFormEmpty()
        {
            return string.IsNullOrWhiteSpace(_profileNameBox.Text)
                && string.IsNullOrWhiteSpace(_boardThicknessBox.Text)
                && string.IsNullOrWhiteSpace(_clearHeightBox.Text)
                && string.IsNullOrWhiteSpace(_sideClearanceBox.Text)
                && string.IsNullOrWhiteSpace(_frontBackClearanceBox.Text)
                && string.IsNullOrWhiteSpace(_handlingClearanceBox.Text)
                && string.IsNullOrWhiteSpace(_safetyFactorBox.Text);
        }

        private bool IsItemFormEmpty()
        {
            return string.IsNullOrWhiteSpace(_itemNameBox.Text)
                && string.IsNullOrWhiteSpace(_itemLengthBox.Text)
                && string.IsNullOrWhiteSpace(_itemWidthBox.Text)
                && string.IsNullOrWhiteSpace(_itemHeightBox.Text)
                && string.IsNullOrWhiteSpace(_quantityPerBoxBox.Text)
                && string.IsNullOrWhiteSpace(_itemNoteBox.Text);
        }

        private void ApplyShelfProfileFormTo(ShelfProfile profile)
        {
            profile.Name = _profileNameBox.Text.Trim();
            profile.BoardThicknessMm = NumberParser.ToDouble(_boardThicknessBox.Text, "Chiều dày tấm ngăn");
            profile.ClearHeightBetweenShelvesMm = NumberParser.ToDouble(_clearHeightBox.Text, "Khoảng cách thông thủy giữa 2 tấm ngăn");
            profile.SideClearanceMm = NumberParser.ToDouble(_sideClearanceBox.Text, "Khoảng hở mép trái/phải");
            profile.FrontBackClearanceMm = NumberParser.ToDouble(_frontBackClearanceBox.Text, "Khoảng hở mép trước/sau");
            profile.HandlingClearanceMm = NumberParser.ToDouble(_handlingClearanceBox.Text, "Khoảng thao tác thêm");
            profile.SafetyFactor = NumberParser.ToDouble(_safetyFactorBox.Text, "Hệ số sử dụng");
        }

        private void ApplyItemFormTo(ItemBoxType item)
        {
            item.Name = _itemNameBox.Text.Trim();
            item.LengthMm = NumberParser.ToDouble(_itemLengthBox.Text, "Dài vật dụng");
            item.WidthMm = NumberParser.ToDouble(_itemWidthBox.Text, "Rộng vật dụng");
            item.HeightMm = NumberParser.ToDouble(_itemHeightBox.Text, "Cao vật dụng");
            item.QuantityPerBox = NumberParser.ToInt(_quantityPerBoxBox.Text, "Số lượng quy đổi / 1 box");
            item.Note = _itemNoteBox.Text ?? "";
        }

        private void AddShelfProfile()
        {
            try
            {
                ShelfProfile profile = new ShelfProfile
                {
                    Name = "Loại kệ mới " + (_data.ShelfProfiles.Count + 1)
                };

                if (!IsShelfProfileFormEmpty())
                {
                    ApplyShelfProfileFormTo(profile);
                    ValidateShelfProfile(profile);
                }

                _data.ShelfProfiles.Add(profile);
                _service.Save(_data);

                RefreshLists(profile.Id, null);
                LoadSelectedProfileToForm();

                MessageBox.Show("Đã tạo và lưu loại kệ mới.", "Shelf Library");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi dữ liệu");
            }
        }

        private void SaveSelectedProfile()
        {
            try
            {
                ShelfProfile profile = SelectedProfile;

                if (profile == null)
                {
                    MessageBox.Show("Hãy chọn một loại kệ trước.", "Shelf Library");
                    return;
                }

                ApplyShelfProfileFormTo(profile);
                ValidateShelfProfile(profile);

                _service.Save(_data);

                RefreshLists(profile.Id, null);
                LoadSelectedProfileToForm();

                MessageBox.Show("Đã lưu loại kệ.", "Shelf Library");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi dữ liệu");
            }
        }

        private void DeleteSelectedProfile()
        {
            ShelfProfile profile = SelectedProfile;

            if (profile == null)
            {
                return;
            }

            if (MessageBox.Show("Xóa loại kệ này?", "Shelf Library", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            _data.ShelfProfiles.RemoveAll(x => x.Id == profile.Id);

            if (_data.ShelfProfiles.Count == 0)
            {
                _data.ShelfProfiles.Add(new ShelfProfile
                {
                    Name = "Kệ cơ bản",
                    BoardThicknessMm = 20,
                    ClearHeightBetweenShelvesMm = 200,
                    SideClearanceMm = 0,
                    FrontBackClearanceMm = 0,
                    HandlingClearanceMm = 80,
                    SafetyFactor = 1.0
                });
            }

            _service.Save(_data);
            RefreshLists();
            LoadSelectedProfileToForm();
        }

        private void AddItemBoxType()
        {
            try
            {
                ItemBoxType item = new ItemBoxType
                {
                    Name = "Vật dụng mới " + (_data.ItemBoxTypes.Count + 1)
                };

                if (!IsItemFormEmpty())
                {
                    ApplyItemFormTo(item);
                    ValidateItem(item);
                }

                _data.ItemBoxTypes.Add(item);
                _service.Save(_data);

                RefreshLists(null, item.Id);
                LoadSelectedItemToForm();

                MessageBox.Show("Đã tạo và lưu vật dụng mới.", "Shelf Library");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi dữ liệu");
            }
        }

        private void SaveSelectedItem()
        {
            try
            {
                ItemBoxType item = SelectedItem;

                if (item == null)
                {
                    MessageBox.Show("Hãy chọn một vật dụng trước.", "Shelf Library");
                    return;
                }

                ApplyItemFormTo(item);
                ValidateItem(item);

                _service.Save(_data);

                RefreshLists(null, item.Id);
                LoadSelectedItemToForm();

                MessageBox.Show("Đã lưu vật dụng.", "Shelf Library");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi dữ liệu");
            }
        }

        private void DeleteSelectedItem()
        {
            ItemBoxType item = SelectedItem;

            if (item == null)
            {
                return;
            }

            if (MessageBox.Show("Xóa vật dụng này?", "Shelf Library", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            _data.ItemBoxTypes.RemoveAll(x => x.Id == item.Id);

            if (_data.ItemBoxTypes.Count == 0)
            {
                _data.ItemBoxTypes.Add(new ItemBoxType
                {
                    Name = "Vật dụng cơ bản",
                    LengthMm = 100,
                    WidthMm = 100,
                    HeightMm = 100,
                    QuantityPerBox = 1,
                    Note = ""
                });
            }

            _service.Save(_data);
            RefreshLists();
            LoadSelectedItemToForm();
        }

        private void ValidateShelfProfile(ShelfProfile profile)
        {
            if (string.IsNullOrWhiteSpace(profile.Name))
            {
                throw new Exception("Tên loại kệ không được trống.");
            }

            if (profile.BoardThicknessMm <= 0)
            {
                throw new Exception("Chiều dày tấm ngăn phải lớn hơn 0.");
            }

            if (profile.ClearHeightBetweenShelvesMm <= 0)
            {
                throw new Exception("Khoảng cách thông thủy phải lớn hơn 0.");
            }

            if (profile.SideClearanceMm < 0)
            {
                throw new Exception("Khoảng hở mép trái/phải không được âm.");
            }

            if (profile.FrontBackClearanceMm < 0)
            {
                throw new Exception("Khoảng hở mép trước/sau không được âm.");
            }

            if (profile.HandlingClearanceMm < 0)
            {
                throw new Exception("Khoảng thao tác thêm không được âm.");
            }

            if (profile.SafetyFactor <= 0 || profile.SafetyFactor > 1.0)
            {
                throw new Exception("Hệ số sử dụng phải trong khoảng 0.01 đến 1.0.");
            }
        }

        private void ValidateItem(ItemBoxType item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
            {
                throw new Exception("Tên vật dụng không được trống.");
            }

            if (item.LengthMm <= 0)
            {
                throw new Exception("Dài vật dụng phải lớn hơn 0.");
            }

            if (item.WidthMm <= 0)
            {
                throw new Exception("Rộng vật dụng phải lớn hơn 0.");
            }

            if (item.HeightMm <= 0)
            {
                throw new Exception("Cao vật dụng phải lớn hơn 0.");
            }

            if (item.QuantityPerBox <= 0)
            {
                throw new Exception("Số lượng quy đổi / 1 box phải lớn hơn 0.");
            }
        }
    }
}