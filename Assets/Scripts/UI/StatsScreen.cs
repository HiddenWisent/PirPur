using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StatsScreen : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI damageText;
    [SerializeField]
    private TextMeshProUGUI targetText;
    [SerializeField]
    private TextMeshProUGUI initiativeText;
    [SerializeField]
    private TextMeshProUGUI hpText;
    [SerializeField]
    private Image sprite;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        BattleSystem.current.onUnitRightClick += OnStatScreenOpen;
        BattleSystem.current.onUnitRightRelease += OnStatScreenClose;

        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnStatScreenOpen(Unit unit)
    {
        SetStats(unit);
        BattleSystem.current.DarkenUI();
        canvasGroup.DOFade(1f, 0.07f);
        canvasGroup.blocksRaycasts = true;
    }

    private void OnStatScreenClose()
    {
        BattleSystem.current.UnDarkenUI();
        canvasGroup.DOFade(0f, 0.07f);
        canvasGroup.blocksRaycasts = false;
    }

    private void OnDestroy()

    {
        BattleSystem.current.onUnitRightClick -= OnStatScreenOpen;
        BattleSystem.current.onUnitRightRelease -= OnStatScreenClose;
    }

    private void SetStats(Unit unit)
    {
        nameText.text = unit.unitName;
        damageText.text = unit.damage.ToString();
        targetText.text = unit.target;
        initiativeText.text = unit.initiative.ToString();
        hpText.text = unit.currentHP.ToString() + " / " + unit.maxHP.ToString();
        sprite.sprite = unit.sprite;
    }
}
