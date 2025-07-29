using B83.LogicExpressionParser;
using UnityEngine;
using System;
using System.Collections.Generic;

public static class Condition
{
    public static bool Compare(string a, Dictionary<string, GameObject> scopeList)
    {
        Parser parser = new Parser();
        foreach (KeyValuePair<string, GameObject> s in scopeList)
            parser.ExpressionContext[s.Key].Set(Utils.GetProperty(s));
        return (parser.Parse(a).GetResult());
    }

    public static bool Collision(string tag, GameObject obj)
    {
        var script = obj.GetComponent(typeof(MonoBehaviour));
        var tagCollisionsField = script.GetType().GetField("TagCollisions");
        if (tagCollisionsField == null) return false;

        var tagCollisions = tagCollisionsField.GetValue(script) as Dictionary<string, HashSet<GameObject>>;
        if (tagCollisions == null || !tagCollisions.ContainsKey(tag)) return false;

        tagCollisions[tag].RemoveWhere(obj => obj == null);

        return tagCollisions[tag].Count > 0;
    }

    public static bool Keyboard(string key, string keyMode)
    {
        KeyCode k = (KeyCode)Enum.Parse(typeof(KeyCode), key);
        switch (keyMode)
        {
            case "Press": return Input.GetKey(k);
            case "Down": return Input.GetKeyDown(k);
            case "Up": return Input.GetKeyUp(k);
            default: break;
        }
        return false;
    }

    //Modificado
    private static bool toggle = false;

    public static bool Touch(string type)
    {
        switch (type)
        {
            case "Press": return Input.GetMouseButton(0);
            case "Toggle":
                {
                    if (Input.GetMouseButton(0))
                    {
                        toggle = !toggle;
                        return toggle;
                    }
                    toggle = false; // Reset cuando se suelta
                    return false;
                }
            default: return false;
        }
    }
}