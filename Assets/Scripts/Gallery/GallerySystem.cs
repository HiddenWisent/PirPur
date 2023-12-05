using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GallerySystem : MonoBehaviour
{
    public static GallerySystem current;

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        HideUnlessNamed("Scarecrow");
    }

    public void OnIdleButtonPress()
    {
        onIdleButton?.Invoke();
    }

    public void OnAttackButtonPress()
    {
        onAttackButton?.Invoke();
    }
    public void OnStaggerButtonPress()
    {
        onStaggerButton?.Invoke();
    }
    public void OnDeathButtonPress()
    {
        onDeathButton?.Invoke();
    }

    public event Action onIdleButton;
    public event Action onAttackButton;
    public event Action onStaggerButton; 
    public event Action onDeathButton;
    public event Action<string> onUnitButton;

    public void HideUnlessNamed(string name)
    {
        onUnitButton?.Invoke(name);
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene(0);
    }
}
