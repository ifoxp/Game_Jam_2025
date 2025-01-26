using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class TutorialChanger : MonoBehaviour
    {
        [SerializeField] private TutorialContainer[] _guides;
        
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _tutorialText;

        [SerializeField] private int _index;

        public void NextGuide()
        {
            _index = (int)Mathf.Repeat(_index + 1, _guides.Length);
            
            _image.sprite = _guides[_index].TutorialPicture;
            _image.rectTransform.anchoredPosition = _guides[_index].PicturePosition;
            
            _tutorialText.text = _guides[_index].TutorialText;
        }
        
        public void PreviousGuide()
        {
            _index = (int)Mathf.Repeat(_index - 1, _guides.Length);
            
            _image.sprite = _guides[_index].TutorialPicture;
            _image.rectTransform.anchoredPosition = _guides[_index].PicturePosition;
            
            _tutorialText.text = _guides[_index].TutorialText;
        }
        
        [NaughtyAttributes.Button]
        public void SetPicturePosition()
        {
            _guides[_index].PicturePosition = _image.rectTransform.anchoredPosition;
        }
    }

    [Serializable]
    public class TutorialContainer  
    {
        [field: SerializeField] public Sprite TutorialPicture { get; private set; } 
        [field: SerializeField] public string TutorialText { get; private set; }
        [field: SerializeField] public Vector3 PicturePosition;
    }
}
