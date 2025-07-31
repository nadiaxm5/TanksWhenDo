using UnityEngine;
using System.Collections.Generic;

public class BlueBulletSpawner : MonoBehaviour {
    public bool Active = true;
    public float despX=0.0f;
    public float despY=1.7f;
    public float despZ=1.35f;
    public float maxAim=200f;
    public float currentAim=0f;
    public Dictionary<string, float> propertyList = new Dictionary<string, float>();
    void FixedUpdate(){
        {
            Action.Edit("this.x","BlueTank.x",scopeList);
            Action.Edit("this.y","BlueTank.y",scopeList);
            Action.Edit("this.z","BlueTank.z",scopeList);
            Action.Edit("this.ry","BlueTank.ry",scopeList);
        }
        if(Condition.Compare("this.currentAim>=this.maxAim",scopeList)){
            Action.Edit("this.currentAim","this.maxAim",scopeList);
        }
    }
    void Update(){
        if(Condition.Keyboard("Space","Press")){
            Action.Edit("this.currentAim","this.currentAim+1",scopeList);
            Action.PlaySound("ShotCharging",gameObject);
        }
        if(Condition.Keyboard("Space","Up")){
            Action.Spawn("Shell", gameObject, "this.despX", "this.despY", "this.despZ", "0", scopeList);
            Action.Edit("this.currentAim","0",scopeList);
            Action.StopSound("ShotCharging",gameObject);
            Action.PlaySound("ShotFiring",gameObject);
        }
    }
    public Dictionary<string, GameObject> scopeList = new Dictionary<string, GameObject>();
    void Start() {
        scopeList = Utils.CreateScope(gameObject.GetInstanceID(),"Edit(this.x,BlueTank.x);Edit(this.y,BlueTank.y);Edit(this.z,BlueTank.z);Edit(this.ry,BlueTank.ry);this.currentAim>=this.maxAim;Edit(this.currentAim,this.maxAim);Edit(this.currentAim,this.currentAim+1);Edit(this.currentAim,0)");
        if (Active) gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
    void Awake() {
        propertyList = Utils.CreateProperties("despX=0.0;despY=1.7;despZ=1.35;maxAim=200;currentAim=0");
    }
}
