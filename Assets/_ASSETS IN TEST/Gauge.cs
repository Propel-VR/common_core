using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// class to control a turning gauge
/// </summary>
public class Gauge : MonoBehaviour
{

    #region Serialized Private
    [SerializeField]
    [Tooltip("The rate at which the gauge will increase in degrees/second")]
    private float _rateOfIncrease;
    [SerializeField]
    [Tooltip("The rate at which the gauge will decrease in degrees/second")]
    private float _rateOfDecrease;

    [SerializeField]
    [Tooltip("In degrees")]
    private float _minValue, _maxValue, _startingValue;

    [SerializeField]
    [Tooltip("In degrees")]
    private float _target, _leniance;

    [SerializeField]
    [Tooltip("the gameobject to rotate as the pin")]
    private GameObject _gauguePin;
    #endregion

    #region private
    float _value;
    bool _onTarget = false;

    private State _state = State.Stable;
    #endregion

    #region events
    public UnityEvent OnTargetReached,OnTargetExited, OnMaxValue;
    #endregion

    #region class enumerators
    protected enum State
    {
        Stable, 
        Increase, 
        Decrease
    }
    #endregion

    private void Awake()
    {
        //set starting value on awake
        _value = _startingValue;
    }

    /// <summary>
    /// Called externally when the gauge should begin increasing
    /// </summary>
    public void OnStartIncrease()
    {
        _state = State.Increase;
    }

    /// <summary>
    /// Called externally when the gauge should begin decreasing
    /// </summary>
    public void OnStartDecrease()
    {
        _state = State.Decrease;
    }

    /// <summary>
    /// Called externally when the gauge should stabalize
    /// </summary>
    public void OnStabalize()
    {
        _state = State.Stable;
    }


    private void Update()
    {
        switch(_state) 
        {

            case State.Increase:
                //increase the gauge value
                if (_value < _maxValue)
                {
                    _value += _rateOfIncrease * Time.deltaTime;

                    if (_value > _maxValue) //limit at max value
                    {
                        _value = _maxValue;
                        OnMaxValue?.Invoke();
                    }
                    //detrermine if gauge just entered or exited the trarget
                    if (!_onTarget)
                    {
                        //check if gague entered target
                        if (_value > _target - _leniance && _value < _target + _leniance)
                        {
                            OnTargetReached?.Invoke();

                        }
                    }
                    else
                    {
                        //check if gauge exited target
                        if (_value < _target - _leniance || _value > _target + _leniance)
                        {
                            OnTargetExited?.Invoke();

                        }
                    }
                }
               

                break;
                
            case State.Decrease:
                //decrease gauge value
                _value -= _rateOfDecrease* Time.deltaTime;

                if (_value < _minValue) //limit at min value
                    _value = _minValue;


                //detrermine if gauge just entered or exited the trarget
                if (!_onTarget)
                {
                    //check if gague entered target
                    if (_value > _target - _leniance && _value < _target + _leniance)
                    {
                        OnTargetReached?.Invoke();

                    }
                }
                else
                {
                    //check if gauge exited target
                    if (_value < _target - _leniance || _value > _target + _leniance)
                    {
                        OnTargetExited?.Invoke();

                    }
                }

                break;

            case State.Stable:
                //no update needed if gauge is stabilized
                break;
        }


        _gauguePin.transform.localRotation = Quaternion.Euler(0, 0, _value);
    }

}
