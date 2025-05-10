using UnityEngine;

public class TeamColorDatabase : MonoBehaviour
{
    public static TeamColorDatabase Instance;
    [SerializeField] private Material[] materialList;
    private void Awake()
    {
        Instance = this;
    }

    public Material GetTeamMaterial(TeamColorEnum teamColorEnum)
    {
        switch (teamColorEnum)
        {
            case TeamColorEnum.Blue:
                return materialList[0];
            case TeamColorEnum.Red:
                return materialList[1];
        }
        return null;
    }

}
