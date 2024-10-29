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
        winnerText.text = GameManager.Instance.winner;
        timeText.text = GameManager.Instance.endTime;
        Gamepad.current.SetMotorSpeeds(0,0);
    }
    
    
}
