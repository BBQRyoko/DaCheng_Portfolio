using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance != null) 
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(this);
    }

    public static void PlayerDeath() 
    {
        instance.Invoke("RestartScene", 1f);
    }

    void RestartScene() 
    {
        //Debug.Log("canRestart");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
