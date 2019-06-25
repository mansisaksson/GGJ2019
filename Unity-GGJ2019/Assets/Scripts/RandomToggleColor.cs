using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomToggleColor : MonoBehaviour
{
    static string color = string.Empty;
    Toggle toggle;
    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();

        if (string.IsNullOrEmpty(color) == false)
        {
            toggle.isOn = color == "orange";
        }
        else
        {
            toggle.isOn = Random.Range(0, 2) == 1;
        }
    }

    void Update()
    {
        color = toggle.isOn ? "orange" : "blue";
    }
}
