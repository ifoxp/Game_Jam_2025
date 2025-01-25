using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace _Scripts.Utilities
{
    public class ChangeAllFont : MonoBehaviour
    {
        [SerializeField] private TMP_FontAsset _newFont;
        private TextMeshProUGUI[] _texts;

        [Button]
        public void ChangeFont()
        {
            if (_texts.Length == 0)
            {
                Debug.Log("No texts added");
                return;
            }

            foreach (var text in _texts)
            {
                text.font = _newFont;
            }
        }

        [Button]
        public void GetAll_TMP_UGUI_OnScene()
        {
            _texts = FindObjectsByType<TextMeshProUGUI>((FindObjectsSortMode)FindObjectsInactive.Include);
        }
    }
}
