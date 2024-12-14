using System.IO;
using UnityEngine;

public static class DialogueDataIO
{
    private static string DialoguePath => Application.dataPath + "/DialogueData/";

    public static void SaveToJSON(DialogueData data, string fileName)
    {
        if (!Directory.Exists(DialoguePath))
            Directory.CreateDirectory(DialoguePath);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(DialoguePath + fileName + ".json", json);
        Debug.Log($"Dialogue saved: {DialoguePath}{fileName}.json");
    }

    public static DialogueData LoadFromJSON(string fileName)
    {
        string path = DialoguePath + fileName + ".json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<DialogueData>(json);
        }

        Debug.LogWarning($"Dialogue file not found: {path}");
        return null;
    }
}
