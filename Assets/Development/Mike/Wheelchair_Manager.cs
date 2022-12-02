using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

public class Wheelchair_Manager : MonoBehaviour
{
    public Rigidbody RHwheel;
    public Rigidbody LHwheel;
    public Rigidbody fwdRHwheel;
    public Rigidbody fwdLHwheel;
    public bool wheelsLocked;

    
    public GameObject RH;

    public GameObject LH;
    public bool RanHandFix;
    
    // Start is called before the first frame update
    void Start()
    {
        //Start the coroutine we define below named ExampleCoroutine.
        // StartCoroutine(RunFixCoroutine());
    }
    void Awake()
    {
        RH = GameObject.Find("RightPhysicalHand");
        LH = GameObject.Find("LeftPhysicalHand");
    }
    IEnumerator RunFixCoroutine()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

       


        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(1);
 HandPhysics RHhandPhysics = RH.GetComponent<HandPhysics>();
        RHhandPhysics.enabled = false;
        LH.GetComponent<HandPhysics>().enabled = false;
        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);

        LH.GetComponent<HandPhysics>().enabled = true;
        RHhandPhysics.enabled = true;
    }
    
    // Update is called once per frame
    public void LockWheels() {
        LHwheel.freezeRotation = true;
        RHwheel.freezeRotation = true;

     //   fwdLHwheel.freezeRotation = true;
     //   fwdRHwheel.freezeRotation = true;
    }
    public void UnlockWheels()
    {
        LHwheel.freezeRotation = false;
        RHwheel.freezeRotation = false;

        fwdLHwheel.freezeRotation = false;
        fwdRHwheel.freezeRotation = false;
    }
    void Update()
    {
        if(!RanHandFix) { StartCoroutine(RunFixCoroutine()); RanHandFix = true; }

        if(wheelsLocked) 
        {
            //RHwheel.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            LHwheel.freezeRotation = true;
            RHwheel.freezeRotation = true;
        }
    }
}
