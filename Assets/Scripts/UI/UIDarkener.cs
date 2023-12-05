using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIDarkener : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void DarkenUI()
    {
        canvasGroup.DOFade(1f, 0.07f);
        canvasGroup.blocksRaycasts = true;
    }

    public void UnDarkenUI()
    {
        if (BattleSystem.current.state != BattleState.SELECT && BattleSystem.current.state != BattleState.LOST && !BattleSystem.current.pause)
        {
            canvasGroup.DOFade(0f, 0.07f);
            canvasGroup.blocksRaycasts = false;
        }
    }
}
