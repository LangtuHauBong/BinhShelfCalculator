using BinhShelfCalculator.Models;
using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace BinhShelfCalculator.Memory
{
    public class ShelfLibraryService
    {
        private readonly string _folderPath;
        private readonly string _filePath;

        public ShelfLibraryService()
        {
            _folderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "BinhShelfCalculator"
            );

            _filePath = Path.Combine(_folderPath, "shelf_library.json");

            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
            }
        }

        public string FilePath
        {
            get { return _filePath; }
        }

        public LibraryData Load()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    LibraryData defaults = CreateDefaultLibrary();
                    Save(defaults);
                    return defaults;
                }

                using (FileStream stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(LibraryData));
                    LibraryData data = serializer.ReadObject(stream) as LibraryData;

                    if (data == null)
                    {
                        data = CreateDefaultLibrary();
                    }

                    EnsureValidData(data);
                    Save(data);
                    return data;
                }
            }
            catch
            {
                LibraryData defaults = CreateDefaultLibrary();
                Save(defaults);
                return defaults;
            }
        }

        public void Save(LibraryData data)
        {
            if (data == null)
            {
                data = CreateDefaultLibrary();
            }

            EnsureValidData(data);

            using (FileStream stream = new FileStream(_filePath, FileMode.Create, FileAccess.Write))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(LibraryData));
                serializer.WriteObject(stream, data);
            }
        }

        private void EnsureValidData(LibraryData data)
        {
            if (data.ShelfProfiles == null)
            {
                data.ShelfProfiles = new System.Collections.Generic.List<ShelfProfile>();
            }

            if (data.ItemBoxTypes == null)
            {
                data.ItemBoxTypes = new System.Collections.Generic.List<ItemBoxType>();
            }

            foreach (ShelfProfile profile in data.ShelfProfiles)
            {
                if (string.IsNullOrWhiteSpace(profile.Id))
                {
                    profile.Id = Guid.NewGuid().ToString();
                }
            }

            foreach (ItemBoxType item in data.ItemBoxTypes)
            {
                if (string.IsNullOrWhiteSpace(item.Id))
                {
                    item.Id = Guid.NewGuid().ToString();
                }

                if (item.QuantityPerGuest <= 0)
                {
                    item.QuantityPerGuest = GetDefaultQuantityPerGuest(item.Name);
                }
            }

            if (data.ShelfProfiles.Count == 0)
            {
                data.ShelfProfiles.Add(CreateDefaultShelfProfile());
            }

            if (data.ItemBoxTypes.Count == 0)
            {
                AddDefaultItems(data);
            }
        }

        private LibraryData CreateDefaultLibrary()
        {
            LibraryData data = new LibraryData();
            data.ShelfProfiles.Add(CreateDefaultShelfProfile());
            AddDefaultItems(data);
            return data;
        }

        private ShelfProfile CreateDefaultShelfProfile()
        {
            return new ShelfProfile
            {
                Name = "Kệ cơ bản",
                BoardThicknessMm = 20,
                ClearHeightBetweenShelvesMm = 200,
                SideClearanceMm = 0,
                FrontBackClearanceMm = 0,
                HandlingClearanceMm = 80,
                SafetyFactor = 1.0
            };
        }

        private void AddDefaultItems(LibraryData data)
        {
            data.ItemBoxTypes.Add(CreateItem("Bếp ga mini", 340, 280, 100, 0.25));
            data.ItemBoxTypes.Add(CreateItem("Bình ga mini", 70, 70, 200, 0.25));
            data.ItemBoxTypes.Add(CreateItem("Cốc uống nước", 80, 80, 100, 1.0));
            data.ItemBoxTypes.Add(CreateItem("Đĩa tròn", 220, 220, 30, 1.0));
            data.ItemBoxTypes.Add(CreateItem("Bát", 130, 130, 70, 1.0));
            data.ItemBoxTypes.Add(CreateItem("Đĩa bánh mì", 180, 180, 25, 1.0));
            data.ItemBoxTypes.Add(CreateItem("Vỉ nướng", 300, 300, 35, 0.5));
            data.ItemBoxTypes.Add(CreateItem("Khay chấm 3 ngăn", 220, 90, 30, 0.25));
            data.ItemBoxTypes.Add(CreateItem("Bát chấm nhỏ", 80, 80, 45, 1.0));
            data.ItemBoxTypes.Add(CreateItem("Âu để rau", 300, 220, 100, 0.25));
        }

        private ItemBoxType CreateItem(string name, double length, double width, double height, double quantityPerGuest)
        {
            return new ItemBoxType
            {
                Name = name,
                LengthMm = length,
                WidthMm = width,
                HeightMm = height,
                QuantityPerBox = 1,
                QuantityPerGuest = quantityPerGuest,
                Note = "Mặc định"
            };
        }

        private double GetDefaultQuantityPerGuest(string itemName)
        {
            string name = (itemName ?? "").Trim().ToLowerInvariant();

            if (name.Contains("bếp ga") || name.Contains("bình ga") || name.Contains("khay chấm") || name.Contains("âu"))
            {
                return 0.25;
            }

            if (name.Contains("vỉ nướng"))
            {
                return 0.5;
            }

            return 1.0;
        }
    }
}
