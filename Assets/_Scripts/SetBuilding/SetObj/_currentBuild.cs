using UnityEngine;

public class _currentBuild : MonoBehaviour
{
    private Building _current;

    [SerializeField] private string Name;
    [SerializeField] private int Price;
    [SerializeField] private GameObject Model;
    
    private bool IsSetable;
    
    void Awake()
    {
        _current = new Building(Name, Price, Model, IsSetable);
    }

    

    void SetIsSetable(bool newIsSetable)
    {
        IsSetable = newIsSetable;
    }
    void SetIsSetable()
    {
        IsSetable = false;
    }
    
}
