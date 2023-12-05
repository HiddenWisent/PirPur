using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitButton : MonoBehaviour
{ 
    public void OnButtonPress(int index)
    {
        Debug.Log("Button " + index);
        BattleSystem.current.UnitButtonPress(index);
    }
}
