using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamModelSpace : MonoBehaviour
{
    [SerializeField]
    private UnitModelSpace[] roster = new UnitModelSpace[8];
    [SerializeField]
    private int alignmentIndex;
    private void Start()
    {
        BattleSystem.current.onCombatStart += On3DUnitsInstantiate;
    }

    public void On3DUnitsInstantiate(Team team)
    {
        if (alignmentIndex == team.alignmentIndex)
            for (int i = 0; i < 8; i++)
            {
                if (team.roster[i] != null)
                    roster[i].Instantiate3D(team.roster[i]);
            }
    }
    private void OnDestroy()
    {
        BattleSystem.current.onCombatStart -= On3DUnitsInstantiate;
    }
}
