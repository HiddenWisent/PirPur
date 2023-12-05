using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnitSelectElement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private Unit unit;
    [SerializeField]
    private Image sprite;

    private void Start()
    {
        sprite.sprite = unit.spriteSmall;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            BattleSystem.current.UnitRightClick(unit);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            BattleSystem.current.UnitRightRelease();
        }
    }

}
