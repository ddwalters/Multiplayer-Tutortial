using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;

    [Header("NETWORK JOIN")]
    [SerializeField] bool startGameAsClient;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (startGameAsClient) 
        {
            startGameAsClient = false;

            // We must shut down because we start as a host (title screen)
            NetworkManager.Singleton.Shutdown();
            // Restart as client
            NetworkManager.Singleton.StartClient();
        }
    }
}
