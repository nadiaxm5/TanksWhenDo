using B83.LogicExpressionParser;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class Action
{
    public static void Edit(string property, string valueExp, Dictionary<string, GameObject> scopeList)
    {
        Parser parser = new Parser();
        foreach (KeyValuePair<string, GameObject> s in scopeList)
        {
            parser.ExpressionContext[s.Key].Set(Utils.GetProperty(s));
        }
        float value = (float)parser.ParseNumber(valueExp).GetNumber();
        GameObject obj = scopeList[property];
        Utils.SetProperty(property, value, obj);
    }

    public static void Spawn(string prefabName, GameObject spawnerObj, string offsetXExp, string offsetYExp, string offsetZExp,
                        string offsetRxExp, string offsetRyExp, string offsetRzExp, Dictionary<string, GameObject> scopeList)
    {
        Parser parser = new Parser();
        foreach (var pair in scopeList)
            parser.ExpressionContext[pair.Key].Set(Utils.GetProperty(pair));

        float ParseNum(string exp) => (float)parser.ParseNumber(exp).GetNumber();

        Vector3 offsetPos = new(ParseNum(offsetXExp), ParseNum(offsetYExp), ParseNum(offsetZExp));
        Vector3 offsetRot = new(ParseNum(offsetRxExp), ParseNum(offsetRyExp), ParseNum(offsetRzExp));

        var prefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");
        if (!prefab)
        {
            Debug.LogWarning($"Prefab '{prefabName}' no encontrado en Resources/Prefabs.");
            return;
        }

        var newObj = Object.Instantiate(prefab);
        newObj.name = prefab.name;
        newObj.SetActive(true);

        newObj.transform.position = spawnerObj.transform.TransformPoint(offsetPos);
        newObj.transform.rotation = spawnerObj.transform.rotation * Quaternion.Euler(offsetRot);
        newObj.transform.localScale = prefab.transform.localScale;

        var scriptType = System.Type.GetType(prefabName);
        if (scriptType?.IsSubclassOf(typeof(MonoBehaviour)) == true)
        {
            var script = newObj.AddComponent(scriptType);
            scriptType.GetField("Active")?.SetValue(script, true);

            if (scriptType.GetField("propertyList")?.GetValue(script) is Dictionary<string, float> props)
                foreach (var (prop, val) in props)
                    scriptType.GetField(prop)?.SetValue(script, val);
        }

        if (newObj.TryGetComponent(out LineRenderer lr))
        {
            Vector3 start = newObj.transform.position;
            Vector3 dir = newObj.transform.forward;
            Vector3 end = Physics.Raycast(start, dir, out RaycastHit hit, 100f)
                ? hit.point
                : start + dir * 100f;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
        }
    }

    public static void Animate(string state, GameObject obj)
    {
        obj.GetComponent<Animator>().SetInteger("State", int.Parse(state));
    }

    public static void Move(string speedExp, string rxExp, string ryExp, string rzExp, GameObject obj, Dictionary<string, GameObject> scopeList)
    {
        Parser parser = new Parser();
        foreach (KeyValuePair<string, GameObject> s in scopeList)
            parser.ExpressionContext[s.Key].Set(Utils.GetProperty(s));
        float speed = (float)parser.ParseNumber(speedExp).GetNumber();
        float rx = (float)parser.ParseNumber(rxExp).GetNumber();
        float ry = (float)parser.ParseNumber(ryExp).GetNumber();
        float rz = (float)parser.ParseNumber(rzExp).GetNumber();

        Vector3 dir = Quaternion.Euler(rx, ry, rz) * Vector3.forward;

        if (dir.sqrMagnitude > 0.0001f) dir.Normalize();

        Vector3 delta = dir * speed * Time.deltaTime;

        Utils.SetProperty("this.x", obj.transform.position.x + delta.x, obj);
        Utils.SetProperty("this.y", obj.transform.position.y + delta.y, obj);
        Utils.SetProperty("this.z", obj.transform.position.z + delta.z, obj);
    }

    public static void MoveTo(string xExp, string zExp, string speedExp, GameObject obj, Dictionary<string, GameObject> scopeList)
    {
        Parser parser = new Parser();
        foreach (KeyValuePair<string, GameObject> s in scopeList)
            parser.ExpressionContext[s.Key].Set(Utils.GetProperty(s));
        float x = (float)parser.ParseNumber(xExp).GetNumber();
        float z = (float)parser.ParseNumber(zExp).GetNumber();
        float speed = (float)parser.ParseNumber(speedExp).GetNumber();
        Utils.SetProperty("this.x", obj.transform.position.x + speed * (x - obj.transform.position.x) * Time.deltaTime, obj);
        Utils.SetProperty("this.z", obj.transform.position.z + speed * (z - obj.transform.position.z) * Time.deltaTime, obj);
    }

    public static void NavigateTo(string xExp, string zExp, string speedExp, GameObject obj, Dictionary<string, GameObject> scopeList)
    {
        Parser parser = new Parser();
        foreach (KeyValuePair<string, GameObject> s in scopeList)
            parser.ExpressionContext[s.Key].Set(Utils.GetProperty(s));

        float x = (float)parser.ParseNumber(xExp).GetNumber();
        float z = (float)parser.ParseNumber(zExp).GetNumber();
        float speed = (float)parser.ParseNumber(speedExp).GetNumber();
        NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.SetDestination(new Vector3(x, obj.transform.position.y, z));
    }

    public static void LoadScene()
    {
        SceneManager.LoadScene(0);
    }

    public static void QuitGame()
    {
        Application.Quit();
    }

    public static void PlaySound(string audioClip, GameObject obj)
    {
        AudioSource[] audios = obj.GetComponents<AudioSource>();
        foreach (AudioSource audio in audios)
        {
            if (audio.clip != null && audio.clip.name == audioClip && !audio.isPlaying)
            {
                audio.Play();
                return;
            }
        }

        AudioSource[] childAudios = obj.GetComponentsInChildren<AudioSource>();
        foreach (AudioSource audio in childAudios)
        {
            if (audio.gameObject == obj) continue;
            if (audio.clip != null && audio.clip.name == audioClip && !audio.isPlaying)
            {
                audio.Play();
                return;
            }
        }
    }

    public static void PlayParticles(string particleSystemName, GameObject obj)
    {
        ParticleSystem ps = obj.GetComponent<ParticleSystem>();
        if (ps != null && obj.name == particleSystemName && !ps.isPlaying)
        {
            ps.Play();
            return;
        }

        ParticleSystem[] systems = obj.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem childPs in systems)
        {
            if (childPs.gameObject == obj) continue;
            if (childPs.gameObject.name == particleSystemName && !childPs.isPlaying)
            {
                childPs.Play();
            }
        }
    }

    public static void Delete(GameObject me)
    {
        Utils.RemoveFromCollisions(me);
        Object.Destroy(me);
    }

    public static void Rotate(string angleExp, string rxExp, string ryExp, string rzExp, GameObject obj, Dictionary<string, GameObject> scopeList)
    {
        Parser parser = new Parser();
        foreach (var pair in scopeList)
            parser.ExpressionContext[pair.Key].Set(Utils.GetProperty(pair));
        float angleSpeed = (float)parser.ParseNumber(angleExp).GetNumber();
        float rx = (float)parser.ParseNumber(rxExp).GetNumber();
        float ry = (float)parser.ParseNumber(ryExp).GetNumber();
        float rz = (float)parser.ParseNumber(rzExp).GetNumber();

        float angleDelta = angleSpeed * Time.deltaTime;

        Vector3 localAxis = new Vector3(rx, ry, rz).normalized;
        if (localAxis == Vector3.zero) localAxis = Vector3.up;
        obj.transform.Rotate(localAxis, angleDelta, Space.Self);

        Utils.SetProperty(obj.name + ".rx", obj.transform.eulerAngles.x, obj);
        Utils.SetProperty(obj.name + ".ry", obj.transform.eulerAngles.y, obj);
        Utils.SetProperty(obj.name + ".rz", obj.transform.eulerAngles.z, obj);
    }

    public static void RotateTo(string xExp, string yExp, string zExp, string speedExp, GameObject obj, Dictionary<string, GameObject> scopeList)
    {
        Parser parser = new Parser();
        foreach (var pair in scopeList)
        {
            parser.ExpressionContext[pair.Key].Set(Utils.GetProperty(pair));
        }

        float x = (float)parser.ParseNumber(xExp).GetNumber();
        float y = (float)parser.ParseNumber(yExp).GetNumber();
        float z = (float)parser.ParseNumber(zExp).GetNumber();
        float speed = (float)parser.ParseNumber(speedExp).GetNumber();

        Vector3 targetPos = new Vector3(x, y, z);
        Vector3 dir = targetPos - obj.transform.position;
        dir.y = 0f;
        if (dir == Vector3.zero) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        obj.transform.rotation = Quaternion.RotateTowards(
            obj.transform.rotation,
            targetRot,
            speed * Time.deltaTime
        );

        Utils.SetProperty(obj.name + ".ry", obj.transform.eulerAngles.y, obj);
    }
}