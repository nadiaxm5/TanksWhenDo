using UnityEngine;
using System.Collections.Generic;

public class BlueWin : MonoBehaviour {
    public bool Active = false;
    public float counter=0f;
    public Dictionary<string, float> propertyList = new Dictionary<string, float>();
    private Dictionary<string, float> timers = new Dictionary<string, float>();
    void FixedUpdate(){
        {
            Action.Edit("this.counter","this.counter+1",scopeList);
        }
        if(Condition.Compare("this.counter==150",scopeList)){
            Action.LoadScene();
        }
    }
    public Dictionary<string, GameObject> scopeList = new Dictionary<string, GameObject>();
    void Start() {
        scopeList = Utils.CreateScope(gameObject.GetInstanceID(),"Edit(this.counter,this.counter+1);Compare(this.counter==150);LoadScene()");
        if (Active) gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
    void Awake() {
        propertyList = Utils.CreateProperties("counter=0");
    }
}
