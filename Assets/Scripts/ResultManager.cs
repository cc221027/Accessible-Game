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
                    break;
                case 2:
                    GameManager.Instance.beatenTrack2 = true;
                    //Save game data
                    break;
                case 3:
                    GameManager.Instance.beatenTrack3 = true;
                    //Save game data
                    break;
                case 4:
                    GameManager.Instance.beatenTrack4 = true;
                    //Save game data
                    break;
            }
        }
        winnerText.text = GameManager.Instance.winner;
        timeText.text = GameManager.Instance.endTime;
        Gamepad.current.SetMotorSpeeds(0,0);
    }
    
    
}
