using UnityEngine;

public static class UnityJsonManager { 
    //JSON string to PlayerPrefs saving class.
    public static void SaveToJSON<T>(string fileName, T obj) {
        string json = JsonUtility.ToJson(obj);
        PlayerPrefs.SetString(fileName, json);
    }

    public static bool GetFromJSON<T>(string fileName, T obj) {

        string json;
        bool result = true;

        if (!PlayerPrefs.HasKey(fileName)) {
            json = JsonUtility.ToJson(obj);
            PlayerPrefs.SetString(fileName, json);
            result = false;
        }
        json = PlayerPrefs.GetString(fileName);
        JsonUtility.FromJsonOverwrite(json, obj);
        return result;
    }
}
