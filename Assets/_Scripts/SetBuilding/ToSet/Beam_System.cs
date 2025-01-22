using UnityEngine;


[System.Serializable]
public class MarksOnBeamRow
{
    public GameObject[] Marks;
}

public class Beam_System : MonoBehaviour
{
 
    [SerializeField] public MarksOnBeamRow[] MarksOnBeam;

    public void ActiveAllUpperMarks()
    {
        DeactivateAllMarks();

        for (int i = 0; i < GetBeamsPerLine(); i++)
        {
            MarksOnBeam[1].Marks[i].SetActive(true);
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

    
    void Update()
    {
        int o = 0;
        o   ++;
    }
}
