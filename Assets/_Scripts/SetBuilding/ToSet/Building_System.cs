using UnityEngine;

public class Building_System : MonoBehaviour
{

    private Vector3 mousePos;
    private Camera mainCamera;
    private float position;
    private Beam_System _currentBeam;
    [SerializeField]private Transform _currentBuilding;
    [SerializeField]private int SizeOfGrid;
    [SerializeField]private LayerMask RayIgnore;



    
    void Awake()
    {
        mainCamera = Camera.main;    
    }


    void Update()
    {
        DragObj(_currentBuilding);
        
        if(IsPointedOnTag("Beam"))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
        
            if(Physics.Raycast(ray, out hit))
            {
                if(_currentBeam != hit.transform.parent.GetComponent<Beam_System>() && _currentBeam != null)
                    _currentBeam.DeactivateAllMarks();

                _currentBeam = hit.transform.parent.GetComponent<Beam_System>();
                
                _currentBeam.ActiveAllMarks();
            }
        }

        
        else if(IsPointedOnTag("Mark"))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        
            RaycastHit hit;
        
            if(Physics.Raycast(ray, out hit))
            {
                
                _currentBuilding.position = hit.transform.position;
                _currentBuilding.rotation = hit.transform.rotation;
                
            }
        }
        else
        {
            
            if(_currentBeam != null)
            {
                _currentBuilding.rotation = _currentBeam.transform.rotation;
                Debug.Log("sd");
                _currentBeam.DeactivateAllMarks();
                _currentBeam = null;
            }
            
            

            
             
        }

        if(Input.GetMouseButtonDown(0))
        {
            if(IsPointedOnTag("Mark"))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        
                RaycastHit hit;
        
                if(Physics.Raycast(ray, out hit))
                {
                    if(!IsColliderIntersecting(FindColloderInChilds(hit.transform.gameObject)) )
                    {
                        var newObj = Instantiate(_currentBuilding, new Vector3(hit.transform.position.x, hit.transform.position.y - 0.3f, hit.transform.position.z), hit.transform.rotation);
                        foreach (var child in newObj.GetComponentsInChildren<Transform>())
                        {
                            child.gameObject.layer = 0;   
                        }
                        newObj.gameObject.layer = 0;
                        hit.transform.gameObject.SetActive(false);
                        Destroy(_currentBuilding.gameObject);
                        _currentBeam.DeactivateAllMarks();
                        DisableBuilding();
                    }
                    else Debug.Log(FindColloderInChilds(hit.transform.gameObject).transform.tag);
                    
                }
            }
        }
        if(Input.GetKey("r"))
        {
            if(IsPointedOnTag("Mark"))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        
                RaycastHit hit;
        
                if(Physics.Raycast(ray, out hit))
                {
                    var _current = hit.transform.gameObject;
                    _currentBeam.ActiveAllClockwiseMarks(_current);

                    var TransNewMarkPos = _currentBeam.GetInfoOfNextMarkByClockwise(_current);
                    _currentBuilding.position = TransNewMarkPos.position;
                    _currentBuilding.rotation = TransNewMarkPos.rotation;
                }
            }

        }
        
        
    }

    bool IsPointedOnTag(string TheTag)
    {
        
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            //Debug.Log(hit.transform.tag);
            return hit.transform.tag == TheTag;
        }
        else return false;
    }

    void DragObj(Transform Obj)
    {
        /*Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        
        RaycastHit hit;
        
        if(Physics.Raycast(ray, out hit))
        { 
            if((int) hit.point.x % SizeOfGrid ==0 ||
               (int) hit.point.y % SizeOfGrid ==0 || 
               (int) hit.point.z % SizeOfGrid ==0 )
            Obj.position = new Vector3( (int) hit.point.x , (int) hit.point.y, (int) hit.point.z);
        }*/
    }

    void ChangeObj(GameObject ObjToBeChanged, GameObject ObjChanges)
    {
        Instantiate(ObjChanges, ObjToBeChanged.transform);
        ObjToBeChanged.SetActive(false);
        DisableBuilding();
    }


    


    public void CurrentBuilding(Transform newBuilding)
    {
        _currentBuilding = newBuilding;
    }

    public void DisableBuilding()
    {
        _currentBuilding = new GameObject().transform;
        GetComponent<Building_System>().enabled = false;
    }

    private bool IsColliderIntersecting(Collider collider)
    {
        // Отримуємо всі колайдери, які перетинаються з поточним
        Collider[] overlappingColliders = Physics.OverlapBox(
            collider.bounds.center, 
            collider.bounds.extents, 
            collider.transform.rotation
            );

        // Перевіряємо, чи є хоч один колайдер, що перетинається, і не є нашим об'єктом
        foreach (var otherCollider in overlappingColliders)
        {
            Debug.Log(otherCollider.transform.name + "hlujikadswfljkhgfoijpzdfskljhbdafs");
            if(otherCollider.transform.tag == "Mark") continue;
            else if (otherCollider != collider ) return true; 
    }

    return false; // Перетину немає
    }

    private Collider FindColloderInChilds(GameObject ParentObj)
    {
        foreach (var child in ParentObj.GetComponentsInChildren<Transform>())
        {
            if(child.GetComponent<Collider>()) return child.GetComponent<Collider>()    ;
        }
        return new Collider();
    }


}
