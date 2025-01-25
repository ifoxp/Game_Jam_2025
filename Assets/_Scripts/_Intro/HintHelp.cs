using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts._Intro
{
    public class HintHelp : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _hintPlace;
        private bool _pressedKey;

        private const float FADE_DURATION = 2f;
        private const string HELP_TEXT = "Start pressing any keys on keyboard";

        private void Start()
        {
            _hintPlace.text = HELP_TEXT;
            ShowHint();
        }

        public void MustPressKeyAgain()
        {
            ShowHint();
            _pressedKey = false;
        }
    
        private void Update()
        {
            if(_pressedKey) return;
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                _pressedKey = true;
                HideHint();
            }
        }

        private void HideHint()
        {
            Tween.Alpha(_hintPlace, 0f, FADE_DURATION);
        }
    
        private void ShowHint()
        {
            Tween.Alpha(_hintPlace, 1f, FADE_DURATION);
        }
    }
}
