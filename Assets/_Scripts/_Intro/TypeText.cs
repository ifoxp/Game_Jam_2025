using System;
using System.Collections;
using _Scripts.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

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
        [SerializeField] private bool _autoType = false; // Додаємо автоматичний режим
        [SerializeField] private float _autoTypeSpeed = 0.05f; // Швидкість автоматичного набору
        private float _nextAutoTypeTime = 0f;

        private bool _isWaitingForKeyPress = false;

        public void SetCanType(bool canType)
        {
            // Якщо ми вже чекаємо, то не дозволяємо змінювати
            if (_isWaitingForKeyPress)
            {
                Debug.LogWarning("Already waiting for key press.");
                return;
            }

            StartCoroutine(WaitUntilCanTypeChange(canType));
        }

        private IEnumerator WaitUntilCanTypeChange(bool canType)
        {
            // Очікуємо завершення набору чи видалення тексту
            while (_isDeleting || _currentCharacterIndex < _textContainers[_currentContainerIndex].TextToType.Length)
            {
                yield return null; // Продовжуємо перевіряти кожен кадр
            }

            // Тепер змінюємо _canType, коли можна
            _canType = canType;
            Debug.Log($"_canType set to: {_canType}");
        }

        private void Update()
        {
            if (!_canType) return;

            if (_isDeleting)
            {
                HandleTextDeletion();
            }
            else
            {
                HandleTextTyping();
            }
        }

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

            // Перевірка, чи потрібно блокувати набір тексту після натискання кнопки
            if (currentContainer.BlockTypingAfterKeyPress && Input.anyKeyDown)
            {
                Debug.Log("Typing is blocked after key press.");
                return;
            }

            if (_currentCharacterIndex < currentContainer.TextToType.Length)
            {
                int typeAmount = currentContainer.TypeAmount;

                // Автоматичний набір тексту
                if (_autoType && Time.time >= _nextAutoTypeTime)
                {
                    _nextAutoTypeTime = Time.time + _autoTypeSpeed;
                    typeAmount = Mathf.Min(typeAmount, currentContainer.TextToType.Length - _currentCharacterIndex);
                }
                // Введення з клавіатури
                else if (!_autoType && Input.anyKeyDown)
                {
                    typeAmount = Mathf.Min(typeAmount, currentContainer.TextToType.Length - _currentCharacterIndex);
                }
                else
                {
                    return;
                }

                for (int i = 0; i < typeAmount; i++)
                {
                    if (_currentCharacterIndex >= currentContainer.TextToType.Length)
                    {
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

                    _textPlace.text += currentChar;
                    _characterAudioPlayer?.PlayShot();
                    _currentCharacterIndex++;
                }

                if (_currentCharacterIndex >= currentContainer.TextToType.Length)
                {
                    HandleContainerCompletion();
                }
            }
            else
            {
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

            if (Input.anyKeyDown || (_autoType && Time.time >= _nextAutoTypeTime))
            {
                _nextAutoTypeTime = Time.time + _autoTypeSpeed;

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
            StartCoroutine(WaitForKeyPressBeforeDeleting());
        }

        private IEnumerator WaitForKeyPressBeforeDeleting()
        {
            Debug.Log("Waiting for key press...");
            yield return new WaitUntil(() => Input.anyKeyDown); // Очікуємо, поки буде натиснута будь-яка клавіша

            Debug.Log("Key pressed. Proceeding...");
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

            [SerializeField] private bool _blockTypingAfterKeyPress = false; // Нове поле для блокування набору тексту після натискання клавіші

            public bool DeTypeAfter => _deTypeAfter;
            public string TextToType => _textToType;
            public int TypeAmount => _typeAmount;
            public int DeleteAmount => _deleteAmount;
            public bool BlockTypingAfterKeyPress => _blockTypingAfterKeyPress; // Геттер для нового поля

            public UnityEvent OnTextEnded => _onTextEnded;
            public UnityEvent OnTextStarted => _onTextStarted;
        }
    }
}
