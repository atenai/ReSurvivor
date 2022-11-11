using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LeftStickTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var current = Gamepad.current;

        if (current == null)
        {
            return;
        }

        var leftStickValue = current.leftStick.x.ReadValue();
        Debug.Log("x‚ÌˆÚ“®—Ê : " + leftStickValue);
    }
}
