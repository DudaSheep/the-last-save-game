using UnityEngine;
using UnityEngine.Playables;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("Estado do Jogo")]
    public bool isCutsceneActive = true;

    [Header("Referência da Cutscene")]
    public PlayableDirector timelineDirector; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        isCutsceneActive = true; 
    }

    void Update()
    {
        if (isCutsceneActive && timelineDirector != null)
        {
            if (timelineDirector.time >= (timelineDirector.duration - 0.1))
            {
                isCutsceneActive = false;
                Debug.Log("A cutscene acabou pelo relógio! O jogador já pode se mover.");
            }
            
            else if (!timelineDirector.gameObject.activeInHierarchy && timelineDirector.time > 1f)
            {
                isCutsceneActive = false;
                Debug.Log("A Timeline foi desativada! O jogador já pode se mover.");
            }
        }
    }
}