using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameButtonControl : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject quitPanel;
    public GameObject goToTitlePanel;
    public GameObject buttonsPanel;
    public GameObject achvPanel;
    bool isButtonsPanel = false;
    public GameObject nextBtn; //인스펙터 할당

    public ScreenTouchManager screenTouchManager;//화면터치
    // Start is called before the first frame update

    public LevelChanger levelChanger;

    public LobbySceneManager lobbySceneManager;
    void Start()
    {
        //SettingManager에서 동시에 Awake()로 객체 생성을 해서 Null오류가 남 -> Start() 함수로 바꿨음
        if (SettingManager.instance != null) settingsPanel = SettingManager.instance.settingsPanel;

        lobbySceneManager = FindObjectOfType<LobbySceneManager>().GetComponent<LobbySceneManager>();
        levelChanger = GameObject.Find("LevelChanger").GetComponent<LevelChanger>();
    }

    //next버튼을 막는다
    void LockNextBtn() {
        nextBtn.SetActive(false);
    }

    //next버튼을 락해제 한다
    public void UnlockNextBtn()
    {
        nextBtn.SetActive(true);
    }

    //설정 관련
    public void SettingsOpen() // 세팅 열기
    {
        settingsPanel.SetActive(true);
        ButtonsPanelSet();
        LockNextBtn();
        screenTouchManager.SetTouchEnable(false);
    }

    public void ButtonsPanelClose()
    {
        isButtonsPanel = false;
        buttonsPanel.gameObject.SetActive(false);
        Time.timeScale = 1;

        
        //nextBtn.SetActive(true);//nextBtn 활성화
        screenTouchManager.SetTouchEnable(true);
    }

    public void ButtonsPanelSet()
    {
        if (!isButtonsPanel)
        {
            screenTouchManager.SetTouchEnable(false);

            //세팅패널 애니메이션
            buttonsPanel.GetComponent<Animation>().Play("BtnPanelAnim");

            isButtonsPanel = true;
            buttonsPanel.gameObject.SetActive(true);

        }
        else
        {
            isButtonsPanel = false;
            buttonsPanel.gameObject.SetActive(false);
            Time.timeScale = 1;
            screenTouchManager.SetTouchEnable(true);
        }
    }
    public void QuitAns() // 종료하시겠습니까?
    {
        quitPanel.SetActive(true);
        screenTouchManager.SetTouchEnable(false);
        Time.timeScale = 0; // 일시정지
    }
    public void QuitNo() // 종료 안함
    {
        quitPanel.SetActive(false);
        screenTouchManager.SetTouchEnable(true);
        Time.timeScale = 1; // 시간 재개
    }
    public void GoToTitleAns() // 타이틀 화면으로 돌아가시겠습니까?
    {
        goToTitlePanel.SetActive(true);
        Time.timeScale = 0;
    }
    public void GoToTitleNo() // 안돌아감
    {
        goToTitlePanel.SetActive(false);
        Time.timeScale = 1;
    }
    public void GoToTitle() // 타이틀 화면으로 이동
    {
        Time.timeScale = 1;
        levelChanger.FadeToLevel(1);
        //lobbySceneManager.OnUsless();
    }


    public void AchvSettingsOpen()
    {
        achvPanel.SetActive(true);
    }

    public void AchvSettingsClose()
    {
        achvPanel.SetActive(false);
    }



    // 세팅버튼 누르면 버튼이 올라갔다 내려갔다 함
    IEnumerator ButtonsPanel()
    {
        if (isButtonsPanel)
        {
            for (int i = 680; i >= -400; i -= 108)
            {
                buttonsPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(i, -1200);
                yield return null;
            }
        }
        else
        {
            for (int i = -400; i <= 680; i += 108)
            {
                buttonsPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(i, -1200);
                yield return null;
            }
        }
    }

    public void SetTouchEnable(bool _tof)
    {
        if (_tof)
            screenTouchManager.SetTouchEnable(true);
        else
            screenTouchManager.SetTouchEnable(false);
    }
}
