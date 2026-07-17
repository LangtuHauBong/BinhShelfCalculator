using BinhShelfCalculator.Models;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

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
                        Save(data);
                    }

                    EnsureValidData(data);
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
            data.ItemBoxTypes.Add(new ItemBoxType { Name = "Bếp ga mini", LengthMm = 340, WidthMm = 280, HeightMm = 100, QuantityPerBox = 1, Note = "Mặc định" });
            data.ItemBoxTypes.Add(new ItemBoxType { Name = "Bình ga mini", LengthMm = 70, WidthMm = 70, HeightMm = 200, QuantityPerBox = 1, Note = "Mặc định" });
            data.ItemBoxTypes.Add(new ItemBoxType { Name = "Cốc uống nước", LengthMm = 80, WidthMm = 80, HeightMm = 100, QuantityPerBox = 1, Note = "Mặc định" });
            data.ItemBoxTypes.Add(new ItemBoxType { Name = "Đĩa tròn", LengthMm = 220, WidthMm = 220, HeightMm = 30, QuantityPerBox = 1, Note = "Mặc định" });
            data.ItemBoxTypes.Add(new ItemBoxType { Name = "Bát", LengthMm = 130, WidthMm = 130, HeightMm = 70, QuantityPerBox = 1, Note = "Mặc định" });
            data.ItemBoxTypes.Add(new ItemBoxType { Name = "Đĩa bánh mì", LengthMm = 180, WidthMm = 180, HeightMm = 25, QuantityPerBox = 1, Note = "Mặc định" });
            data.ItemBoxTypes.Add(new ItemBoxType { Name = "Vỉ nướng", LengthMm = 300, WidthMm = 300, HeightMm = 35, QuantityPerBox = 1, Note = "Mặc định" });
            data.ItemBoxTypes.Add(new ItemBoxType { Name = "Khay chấm 3 ngăn", LengthMm = 220, WidthMm = 90, HeightMm = 30, QuantityPerBox = 1, Note = "Mặc định" });
            data.ItemBoxTypes.Add(new ItemBoxType { Name = "Bát chấm nhỏ", LengthMm = 80, WidthMm = 80, HeightMm = 45, QuantityPerBox = 1, Note = "Mặc định" });
            data.ItemBoxTypes.Add(new ItemBoxType { Name = "Âu để rau", LengthMm = 300, WidthMm = 220, HeightMm = 100, QuantityPerBox = 1, Note = "Mặc định" });
        }
    }
}