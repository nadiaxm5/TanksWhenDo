using UnityEngine;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour {
    public bool Active = true;
    public float despX=15f;
    public float despY=15f;
    public float despZ=10f;
    public Dictionary<string, float> propertyList = new Dictionary<string, float>();
    void FixedUpdate(){
        {
            Action.Edit("Camera.x","(BlueTank.x+RedTank.x)/2 - this.despX",scopeList);
            Action.Edit("Camera.y","this.despY + (abs(BlueTank.x-RedTank.x)+abs(BlueTank.z-RedTank.z))/3",scopeList);
            Action.Edit("Camera.z","(BlueTank.z+RedTank.z)/2 - this.despZ",scopeList);
        }
    }
    public Dictionary<string, GameObject> scopeList = new Dictionary<string, GameObject>();
    void Start() {
        scopeList = Utils.CreateScope(gameObject.GetInstanceID(),"Edit(Camera.x,(BlueTank.x+RedTank.x)/2 - this.despX);Edit(Camera.y,this.despY + (abs(BlueTank.x-RedTank.x)+abs(BlueTank.z-RedTank.z))/3);Edit(Camera.z,(BlueTank.z+RedTank.z)/2 - this.despZ)");
        if (Active) gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
    void Awake() {
        propertyList = Utils.CreateProperties("despX=15;despY=15;despZ=10");
    }
}
