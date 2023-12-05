using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamHUD : MonoBehaviour
{
    public UnitHUD[] roster = new UnitHUD[8];

    public void SetHUD(Team team)
    {
        for (int i = 0; i < 8; i++)
        {
            if (team.roster[i] != null)
                roster[i].SetHUD(team.roster[i]);
            else
                roster[i].EmptyHUD();                 
        }
    }

    public void TeamAffectedInCombat(string actionType, int number, Team affectedTeam)
    {
        for (int i = 0; i < 8; i++)
        {
            if(!affectedTeam.IsNullOrDead(i))
                roster[i].AffectedInCombat(actionType,number, affectedTeam.roster[i]);
        }
    }
}
