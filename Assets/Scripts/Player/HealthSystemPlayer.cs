using UnityEngine;

class HealthSystemPlayer : HealthSystem
{
    [SerializeField] private AudioClip _soundDamage;
    
    protected override void Start()
    {
        base.Start();
        hp = GameData.Instance.playerHp;
        maxHp = GameData.Instance.playerHpMax;
    }

    public override void Damage(int hpAmount)
    {
        base.Damage(hpAmount);
        AudioSystem.Instance.PlaySound(_soundDamage, transform.position);
        if (hp <= 0)
        {
            GameEventDispatcher.TriggerPlayerDefeated();
        }
    }

    private void WriteData()
    {
        GameData.Instance.playerHp = hp;
        GameData.Instance.playerHpMax = maxHp;
    }
    public void OnEnable()
    {
        GameEventDispatcher.SceneExited += WriteData;
    }

    public void OnDisable()
    {
        GameEventDispatcher.SceneExited -= WriteData;
    }

    public void SelfDamage(int hpAmount)
    {
        hp -= hpAmount;

        //tell any subscriber to this event that damage happened!
        //OnDamaged?.Invoke(hpAmount);
        
        //Health Bar
        UpdateHealthBar();

        //If we hit zero health, raise the HitZero event with all that registered
        if (hp <= 0)
        {
            OnZero?.Invoke();
        }
    }
}