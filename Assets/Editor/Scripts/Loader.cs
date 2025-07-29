using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Loader
{
    public static void LoadJson(string fileName)
    {
        string jsonPath = Application.dataPath + "/Resources/Games/" + fileName;
        string json = File.ReadAllText(jsonPath);
        SceneJson scene = JsonUtility.FromJson<SceneJson>(json);
        scene.Cast.Reverse();
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        newScene.name = scene.Name;

        // Eliminar cámara y luz por defecto
        GameObject defaultCamera = GameObject.Find("Main Camera");
        if (defaultCamera != null) Object.DestroyImmediate(defaultCamera);
        GameObject defaultLight = GameObject.Find("Directional Light");
        if (defaultLight != null) Object.DestroyImmediate(defaultLight);

        // Instanciar GameManager desde Prefab
        GameObject gameManagerPrefab = Resources.Load<GameObject>("Prefabs/GameManager");
        if (gameManagerPrefab == null)
        {
            Debug.LogError("Prefab GameManager no encontrado en Resources/Prefabs.");
            return;
        }

        GameObject gmInstance = Object.Instantiate(gameManagerPrefab);
        gmInstance.name = "GameManager";

        // Modificar propiedades de la cámara del GameManager
        Camera cam = gmInstance.GetComponentInChildren<Camera>();
        if (cam != null && scene.CameraPosition != null)
            cam.transform.position = new Vector3(scene.CameraPosition[0], scene.CameraPosition[1], scene.CameraPosition[2]);

        if (cam != null && scene.CameraRotation != null)
            cam.transform.eulerAngles = new Vector3(scene.CameraRotation[0], scene.CameraRotation[1], scene.CameraRotation[2]);

        // Modificar propiedades de la luz del GameManager
        Light sun = gmInstance.GetComponentInChildren<Light>();
        if (sun != null && scene.SunPosition != null)
            sun.transform.position = new Vector3(scene.SunPosition[0], scene.SunPosition[1], scene.SunPosition[2]);

        if (sun != null && scene.SunRotation != null)
            sun.transform.eulerAngles = new Vector3(scene.SunRotation[0], scene.SunRotation[1], scene.SunRotation[2]);

        if (sun != null && scene.SunColor != null)
            sun.color = new Color32(scene.SunColor[0], scene.SunColor[1], scene.SunColor[2], 255);

        if (scene.SunAmbientColor != null)
            RenderSettings.ambientLight = new Color32(scene.SunAmbientColor[0], scene.SunAmbientColor[1], scene.SunAmbientColor[2], 255);

        RenderSettings.ambientMode = AmbientMode.Flat;

        // Load GameObjects
        CreateTags(scene.Cast);
        LoadPrefabs(scene.Cast);
        LoadScripts(scene.Cast);
        Debug.Log("Load " + fileName + " finished");
    }

    public static void CreateTags(List<ActorJson> actorList)
    {
        // Remove Tags
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProperty = tagManager.FindProperty("tags");
        tagsProperty.ClearArray();
        tagManager.ApplyModifiedProperties();
        tagManager.Update();
        // Create Tags
        List<string> tags = new List<string>() { "Untagged", "Respawn", "Finish", "EditorOnly", "MainCamera", "Player", "GameController" };
        foreach (ActorJson actor in actorList)
        {
            if (!tags.Contains(actor.Tag) && actor.Tag != null)
            {
                bool found = false;
                int i = 0;
                while (i < tagsProperty.arraySize && !found)
                {
                    if (tagsProperty.GetArrayElementAtIndex(i).stringValue == actor.Tag) found = true;
                    i++;
                }
                if (!found)
                {
                    tagsProperty.InsertArrayElementAtIndex(0);
                    tagsProperty.GetArrayElementAtIndex(0).stringValue = actor.Tag;
                }
            }
        }
        tagManager.ApplyModifiedProperties();
        tagManager.Update();
    }

    private static void LoadPrefabs(List<ActorJson> actorList)
    {
        foreach (ActorJson actor in actorList)
        {
            Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefabs/" + actor.Prefab + ".prefab", typeof(GameObject));
            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)prefab);
            obj.name = actor.Name;
            if (actor.Tag != null) obj.tag = actor.Tag;
            if (actor.Position != null) obj.transform.position = new Vector3(actor.Position[0], actor.Position[1], actor.Position[2]);
            if (actor.Rotation != null) obj.transform.eulerAngles = new Vector3(actor.Rotation[0], actor.Rotation[1], actor.Rotation[2]);
            if (actor.Scale != null) obj.transform.localScale = new Vector3(actor.Scale[0], actor.Scale[1], actor.Scale[2]);
        }
        AssetDatabase.Refresh();
    }

    private static void LoadScripts(List<ActorJson> actorList)
    {
        Scripts.Create(actorList); // Create Scripts
        var scripts = Resources.LoadAll<MonoScript>("Scripts");
        foreach (var script in scripts)
        { // Add scripts to gameObjects
            GameObject obj = GameObject.Find(script.name);
            obj.AddComponent(script.GetClass());
        }
    }
}