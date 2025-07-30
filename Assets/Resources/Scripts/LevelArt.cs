using UnityEngine;
using System.Collections.Generic;

public class LevelArt : MonoBehaviour {
    public bool Active = true;
    void Start() {
        if (Active) gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
}
