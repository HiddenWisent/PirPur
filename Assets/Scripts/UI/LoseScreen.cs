using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LoseScreen : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private void Start()
    {
        BattleSystem.current.onGameLost += Appear;

        canvasGroup = GetComponent<CanvasGroup>();
    }
    private void OnDestroy()
    {
        BattleSystem.current.onGameLost -= Appear;
    }
    public void OnLoseButtonPress()
    {
        SceneManager.LoadScene(0);
    }

    public void Appear()
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOFade(1f, 0.2f);
    }
}
