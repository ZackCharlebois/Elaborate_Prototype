using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    [SerializeField] private Vector2 projectileOffset;
    [SerializeField] private GameObject[] projectilePrefabs;
    [SerializeField] private Color[] colors;

    private SpriteRenderer _playerSpriteRenderer;
    
    private int _currentShot = 0;
    
    public bool IsInSlimeMode { get; private set; } = false;

    private void Awake()
    {
        _playerSpriteRenderer = GetComponent<SpriteRenderer>();
        _playerSpriteRenderer.color = Color.red;
    }

    private void LateUpdate()
    {
        //changes color after animation may have changed colors
        //right now this sacrifices damage animation for seeing
        //current shot color
        _playerSpriteRenderer.color = colors[_currentShot];
    }
    

    public void Fire(Vector2 direction)
    {
        if (projectilePrefabs.Length <= 0) return;
        
        var ball = 
            Instantiate(projectilePrefabs[_currentShot], 
                (Vector2)transform.position + projectileOffset * transform.localScale,
                Quaternion.identity);
        ball.GetComponent<Projectile>()?.SetDirection(direction);
    }

    public void ReadyNext()
    {
        _currentShot++;
        if (_currentShot >= projectilePrefabs.Length) _currentShot = 0;
        
        IsInSlimeMode = projectilePrefabs[_currentShot].GetComponent<ProjectileSlimer>();
    }
    
    private void OnDrawGizmos()
    {
        Vector2 position = (Vector2) transform.position + projectileOffset * transform.localScale;
        Gizmos.DrawIcon(position, "Animation.FilterBySelection" );
        //icon names can be found here: https://unitylist.com/p/5c3/Unity-editor-icons
    }
}
