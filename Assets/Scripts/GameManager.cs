using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int SelectedCharacterIndex { get; private set; } = -1;
    
    public int SelectedTrackIndex { get; private set; } = -1;

    public string winner;

    public float currentPlayerSpeed;
    public int currentPlayerCheckPoints;
    public int currentPlayerLap = 0;
    
    [SerializeField] public List<GameObject> allCharacters = new List<GameObject>();
    [SerializeField] private List<string> trackSceneNames = new List<string>();

    public List<Transform> checkPoints = new List<Transform>();
    public Transform lapCheckPoint;
    
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
    
    
    
    
    
    
}
