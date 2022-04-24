using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private List<Player> players;
    // Start is called before the first frame update
    void Start()
    {
        InstantiatePlayerCharacters();
    }

    private void InstantiatePlayerCharacters()
    {
        var playerControllers = FindObjectsOfType<PlayerController>();
        foreach (var playerController in playerControllers)
        {
            /*var player = Instantiate(playerController.PlayerAsset.Prefab, Vector3.zero, Quaternion.identity);
            player.name = playerController.PlayerAsset.Name;
            DontDestroyOnLoad(player);
            player.SetActive(false);
            players.Add(player.GetComponent<Player>());*/
        }
    }
}
