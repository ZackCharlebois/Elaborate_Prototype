using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ParticleSystem))]
public class VictoryController : MonoBehaviour
{
    private ParticleSystem _particles;
    private void Awake()
    {
        _particles = GetComponent<ParticleSystem>();
    }
    private void OnEnable()
    {
        GameEventDispatcher.EnemiesAllDefeated += Celebrate;
    }

    private void OnDisable()
    {
        GameEventDispatcher.EnemiesAllDefeated -= Celebrate;
    }

    private void Celebrate()
    {
        _particles.Play();
    }

    //Invoke(nameof(AdvanceScene), 2f);
    //}

    private void AdvanceScene()
    {
        GameEventDispatcher.TriggerSceneExited();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
