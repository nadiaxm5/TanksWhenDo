using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public static class Utils
{
    public static float GetProperty(KeyValuePair<string, GameObject> s)
    {
        string[] elements = s.Key.Split(new string[] { "." }, StringSplitOptions.None);
        if (elements.Length < 2) return float.NaN;
        GameObject obj = s.Value;
        float value = float.NaN;
        if (obj != null)
        { // if the object exist, it is not deleted
            switch (elements[1])
            {
                case "x": value = obj.transform.position.x; break;
                case "y": value = obj.transform.position.y; break;
                case "z": value = obj.transform.position.z; break;
                case "rx": value = obj.transform.eulerAngles.x; break;
                case "ry": value = obj.transform.eulerAngles.y; break;
                case "rz": value = obj.transform.eulerAngles.z; break;
                case "sx": value = obj.transform.localScale.x; break;
                case "sy": value = obj.transform.localScale.y; break;
                case "sz": value = obj.transform.localScale.z; break;
                case "Active": value = obj.activeSelf ? 1f : 0f; break;
                case "value":
                    var slider = obj.GetComponent<UnityEngine.UI.Slider>();
                    if (slider != null) value = slider.value;
                    break;

                case "text":
                    var text = obj.GetComponent<UnityEngine.UI.Text>();
                    if (text != null && float.TryParse(text.text, out float parsed)) value = parsed;
                    break;

                default:
                    {// search on script properties
                        var script = obj.GetComponent(obj.name);
                        value = (float)script.GetType().GetField(elements[1]).GetValue(script);
                        break;
                    }
            }
        }
        return (value);
    }

    public static void SetProperty(string property, float value, GameObject obj)
    {
        string[] elements = property.Split(new string[] { "." }, StringSplitOptions.None);
        if (obj != null)
        {
            var script = obj.GetComponent(obj.name);
            switch (elements[1])
            {
                case "x": obj.transform.position = new Vector3(value, obj.transform.position.y, obj.transform.position.z); break;
                case "y": obj.transform.position = new Vector3(obj.transform.position.x, value, obj.transform.position.z); break;
                case "z": obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, value); break;
                case "rx": obj.transform.eulerAngles = new Vector3(value, obj.transform.eulerAngles.y, obj.transform.eulerAngles.z); break;
                case "ry": obj.transform.eulerAngles = new Vector3(obj.transform.eulerAngles.x, value, obj.transform.eulerAngles.z); break;
                case "rz": obj.transform.eulerAngles = new Vector3(obj.transform.eulerAngles.x, obj.transform.eulerAngles.y, value); break;
                case "sx": obj.transform.localScale = new Vector3(value, obj.transform.localScale.y, obj.transform.localScale.z); break;
                case "sy": obj.transform.localScale = new Vector3(obj.transform.localScale.x, value, obj.transform.localScale.z); break;
                case "sz": obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, value); break;
                case "Active":
                    obj.SetActive(value != 0); // cualquier valor distinto de 0 se considera true
                    if (script != null && script.GetType().GetField("Active") != null)
                        script.GetType().GetField("Active").SetValue(script, value != 0);
                    break;

                case "value":
                    var slider = obj.GetComponent<UnityEngine.UI.Slider>();
                    if (slider != null) slider.value = value;
                    break;

                case "text":
                    var text = obj.GetComponent<UnityEngine.UI.Text>();
                    if (text != null) text.text = value.ToString();
                    break;

                default:
                    {// search on script properties
                        script.GetType().GetField(elements[1]).SetValue(script, value); break;
                    }
            }
        }
    }

    public static Dictionary<string, GameObject> CreateScope(int objID, string scope)
    {
        Dictionary<string, GameObject> scopeList = new Dictionary<string, GameObject>();
        List<string> baseProperties = new List<string> { "x", "y", "z", "rx", "ry", "rz", "sx", "sy", "sz", "Active" };

        // Añadir objetos raíz de la escena
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject root in rootObjects)
        {
            // Añadir todos los hijos
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            {
                GameObject obj = t.gameObject;
                var script = obj.GetComponent(obj.name);
                List<string> properties = new List<string>(baseProperties);

                if (script != null && script.GetType().GetField("propertyList") != null)
                {
                    var propertyList = (Dictionary<string, float>)script.GetType().GetField("propertyList").GetValue(script);
                    properties.AddRange(propertyList.Keys);
                }

                if (obj.GetComponent<UnityEngine.UI.Slider>() != null)
                    properties.Add("value");

                if (obj.GetComponent<UnityEngine.UI.Text>() != null)
                    properties.Add("text");

                properties = properties.Distinct().ToList();

                foreach (string p in properties)
                {
                    string fullName = obj.name + "." + p;
                    if (scope.Contains(fullName) && !scopeList.ContainsKey(fullName))
                    {
                        scopeList.Add(fullName, obj);
                    }
                }

                if (obj.GetInstanceID() == objID)
                {
                    foreach (string p in properties)
                    {
                        string thisProp = "this." + p;
                        if (!scopeList.ContainsKey(thisProp))
                        {
                            scopeList.Add(thisProp, obj);
                        }
                    }
                }
            }
        }

        // Añadir variables globales
        if (GameManager.Instance != null)
        {
            var globals = new Dictionary<string, GameObject>
            {
                { "Camera", GameManager.Instance.MainCamera?.gameObject },
                { "Sun", GameManager.Instance.SunLight?.gameObject }
            };

            foreach (var pair in globals)
            {
                if (pair.Value == null) continue;

                // Añadir tambien clave simple
                if (!scopeList.ContainsKey(pair.Key))
                    scopeList.Add(pair.Key, pair.Value);

                foreach (string p in baseProperties)
                {
                    string fullName = pair.Key + "." + p;
                    if (scope.Contains(fullName) && !scopeList.ContainsKey(fullName))
                    {
                        scopeList.Add(fullName, pair.Value);
                    }
                }
            }
        }

        return scopeList;
    }

    public static Dictionary<string, float> CreateProperties(string s)
    {
        Dictionary<string, float> properties = new Dictionary<string, float>();
        if (s != "")
        {
            var asign = s.Split(new string[] { ";" }, StringSplitOptions.None);
            foreach (string a in asign)
            {
                var elements = a.Split(new string[] { "=" }, StringSplitOptions.None);
                if (!properties.ContainsKey(elements[0]))
                {
                    float value = float.Parse(elements[1], System.Globalization.CultureInfo.InvariantCulture); //Modificado para spawn
                    properties.Add(elements[0], value);
                }
            }
        }
        return (properties);
    }

    public static void RemoveFromCollisions(GameObject me)
    {
        var script = me.GetComponent(me.name);
        if (script == null) return;

        var tagDict = script.GetType().GetField("TagCollisions")?.GetValue(script) as Dictionary<string, HashSet<GameObject>>;
        if (tagDict == null) return;

        foreach (var others in tagDict.Values.ToList())
        {
            foreach (var other in others)
            {
                if (other == null) continue;

                var otherScript = other.GetComponent(other.name);
                var otherTagDict = otherScript?.GetType().GetField("TagCollisions")?.GetValue(otherScript) as Dictionary<string, HashSet<GameObject>>;
                if (otherTagDict == null) continue;

                foreach (var set in otherTagDict.Values)
                {
                    set.Remove(me);
                }
            }
        }
    }
}