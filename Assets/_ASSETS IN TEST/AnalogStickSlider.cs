using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Scrollbar))]
public class AnalogStickSlider : MonoBehaviour
{

    Scrollbar _scrollBar;
    
    private XRIDefaultInputActions inputActions;
    float sensitivity = 1.5f;
    public bool EnableScroll { get; set; }



    private void Awake()
    {
        inputActions = new XRIDefaultInputActions();
        _scrollBar = GetComponent<Scrollbar>();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void AutoScroll(float scrollAmt)
    {
        _scrollBar.value -= scrollAmt;
    }

    private void Update()
    {
        if (EnableScroll)
        {

            Vector2 UINav = inputActions.XRIUI.Navigate.ReadValue<Vector2>();

            float scroll = UINav.y;
            if (scroll != 0)
                _scrollBar.value += scroll * sensitivity * Time.deltaTime;
        }
    }
}
