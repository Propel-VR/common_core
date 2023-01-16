using UnityEngine;
using System;


public class randomRotate:MonoBehaviour{
    Quaternion rotTarget;
    public float rotateEverySecond = 1.0f;
    
    public void Start() {
    	randomRot ();
    	InvokeRepeating("randomRot", 0.0f,rotateEverySecond);
    }
    
    public void Update(){
    	transform.rotation = Quaternion.Lerp(transform.rotation, rotTarget, Time.time*Time.deltaTime);
    
    }
    
    public void randomRot() {
    	 rotTarget = UnityEngine.Random.rotation;
    
    }
}