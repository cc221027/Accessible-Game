using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    
    [SerializeField] private TextMeshProUGUI winnerText;

    [SerializeField] private TextMeshProUGUI timeText;

    [SerializeField] private Button confirmBtn;

    [SerializeField] private GameObject unlockedOfficerJenkinsPanel;
    [SerializeField] private GameObject unlockedRusselPanel;
    [SerializeField] private GameObject unlockedStellaPanel;
    [SerializeField] private GameObject unlockedTrack4Panel;

    private bool _unlockedAll;


    
    void Start()
    {
        resultText.text = GameManager.Instance.playerCharacter == GameManager.Instance.winner ? "You WON" : "You LOST";
        if (GameManager.Instance.playerCharacter == GameManager.Instance.winner)
        {
            switch (GameManager.Instance.SelectedTrackIndex)
            {
                case 0:
                    if (!GameManager.Instance.beatenTrack1)
                    {
                        GameManager.Instance.beatenTrack1 = true;
                        //Save game data
                        unlockedOfficerJenkinsPanel.SetActive(true);
                        EventSystem.current.SetSelectedGameObject(unlockedOfficerJenkinsPanel
                            .GetComponentInChildren<Button>().gameObject);
                    }
                    break;
                case 1:
                    if (!GameManager.Instance.beatenTrack2)
                    {
                        GameManager.Instance.beatenTrack2 = true;
                        //Save game data
                        unlockedRusselPanel.SetActive(true);
                        EventSystem.current.SetSelectedGameObject(unlockedRusselPanel
                            .GetComponentInChildren<Button>().gameObject);
                    }
                    break;
                case 2:
                    if (!GameManager.Instance.beatenTrack3)
                    {
                        GameManager.Instance.beatenTrack3 = true;
                        //Save game data
                        unlockedStellaPanel.SetActive(true);
                        EventSystem.current.SetSelectedGameObject(unlockedStellaPanel
                            .GetComponentInChildren<Button>().gameObject);
                    }
                    break;
                case 3:
                    if(GameManager.Instance.beatenTrack1 && GameManager.Instance.beatenTrack2 && GameManager.Instance.beatenTrack3 && !_unlockedAll)
                    {
                        GameManager.Instance.beatenTrack4 = true;
                        //Save game data
                        unlockedTrack4Panel.SetActive(true);
                        EventSystem.current.SetSelectedGameObject(unlockedTrack4Panel
                            .GetComponentInChildren<Button>().gameObject);
                        _unlockedAll = true;
                    }
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
        
        EventSystem.current.SetSelectedGameObject(confirmBtn.gameObject);
    }
    
}
