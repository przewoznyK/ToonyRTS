using UnityEngine;

public class TeamColorDatabase : MonoBehaviour
{
    public static TeamColorDatabase Instance;
    [SerializeField] private Material[] materialColorList;
    [SerializeField] private Material[] materialUnitList;
    [SerializeField] private Material[] materialBuildingList;
    private void Awake()
    {
        Instance = this;
    }

    public Material GetTeamMaterialColor(TeamColorEnum teamColorEnum)
    {
        switch (teamColorEnum)
        {
            case TeamColorEnum.Blue:
                return materialColorList[0];
            case TeamColorEnum.Red:
                return materialColorList[1];
        }
        return null;
    }

    public Material GetTeamMaterialUnit(TeamColorEnum teamColorEnum)
    {
        switch (teamColorEnum)
        {
            case TeamColorEnum.Blue:
                return materialUnitList[0];
            case TeamColorEnum.Red:
                return materialUnitList[1];
        }
        return null;
    }

    public Material GetTeamBuildingUnit(TeamColorEnum teamColorEnum)
    {
        switch (teamColorEnum)
        {
            case TeamColorEnum.Blue:
                return materialBuildingList[0];
            case TeamColorEnum.Red:
                return materialBuildingList[1];
        }
        return null;
    }
}
