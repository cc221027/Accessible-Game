using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winnerText;
    // Start is called before the first frame update
    void Start()
    {
        winnerText.text = GameManager.Instance.winner;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
