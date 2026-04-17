using UnityEngine;
using UnityEngine.Playables;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private PlayableDirector _director;

    void Start()
    {
        //if we already played the tutorial this session, we are done
        if (GameData.Instance.tutorialPlayed)
            return;
        
        //play the tutorial using the Playable Director component which will use its set Timeline asset
        _director.Play();
        Invoke(nameof(Finish), (float)_director.duration);
    }

    private void Finish()
    {
        GameData.Instance.tutorialPlayed = true;
    }
}
