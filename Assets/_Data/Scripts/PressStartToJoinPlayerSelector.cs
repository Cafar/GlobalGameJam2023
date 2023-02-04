using Rewired;
using SOA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressStartToJoinPlayerSelector : MonoBehaviour
{
    [SerializeField]
    private GameEvent OnPlayerConnected;
    [SerializeField]
    private IntReference idRewired;
    [SerializeField]
    private IntReference maxPlayers;
    [SerializeField]
    private GameEvent GameStart;
    [SerializeField]
    private StringSet nameJoyPlayers;

    private List<PlayerMap> playerMap; // Maps Rewired Player ids to game player ids
    private int gamePlayerIdCounter = 0;
    private bool gameStarted = false;

    void Awake()
    {
        playerMap = new List<PlayerMap>();

        gameStarted = false;
        for (int i = 0; i < ReInput.players.playerCount; i++)
        {
            ReInput.players.GetPlayer(i).controllers.maps.SetMapsEnabled(true, "Assignment");
            ReInput.players.GetPlayer(i).controllers.maps.SetMapsEnabled(false, "Gameplay");
        }
    }

    void Update()
    {
        if (gameStarted) return;
        for (int i = 0; i < ReInput.players.playerCount; i++)
        {
            if (ReInput.players.GetPlayer(i).GetButtonDown("AddPlayer"))
            {
                AssignNextPlayer(i);
            }
            if (ReInput.players.GetPlayer(i).GetButtonDown("StartGame"))
            {
                gameStarted = true;
                GameStart?.Raise();
            }
        }
    }

    private void AssignNextPlayer(int rewiredPlayerId)
    {
        if (playerMap.Count >= maxPlayers)
        {
            Debug.LogError("Max player limit already reached!");
            return;
        }

        int gamePlayerId = GetNextGamePlayerId();

        // Add the Rewired Player as the next open game player slot
        playerMap.Add(new PlayerMap(rewiredPlayerId, gamePlayerId));

        Player rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);

        // Disable the Assignment map category in Player so no more JoinGame Actions return
        rewiredPlayer.controllers.maps.SetMapsEnabled(false, "Assignment");

        // Enable UI control for this Player now that he has joined
        rewiredPlayer.controllers.maps.SetMapsEnabled(true, "Gameplay");

        Debug.Log("Added Rewired Player id " + rewiredPlayerId + " to game player " + gamePlayerId);
        idRewired.Value = rewiredPlayerId;
        rewiredPlayer.SetVibration(0, 1,0.4f);
        OnPlayerConnected.Raise();
    }

    private int GetNextGamePlayerId()
    {
        return gamePlayerIdCounter++;
    }

    // This class is used to map the Rewired Player Id to your game player id
    private class PlayerMap
    {
        public int rewiredPlayerId;
        public int gamePlayerId;

        public PlayerMap(int rewiredPlayerId, int gamePlayerId)
        {
            this.rewiredPlayerId = rewiredPlayerId;
            this.gamePlayerId = gamePlayerId;
        }
    } 
}