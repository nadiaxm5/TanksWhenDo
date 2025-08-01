using UnityEngine;
using System.Collections.Generic;

public class BlueTank : MonoBehaviour {
    public bool Active = true;
    public float speed=10f;
    public float angularSpeed=90f;
    public float health=100f;
    public Dictionary<string, float> propertyList = new Dictionary<string, float>();
    void FixedUpdate(){
        if(Condition.Collision("Shell",gameObject)){
            Action.Edit("this.health","this.health-Shell.damage",scopeList);
        }
        if(Condition.Compare("this.health<=0",scopeList)){
            Action.Edit("RedWin.Active","1",scopeList);
        }
    }
    void Update(){
        if(Condition.Keyboard("D","Press")){
            Action.Rotate("this.angularSpeed"," this.x"," this.y"," this.z",gameObject,scopeList);
            Action.PlayParticles("DustTrail",gameObject);
        }
        if(Condition.Keyboard("A","Press")){
            Action.Rotate("-this.angularSpeed"," this.x"," this.y"," this.z",gameObject,scopeList);
            Action.PlayParticles("DustTrail",gameObject);
        }
        if(Condition.Keyboard("W","Press")){
            Action.Move("this.ry","this.speed",gameObject,scopeList);
        }
        if(Condition.Keyboard("S","Press")){
            Action.Move("this.ry+180","this.speed",gameObject,scopeList);
        }
    }
    public Dictionary<string, GameObject> scopeList = new Dictionary<string, GameObject>();
    void Start() {
        scopeList = Utils.CreateScope(gameObject.GetInstanceID(),"Edit(this.health,this.health-Shell.damage);this.health<=0;Edit(RedWin.Active,1);Move(this.ry,this.speed);Move(this.ry+180,this.speed)");
        if (Active) gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
    public Dictionary<string, HashSet<GameObject>> TagCollisions = new Dictionary<string, HashSet<GameObject>>();
    void OnTriggerEnter(Collider other) {
        if (TagCollisions.ContainsKey(other.tag))
            TagCollisions[other.tag].Add(other.gameObject);
    }
    void OnTriggerExit(Collider other) {
        if (TagCollisions.ContainsKey(other.tag))
            TagCollisions[other.tag].Remove(other.gameObject);
    }
    void Awake() {
        propertyList = Utils.CreateProperties("speed=10;angularSpeed=90;health=100");
        TagCollisions["Untagged"] = new HashSet<GameObject>();
        TagCollisions["Respawn"] = new HashSet<GameObject>();
        TagCollisions["Finish"] = new HashSet<GameObject>();
        TagCollisions["EditorOnly"] = new HashSet<GameObject>();
        TagCollisions["MainCamera"] = new HashSet<GameObject>();
        TagCollisions["Player"] = new HashSet<GameObject>();
        TagCollisions["GameController"] = new HashSet<GameObject>();
        TagCollisions["Obstacle"] = new HashSet<GameObject>();
        TagCollisions["BlueTank"] = new HashSet<GameObject>();
        TagCollisions["RedTank"] = new HashSet<GameObject>();
        TagCollisions["Shell"] = new HashSet<GameObject>();
    }
}
