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
                
                _currentBeam.ActiveAllUpperMarks();
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
                Debug.Log("sd");
                _currentBeam.DeactivateAllMarks();
                _currentBeam = null;
            }

            _currentBuilding.rotation = _currentBeam.transform.rotation;

            
             
        }

        if(Input.GetMouseButtonDown(0))
        {
            if(IsPointedOnTag("Mark"))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        
                RaycastHit hit;
        
                if(Physics.Raycast(ray, out hit))
                {
                    var newObj = Instantiate(_currentBuilding, new Vector3(hit.transform.position.x, hit.transform.position.y - 0.3f, hit.transform.position.z), hit.transform.rotation);
                    foreach (var child in gameObject.GetComponentsInChildren<Transform>())
                    {
                        child.gameObject.layer = 0;
                    }
                    newObj.gameObject.layer = 0;
                    hit.transform.gameObject.SetActive(false);
                    Destroy(_currentBuilding.gameObject);
                    _currentBeam.DeactivateAllMarks();
                    DisableBuilding();
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
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        
        RaycastHit hit;
        
        if(Physics.Raycast(ray, out hit))
        { 
            if((int) hit.point.x % SizeOfGrid ==0 ||
               (int) hit.point.y % SizeOfGrid ==0 || 
               (int) hit.point.z % SizeOfGrid ==0 )
            Obj.position = new Vector3( (int) hit.point.x , (int) hit.point.y, (int) hit.point.z);
        }
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

}
