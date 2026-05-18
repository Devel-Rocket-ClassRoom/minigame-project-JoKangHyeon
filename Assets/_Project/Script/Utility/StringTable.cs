using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

public static class StringTable
{
    static Dictionary<string, string> table;

    static string FormatPath = "{0}.csv";

    public const string tableLocation = "";

    public static string GetString(string key)
    {
        return table[key];
    }

    public static void LoadLanguage(string lang)
    {
        table.Clear();

        var path = string.Format(FormatPath, lang);
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCsv<Data>(textAsset.text);
        foreach (var item in list)
        {
            if (!table.ContainsKey(item.Id))
            {
                table.Add(item.Id, item.String);
            }
            else
            {
                Debug.LogError($"키 중복 : {item.Id}");
            }
        }

    }

    public static List<T> LoadCsv<T>(string csv)
    {
        using (var reader = new StringReader(csv))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csvReader.GetRecords<T>();
            return records.ToList();
        }
    }

    private class Data
    {
        public string Id { get; set; }
        public string String { get; set; }
    }

}
