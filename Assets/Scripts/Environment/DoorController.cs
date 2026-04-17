using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private string doorName = "Make Each Name Unique!";
    [SerializeField] private int requiredSlimeCount = 3;
    [SerializeField] private GameObject blockingObject;
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private AudioClip _soundTalk;
    [SerializeField] private AudioClip _soundOpen;
    
    
    [SerializeField] private GameObject[] confinerObjectsToOpen;

    public void Awake()
    {
        GameData.Instance.RecordClosedDoor(doorName);
        amountText.text = "" + requiredSlimeCount;
    }

    public void Start()
    {
        if (GameData.Instance.openDoors[doorName])
        {
            Destroy(gameObject);
        }
    }

    //If player is in slime shooting mode while next to the door, we will interact
    //Would check in OnTriggerEnter2D except that player might switch to slime shooting mode
    //while in the trigger already.
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player") == false) return;

        //if the player is not currently set to fire the slimer shot, we won't interact
        if (other.GetComponent<ProjectileShooter>().IsInSlimeMode == false)
            return;

        //if the speechbubble isn't already up, let's activate it
        if (speechBubble.activeSelf == false)
        {
            speechBubble.SetActive(true);
            AudioSystem.Instance.PlaySound(_soundTalk, transform.position);
        }

        //Check for necessary slimes
        var collector = other.transform.GetComponent<PlayerCollector>();
        if (!collector) return;

        if (collector.SpendSlimes(requiredSlimeCount))
        {
            OpenDoor();
            AudioSystem.Instance.PlaySound(_soundOpen, transform.position);
        }
    }

    private IEnumerator OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            yield return new WaitForSeconds(1);
            speechBubble.SetActive(false);
        }
    }

    private void OpenDoor()
    {
        GameData.Instance.openDoors[doorName] = true;

        foreach (var o in confinerObjectsToOpen)
        {
            o.SetActive(true);
        }
        
        //this really should be much nicer with animation and a jingle, etc.
        Destroy(gameObject);
    }
}
