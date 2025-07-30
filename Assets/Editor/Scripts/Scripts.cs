using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using UnityEditor;
using UnityEditorInternal;

public static class Scripts
{
    public static void Create(List<ActorJson> actorList)
    {
        Directory.Delete("Assets/Resources/Scripts/", true);
        Directory.CreateDirectory("Assets/Resources/Scripts/");
        foreach (ActorJson actor in actorList)
        {
            List<string> tags = new List<string>(InternalEditorUtility.tags);
            List<string> mouseEvents = new List<string>();
            List<string> scope = new List<string>();
            List<string> spawns = new List<string>();
            List<string> properties = new List<string>();
            string scriptsPath = "Assets/Resources/Scripts/" + actor.Name + ".cs";
            StreamWriter outfile = new StreamWriter(scriptsPath);
            bool hasCollision = false;

            // Header
            outfile.WriteLine("using UnityEngine;");
            outfile.WriteLine("using System.Collections.Generic;");
            outfile.WriteLine("");
            outfile.WriteLine("public class " + actor.Name + " : MonoBehaviour {");

            // Properties
            outfile.WriteLine("    public bool Active = " + actor.Active.ToString().ToLower() + ";");
            foreach (string p in actor.Properties)
            {
                properties.Add(p);
                outfile.WriteLine("    public float " + p + "f;");
            }

            // Acumuladores de estructuras
            string joinProperties = string.Join(";", properties);
            string joinSpawns = string.Join(",", spawns);
            string joinScope = "";

            // Diccionarios
            if (joinProperties.Length > 0)
                outfile.WriteLine("    public Dictionary<string, float> propertyList = new Dictionary<string, float>();");

            // Separar sentencias en Update y FixedUpdate
            List<SentenceJson> updateSentences = new List<SentenceJson>();
            List<SentenceJson> fixedSentences = new List<SentenceJson>();

            foreach (SentenceJson s in actor.Script)
            {
                bool isUpdate = s.When.Any(w => w.Contains("Keyboard") || w.Contains("Touch"));
                if (isUpdate) updateSentences.Add(s);
                else fixedSentences.Add(s);
            }

            // FixedUpdate
            if (fixedSentences.Any())
            {
                outfile.WriteLine("    void FixedUpdate(){");
                foreach (SentenceJson s in fixedSentences)
                {
                    if (s.When.Any())
                    {
                        outfile.Write("        if(");
                        foreach (string c in s.When)
                        {
                            string newC = c;
                            if (c.Contains("Collision")) hasCollision = true;
                            else if (c.Contains("Touch")) mouseEvents.Add(StringToElement(c));
                            else if (!c.Contains("Keyboard"))
                            {
                                scope.Add(c);
                                newC = "Compare(" + c + ")";
                            }
                            outfile.Write("Condition." + StringToCommand(newC));
                            if (s.When.Last() != c) outfile.Write(" && ");
                        }
                        outfile.WriteLine("){");
                    }
                    else
                    {
                        outfile.WriteLine("        {");
                    }

                    foreach (string a in s.Do)
                    {
                        string newA = a;
                        if (a.Contains("="))
                        {
                            var elements = a.Split(new string[] { "=" }, StringSplitOptions.None);
                            newA = "Edit(" + elements[0] + "," + elements[1] + ")";
                            scope.Add(newA);
                        }
                        else if (a.Contains("Spawn")) spawns.Add(StringToElement(newA));
                        else if (a.Contains("Move") || a.Contains("NavigateTo")) scope.Add(a);

                        outfile.WriteLine("            Action." + StringToCommand(newA) + ";");
                    }
                    outfile.WriteLine("        }");
                }
                outfile.WriteLine("    }");
            }

            // Update
            if (updateSentences.Any())
            {
                outfile.WriteLine("    void Update(){");
                foreach (SentenceJson s in updateSentences)
                {
                    if (s.When.Any())
                    {
                        outfile.Write("        if(");
                        foreach (string c in s.When)
                        {
                            string newC = c;
                            if (c.Contains("Collision")) hasCollision = true;
                            else if (c.Contains("Touch")) mouseEvents.Add(StringToElement(c));
                            else if (!c.Contains("Keyboard"))
                            {
                                scope.Add(c);
                                newC = "Compare(" + c + ")";
                            }
                            outfile.Write("Condition." + StringToCommand(newC));
                            if (s.When.Last() != c) outfile.Write(" && ");
                        }
                        outfile.WriteLine("){");
                    }
                    else
                    {
                        outfile.WriteLine("        {");
                    }

                    foreach (string a in s.Do)
                    {
                        string newA = a;
                        if (a.Contains("="))
                        {
                            var elements = a.Split(new string[] { "=" }, StringSplitOptions.None);
                            newA = "Edit(" + elements[0] + "," + elements[1] + ")";
                            scope.Add(newA);
                        }
                        else if (a.Contains("Spawn")) spawns.Add(StringToElement(newA));
                        else if (a.Contains("Move") || a.Contains("NavigateTo")) scope.Add(a);

                        outfile.WriteLine("            Action." + StringToCommand(newA) + ";");
                    }
                    outfile.WriteLine("        }");
                }
                outfile.WriteLine("    }");
            }

            // Awake (acumulador)
            List<string> awakeLines = new List<string>();
            if (joinProperties.Length != 0)
                awakeLines.Add("        propertyList = Utils.CreateProperties(\"" + joinProperties + "\");");
            if (spawns.Count > 0)
            {
                string joinSpawnsNow = string.Join(",", spawns);
            }

            // Start
            scope = scope.Distinct().ToList();
            joinScope = string.Join(";", scope);
            if (joinScope.Length != 0)
                outfile.WriteLine("    public Dictionary<string, GameObject> scopeList = new Dictionary<string, GameObject>();");

            outfile.WriteLine("    void Start() {");
            if (joinScope.Length != 0)
                outfile.WriteLine("        scopeList = Utils.CreateScope(gameObject.GetInstanceID(),\"" + joinScope + "\");");
            outfile.WriteLine("        if (Active) gameObject.SetActive(true);");
            outfile.WriteLine("        else gameObject.SetActive(false);");
            outfile.WriteLine("    }");

            // Collisions
            if (hasCollision)
            {
                tags = tags.Distinct().ToList();
                outfile.WriteLine("    public Dictionary<string, HashSet<GameObject>> TagCollisions = new Dictionary<string, HashSet<GameObject>>();");
                foreach (string t in tags)
                    awakeLines.Add("        TagCollisions[\"" + t + "\"] = new HashSet<GameObject>();");
                outfile.WriteLine("    void OnTriggerEnter(Collider other) {");
                outfile.WriteLine("        if (TagCollisions.ContainsKey(other.tag))");
                outfile.WriteLine("            TagCollisions[other.tag].Add(other.gameObject);");
                outfile.WriteLine("    }");

                outfile.WriteLine("    void OnTriggerExit(Collider other) {");
                outfile.WriteLine("        if (TagCollisions.ContainsKey(other.tag))");
                outfile.WriteLine("            TagCollisions[other.tag].Remove(other.gameObject);");
                outfile.WriteLine("    }");
            }

            // Escribir Awake
            if (awakeLines.Any())
            {
                outfile.WriteLine("    void Awake() {");
                foreach (string line in awakeLines)
                    outfile.WriteLine(line);
                outfile.WriteLine("    }");
            }

            // Mouse Events
            if (mouseEvents.Any())
            {
                foreach (string e in mouseEvents)
                {
                    outfile.WriteLine("    public bool Mouse" + e + " = false;");
                    outfile.WriteLine("    void OnMouse" + e + "(){");
                    outfile.WriteLine("        Mouse" + e + "=true;");
                    outfile.WriteLine("    }");
                }
            }

            // Cierre clase
            outfile.WriteLine("}");
            outfile.Close();
        }
        AssetDatabase.Refresh();
    }

    private static string StringToCommand(string element)
    {// traslate a game.json comand into a valid unity command
        int init = element.IndexOf("(");
        int end = element.LastIndexOf(")");
        string name = element.Substring(0, init);
        string command = name;
        string rest = element.Substring(init + 1, end - init - 1);
        string[] parameters = rest.Split(new string[] { "," }, StringSplitOptions.None);
        command += "(";
        int counter = 0;
        foreach (string s in parameters)
        {
            counter++;
            command += "\"" + s + "\"";
            if (parameters.Length != counter) command += ",";
        }
        if (name == "Compare" || name == "Edit") command += ",scopeList)";
        else if (name == "Move" || name == "MoveTo" || name == "NavigateTo" || name == "RotateTo" || name == "Rotate") command += ",gameObject,scopeList)";
        else if (name == "Collision" || name == "Animate" || name == "PlaySound" || name == "StopSound" || name == "PlayParticles" || name == "StopParticles") command += ",gameObject)";
        else if (name == "Keyboard" || name == "Touch") command += ")";
        else if (name == "Delete") command = "Delete(gameObject)";
        else if (name == "QuitGame" || name == "LoadScene") command = name + "()";
        else if (name == "Spawn")
        {
            string prefab = parameters[0];
            command = $"Spawn(\"{prefab}\", gameObject";

            List<string> extraParams = new List<string>();
            for (int i = 2; i < parameters.Length; i++)
                extraParams.Add($"\"{parameters[i].Trim()}\"");

            while (extraParams.Count < 4)
                extraParams.Add("\"0\"");

            foreach (string param in extraParams)
                command += $", {param}";

            command += ", scopeList)";
        }
        return (command);
    }

    private static string StringToElement(string element)
    {
        int init = element.IndexOf("(");
        int end = element.LastIndexOf(")");
        string tag = element.Substring(init + 1, end - init - 1);
        return (tag);
    }
}