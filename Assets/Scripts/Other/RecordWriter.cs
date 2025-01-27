using UnityEngine;

public static class RecordWriter {
    private const string RecordKeyPrefix = "LevelName_";

    public static void SaveRecord(string levelName, int score) {
        string key = GetRecordKey(levelName);
        int currentRecord = PlayerPrefs.GetInt(key, 0);

        if (score > currentRecord) {
            PlayerPrefs.SetInt(key, score);
        }
    }

    public static int LoadRecord(string levelName) {
        string key = GetRecordKey(levelName);
        return PlayerPrefs.GetInt(key, 0);
    }

    private static string GetRecordKey(string levelName) {
        return $"{RecordKeyPrefix}{levelName}";
    }
}
