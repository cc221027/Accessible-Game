using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    
    [SerializeField] private TextMeshProUGUI winnerText;

    [SerializeField] private TextMeshProUGUI timeText;

    [SerializeField] private GameObject unlockedOfficerJenkinsPanel;
    [SerializeField] private GameObject unlockedRusselPanel;
    [SerializeField] private GameObject unlockedStellaPanel;
    [SerializeField] private GameObject unlockedTrack4Panel;


    
    void Start()
    {
        resultText.text = GameManager.Instance.playerCharacter == GameManager.Instance.winner ? "You WON" : "You LOST";
        if (GameManager.Instance.playerCharacter == GameManager.Instance.winner)
        {
            switch (GameManager.Instance.SelectedTrackIndex)
            {
                case 1:
                    GameManager.Instance.beatenTrack1 = true;
                    //Save game data
                    unlockedOfficerJenkinsPanel.SetActive(true);
                    break;
                case 2:
                    GameManager.Instance.beatenTrack2 = true;
                    //Save game data
                    unlockedRusselPanel.SetActive(true);
                    break;
                case 3:
                    GameManager.Instance.beatenTrack3 = true;
                    //Save game data
                    unlockedStellaPanel.SetActive(true);
                    break;
                case 4:
                    GameManager.Instance.beatenTrack4 = true;
                    //Save game data
                    unlockedTrack4Panel.SetActive(true);
                    break;
            }
        }
        winnerText.text = GameManager.Instance.winner;
        timeText.text = GameManager.Instance.endTime;
        Gamepad.current.SetMotorSpeeds(0,0);
    }

    public void ConfirmUnlock()
    {
        unlockedOfficerJenkinsPanel.SetActive(false);
        unlockedRusselPanel.SetActive(false);
        unlockedStellaPanel.SetActive(false);
        unlockedTrack4Panel.SetActive(false);
    }
    
}
