using UnityEngine;
using System.Collections.Generic;

public class RedAim : MonoBehaviour {
    public bool Active = true;
    void FixedUpdate(){
        {
            Action.Edit("this.x","RedTank.x",scopeList);
            Action.Edit("this.y","RedTank.y",scopeList);
            Action.Edit("this.z","RedTank.z",scopeList);
            Action.Edit("this.ry","RedTank.ry",scopeList);
            Action.Edit("this.value","RedBulletSpawner.currentAim",scopeList);
        }
    }
    public Dictionary<string, GameObject> scopeList = new Dictionary<string, GameObject>();
    void Start() {
        scopeList = Utils.CreateScope(gameObject.GetInstanceID(),"Edit(this.x,RedTank.x);Edit(this.y,RedTank.y);Edit(this.z,RedTank.z);Edit(this.ry,RedTank.ry);Edit(this.value,RedBulletSpawner.currentAim)");
        if (Active) gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
}
