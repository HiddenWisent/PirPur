using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XScript : MonoBehaviour
{
    public Material material;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }
}
