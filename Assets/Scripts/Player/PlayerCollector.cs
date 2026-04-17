using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class PlayerCollector : MonoBehaviour
{
    [SerializeField] private int foodHpBoost;
    [SerializeField] private AudioClip _soundEat;
    [SerializeField] private AudioClip _soundCollect;

    private HealthSystem _healthSystem;

    public int SlimeCount { get; private set; }

    private void Start()
    {
        SlimeCount = GameData.Instance.playerSlimeCount;
        _healthSystem = GetComponent<HealthSystem>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Slime"))
        {
            Destroy(other.gameObject);
            SlimeCount++;
            GameEventDispatcher.TriggerSlimeCollected();
            AudioSystem.Instance.PlaySound(_soundCollect, transform.position);
        }

        if (other.transform.CompareTag("Food"))
        {
            Destroy(other.gameObject);
            _healthSystem.Heal(foodHpBoost);
            AudioSystem.Instance.PlaySound(_soundEat, transform.position);
        }
    }

    public bool SpendSlimes(int amt)
    {
        if (amt <= SlimeCount)
        {
            SlimeCount -= amt;
            return true;
        }

        return false;
    }

    private void WriteData()
    {
        GameData.Instance.playerSlimeCount = SlimeCount;
    }

    public void OnEnable()
    {
        GameEventDispatcher.SceneExited += WriteData;
    }

    public void OnDisable()
    {
        GameEventDispatcher.SceneExited -= WriteData;
    }
}