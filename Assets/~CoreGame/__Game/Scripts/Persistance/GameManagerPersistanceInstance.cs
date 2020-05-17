using CoreGame;
using UnityEngine;

public class GameManagerPersistanceInstance : MonoBehaviour
{

    public GameObject GameManagerPersistancePrefab;

    private static GameManagerPersistanceInstance Instance;

    public void Init()
    {
        if (Instance == null)
        {
            Instance = this;
            Instantiate(GameManagerPersistancePrefab, transform);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (gameObject != Instance.gameObject)
            {
                Destroy(gameObject);
            }
        }
    }

}
