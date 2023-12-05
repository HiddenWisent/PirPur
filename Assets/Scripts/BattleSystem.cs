using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public enum BattleState { ADVENTURE, SETUP, PLAYERTURN, ENEMYTURN, WON, LOST, WAIT, SELECT }

public class BattleSystem : MonoBehaviour
{
    #region Variables
    public static BattleSystem current;

    private void Awake()
    {
        current = this;
    }

    [SerializeField]
    private UIDarkener UIDarkener;
    public TeamHUD adventureTeamHUD;
    [SerializeField]
    private TeamHUD adventureEnemyHUD;
    [SerializeField]
    private TeamHUD playerTeamHUD;
    [SerializeField]
    private TeamHUD enemyTeamHUD;
    [SerializeField]
    private Team defaultPlayerTeam;
    [SerializeField]
    private UnitSelector unitSelector;
    [Space]
    [SerializeField]
    private GameObject AdventureUI;
    [SerializeField]
    private GameObject combatUI;
    [SerializeField]
    private GameObject startCombatButton;
    [SerializeField]
    private VictoryScreen victoryScreen;
    [SerializeField]
    private PauseMenu pauseMenu;

    public bool pause { get; set; } = false;
    [NonSerialized]
    public int kills = 0;
    private int victories = 0;

    public Team playerTeam { get; private set; }
    private Team enemyTeam;

    private int currentUnitIndex;
    private Unit currentUnit;
    private Queue<int> turnOrder = new Queue<int>();
    public List<int> viableTargets { get; private set; } = new List<int> { };
    public BattleState state;
    public Unit[] allUnits { get; private set; } = new Unit[16];
    public BattleState GetBattleState()
    {
        return state;
    }
    [NonSerialized]
    static public int draggedUnitIndex;
    #endregion

    #region Adventure
    void Start()
    {
        CameraStart();
        state = BattleState.SELECT;
        playerTeam = Instantiate(defaultPlayerTeam);
        playerTeam.InstantiateTeam(defaultPlayerTeam);
        playerTeam.setAlive();
        playerTeam.alignmentIndex = 0;

        adventureTeamHUD.SetHUD(playerTeam);

        AdventureUI.SetActive(true);
        combatUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            PauseMenu();
    }
    public void ShowAdventureEnemyHUD(Team team)
    {
        adventureEnemyHUD.SetHUD(team);
        adventureEnemyHUD.GetComponent<CanvasGroup>().DOFade(1f, 0.07f);
        adventureEnemyHUD.GetComponent<CanvasGroup>().blocksRaycasts = true;
        DarkenUI();
    }
    public void HideAdventureEnemyHUD()
    {
        adventureEnemyHUD.GetComponent<CanvasGroup>().DOFade(0f, 0.07f);
        adventureEnemyHUD.GetComponent<CanvasGroup>().blocksRaycasts = false;
        UnDarkenUI();
    }

    #endregion

    #region CombatStartEnd
    public void CombatStart(Team team)
    {
        CameraSwap();
        enemyTeam = Instantiate(team);
        enemyTeam.InstantiateTeam(team);
        enemyTeam.alignmentIndex = 8;

        SwapUI();
        state = BattleState.WAIT;
        StartCoroutine(SetupCombat());
    }

    IEnumerator SetupCombat()
    {
        turnOrder.Clear();
        enemyTeam.setAlive();

        playerTeamHUD.SetHUD(playerTeam);
        enemyTeamHUD.SetHUD(enemyTeam);

        yield return new WaitForSeconds(.5f);

        startCombatButton.SetActive(true);

        state = BattleState.SETUP;
    }

    public void OnStartCombatButton()
    {
        onCombatStart(playerTeam);
        onCombatStart(enemyTeam);

        state = BattleState.WAIT;
        startCombatButton.SetActive(false);
        playerTeam.CreateHUD(playerTeamHUD);
        enemyTeam.CreateHUD(enemyTeamHUD);

        for (int i = 0; i < 8; i++)
        {
            allUnits[i] = playerTeam.roster[i];
        }

        for (int i = 0; i < 8; i++)
        {
            allUnits[i + 8] = enemyTeam.roster[i];
        }

        StartCoroutine(BeginWait());
    }
    IEnumerator BeginWait()
    {
        yield return new WaitForSeconds(0.3f);
        NewTurnStart();
    }
    public void FinishCombat(Team losingTeam)
    {
        if (losingTeam == playerTeam)
        {
            playerTeam.SetHUD();
            state = BattleState.LOST;
            onGameLost?.Invoke();
            DarkenUI();
        }
        else
        {
            playerTeam.StopDefending();
            playerTeam.SetHUD();
            enemyTeam.SetHUD();
            victories++;
            if (victories == 5)
            {
                state = BattleState.WON;
                victoryScreen.Activate(kills);
            }
            else
            {
                unitSelector.VictoryReward();
            }
        }
    }

    public void PostVictory()
    {
        CameraSwap();
        enemyTeam.KillTeam();
        playerTeam.HealAll();
        playerTeam.DestroyDead();
        playerTeam.SetHUD();
        onCombatEnd();
        SwapUI();
        adventureTeamHUD.SetHUD(playerTeam);
    }

    #endregion

    #region Combat
    private void NewTurnStart()
    {
        List<Speed> turnOrderList = new List<Speed>();
        for (int i = 0; i < 8; i++)
        {
            if (!playerTeam.IsNullOrDead(i))
                turnOrderList.Add(new Speed(playerTeam.roster[i].initiative + UnityEngine.Random.Range(0, 3), i));
            if (!enemyTeam.IsNullOrDead(i))
                turnOrderList.Add(new Speed(enemyTeam.roster[i].initiative + UnityEngine.Random.Range(0, 3), i + 8));
        }

        turnOrderList.Sort((b, a) => a.initiative.CompareTo(b.initiative + UnityEngine.Random.Range(-0.3f, 0.3f)));
        foreach (Speed unit in turnOrderList)
        {
            turnOrder.Enqueue(unit.index);
        }

        for (int i = 0; i < 16; i++)
        {
            if (allUnits[i] != null)
                allUnits[i].waited = false;

        }
        ChooseNewUnit();
    }

    private void ChooseNewUnit()
    {

        viableTargets.Clear();
        if (turnOrder.Count > 0)
        {
            currentUnitIndex = turnOrder.Dequeue();

            if (currentUnitIndex < 8)
            {
                if (!playerTeam.IsNullOrDead(currentUnitIndex))
                {
                    currentUnit = playerTeam.roster[currentUnitIndex];
                    onNewUnitChosen?.Invoke(currentUnitIndex);
                    currentUnit.defending = false;
                    playerTeam.SetHUD();
                    PlayerTurn();
                }
                else
                {
                    ChooseNewUnit();
                }
            }
            else if (!enemyTeam.IsNullOrDead(currentUnitIndex - 8))
            {
                onNewUnitChosen?.Invoke(currentUnitIndex);
                currentUnitIndex -= 8;
                currentUnit = enemyTeam.roster[currentUnitIndex];
                currentUnit.defending = false;
                enemyTeam.SetHUD();
                StartCoroutine(EnemyTurn());
            }
            else
            {
                ChooseNewUnit();
            }
        }
        else
        {
            NewTurnStart();
        }
    }

    private void PlayerTurn()
    {
        state = BattleState.PLAYERTURN;
        currentUnit.HUD.SetHUDColor(new Color32(178, 141, 0, 255));
        viableTargets = ViableTargets(currentUnitIndex, enemyTeam, currentUnit.target);
        onUnitSelect?.Invoke(currentUnitIndex);

        for (int i = 0; i < viableTargets.Count; i++)
        {
            if (!enemyTeam.IsNullOrDead(viableTargets[i])) enemyTeamHUD.roster[viableTargets[i]].SetHUDColor(new Color32(220, 0, 0, 255));
        }

    }

    IEnumerator Attack(int targetIndex, Team targetTeam)
    {
        state = BattleState.WAIT;

        currentUnit.HUD.SetHUDColor(new Color32(0, 0, 0, 255));
        playerTeam.SetHUD();
        enemyTeam.SetHUD();
        onAttack?.Invoke(currentUnitIndex + 8 - targetTeam.alignmentIndex);


        yield return new WaitForSeconds(currentUnit.animationAttackTimer);

        if (currentUnit.target == "All")
        {
            targetTeam.DamageAll(currentUnit.damage);
            foreach (int index in viableTargets)
            {
                onHit?.Invoke(index + targetTeam.alignmentIndex, currentUnit.unitModel.GetComponent<UnitModel>().damageParticle);
            }
        }
        else
        {
            targetTeam.roster[targetIndex].TakeDamage(currentUnit.damage, targetTeam.HUD.roster[targetIndex]);
            onHit?.Invoke(targetIndex + targetTeam.alignmentIndex, currentUnit.unitModel.GetComponent<UnitModel>().damageParticle);
        }

        yield return new WaitForSeconds(1.1f - currentUnit.animationAttackTimer);

        if (targetTeam.IsTeamDead())
        {
            FinishCombat(targetTeam);
        }
        else
        {
            targetTeam.HUD.SetHUD(targetTeam);
            ChooseNewUnit();
        }
    }

    public void UnitButtonPress(int globalIndex)
    {
        if (state != BattleState.PLAYERTURN)
            return;

        if (globalIndex < 8)
            return;

        OnAttackButton(globalIndex - 8);
    }
    private void OnAttackButton(int targetIndex)
    {
        if (!viableTargets.Contains(targetIndex))
            return;

        StartCoroutine(Attack(targetIndex, enemyTeam));
    }

    public void OnSkipButton()
    {
        if (state != BattleState.PLAYERTURN || currentUnit.waited)
            return;

        onWait?.Invoke(currentUnitIndex);
        turnOrder.Enqueue(currentUnitIndex);
        currentUnit.waited = true;
        StartCoroutine(Skip());
    }

    public void OnDefendButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        onDefend?.Invoke(currentUnitIndex);
        currentUnit.defending = true;
        StartCoroutine(Skip());
    }

    IEnumerator Skip()
    {
        state = BattleState.WAIT;
        playerTeam.SetHUD();
        playerTeamHUD.roster[currentUnitIndex].SetHUDColor(new Color32(0, 0, 0, 255));
        for (int i = 0; i < 8; i++)
        {
            if (enemyTeam.roster[i] != null && enemyTeam.roster[i].isAlive) enemyTeamHUD.roster[i].SetHUDColor(new Color32(0, 0, 0, 255));
        }

        yield return new WaitForSeconds(0.3f);

        ChooseNewUnit();
    }
    IEnumerator EnemyTurn()
    {
        state = BattleState.ENEMYTURN;
        enemyTeamHUD.roster[currentUnitIndex].SetHUDColor(new Color32(178, 141, 0, 255));

        yield return new WaitForSeconds(0.5f);

        int enemyTarget = EnemyTarget();
        if (enemyTarget == -1 || currentUnit.damage == 0)
        {
            enemyTeamHUD.roster[currentUnitIndex].SetHUDColor(new Color32(0, 0, 0, 255));
            onWait(currentUnitIndex + 8);
            currentUnit.defending = true;
            enemyTeam.SetHUD();
            ChooseNewUnit();
        }
        else
        {
            onWait(currentUnitIndex + 8);
            StartCoroutine(Attack(enemyTarget, playerTeam));
        }
    }
    #endregion

    #region Targeting
    private int EnemyTarget()
    {
        viableTargets = ViableTargets(currentUnitIndex, playerTeam, currentUnit.target);
        if (viableTargets.Count == 0)
            return -1;
        return viableTargets[UnityEngine.Random.Range(0, viableTargets.Count)];
    }

    private List<int> ViableTargets(int unitIndex, Team targetTeam, string range)
    {
        Team actingTeam;
        if (targetTeam == playerTeam)
            actingTeam = enemyTeam;
        else
            actingTeam = playerTeam;



        if (range == "Ranged" || range == "All")
        {
            List<int> targets = new List<int>();

            for (int i = 0; i < 8; i++)
            {
                if (!targetTeam.IsNullOrDead(i))
                    targets.Add(i);
            }

            return targets;
        }

        if (unitIndex > 3)
        {
            if (!actingTeam.IsRowDead(0))
                return new List<int>();
            unitIndex -= 4;
        }
        int rangeMod = 0;
        if (targetTeam.IsRowDead(0))
            rangeMod = 4;

        return MeleeViableTargets(unitIndex, rangeMod, targetTeam);
    }

    private List<int> MeleeViableTargets(int unitIndex, int rangeMod, Team targetTeam)
    {
        List<int> targets = new List<int>();

        if (!targetTeam.IsNullOrDead(unitIndex + rangeMod))
        {
            targets.Add(unitIndex + rangeMod);
        }

        for (int i = 0; i < 3; i++)
        {
            if (unitIndex > 0 + i)
                if (!targetTeam.IsNullOrDead(unitIndex - 1 - i + rangeMod))
                    targets.Add(unitIndex - 1 - i + rangeMod);

            if (unitIndex < 3 - i)
                if (!targetTeam.IsNullOrDead(unitIndex + 1 + i + rangeMod))
                    targets.Add(unitIndex + 1 + i + rangeMod);

            if (targets.Count != 0)
                return targets;
        }

        return targets;
    }
    #endregion

    #region Events

    public void SwapUnitsInTeam(int index)
    {
        if (index != draggedUnitIndex)
        {
            Unit tempUnit = playerTeam.roster[index];
            playerTeam.roster[index] = playerTeam.roster[draggedUnitIndex];
            playerTeam.roster[draggedUnitIndex] = tempUnit;
            playerTeam.SetIndexes();

            if (state == BattleState.ADVENTURE)
                adventureTeamHUD.SetHUD(playerTeam);
            else
                playerTeamHUD.SetHUD(playerTeam);
        }
    }
    public bool IsUnitThere(int index)
    {
        return playerTeam.roster[index] is Unit;
    }

    public bool AreUnitsDraggable()
    {
        return state == BattleState.ADVENTURE || state == BattleState.SETUP;
    }

    public bool AreUnitsClickable()
    {
        return state == BattleState.PLAYERTURN || state == BattleState.SELECT || AreUnitsDraggable();
    }

    public event Action<Unit> onUnitRightClick;
    public void UnitRightClick(int index)
    {
        if (AreUnitsClickable())
            if (index < 8)
            {
                if (playerTeam.roster[index] != null)
                    onUnitRightClick?.Invoke(playerTeam.roster[index]);
            }
            else if (enemyTeam.roster[index - 8] != null)
                onUnitRightClick?.Invoke(enemyTeam.roster[index - 8]);
    }

    public void UnitRightClick(Unit unit)
    {
        if (AreUnitsClickable())
            onUnitRightClick?.Invoke(unit);
    }

    public event Action onUnitRightRelease;
    public void UnitRightRelease()
    {
        onUnitRightRelease?.Invoke();
    }

    public event Action<Team> onCombatStart;

    public event Action onCombatEnd;

    public event Action<int> onUnitSelect;

    public event Action<int> onAttack;

    public event Action<int, ParticleSystem> onHit;

    public event Action<int> onNewUnitChosen;

    public event Action<int> onWait;

    public event Action<int> onDefend;

    public event Action onGameLost;

    #endregion

    #region Camera
    [Space]
    [SerializeField]
    public Camera adventureCamera;
    [SerializeField]
    private Camera combatCamera;

    private void CameraStart()
    {
        combatCamera.enabled = false;
        adventureCamera.enabled = true;


    }
    private void CameraSwap()
    {
        combatCamera.enabled = !combatCamera.enabled;
        adventureCamera.enabled = !adventureCamera.enabled;

        combatCamera.GetComponent<PhysicsRaycaster>().enabled = !combatCamera.GetComponent<PhysicsRaycaster>().enabled;
        adventureCamera.GetComponent<PhysicsRaycaster>().enabled = !adventureCamera.GetComponent<PhysicsRaycaster>().enabled;
    }

    #endregion

    #region UI

    public void DarkenUI()
    {
        UIDarkener.DarkenUI();
    }

    public void UnDarkenUI()
    {
        UIDarkener.UnDarkenUI();
    }

    public void SwapUI()
    {
        AdventureUI.SetActive(!AdventureUI.activeSelf);
        combatUI.SetActive(!combatUI.activeSelf);
    }

    public void PauseMenu()
    {
        if (state != BattleState.LOST && state != BattleState.SELECT && state != BattleState.WON)
        {
            if (!pause)
            {
                pauseMenu.ShowMenu();
                pause = true;
                DarkenUI();
            }
            else
                pauseMenu.HideMenu();
        }
    }

    #endregion

    [SerializeField]
    private NodesHandler nodesHandler;

    public void DropTheNodes()
    {
        nodesHandler.DropAllNodes();
    }

    [SerializeField]
    private PlayerScript player;
    public void MovePlayer(Vector3 vector)
    {
        player.MovePlayer(vector);
    }
}
