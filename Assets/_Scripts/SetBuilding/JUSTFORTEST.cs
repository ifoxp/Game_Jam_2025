using UnityEngine;

public class JUSTFORTEST : MonoBehaviour
{
    public void CreateObj(GameObject Obj)
    {
        var newObj = Instantiate(Obj);
        Camera.main.GetComponent<Building_System>().enabled = true;
        Camera.main.GetComponent<Building_System>().CurrentBuilding(newObj.transform);
    }
}
