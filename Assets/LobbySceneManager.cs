using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LobbySceneManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject[] uselessObjects;

    public GameObject yesOrNoPanel;
    public GameObject quitAskPanel;
    public GameObject loadPanel;
    public GameObject canvas;

    private GameObject lobbyLoadedData;
    private GameObject lobbyLevelChanger;
    private GameObject galleryManager;

    public void Start()
    {
        lobbyLoadedData = GameObject.Find("LoadDataObject");
        lobbyLevelChanger = GameObject.Find("LevelChanger");
        galleryManager = GameObject.Find("GalleryManager");

        DontDestroyOnLoad(lobbyLoadedData);
        DontDestroyOnLoad(lobbyLevelChanger);
        DontDestroyOnLoad(galleryManager);

        // AchievementXmlManager의 static XmlSerializer를 미리 초기화 (~100ms 비용을 게임씬 전환 시점 대신 여기로 이동)
        new AchievementXmlManager();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().buildIndex == 1)
                AskQuit();
        }
    }
    public void SettingPanelOpen()
    {
        settingsPanel.SetActive(true);
    }
    
    public void OffUseless()
    {
        foreach(var uselessObject in uselessObjects)
        {
            uselessObject.SetActive(false);
        }
    }
    public void OnUsless()
    {
        foreach (var uselessObject in uselessObjects)
        {
            uselessObject.SetActive(true);
        }
        SlotManager.instance.CheckSlot();
    }

    public void StartButton()
    {
        if (File.Exists(Application.persistentDataPath + "_GameData4.json"))
            yesOrNoPanel.SetActive(true);
        else
            YesStart();
    }

    public void YesStart()
    {
        yesOrNoPanel.SetActive(false);
    }

    public void NoStart()
    {
        yesOrNoPanel.SetActive(false);
    }

    public void AskQuit()
    {
        quitAskPanel.SetActive(true);
    }

    public void YesQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void NoQuit()
    {
        quitAskPanel.SetActive(false);
    }
    
    public void CloseLoadPanel()
    {
        loadPanel.SetActive(false);
    }
}
