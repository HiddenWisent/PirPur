using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;

public class UnitHUD : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI hpText;
    [SerializeField]
    private Slider hpSlider;
    [SerializeField]
    private Image sprite;
    [SerializeField]
    private TextMeshProUGUI damageNumber;
    [SerializeField]
    private Image border;
    [SerializeField]
    private Image textBackground;
    [SerializeField]
    private Image defendIcon;

    public void SetHUD(Unit unit)
    {
        hpText.text = $"{unit.currentHP} / {unit.maxHP}";
        hpSlider.minValue = -unit.maxHP;
        hpSlider.value = -unit.currentHP;
        sprite.sprite = unit.spriteSmall;
        sprite.gameObject.SetActive(true);
        damageNumber.text = "";
        SetHUDColor(new Color32(0, 0, 0, 255));
        textBackground.gameObject.SetActive(true);
        defendIcon.gameObject.SetActive(unit.defending);
    }

    public void EmptyHUD()
    {
        hpText.text = "";
        hpSlider.minValue = -1;
        hpSlider.value = -1;
        sprite.sprite = null;
        sprite.gameObject.SetActive(false);
        SetHUDColor(new Color32(0, 0, 0, 128));
        textBackground.gameObject.SetActive(false);
    }

    public void SetHP(int hp, int max)
    {
        hpSlider.value = -hp;
        hpText.text = $"{hp} / {max}";
    }

    public void AffectedInCombat(string actionType, int number, Unit unit)
    {
        damageNumber.text = "-" + number;
        hpText.text = $"{unit.currentHP} / {unit.maxHP}";
        hpSlider.value = 0;
    }


    public void SetHUDColor(Color32 newColor)
    {
        border.color = newColor;
    }

}
