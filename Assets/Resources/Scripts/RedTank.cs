using UnityEngine;
using System.Collections.Generic;

public class RedTank : MonoBehaviour {
    public bool Active = true;
    public float speed=10f;
    public float angularSpeed=90f;
    public float health=100f;
    public float despY=1.7f;
    public float despZ=1.35f;
    public float maxAim=200f;
    public float currentAim=0f;
    public Dictionary<string, float> propertyList = new Dictionary<string, float>();
    void FixedUpdate(){
        if(Condition.Collision("Shell",gameObject)){
            Action.Edit("this.health","this.health-Shell.damage",scopeList);
        }
        if(Condition.Compare("this.health<=0",scopeList)){
            Action.Edit("BlueWin.Active","1",scopeList);
        }
        if(Condition.Compare("this.currentAim>=this.maxAim",scopeList)){
            Action.Edit("this.currentAim","this.maxAim",scopeList);
        }
    }
    void Update(){
        if(Condition.Keyboard("RightArrow","Press")){
            Action.Rotate("this.angularSpeed","this.rx","this.ry","this.rz",gameObject,scopeList);
            Action.PlayParticles("DustTrail",gameObject);
        }
        if(Condition.Keyboard("LeftArrow","Press")){
            Action.Rotate("-this.angularSpeed","this.rx","this.ry","this.rz",gameObject,scopeList);
            Action.PlayParticles("DustTrail",gameObject);
        }
        if(Condition.Keyboard("UpArrow","Press")){
            Action.Move("this.speed","0","this.ry","0",gameObject,scopeList);
        }
        if(Condition.Keyboard("DownArrow","Press")){
            Action.Move("this.speed","0","this.ry+180","0",gameObject,scopeList);
        }
        if(Condition.Keyboard("Return","Press")){
            Action.Edit("this.currentAim","this.currentAim+1",scopeList);
            Action.PlaySound("ShotCharging",gameObject);
        }
        if(Condition.Keyboard("Return","Up")){
            Action.Spawn("Shell", gameObject, "0", "this.despY", "this.despZ", "0", scopeList);
            Action.Edit("this.currentAim","0",scopeList);
            Action.PlaySound("ShotFiring",gameObject);
        }
    }
    public Dictionary<string, GameObject> scopeList = new Dictionary<string, GameObject>();
    void Start() {
        scopeList = Utils.CreateScope(gameObject.GetInstanceID(),"Edit(this.health,this.health-Shell.damage);this.health<=0;Edit(BlueWin.Active,1);this.currentAim>=this.maxAim;Edit(this.currentAim,this.maxAim);Move(this.speed,0,this.ry,0);Move(this.speed,0,this.ry+180,0);Edit(this.currentAim,this.currentAim+1);Edit(this.currentAim,0)");
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
        propertyList = Utils.CreateProperties("speed=10;angularSpeed=90;health=100;despY=1.7;despZ=1.35;maxAim=200;currentAim=0");
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
