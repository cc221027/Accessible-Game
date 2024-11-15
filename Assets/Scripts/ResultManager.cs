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
    
    
    void Start()
    {
        resultText.text = GameManager.Instance.playerCharacter == GameManager.Instance.winner ? "You WON" : "You LOST";
        
        Debug.Log(GameManager.Instance.playerCharacter);
        Debug.Log(GameManager.Instance.winner);
        
        if (GameManager.Instance.playerCharacter == GameManager.Instance.winner)
        {
            switch (GameManager.Instance.SelectedTrackIndex)
            {
                case 0:
                    if (!GameManager.Instance.beatenTrack1)
                    {
                        GameManager.Instance.beatenTrack1 = true;
                    }
                    break;
                case 1:
                    if (!GameManager.Instance.beatenTrack2)
                    {
                        GameManager.Instance.beatenTrack2 = true;
                    }
                    break;
                case 2:
                    if (!GameManager.Instance.beatenTrack3)
                    {
                        GameManager.Instance.beatenTrack3 = true;
                    }
                    break;
                case 3:
                    if(GameManager.Instance.beatenTrack1 && GameManager.Instance.beatenTrack2 && GameManager.Instance.beatenTrack3 && !GameManager.Instance.beatenTrack4)
                    {
                        GameManager.Instance.beatenTrack4 = true;
                    }
                    break;
            }
        }
        winnerText.text = GameManager.Instance.winner;
        timeText.text = GameManager.Instance.endTime;
        Gamepad.current.SetMotorSpeeds(0,0);
        
        SaveSystem.SavePlayer(GameManager.Instance);
    }

    
    
}
