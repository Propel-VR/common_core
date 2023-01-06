using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayHighestValue : MonoBehaviour
{
    [SerializeField]
    string _prefix, postfix;

    List<int>   _values = new List<int>();


    /*TEMP*/
    public static DisplayHighestValue Instance { get; private set; }

    private void Awake()
    {
        Instance= this;
    }
    /*TEMP*/

    public void AddValue(int v)
    {
        _values.Add(v);
    }

    private void FixedUpdate()
    {
        int highest = int.MinValue;

        foreach(int i in _values) 
        {
        
            if(highest < i) highest= i;
        }

        _values.Clear();
    }
}
