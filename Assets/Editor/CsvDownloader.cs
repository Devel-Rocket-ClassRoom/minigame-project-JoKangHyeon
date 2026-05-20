using UnityEngine;
using UnityEditor;
using System.Collections.Generic;



public static class CsvDownloader
{
    static string localizationBaseUrl = "https://docs.google.com/spreadsheets/d/1M9XFK3p1eoaYBxoq_WKnjSVyra220HgAOmCkx048Bmw/gviz/tq?tqx=out:csv&sheet={0}";
    static List<string> localizationCsvSheets = new List<string>()
    {
        "ko_kr"
    };

    [MenuItem("csv/번역 다운로드")]
    public static void DownloadLocalizingCsv()
    {
        foreach (string sheet in localizationCsvSheets)
        {
            string url = string.Format(localizationBaseUrl, sheet);
            using (var webClient = new System.Net.WebClient())
            {
                string csvData = webClient.DownloadString(url);
                System.IO.File.WriteAllText(Application.streamingAssetsPath + "/Localization/" + sheet + ".csv", csvData);
            }
        }
        Debug.Log("다운로드 완료");
        AssetDatabase.Refresh();
    }
    
}
