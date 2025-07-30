using UnityEngine;
using System.Collections.Generic;

public class RedBulletSpawner : MonoBehaviour {
    public bool Active = true;
    public float despX=0.0f;
    public float despY=1.7f;
    public float despZ=1.35f;
    public Dictionary<string, float> propertyList = new Dictionary<string, float>();
    void FixedUpdate(){
        {
            Action.Edit("this.x","RedTank.x",scopeList);
            Action.Edit("this.y","RedTank.y",scopeList);
            Action.Edit("this.z","RedTank.z",scopeList);
            Action.Edit("this.ry","RedTank.ry",scopeList);
        }
    }
    void Update(){
        if(Condition.Keyboard("Return","Down")){
            Action.Spawn("Shell", gameObject, "this.despX", "this.despY", "this.despZ", "0", scopeList);
        }
    }
    public Dictionary<string, GameObject> scopeList = new Dictionary<string, GameObject>();
    void Start() {
        scopeList = Utils.CreateScope(gameObject.GetInstanceID(),"Edit(this.x,RedTank.x);Edit(this.y,RedTank.y);Edit(this.z,RedTank.z);Edit(this.ry,RedTank.ry)");
        if (Active) gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
    void Awake() {
        propertyList = Utils.CreateProperties("despX=0.0;despY=1.7;despZ=1.35");
    }
}
