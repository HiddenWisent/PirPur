using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AdventureTeamHUD : MonoBehaviour
{
    private int multiplier = -1;
    public void ShowHideTeam()
    {
        GetComponent<RectTransform>().DOAnchorPosX(175 * multiplier, 0.6f, true);
        multiplier *= -1;
    }
}
