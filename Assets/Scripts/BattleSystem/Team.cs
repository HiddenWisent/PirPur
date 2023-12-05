using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    public Unit[] roster = new Unit[8];
    public TeamHUD HUD { get; private set; }
    public int alignmentIndex;
    public void CreateHUD(TeamHUD hud)
    {
        HUD = hud;

        for (int i = 0; i < 8; i++)
        {
            if (!IsNullOrDead(i))
                roster[i].HUD = HUD.roster[i];
        }
    }

    public void SetHUD()
    {
        HUD.SetHUD(this);
    }

    public void setAlive()
    {
        for (int i = 0; i < 8; i++)
        {
            if (roster[i] != null)
                if (!roster[i].isAlive)
                    roster[i].InvertDeath();
        }
    }

    public bool IsNullOrDead(int unitIndex)
    {
        if (roster[unitIndex] != null)
            return !roster[unitIndex].isAlive;
        return true;
    }

    public void InstantiateTeam(Team startingRoster)
    {
        for (int i = 0; i < 8; i++)
        {
            if (roster[i] != null)
            {
                Unit temporaryUnit = Instantiate(startingRoster.roster[i]);
                temporaryUnit.gameObject.transform.parent = gameObject.transform;
                roster[i] = temporaryUnit;
                roster[i].SetIndex(i);
            }
        }
    }
    public void SetIndexes()
    {

        for (int i = 0; i < 8; i++)
        {
            if (roster[i] != null)
            {
                roster[i].SetIndex(i);
            }
        }
    }
    public bool IsTeamDead()
    {
        for (int i = 0; i < 8; i++)
        {
            if (!IsNullOrDead(i))
                return false;
        }
        return true;
    }

    public bool IsRowDead(int rowStart)
    {
        for (int i = rowStart; i < rowStart + 4; i++)
        {
            if (!IsNullOrDead(i))
                return false;
        }
        return true;
    }

    public void DamageAll(int damage)
    {
        for (int i = 0; i < 8; i++)
        {

            if (!IsNullOrDead(i))
                roster[i].TakeDamage(damage, HUD.roster[i]);
        }
    }

    public void KillTeam()
    {
        Destroy(gameObject);
    }

    public void HealAll()
    {
        for (int i = 0; i < 8; i++)
            if (!IsNullOrDead(i))
            {
                roster[i].heal();
            }
    }

    public void AddUnit(Unit unit, int index = -1)
    {
        Unit temporaryUnit = Instantiate(unit);
        temporaryUnit.gameObject.transform.parent = gameObject.transform;
        if (index != -1)
        {
            roster[index] = temporaryUnit;
            roster[index].SetIndex(index);
        }
        else
            for (int i = 0; i < 8; i++)
            {
                if (roster[i] == null)
                {
                    roster[i] = temporaryUnit;
                    roster[i].SetIndex(i);
                    BattleSystem.current.adventureTeamHUD.SetHUD(this);
                    return;
                }
            }


    }

    public void DestroyDead()
    {
        for (int i = 0; i < 8; i++)
        {
            if (roster[i] != null)
            {
                if (!roster[i].isAlive)
                {
                    Destroy(roster[i].gameObject);
                    roster[i] = null;
                }
            }
        }
    }

    public void StopDefending()
    {
        for (int i = 0; i < 8; i++)
        {
            if (roster[i] != null)
            {
                roster[i].defending = false;
            }
        }
    }
}
