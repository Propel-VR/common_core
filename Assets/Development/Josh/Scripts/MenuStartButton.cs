using LM.Management;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dev.Josh
{
    public class MenuStartButton : MonoBehaviour
    {
        public void OnClick()
        {
            Configurator.Instance.LoadConfiguration();
        }
    }
}
