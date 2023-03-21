using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A class used to controll the scrollbar of a UI using VR input actions
/// </summary>
[RequireComponent(typeof(Scrollbar))]
public class AnalogStickSlider : MonoBehaviour
{

    Scrollbar _scrollBar;
    private XRIDefaultInputActions _inputActions;
    private float _sensitivity = 1.5f;
    public bool EnableScroll { get; set; }



    private void Awake()
    {
        _inputActions = new XRIDefaultInputActions();
        _scrollBar = GetComponent<Scrollbar>();
    }

    private void OnEnable()
    {
        if(_inputActions!=null)
            _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    public void AutoScroll(float scrollAmt)
    {
        _scrollBar.value -= scrollAmt;
    }

    private void Update()
    {
        if (EnableScroll)
        {

            Vector2 UINav = _inputActions.XRIUI.Navigate.ReadValue<Vector2>();

            float scroll = UINav.y;
            if (scroll != 0)
                _scrollBar.value += scroll * _sensitivity * Time.deltaTime;
        }
    }
}
