using UnityEngine;

public class ProjectileDamageEnemy : Projectile
{
    [SerializeField] private AudioClip _soundDamageEnemy;
    [SerializeField] private int damage = 5;

    private void OnCollisionEnter2D(Collision2D other)    
    {
        //this projectile goes away when hitting anything
        Destroy(gameObject);
        
        //Check by layer 
        //if ((layersToDamage.value & (1 << other.gameObject.layer)) > 0)

        if (other.transform.CompareTag(_tagToAffect))
        {
            other.transform.GetComponent<HealthSystem>()?.Damage(damage);
            AudioSystem.Instance.PlaySound(_soundDamageEnemy, transform.position);
        }
        else
        {
            //AudioSystem.Instance.PlaySound(_soundNoEffect, transform.position);
        }
    }
}