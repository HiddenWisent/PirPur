using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UnitPlatform : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private int index;
    [SerializeField]
    private Light targetLight;
    [SerializeField]
    private Renderer targetIndicator;
    private GameObject model;
    private bool cursorIndicator = false;

    private List<Animator> animators = new List<Animator>();

    public void GetIndex(int index)
    {
        this.index = index;
    }
    private void Start()
    {
        BattleSystem.current.onCombatEnd += OnUnitDestroy;
        BattleSystem.current.onNewUnitChosen += OnLightTurnOn;
        BattleSystem.current.onAttack += OnAttackAnimation;
        BattleSystem.current.onAttack += OnLightTurnOff;
        BattleSystem.current.onHit += OnHitAnimation;
        BattleSystem.current.onWait += OnLightTurnOff;
        BattleSystem.current.onDefend += OnLightTurnOff;
        BattleSystem.current.onUnitSelect += Appear;

        model = Instantiate(BattleSystem.current.allUnits[index].unitModel, transform);

        animators.Add(model.GetComponent<UnitModel>().model.GetComponent<Animator>());

        if (model.GetComponent<UnitModel>().headless)
            animators.Add(model.GetComponent<UnitModel>().model.GetComponent<UnitModel>().model.GetComponent<Animator>());
    }

    private void OnDestroy()
    {
        BattleSystem.current.onCombatEnd -= OnUnitDestroy;
        BattleSystem.current.onNewUnitChosen -= OnLightTurnOn;
        BattleSystem.current.onAttack -= OnAttackAnimation;
        BattleSystem.current.onAttack -= OnLightTurnOff;
        BattleSystem.current.onHit -= OnHitAnimation;
        BattleSystem.current.onWait -= OnLightTurnOff;
        BattleSystem.current.onDefend -= OnLightTurnOff;
        BattleSystem.current.onUnitSelect -= Appear;

        Destroy(model);

    }
    private void OnUnitDestroy()
    {
        Destroy(gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            BattleSystem.current.UnitRightClick(index);
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (BattleSystem.current.viableTargets.Contains(index - 8))
                Disappear();
            BattleSystem.current.UnitButtonPress(index);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            BattleSystem.current.UnitRightRelease();
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cursorIndicator = true;

        if (BattleSystem.current.AreUnitsClickable())
        {
            Appear();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cursorIndicator = false;
        Disappear();
    }

    #region targetIndicator
    private void Appear(int index = 0)
    {

        if (cursorIndicator)
        {
            Color32 color;

            if (BattleSystem.current.viableTargets.Contains(this.index - 8))
                color = new Color32(230, 0, 0, 0);
            else
                color = new Color32(210, 215, 130, 0);

            targetIndicator.material.color = color;

            targetIndicator.material.DOFade(0.9f, 0.15f);
        }
    }

    private void Disappear(int index = 0)
    {
        targetIndicator.material.DOFade(0f, 0.15f);
    }
    #endregion

    #region animations

    private void OnAttackAnimation(int index)
    {
        if (index == this.index)
        {
            foreach (Animator anim in animators)
                anim.SetTrigger("TrAttack");
        }
    }

    private void OnHitAnimation(int index, ParticleSystem particle)
    {
        if (index == this.index)
        {
            ParticleSystem theParticle = Instantiate(particle, transform);
            theParticle.Play();
            if (BattleSystem.current.allUnits[index].isAlive)
                foreach (Animator anim in animators)
                    anim.SetTrigger("TrStagger");
            else
            {
                foreach (Animator anim in animators)
                    anim.SetTrigger("TrDeath");
                if (model.GetComponent<UnitModel>().headless)
                    model.GetComponent<UnitModel>().model.GetComponent<HeadlessDeath>().Kill();
            }
        }
    }

    #endregion

    #region light

    public void OnLightTurnOn(int index)
    {
        if (index == this.index)
        {
            targetLight.DOIntensity(1f, 0.3f);
        }
    }

    public void OnLightTurnOff(int index)
    {
        if (index == this.index)
            StartCoroutine(WaitForLight());
    }

    IEnumerator WaitForLight()
    {
        yield return new WaitForSeconds(0.05f);
        targetLight.DOIntensity(0f, 0.25f);

    }

    #endregion
}
