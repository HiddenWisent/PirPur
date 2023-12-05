using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Node : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    [SerializeField]
    private Team enemyTeam;
    [SerializeField]
    private Light nodeSpotlight;
    public int unitCount { get; set; }
    [SerializeField]
    private List<Unit> availableUnits;
    [SerializeField]
    private XScript xMark;
    private bool stopLight = true;

    private void Awake()
    {
        Vector3 pos = transform.position;
        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            transform.eulerAngles.y + Random.Range(0f, 45f),
            transform.eulerAngles.z
            );
        transform.position = new Vector3(pos.x + Random.Range(-5f, 5f), pos.y, pos.z + Random.Range(-5f, 5f));
    }
    private void Start()
    {
        GenerateTeam();
    }

    public void StartCombat()
    {
        BattleSystem.current.MovePlayer(transform.position);
        BattleSystem.current.kills += unitCount;
        StartCoroutine(CombatRoutine());

    }

    IEnumerator CombatRoutine()
    {
        yield return new WaitForSeconds(1.35f);
        BattleSystem.current.CombatStart(enemyTeam);
        DisableNode();
    }
    private void DisableNode()
    {
        GetComponent<BoxCollider>().enabled = false;
        xMark.material.color = new Color32(202, 30, 30, 100);
    }

    #region TeamGeneration

    public void GenerateTeam()
    {
        for (int i = 0; i < unitCount; i++)
        {
            int randomIndex = Random.Range(0, availableUnits.Count);

            enemyTeam.AddUnit(availableUnits[randomIndex], ChooseIndex(availableUnits[randomIndex]));
        }
    }

    private int ChooseIndex(Unit unit)
    {
        List<int> availablePositions = new List<int>();
        int isRanged = (unit.target != "Melee").GetHashCode();
        for (int i = 0 + (4 * isRanged); i < 4 + (4 * isRanged); i++)
        {
            if (enemyTeam.roster[i] == null)
                availablePositions.Add(i);
        }
        if (availablePositions.Count == 0)
            for (int i = 4 - (4 * isRanged); i < 8 - (4 * isRanged); i++)
            {
                if (enemyTeam.roster[i] == null)
                    availablePositions.Add(i);
            }

        return availablePositions[Random.Range(0, availablePositions.Count)];
    }

    #endregion

    #region Pointers
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            stopLight = false;
            BattleSystem.current.ShowAdventureEnemyHUD(enemyTeam);
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            StartCombat();
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            stopLight = true;
            SetLightOff();
            BattleSystem.current.HideAdventureEnemyHUD();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetLightOn();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetLightOff();
    }


    #endregion

    #region Lighting

    private void SetLightOn()
    {
        if (stopLight)
            nodeSpotlight.DOIntensity(1.1f, 0.3f);
    }

    private void SetLightOff()
    {
        if (stopLight)
            StartCoroutine(WaitForLight());
    }

    IEnumerator WaitForLight()
    {
        yield return new WaitForSeconds(0.05f);
        nodeSpotlight.DOIntensity(0f, 0.25f);
    }

    #endregion

    public void Drop()
    {
        transform.DOMoveY(0f, 1.5f, false);
    }
}


