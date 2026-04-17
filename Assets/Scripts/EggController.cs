using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class EggController : MonoBehaviour
{
    [SerializeField] private int totalSpiders = 8;
    [SerializeField] private float secondsBetweenReleases;
    [SerializeField] private GameObject spiderPrefab;
    
    private Rigidbody2D _rb;
    private Animator _animator;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    public void Open()
    {
        //take it out of physics simulation
        _rb.simulated = false;
        _animator.Play("egg_bust");

        StartCoroutine(nameof(ReleaseSpiders));
    }

    public IEnumerator ReleaseSpiders()
    {
        for (int i = totalSpiders; i > 0; i--)
        {
            Instantiate(spiderPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(secondsBetweenReleases);
        }
        yield break;
    }
}
