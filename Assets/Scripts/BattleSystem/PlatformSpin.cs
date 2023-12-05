using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlatformSpin : MonoBehaviour
{

    public float speed;
    void FixedUpdate()
    {
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}
