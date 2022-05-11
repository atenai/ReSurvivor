using UnityEngine;

namespace Model
{
    public interface IHidable
    {
        KeyCode HideKey();
        bool ValidKey(KeyCode key);
        Transform GetExit(KeyCode key);
        bool IsAccessable(GameObject gameObject);
        bool IsOccupied();
        void Hide(GameObject gameObject);
        void GoOut(Transform exit);
    }
}
