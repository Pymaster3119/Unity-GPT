using System.Collections;
using System.Collections.Generic;
using Shibuya24.Utility;
using UnityEditor;
using UnityEngine;
using KtxUnity;
using System.IO;
using Unity.Collections;
using System.Collections;




public class GenerationController : MonoBehaviour
{
    public TMPro.TMP_InputField field;
    public void ImportImage()
    {
        string filePath = field.text;
        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData); // Automatically resizes the texture dimensions.
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(Path.Combine(Path.GetDirectoryName("Assets/Images/"), Path.GetFileName(filePath)), bytes);
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }

    }
    
}
