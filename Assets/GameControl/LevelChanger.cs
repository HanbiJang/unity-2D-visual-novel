using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    public Animator animator;
    private int levelToLoad;
    private LobbySceneManager lobbySceneManager;
    private int currentScene
    {
        get
        {
            return SceneManager.GetActiveScene().buildIndex;
        }
    }

    private void Start()
    {
        lobbySceneManager = FindObjectOfType<LobbySceneManager>();
        if (currentScene == 0)
        {
            StartCoroutine(Logo());

        }
        if (currentScene == 1)
        {
            lobbySceneManager.OnUsless();
            SoundManager.instance.PlayBGM("타이틀");
        }

        if (currentScene == 3)
            lobbySceneManager.OnUsless();
    }

    public void FadeInStart() {

    }

    IEnumerator Logo()
    {
        yield return new WaitForSeconds(2.0f);
        FadeToLevel(1);
    }

    
    public void Next_Clicked()
    {
        
        FadeToLevel(currentScene + 1);
    }
    public void Back_Clicked()
    {
        FadeToLevel(currentScene - 1);
    }

    public void GameStart()
    {
        if (!File.Exists(Application.persistentDataPath + "_GameData4.json"))
            Next_Clicked();
        else
            return;
    }

    public void FadeToLevel(int levelIndex)
    {
        if(currentScene != 0)
        {
            SoundManager.instance.StopBGM();
        }
        
        animator.SetTrigger("FadeOut");
        levelToLoad = levelIndex;
    }

    public void RemoveCanvas_Lobby() //팀 로고 눌렀을 시 필요없는 요소 삭제 & levelchanger blackfade 이미지 안보이게
    {
        //Destroy(GameObject.Find("LevelChanger"));
        //Destroy(GameObject.Find("LoadDataObject"));
        Destroy(GameObject.Find("Canvas_Lobby"));
        //Destroy(GameObject.Find("SoundManager"));
    }

    public IEnumerator OnFadeComplete()
    {

        AsyncOperation asyncOper = SceneManager.LoadSceneAsync(levelToLoad);

        while (!asyncOper.isDone)
            yield return null;

        if (currentScene == 2)
        {
            lobbySceneManager.OffUseless();
            SettingManager.instance.FindScreenTouchManager();
        }

        else if (currentScene == 3)
        {
            RemoveCanvas_Lobby();
            animator.SetTrigger("FadeIn");
        }

        else if (currentScene == 1) {
            animator.SetTrigger("FadeIn");
        }

    }
}
