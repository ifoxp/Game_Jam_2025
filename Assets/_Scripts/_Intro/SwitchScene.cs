using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts._Intro
{
    public class SwitchScene : MonoBehaviour
    {
        [SerializeField] private string _sceneName;

        public void _Change()
        {
            SceneManager.LoadScene(_sceneName);
        }
    }
}
