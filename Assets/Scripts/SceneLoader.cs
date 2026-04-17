using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    void Awake()
    {
        var healthObjects = FindObjectsOfType<HealthSystem>();
        foreach (var h in healthObjects)
        {
            if (h.transform.CompareTag("Player"))
            {
                int test = GameData.Instance.playerHp;
            }
        }
    }
}
