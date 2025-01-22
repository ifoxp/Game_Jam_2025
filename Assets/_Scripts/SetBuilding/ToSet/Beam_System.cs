using UnityEngine;


[System.Serializable]
public class MarksOnBeamRow
{
    public GameObject[] Marks;
}

public class Beam_System : MonoBehaviour
{
 
    [SerializeField] public MarksOnBeamRow[] MarksOnBeam;

    public void ActiveAllMarks()
    {
        DeactivateAllMarks();

        for (int i = 0; i < 4; i++)
        {
            
            for (int j = 0; j < GetBeamsPerLine(); j++)
            {
                MarksOnBeam[i].Marks[j].SetActive(true);    
            }
            
        }
    }
    
    public void ActiveAllClockwiseMarks(int NumberMark)
    {
        DeactivateAllMarks();

        for (int i = 0; i < 4; i++)
        {
            MarksOnBeam[i].Marks[NumberMark].SetActive(true);
        }
    }

    public void ActiveAllClockwiseMarks(GameObject MarkSample)
    {
        int NumberMark = GetNumberInLineOfMark(MarkSample);
        DeactivateAllMarks();

        for (int i = 0; i < 4; i++)
        {
            MarksOnBeam[i].Marks[NumberMark].SetActive(true);
        }
    }

    public void DeactivateAllMarks()
    {
        for (int i = 0; i < 4; i++) // ну у нас же не будет больше сторон чем 4
        {
            for (int j = 0; j < GetBeamsPerLine(); j++)
            {
                MarksOnBeam[i].Marks[j].SetActive(false);
            }
        }
    }

    private int GetBeamsPerLine()
    {
        int RetInt = 0;
        foreach (var item in MarksOnBeam[1].Marks)
        {
            RetInt++;
        }

        return RetInt;
    }

    private int GetNumberInLineOfMark(GameObject ObjToFind)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < GetBeamsPerLine(); j++)
            {
                if(MarksOnBeam[i].Marks[j] == ObjToFind) return j;
            }
        }
        return 0;
    }

    private int GetNumberOfLineOfMark(GameObject ObjToFind)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < GetBeamsPerLine(); j++)
            {
                if(MarksOnBeam[i].Marks[j] == ObjToFind) return i;
            }
        }
        return 0;
    }

    public Transform GetInfoOfNextMarkByClockwise(GameObject _currentMark) 
    {
        int line = GetNumberOfLineOfMark(_currentMark);

        if(line == 4) line = 1;
        else line++;

        return MarksOnBeam[line].Marks[GetNumberInLineOfMark(_currentMark)].transform;
    }

    
    void Update()
    {
        int o = 0;
        o   ++;
    }
}
