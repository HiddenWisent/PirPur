using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalleryModel : MonoBehaviour
{
    [SerializeField]
    private string unitName;
    private Animator myAnimator;
    [SerializeField]
    private ParticleSystem particle;
    private void Start()
    {
        GallerySystem.current.onIdleButton += OnIdleAnimation;
        GallerySystem.current.onAttackButton += OnAttackAnimation;
        GallerySystem.current.onStaggerButton += OnStaggerAnimation;
        GallerySystem.current.onDeathButton += OnDeathAnimation;
        GallerySystem.current.onUnitButton += OnDisabledUnit;

        myAnimator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        GallerySystem.current.onIdleButton -= OnIdleAnimation;
        GallerySystem.current.onAttackButton -= OnAttackAnimation;
        GallerySystem.current.onStaggerButton -= OnStaggerAnimation;
        GallerySystem.current.onDeathButton -= OnDeathAnimation;
    }

    void OnIdleAnimation()
    {
        myAnimator.SetTrigger("TrIdle");
    }

    void OnAttackAnimation()
    {
        if (gameObject.activeSelf)
        {
            myAnimator.SetTrigger("TrAttack");

            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.4f);
        particle.Play();
    }
    void OnStaggerAnimation()
    {
        myAnimator.SetTrigger("TrStagger");
    }

    void OnDeathAnimation()
    {
        myAnimator.SetTrigger("TrDeath");
    }

    void OnDisabledUnit(string name)
    {
        if (name == this.unitName)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }
}
