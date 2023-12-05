using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI killsText;


    public void Activate(int kills)
    {
        killsText.text = ($"You killed {kills} Enemies!");
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        GetComponent<CanvasGroup>().DOFade(1f, 0.2f);
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene(0);
    }
}
