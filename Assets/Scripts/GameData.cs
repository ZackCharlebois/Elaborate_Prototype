using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps track of the data of the game during one session
/// </summary>
public class GameData : SingletonPersistent<GameData>
{
    //public PlayerData playerData;

    //Player DATA
    [SerializeField] private PlayerDataScriptableObject startingPlayerDataSO;

    public int playerHpMax = 15;
    public int playerHp = 12;
    public int playerSlimeCount = 0;
    public int entranceNumber = -1; //entrance number Player will arrive at
    public bool tutorialPlayed = false;

    //Game MAP DATA
    public Dictionary<string, bool> openDoors = new Dictionary<string, bool>();

    public void RecordClosedDoor(string doorName)
    {
        if (openDoors.ContainsKey(doorName)) return; 
        Debug.Log("Closed Door Recorded: " + doorName);
        openDoors.Add(doorName, false);
    }
    
    protected override void Awake()
    {
        base.Awake();
        if (startingPlayerDataSO == null) return;
        
        playerHpMax = startingPlayerDataSO.playerHpMax;
        playerHp = startingPlayerDataSO.playerHp;
        playerSlimeCount = startingPlayerDataSO.playerSlimeCount;
        entranceNumber = startingPlayerDataSO.entranceNumber;

        startingPlayerDataSO = null; //once we copy our starting data, ditch it
    }
}