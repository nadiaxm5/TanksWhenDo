using System.Collections.Generic;

[System.Serializable]
public class SceneJson {
    public string Name;
    public float[] CameraPosition;
    public float[] CameraRotation;
    public float[] SunPosition;
    public float[] SunRotation;
    public byte[] SunColor;
    public byte[] SunAmbientColor;
    public List<ActorJson> Cast;
}

[System.Serializable]
public class ActorJson {
    public string Name;
    public bool Active = true;
    public string Prefab;
    public string Tag;
    public float[] Position;
    public float[] Rotation;
    public float[] Scale;
    public List<string> Properties;
    public List<SentenceJson> Script = new List<SentenceJson>();
}

[System.Serializable]
public class SentenceJson {
    public List<string> When;
    public List<string> Do;
}
