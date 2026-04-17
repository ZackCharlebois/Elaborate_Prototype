using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class ExitTrigger : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private int entranceNumber = 1;

    public int GetSceneEntranceNumber()
    {
        return entranceNumber;
    }

    public string GetScene()
    {
        return sceneToLoad;
    }
    
    private void OnDrawGizmos()
    {
        var points = GetComponent<PolygonCollider2D>().points;
        Gizmos.color = Color.yellow;
        
        /*
        foreach (var p in points)
            Gizmos.DrawRay(transform.position, p);
            */
        
        Vector2 center = transform.position;
        for (int i = 0, j = 1; j < points.Length; i++, j++)
        {
            Gizmos.DrawLine(center + points[i], center + points[j]);
        }

        if (points.Length > 1)
        {
            Gizmos.DrawLine(center + points[0], center + points[points.Length - 1]);
        }
        
    }

    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            //Update entranceNumber for next scene
            GameData.entranceNumber = entranceNumber;
            
            //Send out an event to every object that cares about the stage ending
            //for instance, player will need to store health in GameData...
            GameEventDispatcher.StageComplete();

            SceneManager.LoadScene(sceneToLoad);

            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    */
}
