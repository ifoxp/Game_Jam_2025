using _Scripts.Interact;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class HouseGenerateResource : MonoBehaviour
{
    [Header("г�� ��������")]
    [SerializeField] private List<UpgradeLevel> upgradeLevels;

    [Header("�������� �����������")]
    [SerializeField] private float productionInterval = 5f; // ��� �� ������������ (� ��������)

    private float productionTimer;
    private int currentLevel = 0; // �������� ����� (0 - ������� �����)
    [SerializeField] private string buildingID; // ��������� ������������� �����

    private void Start()
    {
        buildingID = gameObject.name + "_Level";

        LoadLevel();

        InitializePlayerPrefs();

        productionTimer = productionInterval;
    }

    private void Update()
    {
        productionTimer -= Time.deltaTime;

        if (productionTimer <= 0f)
        {
            productionTimer = productionInterval;

            if (currentLevel >= 0 && CanProduceResources())
            {
                ProduceResources();
            }

            DisplayResources();
        }
    }

    private void InitializePlayerPrefs()
    {
        foreach (var level in upgradeLevels)
        {
            foreach (var resource in level.resourcesProduced)
            {
                if (!PlayerPrefs.HasKey(resource.Name.ToString()))
                {
                    PlayerPrefs.SetInt(resource.Name.ToString(), 0);
                }
            }

            foreach (var resource in level.resourcesRequired)
            {
                if (!PlayerPrefs.HasKey(resource.Name.ToString()))
                {
                    PlayerPrefs.SetInt(resource.Name.ToString(), 0);
                }
            }
        }
    }

    private bool CanProduceResources()
    {
        if (currentLevel < 0) return false;

        foreach (var requirement in upgradeLevels[currentLevel].resourcesRequired)
        {
            int currentAmount = PlayerPrefs.GetInt(requirement.Name.ToString(), 0);
            if (currentAmount < requirement.Amount)
            {
                return false;
            }
        }
        return true;
    }

    private void ProduceResources()
    {
        foreach (var requirement in upgradeLevels[currentLevel].resourcesRequired)
        {
            int currentAmount = PlayerPrefs.GetInt(requirement.Name.ToString());
            PlayerPrefs.SetInt(requirement.Name.ToString(), currentAmount - requirement.Amount);
        }

        foreach (var production in upgradeLevels[currentLevel].resourcesProduced)
        {
            int currentAmount = PlayerPrefs.GetInt(production.Name.ToString());
            PlayerPrefs.SetInt(production.Name.ToString(), currentAmount + production.Amount);
        }
    }

    private void DisplayResources()
    {
        foreach (var resource in upgradeLevels[currentLevel].resourcesProduced)
        {
        }

        foreach (var resource in upgradeLevels[currentLevel].resourcesRequired)
        {
        }
    }

    public void Upgrade()
    {
        if (currentLevel + 1 < upgradeLevels.Count)
        {
            currentLevel++;
            SaveLevel();
            Debug.Log("г���� �������� ��: " + (currentLevel + 1));
        }
        else
        {
            Debug.Log("������������ ����� ���������!");
        }
    }
    public UpgradeLevel GetCurrentUpgradeLevel()
    {
        return upgradeLevels[currentLevel];
    }

    public UpgradeLevel GetNextUpgradeLevel()
    {
        return currentLevel + 1 < upgradeLevels.Count ? upgradeLevels[currentLevel + 1] : null;
    }
    private void SaveLevel()
    {
        PlayerPrefs.SetInt(buildingID, currentLevel);
        PlayerPrefs.Save();
        Debug.Log("г���� ����� ���������: " + currentLevel);
    }

    private void LoadLevel()
    {
        if (PlayerPrefs.HasKey(buildingID))
        {
            currentLevel = PlayerPrefs.GetInt(buildingID);
            Debug.Log("г���� ����� �����������: " + currentLevel);
        }
        else
        {
            Debug.Log("г���� ����� �� ��������, ��������������� ������� �����.");
        }
    }

    [Button]
    public void UppateBuild()
    {
        Upgrade();
    }
}

public enum ResourceType
{
    Food,
    Junk,
    Material,
    Energy,
    Population,
    PopulationActive
}

[System.Serializable]
public class ResourceData
{
    [SerializeField]
    public ResourceType Name;

    public int Amount;   // ʳ������ �������
}

[System.Serializable]
public class UpgradeLevel
{
    public List<ResourceData> resourcesProduced; // �������, �� ������������ �� ����� ���
    public List<ResourceData> resourcesRequired; // �������, �������� ��� ����������� �� ����� ���
}
