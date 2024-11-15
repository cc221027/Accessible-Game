using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuUnlocksManager : MonoBehaviour
{
    [SerializeField] private GameObject unlockedOfficerJenkinsPanel;
    [SerializeField] private GameObject unlockedRusselPanel;
    [SerializeField] private GameObject unlockedStellaPanel;
    [SerializeField] private GameObject unlockedTrack4Panel;
    
    [SerializeField] private GameObject nextSelectedObject;
    
    void Start()
    {
        if (GameManager.Instance.playerCharacter == GameManager.Instance.winner)
        {
            switch (GameManager.Instance.SelectedTrackIndex)
            {
                case 0:
                    if (!GameManager.Instance.beatenTrack1)
                    {
                        unlockedOfficerJenkinsPanel.SetActive(true);
                        EventSystem.current.SetSelectedGameObject(unlockedOfficerJenkinsPanel
                            .GetComponentInChildren<Button>().gameObject);
                    }
                    break;
                case 1:
                    if (!GameManager.Instance.beatenTrack2)
                    {
                        unlockedRusselPanel.SetActive(true);
                        EventSystem.current.SetSelectedGameObject(unlockedRusselPanel
                            .GetComponentInChildren<Button>().gameObject);
                    }
                    break;
                case 2:
                    if (!GameManager.Instance.beatenTrack3)
                    {
                        unlockedStellaPanel.SetActive(true);
                        EventSystem.current.SetSelectedGameObject(unlockedStellaPanel
                            .GetComponentInChildren<Button>().gameObject);
                    }
                    break;
                case 3:
                    if(GameManager.Instance.beatenTrack1 && GameManager.Instance.beatenTrack2 && GameManager.Instance.beatenTrack3 && !GameManager.Instance.beatenTrack4)
                    {
                        unlockedTrack4Panel.SetActive(true);
                        EventSystem.current.SetSelectedGameObject(unlockedTrack4Panel
                            .GetComponentInChildren<Button>().gameObject);
                    }
                    break;
            }
        }
    }
    
    public void ConfirmUnlock()
    {
        unlockedOfficerJenkinsPanel.SetActive(false);
        unlockedRusselPanel.SetActive(false);
        unlockedStellaPanel.SetActive(false);
        unlockedTrack4Panel.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(nextSelectedObject);
    }
    
}
