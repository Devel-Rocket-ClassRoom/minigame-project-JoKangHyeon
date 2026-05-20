using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

public static class StringTable
{
    static Dictionary<string, string> table;

    static string _localizationTextLocation;
    static List<string> _localizationList;
    static string LocalizationTextLocation
    {
        get
        {
            if (string.IsNullOrEmpty(_localizationTextLocation))
            {
                LoadLocalizationLoaction();
            }

            return _localizationTextLocation;
        }
    }

    static List<string> LocalizationList
    {
        get
        {
            if (_localizationList == null || _localizationList.Count == 0)
            {
                LoadLocalizationLoaction();
            }
            return _localizationList;
        }
    }

    public const string tableLocation = "";
    public const string defaultLanguage = "ko_kr";

    public static void LoadLocalizationLoaction()
    {
        _localizationTextLocation = Application.streamingAssetsPath + "/Localization/{0}.csv";
        _localizationList = new List<string>();

        var directoryInfo = new DirectoryInfo(Application.streamingAssetsPath + "/Localization");
        foreach( var file in directoryInfo.GetFiles("*.csv") )
        {
            _localizationList.Add(file.Name.Replace(".csv", ""));
        }
    }

    public static string GetString(string key)
    {
        if (table == null)
        {
            LoadLanguage(defaultLanguage);
        }

        if (string.IsNullOrEmpty(key))
            return string.Empty;

        return table[key];
    }

    public static void LoadLanguage(string lang)
    {
        if(table == null)
        {
            table = new Dictionary<string, string>();
        }

        table.Clear();

        var path = string.Format(LocalizationTextLocation, lang);
        string result = null;
        using (StreamReader reader = new StreamReader(path, System.Text.Encoding.UTF8))
        {
            result = reader.ReadToEnd();
        }

        if (result == null)
        {
            Debug.LogError("LANG LOAD FAILED : " + path);
            return;
        }

        Debug.Log(result);

        var list = LoadCsv<Data>(result);
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
