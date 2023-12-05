using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraSweep : MonoBehaviour
{
    public void Sweep()
    {
        transform.DOJump(new Vector3(88.7f, 39.3f, 43.9f), 4f, 1, 2f, false);
    }


}
