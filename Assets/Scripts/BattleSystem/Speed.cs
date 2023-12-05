using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed
{
    public float initiative { get; set; }
    public int index { get; set; }

    public Speed(int initiative, int index)
    {
        this.initiative = initiative;
        this.index = index;
    }
}
