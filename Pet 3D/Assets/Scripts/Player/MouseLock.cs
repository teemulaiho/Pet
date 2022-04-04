using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLock : MonoBehaviour
{
    public delegate void MouseLockStateChange();
    public event MouseLockStateChange onMouseLockStateChange;
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        onMouseLockStateChange();
    }

    public void ReleaseCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        onMouseLockStateChange();
    }
}
