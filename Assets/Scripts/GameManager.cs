using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int SelectedCharacterIndex { get; set; } = -1;
    
    private int SelectedTrackIndex { get; set; } = -1;

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
        Debug.Log("Track " + index + " selected");

    }
    public void ResetSelection()
    {
        SelectedCharacterIndex = -1;
        SelectedTrackIndex = -1;
        Debug.Log("Reset Selection");
    }
    
    public void SpawnSelectedCharacter(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        Instantiate(allCharacters[SelectedCharacterIndex], spawnPosition, spawnRotation);
    }
    
}
