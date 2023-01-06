using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gauge : MonoBehaviour
{

    [SerializeField]
    private float _rateOfIncrease, _rateOfDecrease;

    [SerializeField]
    private float _minValue, _maxValue, _startingValue;

    [SerializeField]
    GameObject gauguePin;

    float _value;

    private State state = State.Stable;

    protected enum State
    {
        Stable, 
        Increase, 
        Decrease
    }


    private void Awake()
    {
        _value = _startingValue;
    }

    public void OnStartIncrease()
    {
        state= State.Increase;
    }


    public void OnStartDecrease()
    {
        state= State.Decrease;
    }

    public void OnStabalize()
    {
        state=State.Stable;
    }


    private void Update()
    {
        switch(state) 
        {

            case State.Increase:
                _value += _rateOfIncrease * Time.deltaTime;

                if(_value> _maxValue)
                    _value= _maxValue;
                
                break;
                
            case State.Decrease:
                _value -= _rateOfDecrease* Time.deltaTime;

                if (_value < _minValue)
                    _value = _minValue;


                break;

            case State.Stable:
                break;
        }


        gauguePin.transform.rotation = Quaternion.Euler(0, 0, _value);
    }

}
