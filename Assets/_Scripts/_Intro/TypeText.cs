using System;
using _Scripts.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _Scripts._Intro
{
    public class TypeText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textPlace;
        [SerializeField] private TypeTextContainer[] _textContainers;
        
        [SerializeField] private AudioPlayer _characterAudioPlayer;
        
        private int _currentContainerIndex;
        private int _currentCharacterIndex;
        private bool _isDeleting;

        [SerializeField] private bool _canType = true;

        public void SetCanType(bool canType)
        {
            _canType = canType;
        }
        
        private void Update()
        {
            if(!_canType) return;
            if (_isDeleting)
            {
                HandleTextDeletion();
            }
            else
            {
                HandleTextTyping();
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void HandleTextTyping()
        {
            if (_textContainers == null || _textContainers.Length == 0)
            {
                Debug.LogError("Text containers are empty or null.");
                _canType = false;
                return;
            }

            if (_currentContainerIndex >= _textContainers.Length)
            {
                Debug.LogError($"Container index {_currentContainerIndex} is out of bounds.");
                _canType = false;
                return;
            }

            var currentContainer = _textContainers[_currentContainerIndex];
            if (currentContainer == null || string.IsNullOrEmpty(currentContainer.TextToType))
            {
                Debug.LogError($"Container {_currentContainerIndex} is null or has no text.");
                HandleContainerCompletion();
                return;
            }

            if (_currentCharacterIndex < currentContainer.TextToType.Length)
            {
                int typeAmount = currentContainer.TypeAmount;

                if (Keyboard.current.anyKey.wasPressedThisFrame)
                {
                    typeAmount = Mathf.Min(typeAmount, currentContainer.TextToType.Length - _currentCharacterIndex);

                    for (int i = 0; i < typeAmount; i++)
                    {
                        if (_currentCharacterIndex >= currentContainer.TextToType.Length)
                        {
                            Debug.LogWarning($"Character index {_currentCharacterIndex} is out of bounds for container {_currentContainerIndex}.");
                            break;
                        }

                        char currentChar = currentContainer.TextToType[_currentCharacterIndex];

                        if (currentChar == '<')
                        {
                            int endIndex = currentContainer.TextToType.IndexOf('>', _currentCharacterIndex);
                            if (endIndex != -1)
                            {
                                _textPlace.text += currentContainer.TextToType.Substring(_currentCharacterIndex,
                                    endIndex - _currentCharacterIndex + 1);
                                _currentCharacterIndex = endIndex + 1;
                                continue;
                            }
                        }

                        if (currentChar is ' ' or ',' or '\n' or '\r' or '\t')
                        {
                            _textPlace.text += currentChar;
                        }
                        else
                        {
                            _textPlace.text += currentChar;
                            _characterAudioPlayer?.PlayShot();
                        }

                        _currentCharacterIndex++;
                    }

                    if (_currentCharacterIndex >= currentContainer.TextToType.Length)
                    {
                        Debug.Log($"Text in container {_currentContainerIndex} ended.");
                        HandleContainerCompletion();
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Character index {_currentCharacterIndex} is out of bounds for container {_currentContainerIndex}.");
                HandleContainerCompletion();
            }
        }

        private void HandleTextDeletion()
        {
            if (_currentContainerIndex >= _textContainers.Length || _textContainers[_currentContainerIndex] == null)
            {
                Debug.LogError($"Container index {_currentContainerIndex} is out of bounds or null during deletion.");
                _isDeleting = false;
                return;
            }

            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                int deleteAmount = _textContainers[_currentContainerIndex].DeleteAmount;

                deleteAmount = Mathf.Min(deleteAmount, _textPlace.text.Length);

                if (_textPlace.text.Length > 0)
                {
                    _textPlace.text = _textPlace.text.Substring(0, _textPlace.text.Length - deleteAmount);
                    _characterAudioPlayer?.PlayShot();
                }
                else
                {
                    _isDeleting = false;
                    _currentContainerIndex++;
                    _currentCharacterIndex = 0;

                    if (_currentContainerIndex < _textContainers.Length)
                    {
                        StartTextTyping();
                    }
                }
            }
        }

        private void HandleContainerCompletion()
        {
            if (_textContainers[_currentContainerIndex].DeTypeAfter)
            {
                _isDeleting = true;
            }
            else
            {
                _textContainers[_currentContainerIndex].OnTextEnded?.Invoke();

                if (_currentContainerIndex < _textContainers.Length - 1)
                {
                    _currentContainerIndex++;
                    _currentCharacterIndex = 0;

                    if (!_textContainers[_currentContainerIndex - 1].DeTypeAfter)
                    {
                        _textPlace.text = string.Empty;
                    }

                    StartTextTyping();
                }
                else
                {
                    Debug.Log("All containers have been processed.");
                    _canType = false;
                }
            }
        }

        private void StartTextTyping()
        {
            _textContainers[_currentContainerIndex].OnTextStarted?.Invoke();
    
            _isDeleting = false;
        }
        
        [Serializable]
        public class TypeTextContainer
        {
            [TextArea(5, 10)]
            [SerializeField] private string _textToType;
            [SerializeField] private bool _deTypeAfter;

            [Header("Customizable Typing & Deleting")]
            [SerializeField] private int _typeAmount = 1;
            [SerializeField] private int _deleteAmount = 1;

            [SerializeField] private UnityEvent _onTextEnded;
            [SerializeField] private UnityEvent _onTextStarted;

            public bool DeTypeAfter => _deTypeAfter;
            public string TextToType => _textToType;
            public int TypeAmount => _typeAmount;
            public int DeleteAmount => _deleteAmount;

            public UnityEvent OnTextEnded => _onTextEnded;
            public UnityEvent OnTextStarted => _onTextStarted;
        }
    }
}
