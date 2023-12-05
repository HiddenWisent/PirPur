using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void ShowMenu()
    {
        canvasGroup.DOFade(1f, 0.2f);
        canvasGroup.blocksRaycasts = true;

    }

    public void HideMenu()
    {
        canvasGroup.DOFade(0f, 0.2f);
        canvasGroup.blocksRaycasts = false;
        BattleSystem.current.pause = false;
        BattleSystem.current.UnDarkenUI();
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
