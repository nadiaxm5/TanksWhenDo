using UnityEngine;
using System.Collections.Generic;

public class ShellExplosion : MonoBehaviour {
    public bool Active = false;
    public float counter=20f;
    public Dictionary<string, float> propertyList = new Dictionary<string, float>();
    void FixedUpdate(){
        {
            Action.Edit("this.counter","this.counter-1",scopeList);
            Action.PlayParticles("ShellExplosion",gameObject);
            Action.PlaySound("ShellExplosion",gameObject);
        }
        if(Condition.Compare("this.counter<0",scopeList)){
            Action.Delete(gameObject);
        }
    }
    public Dictionary<string, GameObject> scopeList = new Dictionary<string, GameObject>();
    void Start() {
        scopeList = Utils.CreateScope(gameObject.GetInstanceID(),"Edit(this.counter,this.counter-1);this.counter<0");
        if (Active) gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
    void Awake() {
        propertyList = Utils.CreateProperties("counter=20");
    }
}
