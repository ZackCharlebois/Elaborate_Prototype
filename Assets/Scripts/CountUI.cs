using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CountUI : MonoBehaviour
{
    //[SerializeField] private int startingScore;
    private int _count;
    private TextMeshProUGUI _textMesh;
    
    // Start is called before the first frame update
    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        _count = GameData.Instance.playerSlimeCount;
        _textMesh.text = "" + _count;
    }

    // Update is called once per frame
    public void UpdateCount()
    {
        _count++;
        _textMesh.text = "" + _count;
        
        /*
        if (_count == GameManager.Instance.playerSlimeTargetCount)
        {
            GameEventDispatcher.TriggerSlimeTargetReached();
        }
    */
    }
    
    private void OnEnable()
    {
        GameEventDispatcher.SlimeCollected += UpdateCount;
    }

    private void OnDisable()
    {
        GameEventDispatcher.SlimeCollected -= UpdateCount;
    }
}
