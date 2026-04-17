using System;

public static class GameEventDispatcher
{
    //Event Setup
    public static event Action EnemyDefeated;
    public static event Action EnemiesAllDefeated;
    public static event Action SlimeCollected;
    public static event Action SceneExited;
    public static event Action PlayerDefeated;

    public static void TriggerPlayerDefeated()
    {
        PlayerDefeated?.Invoke();
    }

    public static void TriggerEnemyDefeated()
    {
        EnemyDefeated?.Invoke();
    }

    public static void TriggerEnemiesAllDefeated()
    {
        EnemiesAllDefeated?.Invoke();
    }

    public static void TriggerSlimeCollected()
    {
        SlimeCollected?.Invoke();
    }

    public static void TriggerSceneExited()
    {
        SceneExited?.Invoke();
    }
}