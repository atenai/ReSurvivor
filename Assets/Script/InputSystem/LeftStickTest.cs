using UnityEngine;
using UnityEngine.InputSystem;

public class LeftStickTest : MonoBehaviour
{
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
