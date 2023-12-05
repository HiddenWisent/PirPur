using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler, IPointerUpHandler
{
    [SerializeField]
    private Canvas HUDCanvas;
    [SerializeField]
    private Image unitArt;
    [SerializeField]
    private int index;
    private int siblingIndex;

    private CanvasGroup artCanvasGroup;
    private RectTransform rectTransform;
    private Canvas artCanvas;

    private void Awake()
    {
        artCanvas = unitArt.GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        artCanvasGroup = unitArt.GetComponent<CanvasGroup>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            BattleSystem.current.UnitRightClick(index);
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
            BattleSystem.current.UnitButtonPress(index);
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            return;
        else if (index < 8 && BattleSystem.current.IsUnitThere(index) && BattleSystem.current.AreUnitsDraggable())
        {
            artCanvas.sortingOrder = 2;
            artCanvasGroup.alpha = .95f;
            artCanvasGroup.blocksRaycasts = false;
            BattleSystem.draggedUnitIndex = index;
        }
        else
            eventData.pointerDrag = null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            unitArt.rectTransform.anchoredPosition += eventData.delta / HUDCanvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            artCanvas.sortingOrder = 1;
            artCanvasGroup.alpha = 1f;
            artCanvasGroup.blocksRaycasts = true;
            unitArt.rectTransform.position = rectTransform.position + new Vector3(0, 10) * HUDCanvas.scaleFactor;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (eventData.pointerDrag != null)
                BattleSystem.current.SwapUnitsInTeam(index);
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
