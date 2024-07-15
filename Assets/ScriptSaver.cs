using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class ScriptSaver : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private string savePath = "Assets/Scripts/";

    public void SaveScript()
    {

        string userScript = inputField.text;
        string className = ExtractClassName(userScript);

        if (string.IsNullOrEmpty(className))
        {
            Debug.LogError("Class name not found in the script.");
            return;
        }

        string directoryPath = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string filePath = Path.Combine(savePath, className + ".cs");

        try
        {
            File.WriteAllText(filePath, userScript);
            Debug.Log($"Script saved as {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save script: {e.Message}");
        }

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    private string ExtractClassName(string script)
    {
        string pattern = @"\bclass\s+(\w+)\b";
        Match match = Regex.Match(script, pattern);

        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        else
        {
            return null;
        }
    }
}
