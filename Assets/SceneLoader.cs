using UnityEngine;
using TMPro;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

public class SceneLoader : MonoBehaviour
{
    public TMP_InputField inputField;

    private void Start()
    {
    }

    public void LoadSceneFromJson()
    {
        string jsonString = inputField.text;
        try
        {
            JObject sceneData = JObject.Parse(jsonString);
            var gameObjects = sceneData["scene"]["gameObjects"];

            foreach (var gameObjectData in gameObjects)
            {
                string name = gameObjectData["name"].ToString();
                Debug.Log($"Creating GameObject: {name}");
                GameObject newGameObject = new GameObject(name);

                var components = gameObjectData["components"];
                foreach (var component in components)
                {
                    AddComponentToGameObject(newGameObject, component);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load scene from JSON: {e.Message}");
        }
    }

    private void AddComponentToGameObject(GameObject gameObject, JToken componentData)
    {
        string componentName = ((JProperty)componentData).Name;
        Type componentType = GetTypeFromAllAssemblies(componentName);

        if (componentType == null)
        {
            Debug.LogWarning($"Component type '{componentName}' not found. Skipping.");
            return;
        }

        Debug.Log($"Adding Component: {componentName} to GameObject: {gameObject.name}");
        Component component = gameObject.AddComponent(componentType);
        component = gameObject.GetComponent(componentType);
        var properties = componentData.First;

        foreach (var property in properties)
        {
            string propertyName = ((JProperty)property).Name;
            JToken propertyValue = property.First;

            PropertyInfo propertyInfo = componentType.GetProperty(propertyName);
            FieldInfo fieldInfo = componentType.GetField(propertyName);

            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(component, ConvertValue(propertyValue, propertyInfo.PropertyType), null);
                Debug.Log($"Set Property: {propertyName} to {propertyValue}");
            }
            else if (fieldInfo != null)
            {
                fieldInfo.SetValue(component, ConvertValue(propertyValue, fieldInfo.FieldType));
                Debug.Log($"Set Field: {propertyName} to {propertyValue}");
            }
            else
            {
                Debug.LogWarning($"Property or field '{propertyName}' not found in component '{componentName}'. Skipping.");
            }
        }
    }

    private Type GetTypeFromAllAssemblies(string typeName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.Name == typeName)
                {
                    return type;
                }
            }
        }
        return null;
    }

    private object ConvertValue(JToken token, Type targetType)
    {
        if (targetType == typeof(Vector3))
        {
            return new Vector3((float)token[0], (float)token[1], (float)token[2]);
        }
        if (targetType == typeof(Vector2))
        {
            return new Vector2((float)token[0], (float)token[1]);
        }
        if (targetType == typeof(Quaternion))
        {
            return Quaternion.Euler((float)token[0], (float)token[1], (float)token[2]);
        }
        if (targetType.IsEnum)
        {
            return Enum.Parse(targetType, token.ToString());
        }
        if (targetType == typeof(string))
        {
            return token.ToString();
        }
        return Convert.ChangeType(token, targetType);
    }
}
