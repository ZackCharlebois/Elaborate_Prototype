using System;
using UnityEngine;

public class ProjectileSlimer : Projectile
{
    [SerializeField] private AudioClip _soundSlimed;
    [SerializeField] private GameObject slimePrefab;
    
    //[SerializeField] private LayerMask layersToDamage; //check by layer
    
    private void OnCollisionEnter2D(Collision2D other)    
    {
        //this projectile goes away when hitting anything
        Destroy(gameObject);
        
        //Check by layer 
        //if ((layersToDamage.value & (1 << other.gameObject.layer)) > 0)
        
        //Check by tag
        if (other.transform.CompareTag(_tagToAffect))
        {
            //Eggs are enemies that can't be changed into slimes
            if (other.transform.GetComponent<EggController>())
            {
                AudioSystem.Instance.PlaySound(_soundNoEffect, transform.position);
                return;
            }
            
            AudioSystem.Instance.PlaySound(_soundSlimed, transform.position);
            
            //destroy enemy hit (it will be around for the rest of this method)
            Destroy(other.gameObject);
            
            //create a slime object (instantiate)
            var slime = Instantiate(slimePrefab, other.transform.position, Quaternion.identity);
            if (!slime) return;
            
            //Transfer the velocity from the enemy hit to the new slime object
            var slimeRb = slime.transform.GetComponent<Rigidbody2D>();
            if (!slimeRb) return;
            
            var otherRb = other.transform.GetComponent<Rigidbody2D>();
            if (!otherRb) return; 

            slimeRb.velocity = otherRb.velocity;
            slimeRb.gravityScale = otherRb.gravityScale;
        }
        else
        {
            //AudioSystem.Instance.PlaySound(_soundNoEffect, transform.position);
        }
    }
}