using System;
using System.Collections;
using _Scripts.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace _Scripts._Intro
{
    public class TypeText : MonoBehaviour
    {
        // Зараз 2:40AM. Я сильно хардкоджу, бо геймджем закінчується сьогодні
        // Пробачте та зрозумійте всі, хто побачить цей код :)
        // ggStrider <3
        [SerializeField] private float _typeDelay = 0.1f;
        
        [Space]
        [SerializeField] private TextMeshProUGUI _textPlace;
        [SerializeField] private TypeTextContainer[] _textContainers;

        [SerializeField] private AudioPlayer _characterAudioPlayer;
        
        private HintHelp _hintHelp;

        private int _currentContainerIndex;
        private int _currentCharacterIndex;

        private bool _canType = true;
        private bool _isTyping;

        private Coroutine _currentCoroutine;
        bool Del = false;
        private void Awake()
        {
            _hintHelp = FindAnyObjectByType<HintHelp>();
        }

        public void SetCanType(bool canType)
        {
            _canType = canType;
        }
        
        private void Update()
        {
            if (!_canType) return;
            if (_currentContainerIndex >= _textContainers.Length) return;
            
            if (Keyboard.current.anyKey.wasPressedThisFrame && !_isTyping)
            {
                _currentCoroutine = StartCoroutine(_TypeText());
            }
        }

        private IEnumerator _TypeText()
        {
            if (_currentContainerIndex >= _textContainers.Length) yield break;

            _textPlace.text = string.Empty;

            var currentContainer = _textContainers[_currentContainerIndex];
            currentContainer.OnTextStarted?.Invoke();

            _isTyping = true;
            _currentCharacterIndex = 0;

            var rawText = currentContainer.TextToType;
            var regex = new System.Text.RegularExpressions.Regex("<[^>]+>");

            while (_currentCharacterIndex < rawText.Length)
            {
                var match = regex.Match(rawText, _currentCharacterIndex);

                if (match.Success && match.Index == _currentCharacterIndex)
                {
                    _textPlace.text += match.Value;
                    _currentCharacterIndex += match.Length;
                }
                else
                {
                    var currentChar = rawText[_currentCharacterIndex];
                    _textPlace.text += currentChar;

                    if (currentChar != ' ' && currentChar != '\n')
                    {
                        var random = Random.Range(0, 2);
                        if (random == 1)
                        {
                            _characterAudioPlayer.PlayShot();
                        }
                    }

                    _currentCharacterIndex++;

                    yield return new WaitForSeconds(_typeDelay);
                }
            }

            EndCoroutine();

            if (!currentContainer.DeTypeAfter)
            {
                AddToNextContainer();
                if (_currentContainerIndex < _textContainers.Length)
                {
                    _hintHelp.MustPressKeyAgain();
                }
            }
            else
            {
                Del = true;
                StartCoroutine(_DeTypeText());
            }
        }

        private IEnumerator _DeTypeText()
        {
            while (!Keyboard.current.anyKey.wasPressedThisFrame || !Del)
            {
                Del = false;
                yield return null;  // Очікуємо на натискання кнопки
            }
            Del = false;
            var currentContainer = _textContainers[_currentContainerIndex];
            currentContainer.OnTextStarted?.Invoke();

            

            while (_textPlace.text.Length > 0)
            {
                _textPlace.text = _textPlace.text.Substring(0, _textPlace.text.Length - 1);

                var random = Random.Range(0, 2);
                if (random == 1)
                {
                    _characterAudioPlayer.PlayShot();
                }

                yield return new WaitForSeconds(_typeDelay);
            }
            _isTyping = true;
            EndCoroutine();

            if (_currentContainerIndex < _textContainers.Length)
            {
                _hintHelp.MustPressKeyAgain();
            }

            AddToNextContainer();
        }


        private void EndCoroutine()
        {
            _textContainers[_currentContainerIndex].OnTextEnded?.Invoke();
            _isTyping = false;

            _currentCoroutine = null;
        }

        private void AddToNextContainer()
        {
            if (_currentContainerIndex < _textContainers.Length)
            {
                _currentContainerIndex++;
            }
        }
        
        [Serializable]
        public class TypeTextContainer
        {
            [TextArea(5, 10)]
            [SerializeField] private string _textToType;
            [SerializeField] private bool _deTypeAfter;

            [Header("Customizable Typing & Deleting")]

            [SerializeField] private UnityEvent _onTextEnded;
            [SerializeField] private UnityEvent _onTextStarted;

            public bool DeTypeAfter => _deTypeAfter;
            public string TextToType => _textToType;

            public UnityEvent OnTextEnded => _onTextEnded;
            public UnityEvent OnTextStarted => _onTextStarted;
        }
    }
}