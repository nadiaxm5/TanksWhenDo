using UnityEngine;
using System.Collections.Generic;

public class BlueTank : MonoBehaviour {
    public bool Active = true;
    public float speed=10f;
    public float angularSpeed=90f;
    public Dictionary<string, float> propertyList = new Dictionary<string, float>();
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
        scopeList = Utils.CreateScope(gameObject.GetInstanceID(),"Move(this.ry,this.speed);Move(this.ry+180,this.speed)");
        if (Active) gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
    void Awake() {
        propertyList = Utils.CreateProperties("speed=10;angularSpeed=90");
    }
}
