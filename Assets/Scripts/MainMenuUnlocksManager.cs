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

    [SerializeField] private GameObject[] uiToDisable;
    
    void Start()
    {
        if (GameManager.Instance.playerCharacter == GameManager.Instance.winner)
        {
            switch (GameManager.Instance.SelectedTrackIndex)
            {
                case 0:
                    if (GameManager.Instance.beatenTrack1 && !GameManager.Instance.unlockedJenkins)
                    {
                        foreach (GameObject element in uiToDisable) {element.SetActive(false); }
                        
                        GameManager.Instance.unlockedJenkins = true;
                        
                        unlockedOfficerJenkinsPanel.SetActive(true);
                        EventSystem.current.SetSelectedGameObject(unlockedOfficerJenkinsPanel
                            .GetComponentInChildren<Button>().gameObject);
                    }
                    break;
                case 1:
                    if (GameManager.Instance.beatenTrack2 && !GameManager.Instance.unlockedRussel)
                    {
                        foreach (GameObject element in uiToDisable) {element.SetActive(false); }
                        
                        GameManager.Instance.unlockedRussel = true;
                        
                        unlockedRusselPanel.SetActive(true);
                        EventSystem.current.SetSelectedGameObject(unlockedRusselPanel
                            .GetComponentInChildren<Button>().gameObject);
                    }
                    break;
                case 2:
                    if (GameManager.Instance.beatenTrack3 && !GameManager.Instance.unlockedStella)
                    {
                        foreach (GameObject element in uiToDisable) {element.SetActive(false); }

                        GameManager.Instance.unlockedStella = true;

                        unlockedStellaPanel.SetActive(true);
                        EventSystem.current.SetSelectedGameObject(unlockedStellaPanel
                            .GetComponentInChildren<Button>().gameObject);
                    }
                    break;
                case 3:
                    if(GameManager.Instance.beatenTrack4 && !GameManager.Instance.unlockedT4)
                    {
                        foreach (GameObject element in uiToDisable) {element.SetActive(false); }

                        GameManager.Instance.unlockedT4 = true;
                        
                        unlockedTrack4Panel.SetActive(true);
                        EventSystem.current.SetSelectedGameObject(unlockedTrack4Panel
                            .GetComponentInChildren<Button>().gameObject);
                    }
                    break;
            }
            SaveSystem.SavePlayer(GameManager.Instance);
        }
    }
    
    public void ConfirmUnlock()
    {
        unlockedOfficerJenkinsPanel.SetActive(false);
        unlockedRusselPanel.SetActive(false);
        unlockedStellaPanel.SetActive(false);
        unlockedTrack4Panel.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(nextSelectedObject);

        foreach (GameObject element in uiToDisable) { element.SetActive(true); }
    }
    
}
