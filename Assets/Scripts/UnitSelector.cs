using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UnitSelector : MonoBehaviour
{
    [SerializeField]
    private GameObject[] selectElements = new GameObject[3];
    [SerializeField]
    private TextMeshProUGUI counterText;
    private int counter;
    private int deadUnit;
    private CanvasGroup canvasGroup;
 
    private void Start()
    {
        deadUnit = 0;
        counter = 3;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.DOFade(1f, 0.2f);
    }

    public void VictoryReward()
    {
        BattleSystem.current.state = BattleState.SELECT;
        canvasGroup.blocksRaycasts = true;
        deadUnit = Random.Range(0, 3);
        selectElements[deadUnit].gameObject.SetActive(false);

        counter = 1;
        counterText.text = "Remaining:\n" + counter;
        canvasGroup.DOFade(1f, 0.2f);
    }

    public void AddUnit(Unit unit)
    {
        BattleSystem.current.playerTeam.AddUnit(unit);
        counter -= 1;
        if (counter == 0)
        {
            BattleSystem.current.state = BattleState.ADVENTURE;
            HideSelector();
            if (selectElements[deadUnit].gameObject.activeSelf == false)
            {
                selectElements[deadUnit].gameObject.SetActive(true);
                BattleSystem.current.PostVictory();
            }
            else
            {
                BattleSystem.current.adventureCamera.GetComponent<CameraSweep>().Sweep();
                BattleSystem.current.DropTheNodes();
            }
            return;
        }
        counterText.text = "Remaining:\n" + counter;
    }

    public void HideSelector()
    {
        canvasGroup.DOFade(0f, 0.2f);
        canvasGroup.blocksRaycasts = false;
        BattleSystem.current.UnDarkenUI();
    }
}
