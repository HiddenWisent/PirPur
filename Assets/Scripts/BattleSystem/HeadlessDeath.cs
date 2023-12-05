using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HeadlessDeath : MonoBehaviour
{
    [SerializeField]
    private GameObject body;
    private Material unitMaterial;
    [SerializeField]
    private ParticleSystem head;

    private void Start()
    {
        unitMaterial = body.GetComponent<Renderer>().material;
        unitMaterial.DOFade(.8f, 0f);
    }

    public void Kill()
    {
        if (gameObject.activeSelf)
            StartCoroutine(OnDeath());
    }
    IEnumerator OnDeath()
    {
        head.Play();
        unitMaterial.DOFade(.8f, 0f);
        yield return new WaitForSeconds(0.3f);
        head.Stop();
        unitMaterial.DOFade(0f, 0.5f);
    }

    public void OnAppear()
    {
        head.Play();
        unitMaterial.DOFade(.8f, 0.2f);
    }

}
