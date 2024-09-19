using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winnerText;

    [SerializeField] private TextMeshProUGUI timeText;
    
    private AudioSource _winnerAudio;
    private AudioSource _character1Audio;
    private AudioSource _character2Audio;
    private AudioSource _character3Audio;
    private AudioSource _character4Audio;
    
    void Start()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();

        if (audioSources.Length >= 2)
        {
            _winnerAudio = audioSources[0];
            _character1Audio = audioSources[1];
            _character2Audio = audioSources[2];
            _character3Audio = audioSources[3];
            _character4Audio = audioSources[4];

        }
        
        winnerText.text = GameManager.Instance.winner;
        timeText.text = GameManager.Instance.endTime;
        Gamepad.current.SetMotorSpeeds(0,0);
        
        _winnerAudio.Play();
        switch (GameManager.Instance.winner)
        {
            case "Jimmy":
                StartCoroutine(PlayAfterSoundIsFinished(_winnerAudio, _character1Audio));
                break;
            case "Officer Jenkins":
                StartCoroutine(PlayAfterSoundIsFinished(_winnerAudio, _character2Audio));
                break;
            case "Russel":
                StartCoroutine(PlayAfterSoundIsFinished(_winnerAudio, _character3Audio));
                break;
            case "Stella":
                StartCoroutine(PlayAfterSoundIsFinished(_winnerAudio, _character4Audio));
                break;
        }
    }

    private IEnumerator PlayAfterSoundIsFinished(AudioSource sound, AudioSource soundToPlay)
    {
        yield return new WaitForSeconds(sound.clip.length + 0.5f);
        soundToPlay.Play();
    }
    
}
