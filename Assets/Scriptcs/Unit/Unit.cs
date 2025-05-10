using UnityEngine;

public class Unit : MonoBehaviour, IActiveClickable
{
    Transform activator;
    private void Start()
    {
        activator = transform.GetChild(0);
    }

    ObjectTypeEnum IActiveClickable.ActiveObject()
    {
        activator.gameObject.SetActive(true);
        return ObjectTypeEnum.unit;
    }

    public void DeActiveObject()
    {
        activator.gameObject.SetActive(false);
    }


}
