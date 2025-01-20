using UnityEngine;

namespace _Scripts.Utilities
{
    public static class CursorStateChanger
    {
        public static void SetCursorState(CursorLockMode lockMode, bool visible)
        {
            Cursor.lockState = lockMode;
            Cursor.visible = visible;
        }
    }
}