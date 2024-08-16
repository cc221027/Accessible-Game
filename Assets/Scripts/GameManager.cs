using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int SelectedCharacterIndex { get; private set; } = -1;
    
    public int SelectedTrackIndex { get; private set; } = -1;

    public string winner;
    
    [SerializeField] private List<GameObject> allCharacters = new List<GameObject>();
    [SerializeField] private List<string> trackSceneNames = new List<string>(); 
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public void SelectCharacter(int index)
    {
        SelectedCharacterIndex = index;
        SceneManager.LoadScene(trackSceneNames[SelectedTrackIndex]);
        Debug.Log("Character " + index + " selected");
    }
    public void SelectTrack(int index)
    {
        SelectedTrackIndex = index;
        SceneManager.LoadScene("Character Selection");

    }
    public void ResetSelection()
    {
        SelectedCharacterIndex = -1;
        SelectedTrackIndex = -1;
    }
    
    public void SpawnSelectedCharacter(Vector3 spawnPosition, Quaternion spawnRotation)
    {
       GameObject player = Instantiate(allCharacters[SelectedCharacterIndex], spawnPosition, spawnRotation);

       if (Camera.main != null)
       {
           Camera.main.transform.SetParent(player.transform);

           // Adjust the camera's position and rotation relative to the player
           Camera.main.transform.localPosition = new Vector3(0, 5, -10); // Example offset
           Camera.main.transform.localRotation = Quaternion.identity;
       }
    }
    public void SpawnOpponents(int index, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        GameObject opponent = Instantiate(allCharacters[index], spawnPosition, spawnRotation);
        
        // Disable the PlayerInput component if it exists
        PlayerInput playerInput = opponent.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }

        // Disable the PlayerController script if it exists
        PlayerController playerController = opponent.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }
    }
    
}
