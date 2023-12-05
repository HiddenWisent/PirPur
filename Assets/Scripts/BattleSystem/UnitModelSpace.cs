using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitModelSpace : MonoBehaviour
{
    private GameObject model;
    [SerializeField]
    private int index;
    [SerializeField]
    private GameObject platform;

    private void Start()
    {
        
    }
    public void Instantiate3D (Unit unit)
    {
        model = Instantiate(platform, transform.position, transform.rotation);
        model.gameObject.transform.parent = gameObject.transform;
        model.SendMessage("GetIndex", index);
    }
    public void Set3D(Unit unit)
    {

    }
}
