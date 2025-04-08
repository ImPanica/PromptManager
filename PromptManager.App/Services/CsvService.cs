using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using PromptManager.App.Models;

namespace PromptManager.App.Services
{
    public class CsvService
    {
        private readonly string _baseDirectory;
        private readonly CsvConfiguration _csvConfig;

        public CsvService()
        {
            var exePath = AppDomain.CurrentDomain.BaseDirectory;
            _baseDirectory = Path.Combine(exePath, "templates");

            if (!Directory.Exists(_baseDirectory))
            {
                Directory.CreateDirectory(_baseDirectory);
            }

            _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null
            };
        }

        public IEnumerable<Section> LoadAllSections()
        {
            var sections = new List<Section>();

            if (!Directory.Exists(_baseDirectory))
            {
                return sections;
            }

            var files = Directory.GetFiles(_baseDirectory, "*.csv");
            if (files.Length == 0)
            {
                return sections;
            }

            foreach (var file in files)
            {
                try
                {
                    var section = LoadSection(Path.GetFileNameWithoutExtension(file));
                    if (section != null)
                    {
                        sections.Add(section);
                    }
                }
                catch (Exception)
                {
                    // Skip sections that fail to load
                    continue;
                }
            }

            return sections;
        }

        private Section? LoadSection(string sectionName)
        {
            var filePath = GetSectionFilePath(sectionName);
            if (!File.Exists(filePath))
            {
                return null;
            }

            try
            {
                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, _csvConfig);

                var prompts = csv.GetRecords<Prompt>().ToList();

                return new Section
                {
                    Name = sectionName,
                    Prompts = new ObservableCollection<Prompt>(prompts)
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load section {sectionName}: {ex.Message}");
            }
        }

        public void SaveSection(Section section)
        {
            if (string.IsNullOrWhiteSpace(section.Name))
            {
                throw new ArgumentException("Section name cannot be empty.");
            }

            var filePath = GetSectionFilePath(section.Name);
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, _csvConfig);

            csv.WriteRecords(section.Prompts);
        }

        public void DeleteSection(Section section)
        {
            var filePath = GetSectionFilePath(section.Name);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private string GetSectionFilePath(string sectionName)
        {
            return Path.Combine(_baseDirectory, $"{sectionName}.csv");
        }
    }
}