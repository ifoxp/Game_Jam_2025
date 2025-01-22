using UnityEngine;

public class Building
{
    private string Name {get;set;}
    private int Price {get;set;}
    private GameObject Model {get;set;}
    
    public bool IsSetable {get;set;}

    public Building(string Name, int Price, GameObject Model, bool IsSetable)
    {
        this.Name = Name;
        this.Price = Price;
        this.Model = Model;
        this.IsSetable = IsSetable;
    }
}
