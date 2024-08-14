using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int SelectedCharacterIndex { get; private set; } = -1;
    
    public int SelectedTrackIndex { get; private set; } = -1;

    // List to track enemies
    public List<CompetitorsBehaviour> Competitors { get; private set; } = new List<CompetitorsBehaviour>();
    
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
    
}
