using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]

public class GameManager : MonoBehaviour
{
    public StatusManager statusManager;
    public IllustManager illustManager;
    public DataManager dataManager;
    // 게임 패널 on/off 세팅을 settingsManager -> GameButtonControl로 이관 220101
    public GameButtonControl gameButtonControl;

    public ScrollViewManager scrollViewManager;
    public ScrollViewManager mobileScrollViewManager;
    public Scrollbar scrollbar_mode0;
    public Scrollbar scrollbar_mode1;
    public Scrollbar scrollbar_mode2;

    //xml script 변수
    public string scriptPath;

    //일러스트 관련 변수
    public int illustIndex = 0;
    private bool isIllust = false;

    //스크롤뷰 관련 변수
    public RectTransform scrollView;
    public bool isScroll = false;
    //public Text textPrefab;

    GameObject newContent = null; //새로 만들 콘텐츠
    public GameObject contentPrefab;
    public GameObject content; //일반모드에서 컨텐츠 내용물

    public GameObject mobilePrefab;
    public GameObject mobileContent; //모바일톡 모드에서 컨텐츠 내용물

    public GameObject mode2Prefab;
    public GameObject mode2Content; //mode2 모드에서 컨텐츠 내용물

    public List<GameObject> contentObjects = new List<GameObject>(); //현재 보이는 컨텐츠(글,그림) 리스트
    public int textIndex;
    public List<ScriptData> scriptList = new List<ScriptData>();
    //public Button nextButton;
    public int scriptIndex = 0;
    public Image gameBackGround; // 배경이미지

    //저장관련 변수
    public GameObject ConfirmPanel;
    public GameObject SaveSlotPanel;

    public Button SlotQuitBtn;
    public Button Slot0;
    public Button Slot1;
    public Button Slot2;
    public Button saveButton;
    public bool isSet = false;
    public int slotIndex = 0;

    public float y = 0.0f;

    public float illustSizeY = 900f;

    public float space = 0f;



    //분기 관련 변수
    public int tg = 0;
    public bool choiceNow = false;
    public OneChoicePanel oneChoicePanel;
    public TwoChoicePanel twoChoicePanel;
    public ThreeChoicePanel threeChoicePanel;
    public FourChoicePanel fourChoicePanel;
    public FiveChoicePanel fiveChoicePanel;


    // setting을 누르면 게임저장, 설정, 종료 버튼이 나옴
    //public GameObject buttonsPanel;
    //bool isButtonsPanel = false;
    float posy = -400f;

    //설정 관련
    public GameObject saveSuccessPanel;
    public VerticalLayoutGroup scrollContents;
    public VerticalLayoutGroup mobileContents;

    //모바일 UI 관련
    public int preMode = 2;
    public GameObject mobileTalkPanel;

    //게임오버
    public GameObject gameOverPanel;

    // 랜덤 변수
    public int randomNumber = 0;

    //업적
    public AchievementManager achievementManager; //인스펙터에서 할당

    //텍스트 이펙트
    public TextEffect textEffect; //인스펙터에서 할당
    public int ClickCnt = 0; //화면의 nextButton이 클릭되었는지 판별하는 변수

    //2개 프레임 애니메이션 출력
    public Anim2Frame anim2Frame; //인스펙터에서 할당 

    // 바탕화면에 몇번 나갔다 왔는지 체크하는 변수
    public int outCount = 0;
    public bool isBellRun = false; // 벨튀 업적 달성 여부, 저장됨

    // 내려진 상태인지 아닌지 상태 확인 // OnApplicationPause 테스트 용도
    private bool bPaused;
    private DateTime pausedTime;
    private DateTime firstPausedTime; // 벨튀 업적체크를 위함
    private TimeSpan timeGap;

    private DataManager dm;

    //스크린 터치 관련
    public ScreenTouchManager screenTouchManager;

    public bool isScrollDown = false; //스크롤 다운

    //엔딩롤
    public GameObject EndingFade, EndingRoll; //엔딩롤이 아니라 일러스트로 바꿈
    int curEndingNum = -1; //0부터 시작
    public GameObject mode2Panel;

    public Mode2ContentControl mode2ContentControl;

    public string playingBGM = "";
    public string nowBackGround = "Images/Illusts/BackGround/default";
    public int isChoice = 0;
    public string[] choiceText = { " ", " ", " ", " ", " ", " "};

    #region 시작 세팅
    public void Awake()
    {
        //프리펩 -  일러스트 매니저 설정
        illustManager.SetPath("Images/Illusts"); //일러스트가 들어있는 폴더 경로 설정

        LoadOrStartNewGame();
    }
    
    public void LoadOrStartNewGame()
    {
        InitSaveSlot();
        dm = FindObjectOfType<DataManager>();
        if (dm.loadClicked)
        {
            dataManager = GameObject.Find("LoadDataObject").GetComponent<DataManager>();
            dataManager.LoadGameData(SaveID.saveID);
            SoundManager.instance.PlayBGM(playingBGM);
            gameBackGround.sprite = Resources.Load(nowBackGround, typeof(Sprite)) as Sprite;
            if(scriptList[scriptIndex].mode == 2) { Mode2Open(); }
        }
        else
        {
            Mode2Open();
            scriptList = scrollViewManager.xmlScriptParsing(scriptPath);
            OutputTextStr("처음 뵙는군요");
        }

    }
    #endregion
    IEnumerator ScrollDown()
    {
        yield return new WaitForSeconds(0.1f);
        isScroll = false;
    }

    void Update()
    {
        // Text setting 220101 settingManager -> GameManager
        SetTextSize(SettingManager.instance.textSize);
        

        //안드로이드 뒤로가기 기능
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                gameButtonControl.QuitAns();
            }
        }

    }

    public void QuitApplication() // 게임종료
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }




    #region 텍스트 출력 관련
    //새 텍스트 추가 전 처리 (모드 변경, 분기 구분,처리)
    public void PreprocessNewTexts()
    {
        if (!textEffect.isTextEffect)
        {
            ClickCnt++;

            
            RemovePreText();
            CondCheck();
        }
        else
        { //텍스트 이펙트 실행중
            if (newContent != null)
                textEffect.StartTextEffectCoroutine(scriptIndex, scriptList, newContent.GetComponentInChildren<Text>());
        }

    }

    void IdCheck()
    {
        switch (scriptList[scriptIndex].id)
        {
            case 0: // 텍스트 출력
                isIllust = false;
                ModePreprocess();
                break;
            #region 선택지 출력
            case 1: // 1~5 선택지 출력
                isChoice = 1;
                choiceNow = true;
                screenTouchManager.SetTouchEnable(false);
                saveButton.interactable = false;

                oneChoicePanel.ViewTrue(); //1개 선택지 팝업 띄움
                oneChoicePanel.How.text = scriptList[scriptIndex].script; //선택지 설명 스크립트
                choiceText[0] = scriptList[scriptIndex].script;
                scriptIndex++;
                for (int i = 0; i < 1; ++i)
                { // 반복문으로 선택지 개수만큼 선택지출력
                    oneChoicePanel.buttonTexts[i].text = scriptList[scriptIndex].script;
                    choiceText[i + 1] = scriptList[scriptIndex].script;

                    if (string.Compare(scriptList[scriptIndex].path[0], "0") != 0)
                    {
                        Sprite choiceImage = Resources.Load(scriptList[scriptIndex].path[0], typeof(Sprite)) as Sprite;
                        oneChoicePanel.buttons[i].GetComponent<Image>().sprite = choiceImage;
                    }
                    switch (scriptList[scriptIndex].cond[0])
                    {
                        case "mo":
                            if (statusManager.moPoint < System.Convert.ToInt32(scriptList[scriptIndex].cond[1]))
                            {
                                oneChoicePanel.DisableButton(oneChoicePanel.buttonTexts[i]);
                                return;
                            }
                            break;
                        case "dre":
                            if (statusManager.dre < System.Convert.ToInt32(scriptList[scriptIndex].cond[1]))
                            {
                                oneChoicePanel.DisableButton(oneChoicePanel.buttonTexts[i]);
                                return;
                            }
                            break;
                        case "fam":
                            if (statusManager.fam < System.Convert.ToInt32(scriptList[scriptIndex].cond[1]))
                            {
                                oneChoicePanel.DisableButton(oneChoicePanel.buttonTexts[i]);
                                return;
                            }
                            break;
                        default:
                            break;
                    }
                    scriptIndex++;
                }
                break;
            case 2:
                isChoice = 2;
                choiceNow = true;
                screenTouchManager.SetTouchEnable(false);
                saveButton.interactable = false;

                twoChoicePanel.ViewTrue(); //2개 선택지 팝업 띄움
                twoChoicePanel.How.text = scriptList[scriptIndex].script; //선택지 설명 스크립트
                choiceText[0] = scriptList[scriptIndex].script;
                scriptIndex++;
                for (int i = 0; i < 2; ++i)
                {
                    twoChoicePanel.buttonTexts[i].text = scriptList[scriptIndex].script;
                    choiceText[i + 1] = scriptList[scriptIndex].script;

                    if (string.Compare(scriptList[scriptIndex].path[0], "0") != 0)
                    {
                        Sprite choiceImage = Resources.Load(scriptList[scriptIndex].path[0], typeof(Sprite)) as Sprite;
                        twoChoicePanel.buttons[i].GetComponent<Image>().sprite = choiceImage;
                    }
                    switch (scriptList[scriptIndex].cond[0])
                    {
                        case "mo":
                            if (statusManager.moPoint < System.Convert.ToInt32(scriptList[scriptIndex].cond[1]))
                            {
                                twoChoicePanel.DisableButton(twoChoicePanel.buttonTexts[i]);
                                return;
                            }
                            break;
                        case "dre":
                            if (statusManager.dre < System.Convert.ToInt32(scriptList[scriptIndex].cond[1]))
                            {
                                twoChoicePanel.DisableButton(twoChoicePanel.buttonTexts[i]);
                                return;
                            }
                            break;
                        case "fam":
                            if (statusManager.fam < System.Convert.ToInt32(scriptList[scriptIndex].cond[1]))
                            {
                                twoChoicePanel.DisableButton(twoChoicePanel.buttonTexts[i]);
                                return;
                            }
                            break;
                        default:
                            break;
                    }
                    scriptIndex++;
                }
                break;
            case 3:
                isChoice = 3;
                choiceNow = true;
                screenTouchManager.SetTouchEnable(false);
                saveButton.interactable = false;

                threeChoicePanel.ViewTrue();
                threeChoicePanel.How.text = scriptList[scriptIndex].script;
                choiceText[0] = scriptList[scriptIndex].script;
                scriptIndex++;
                for (int i = 0; i < 3; ++i)
                {
                    threeChoicePanel.buttonTexts[i].text = scriptList[scriptIndex].script;
                    choiceText[i + 1] = scriptList[scriptIndex].script;
                    if (string.Compare(scriptList[scriptIndex].path[0], "0") != 0)
                    {
                        Sprite choiceImage = Resources.Load(scriptList[scriptIndex].path[0], typeof(Sprite)) as Sprite;
                        threeChoicePanel.buttons[i].GetComponent<Image>().sprite = choiceImage;
                    }
                    switch (scriptList[scriptIndex].cond[0])
                    {
                        case "mo":
                            if (statusManager.moPoint < System.Convert.ToInt32(scriptList[scriptIndex].cond[1]))
                            {
                                threeChoicePanel.DisableButton(threeChoicePanel.buttonTexts[i]);
                                return;
                            }
                            break;
                        case "dre":
                            if (statusManager.dre < System.Convert.ToInt32(scriptList[scriptIndex].cond[1]))
                            {
                                threeChoicePanel.DisableButton(threeChoicePanel.buttonTexts[i]);
                                return;
                            }
                            break;
                        case "fam":
                            if (statusManager.fam < System.Convert.ToInt32(scriptList[scriptIndex].cond[1]))
                            {
                                threeChoicePanel.DisableButton(threeChoicePanel.buttonTexts[i]);
                                return;
                            }
                            break;
                        default:
                            break;
                    }
                    scriptIndex++;
                }
                break;
            case 4:
                isChoice = 4;
                choiceNow = true;
                screenTouchManager.SetTouchEnable(false);
                saveButton.interactable = false;

                fourChoicePanel.ViewTrue();
                fourChoicePanel.How.text = scriptList[scriptIndex].script;
                choiceText[0] = scriptList[scriptIndex].script;
                scriptIndex++;
                for (int i = 0; i < 4; ++i)
                {
                    fourChoicePanel.buttonTexts[i].text = scriptList[scriptIndex].script;
                    choiceText[i + 1] = scriptList[scriptIndex].script;
                    if (string.Compare(scriptList[scriptIndex].path[0], "0") != 0)
                    {
                        Sprite choiceImage = Resources.Load(scriptList[scriptIndex].path[0], typeof(Sprite)) as Sprite;
                        fourChoicePanel.buttons[i].GetComponent<Image>().sprite = choiceImage;
                    }
                    switch (scriptList[scriptIndex].cond[0])
                    {
                        case "mo":
                            if (statusManager.moPoint < System.Convert.ToInt32(scriptList[scriptIndex].cond[1]))
                            {
                                fourChoicePanel.DisableButton(fourChoicePanel.buttonTexts[i]);
                                return;
                            }
                            break;
                        case "dre":
                            if (statusManager.dre < System.Convert.ToInt32(scriptList[scriptIndex].cond[1]))
                            {
                                fourChoicePanel.DisableButton(fourChoicePanel.buttonTexts[i]);
                                return;
                            }
                            break;
                        case "fam":
                            if (statusManager.fam < System.Convert.ToInt32(scriptList[scriptIndex].cond[1]))
                            {
                                fourChoicePanel.DisableButton(fourChoicePanel.buttonTexts[i]);
                                return;
                            }
                            break;
                        default:
                            break;
                    }
                    scriptIndex++;
                }
                break;
            case 5:
                isChoice = 5;
                choiceNow = true;
                screenTouchManager.SetTouchEnable(false);
                saveButton.interactable = false;

                fiveChoicePanel.ViewTrue();
                fiveChoicePanel.How.text = scriptList[scriptIndex].script;
                choiceText[0] = scriptList[scriptIndex].script;
                scriptIndex++;
                for (int i = 0; i < 5; ++i)
                {
                    fiveChoicePanel.buttonTexts[i].text = scriptList[scriptIndex].script;
                    choiceText[i + 1] = scriptList[scriptIndex].script;
                    if (string.Compare(scriptList[scriptIndex].path[0], "0") != 0)
                    {
                        Sprite choiceImage = Resources.Load(scriptList[scriptIndex].path[0], typeof(Sprite)) as Sprite;
                        fiveChoicePanel.buttons[i].GetComponent<Image>().sprite = choiceImage;
                    }
                    switch (scriptList[scriptIndex].cond[0])
                    {
                        case "mo":
                            if (statusManager.moPoint < System.Convert.ToInt32(scriptList[scriptIndex].cond[1]))
                            {
                                fiveChoicePanel.DisableButton(fiveChoicePanel.buttonTexts[i]);
                                return;
                            }
                            break;
                        case "dre":
                            if (statusManager.dre < System.Convert.ToInt32(scriptList[scriptIndex].cond[1]))
                            {
                                fiveChoicePanel.DisableButton(fiveChoicePanel.buttonTexts[i]);
                                return;
                            }
                            break;
                        case "fam":
                            if (statusManager.fam < System.Convert.ToInt32(scriptList[scriptIndex].cond[1]))
                            {
                                fiveChoicePanel.DisableButton(fiveChoicePanel.buttonTexts[i]);
                                return;
                            }
                            break;
                        default:
                            break;
                    }
                    scriptIndex++;
                }
                break;
            #endregion
            case 6: // 이미지, 텍스트 출력
                isIllust = true;
                ModePreprocess();
                break;
            case 7: // Tg, randomNumber를 0으로 초기화
                if(tg != 0)
                    SetTg0();
                else if(scriptList[scriptIndex].cond.Length != 1)
                {
                    while (scriptList[scriptIndex].cond.Length == 1)
                    {
                        scriptIndex++;
                    }
                }
                randomNumber = 0;
                OutPutText();
                break;
            case 8: // 다음 XML 호출
                CheckNewXML();
                break;
            default:
                break;
        }
    }

    void TgCheck()
    {
        while (scriptList[scriptIndex].tg != tg)
        {
            scriptIndex++;
        }
        IdCheck();
    }
    void RandCheck()
    {
        while (scriptList[scriptIndex].rand != randomNumber)
        {
            scriptIndex++;
        }
        TgCheck();
    }

    void CondCheck()
    {
        for (int i = 0; i < scriptList[scriptIndex].cond.Length; i += 2)
        {
            switch (scriptList[scriptIndex].cond[i]) // 조건관리, 211105부로 switch문으로 변경
            {
                case "me":
                    if (System.Convert.ToInt32(scriptList[scriptIndex].cond[i + 1]) == 0 && statusManager.mePoint != 0)
                    {
                        scriptIndex++;
                        CondCheck();
                        return; // 치킨 먹는 선택지가 존재하는 XML로 로드
                    }
                    else if(statusManager.mePoint < System.Convert.ToInt32(scriptList[scriptIndex].cond[i + 1]))
                    {
                        scriptIndex++;
                        CondCheck();
                        return;
                    }
                    break;
                case "mo":
                    if (statusManager.moPoint < System.Convert.ToInt32(scriptList[scriptIndex].cond[i + 1]))
                    {
                        scriptIndex++;
                        CondCheck();
                        return;
                    }
                    break;
                case "dre":
                    if (statusManager.dre < System.Convert.ToInt32(scriptList[scriptIndex].cond[i + 1]))
                    {
                        scriptIndex++;
                        CondCheck();
                        return;
                    }
                    break;
                case "fam":
                    if (statusManager.fam < System.Convert.ToInt32(scriptList[scriptIndex].cond[i + 1]))
                    {
                        scriptIndex++;
                        CondCheck();
                        return;
                    }
                    break;
                default:
                    break;
            }
        }
        RandCheck();
    }

    void EvCheck()
    {
        for (int i = 0; i < scriptList[scriptIndex].ev.Length; i += 2)
        {
            switch (scriptList[scriptIndex].ev[i]) // 스탯 관리, 211105부로 swich문으로 변경
            {
                case "me":
                    statusManager.Men(System.Convert.ToInt32(scriptList[scriptIndex].ev[i + 1]));
                    break;
                case "mo": // money 관련, 211126 부활
                    statusManager.Mo(System.Convert.ToInt32(scriptList[scriptIndex].ev[i + 1]));
                    break;
                case "dre":
                    statusManager.Dre(System.Convert.ToInt32(scriptList[scriptIndex].ev[i + 1]));
                    break;
                case "fam":
                    statusManager.Fam(System.Convert.ToInt32(scriptList[scriptIndex].ev[i + 1]));
                    break;
                case "rand":
                    randomNumber = UnityEngine.Random.Range(1, System.Convert.ToInt32(scriptList[scriptIndex].ev[i + 1]) + 1);
                    break;
                case "overCheck": // 211124 추가
                    if (statusManager.mePoint <= System.Convert.ToInt32(scriptList[scriptIndex].ev[i + 1]))
                        GameOver();
                    break;
                case "bgm":
                    if (scriptList[scriptIndex].ev[i + 1] == "STOP")
                    {
                        SoundManager.instance.StopBGM();
                        playingBGM = "";
                        break;
                    }
                    SoundManager.instance.PlayBGM(scriptList[scriptIndex].ev[i + 1]);
                    playingBGM = scriptList[scriptIndex].ev[i + 1];
                    break;
                case "effect":
                    if (scriptList[scriptIndex].ev[i + 1] == "STOP")
                    {
                        SoundManager.instance.StopAllSE();
                        break;
                    }
                    SoundManager.instance.PlaySE(scriptList[scriptIndex].ev[i + 1]);
                    break;
                case "ending":
                    if (scriptList[scriptIndex].ev[i + 1] == "Ending1") { //트루엔딩
                        curEndingNum = ((int)Endings.Ending1);
                    }
/*                    else if (scriptList[scriptIndex].ev[i + 1] == "Ending2") {
                        curEndingNum = ((int)Endings.Ending2);
                    }
                    else if (scriptList[scriptIndex].ev[i + 1] == "Ending3")
                    {
                        curEndingNum = ((int)Endings.Ending3);
                    }
                    else if (scriptList[scriptIndex].ev[i + 1] == "Ending4")
                    {
                        curEndingNum = ((int)Endings.Ending4);
                    }
                    else if (scriptList[scriptIndex].ev[i + 1] == "GameOver")
                    {
                        curEndingNum = ((int)Endings.GameOver);
                    }*/

                    else if (scriptList[scriptIndex].ev[i + 1] == "DreamCatcher") {
                        EndingRoll.GetComponent<EndingRoll>().ShowEndingRoll(curEndingNum);
                        GalleryManager.SaveIllustData("Images/Illusts/GameIllusts/엔딩 삽화");
                    }

                    break;
                case "day_end": // day_end,1로 넣으면 됨
                    for (int j = 0; j < contentObjects.Count; j++)
                    {
                        Destroy(contentObjects[j]);
                    }
                    contentObjects.Clear();
                    break;
                default:
                    break;
            }
        }
        achievementManager.OnNotify();
        if (scriptIndex >= scriptList.Count - 1)
        {
            return;
        }
        else
        {
            OutPutText();
        }
    }

    public void OutPutText()
    {

        isSet = true;

        preMode = scriptList[scriptIndex].mode;
        if (scriptList[scriptIndex].mode == 0)
        {
            newContent = Instantiate(contentPrefab); // 새 내용(글 또는 그림)
            newContent.transform.SetParent(content.transform, false); //부모 설정
            newContent.GetComponent<RectTransform>().rect.Set(0, 0, 1300, 100);
        }
        else if (scriptList[scriptIndex].mode == 1)
        {
            newContent = Instantiate(mobilePrefab); // 새 내용(글 또는 그림)
            newContent.transform.SetParent(mobileContent.transform, false); //부모 설정
        }
        else if (scriptList[scriptIndex].mode == 2)
        {
            newContent = Instantiate(mode2Prefab); // 새 내용(글 또는 그림)
            newContent.transform.SetParent(mode2Content.transform, false); //부모 설정
        }

        //이미지 & 텍스트 애니메이션 재생
        if (scriptList[scriptIndex].mode == 0)
        {
            newContent.GetComponent<Animation>().Play("Mode0prefabAnim");
        }
        else if (scriptList[scriptIndex].mode == 2)
        {
            newContent.GetComponent<Animation>().Play("Mode2PrefabAnim");
        }

        if (scriptList[scriptIndex].script == "0")
        {
            scriptIndex++;
            IdCheck();
            return;
        }
        // 텍스트 출력 (추가)
        if (newContent != null)
        {
            var newText = scriptList[scriptIndex].script; //새로 불러온 텍스트 

            newContent.GetComponentInChildren<Text>().fontSize = SettingManager.instance.textSize;


            switch (SettingManager.instance.textGap) // 줄 간격 설정값에 따라 변환
            {
                case 1:
                    newContent.GetComponentInChildren<Text>().lineSpacing = 1;
                    scrollContents.spacing = 10;
                    mobileContents.spacing = 10;
                    break;
                case 2:
                    newContent.GetComponentInChildren<Text>().lineSpacing = 2; // 줄 간격 조정
                    scrollContents.spacing = 60;
                    mobileContents.spacing = 60;
                    break;
                case 3:
                    newContent.GetComponentInChildren<Text>().lineSpacing = 3;
                    scrollContents.spacing = 110;
                    mobileContents.spacing = 110;
                    break;
                default:
                    QuitApplication();
                    break;
            }
            #region 일러스트 출력 관련
            if (scriptList[scriptIndex].mode == 0 || scriptList[scriptIndex].mode == 2)
            {
                illustManager.SetImagePannel(newContent.transform.Find("ImageButton").GetComponent<Image>()); //이미지 얹을 오브젝트 설정
            }
            else if (scriptList[scriptIndex].mode == 1)
            {
                illustManager.SetImagePannel(newContent.transform.Find("Image").GetComponent<Image>()); //이미지 얹을 오브젝트 설정
            }
            
            //path가 1개일 시 이미지 출력, 2개 이상일 시 애니메이션 출력
            if (isIllust)
            {
                newContent.GetComponent<RectTransform>().rect.Set(0, 0, 1300, 900);

                illustManager.EnableImage(); //이미지 활성화

                if (scriptList[scriptIndex].path.Length == 1)
                {

                    illustManager.LoadImage(scriptList[scriptIndex].path[0]); //이미지 설정
                                                                              //illustManager.EnableImage(); //이미지 활성화
                                                                              //illustIndex++;

                    //일러스트 출력(시청?) 결과 저장
                    GalleryManager.SaveIllustData(scriptList[scriptIndex].path[0]);

                }

                //path가 여러개일 때 애니메이션 출력
                else anim2Frame.StartAnim2Frame(newContent, scriptIndex, scriptList);

                newContent.GetComponentInChildren<Text>().text = "\n" + newContent.GetComponentInChildren<Text>().text; // 이미지가 있으면 한칸 띄움
                newContent.GetComponent<RectTransform>().sizeDelta = new Vector2(newContent.GetComponent<RectTransform>().sizeDelta.x,illustSizeY); //이미지 크기 설정

                isIllust = false;
            }

            else if (scriptList[scriptIndex].id != 8 && scriptList[scriptIndex].path != null)
            {
                if (scriptList[scriptIndex].path[0] != "0")
                {
                    gameBackGround.sprite = Resources.Load(scriptList[scriptIndex].path[0], typeof(Sprite)) as Sprite;
                    nowBackGround = scriptList[scriptIndex].path[0];
                }
                    
            }
            #endregion
            

            if (textEffect.BTextEffectOn)
            {
                textEffect.StartTextEffectCoroutine(scriptIndex, scriptList, newContent.GetComponentInChildren<Text>());
            }
            else
            {
                newContent.GetComponentInChildren<Text>().text = newText; //바로 텍스트 집어넣기
            }

            contentObjects.Add(newContent); //[현재 화면 상에 보이는 컨텐츠를 저장하는 리스트에 새로 불러온 콘텐츠를 추가함

            

            scriptIndex++;
        }

        if (scriptList[scriptIndex].mode == 0)
        {
            StartCoroutine(scrollbardown(scrollbar_mode0));
        }
        else if (scriptList[scriptIndex].mode == 1)
        {
            StartCoroutine(scrollbardown(scrollbar_mode1));
        }
        else
        { //mode == 2
            StartCoroutine(scrollbardown(scrollbar_mode2));
        }
        isScroll = true;
    }
    public void OutputTextStr(string _str)
    {
        isSet = true;

        preMode = scriptList[scriptIndex].mode;
        if (scriptList[scriptIndex].mode == 0)
        {
            newContent = Instantiate(contentPrefab); // 새 내용(글 또는 그림)
            newContent.transform.SetParent(content.transform, false); //부모 설정
            newContent.GetComponent<RectTransform>().rect.Set(0, 0, 1300, 100);
        }
        else if (scriptList[scriptIndex].mode == 1)
        {
            newContent = Instantiate(mobilePrefab); // 새 내용(글 또는 그림)
            newContent.transform.SetParent(mobileContent.transform, false); //부모 설정
        }
        else if (scriptList[scriptIndex].mode == 2)
        {
            newContent = Instantiate(mode2Prefab); // 새 내용(글 또는 그림)
            newContent.transform.SetParent(mode2Content.transform, false); //부모 설정
        }

        //이미지 & 텍스트 애니메이션 재생
        if (scriptList[scriptIndex].mode == 0)
        {
            newContent.GetComponent<Animation>().Play("Mode0prefabAnim");
        }
        else if (scriptList[scriptIndex].mode == 2)
        {
            newContent.GetComponent<Animation>().Play("Mode2PrefabAnim");
        }

        // 텍스트 출력 (추가)
        if (newContent != null)
        {
            var newText = _str; //새로 불러온 텍스트 

            newContent.GetComponentInChildren<Text>().fontSize = SettingManager.instance.textSize;


            switch (SettingManager.instance.textGap) // 줄 간격 설정값에 따라 변환
            {
                case 1:
                    newContent.GetComponentInChildren<Text>().lineSpacing = 1;
                    scrollContents.spacing = 10;
                    mobileContents.spacing = 10;
                    break;
                case 2:
                    newContent.GetComponentInChildren<Text>().lineSpacing = 2; // 줄 간격 조정
                    scrollContents.spacing = 60;
                    mobileContents.spacing = 60;
                    break;
                case 3:
                    newContent.GetComponentInChildren<Text>().lineSpacing = 3;
                    scrollContents.spacing = 110;
                    mobileContents.spacing = 110;
                    break;
                default:
                    QuitApplication();
                    break;
            }


            newContent.GetComponentInChildren<Text>().text = newText; //바로 텍스트 집어넣기

            contentObjects.Add(newContent); //[현재 화면 상에 보이는 컨텐츠를 저장하는 리스트에 새로 불러온 콘텐츠를 추가함
        }

        if (scriptList[scriptIndex].mode == 0)
        {
            StartCoroutine(scrollbardown(scrollbar_mode0));
        }
        else if (scriptList[scriptIndex].mode == 1)
        {
            StartCoroutine(scrollbardown(scrollbar_mode1));
        }
        else
        { //mode == 2
            StartCoroutine(scrollbardown(scrollbar_mode2));
        }
        isScroll = true;

    }
    void CheckNewXML()
    {
        LoadNewScripts(scriptList[scriptIndex].path[0]);
        
        SetTg0();
        randomNumber = 0;
        scriptIndex = 0;// id가 8이면 새로운 XML파일 로드
        IdCheck();
    }
    void ModePreprocess()
    {
        //직전 스크립트의 mode와 다르다면 텍스트 초기화하기
        if (preMode != scriptList[scriptIndex].mode)
        {
            for (int i = 0; i < contentObjects.Count; i++)
            {
                Destroy(contentObjects[i]);
            }
            y = 0;
            contentObjects.Clear();

            if (preMode == 2)
            { //mode2 에서 다른 모드로
                Mode2Close();
            }

        }

        if (scriptList[scriptIndex].mode == 0)
        {
            //모바일 팝업창 없애기
            MobileClose();
        }
        //Mode가 1이라면 모바일 팝업창을 띄우기 ( (cf) 종료 ID = 6)
        else if (scriptList[scriptIndex].mode == 1)
        {
            //모바일 팝업창 띄우기
            MobileOpen();
        }
        else if (scriptList[scriptIndex].mode == 2)
        {
            //mode2 띄우기
            if (preMode != 2) {
                Mode2Open();
            }
           
        }

        EvCheck();
    }
    void RemovePreText()
    {
        // 스크롤뷰에 있는 텍스트가 40개 이상일 경우 첫줄부터 삭제 211105 수정
        if (contentObjects.Count >= 40)
        {
            Destroy(contentObjects[0]);
            contentObjects.RemoveAt(0);
        }

    }
    IEnumerator scrollbardown(Scrollbar scrollbar) {
        screenTouchManager.SetTouchEnable(false);
        yield return new WaitForSeconds(0.01f);

        float minusValue = scrollbar.value / 20f;
        while (scrollbar.value > 0)
        {
            scrollbar.value -= minusValue;
            yield return new WaitForSeconds(0.005f);
        }
        screenTouchManager.SetTouchEnable(true);

    }

    public void AddTmpContent()
    {
        //스크롤 위치 조정을 위해 유령 객체를 순간적으로 넣었다 뺌
        GameObject invisibleCon = Instantiate(contentPrefab);
        invisibleCon.transform.SetParent(content.transform, false); //부모 설정
        Destroy(invisibleCon);
    }

    public void ScrollToBottom(int mode)
    {
        if (mode == 0) scrollbar_mode0.value = 0f;
        else if (mode == 1) scrollbar_mode1.value = 0f;
        else scrollbar_mode2.value = 0f;
    }

    //일러스트를 출력한다
    public void CheckIsIllust(GameObject newContent)
    {
        if (scriptList[scriptIndex].mode == 0 || scriptList[scriptIndex].mode == 2)
        {
            illustManager.SetImagePannel(newContent.transform.Find("ImageButton").GetComponent<Image>()); //이미지 얹을 오브젝트 설정
        }
        else if (scriptList[scriptIndex].mode == 1)
        {
            illustManager.SetImagePannel(newContent.transform.Find("Image").GetComponent<Image>()); //이미지 얹을 오브젝트 설정
        }



        //path가 1개일 시 이미지 출력, 2개 이상일 시 애니메이션 출력
        if (scriptList[scriptIndex].id == 6)
        {
            newContent.GetComponent<RectTransform>().rect.Set(0, 0, 1300, 900);

            isIllust = true;
            illustManager.EnableImage(); //이미지 활성화

            if (scriptList[scriptIndex].path.Length == 1)
            {

                illustManager.LoadImage(scriptList[scriptIndex].path[0]); //이미지 설정

                //일러스트 출력(시청?) 결과 저장
                GalleryManager.SaveIllustData(scriptList[scriptIndex].path[0]);

            }

            //path가 여러개일 때 애니메이션 출력
            else anim2Frame.StartAnim2Frame(newContent, scriptIndex, scriptList);

            newContent.GetComponentInChildren<Text>().text = "\n" + newContent.GetComponentInChildren<Text>().text; // 이미지가 있으면 한칸 띄움
            newContent.GetComponent<RectTransform>().sizeDelta = new Vector2(newContent.GetComponent<RectTransform>().sizeDelta.x,
                illustSizeY
                ); //이미지 크기 설정

        }
       
        else if (scriptList[scriptIndex].id != 8 && scriptList[scriptIndex].path != null)
        {
            if(scriptList[scriptIndex].path[0] != "0")
                gameBackGround.sprite = Resources.Load(scriptList[scriptIndex].path[0], typeof(Sprite)) as Sprite;
        }

    }

    public void LoadNewScripts(string path) // xml파일 불러와서 scriptList에 넣기
    {
        scriptList.Clear();
        scriptList = scrollViewManager.xmlScriptParsing(path);
        scriptPath = path;
    }

    #endregion

    #region 분기(트리거) 설정
    public void SetTg0()
    {
        if (tg == 0)
        {
            while (scriptList.Count > scriptIndex && scriptList[scriptIndex].id != 0)
            {
                scriptIndex++;
            }
        }
        tg = 0;
    }
    public void SetTg1()
    {
        tg = 1;
        AfterSelect();
    }
    public void SetTg2()
    {
        tg = 2;
        AfterSelect();
    }
    public void SetTg3()
    {
        tg = 3;
        AfterSelect();
    }

    public void SetTg4()
    {
        tg = 4;
        AfterSelect();
    }

    public void SetTg5()
    {
        tg = 5;
        AfterSelect();
    }
    IEnumerator WaitAfterChoice()
    {
        yield return new WaitForSeconds(0.2f);
        AfterSelect();
    }
    public void AfterSelect()
    {
        isChoice = 0;
        choiceNow = false;
        oneChoicePanel.ViewFalse();
        twoChoicePanel.ViewFalse();
        threeChoicePanel.ViewFalse();
        fourChoicePanel.ViewFalse();
        fiveChoicePanel.ViewFalse();
        oneChoicePanel.ActiveAllButton();
        twoChoicePanel.ActiveAllButton();
        threeChoicePanel.ActiveAllButton();
        fourChoicePanel.ActiveAllButton();
        fiveChoicePanel.ActiveAllButton();
        PreprocessNewTexts();
        screenTouchManager.SetTouchEnable(true);
        saveButton.interactable = true;
    }
    #endregion


    public void MobileOpen()  // 모바일톡 팝업 열기
    {
        mobileTalkPanel.SetActive(true);
    }

    public void MobileClose()  // 모바일톡 팝업 닫기
    {
        mobileTalkPanel.SetActive(false);
    }

    public void Mode2Open()  // Mode2 팝업 열기
    {
       
        MobileClose();
        mode2Panel.SetActive(true);
        mode2Panel.GetComponent<Animation>().Play("Mode2OpenAnim");

    }

    public void Mode2Close()  // Mode2 팝업 닫기
    {
        mode2Panel.GetComponent<Animation>().Play("Mode2CloseAnim");
        //mode2Panel.SetActive(false);
    }

    public void GameOver()
    {
        //achievementManager.OnNotify(); //정신력 0 업적
        gameOverPanel.SetActive(true);
        screenTouchManager.SetTouchEnable(false);
    }

    

    #region 저장관련
    IEnumerator SaveSuccess()
    {
        saveSuccessPanel.SetActive(true);
        Color pA = saveSuccessPanel.GetComponent<Image>().color;
        Color cA = saveSuccessPanel.GetComponentInChildren<Text>().color;
        float a;
        for (int i = 200; i >= 0; --i)
        {
            a = (float)i / 200;
            pA.a = a;
            cA.a = a;
            saveSuccessPanel.GetComponent<Image>().color = pA;
            saveSuccessPanel.GetComponentInChildren<Text>().color = cA;
            yield return null;
        }
    }
    /// <summary>
    /// save 관련 기능
    /// </summary>
    /// 

    T LoadJsonFile<T>(string loadPath, string fileName)
    {
        FileStream fileStream = new FileStream(string.Format("{0}_{1}.json", loadPath, fileName), FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = Encoding.UTF8.GetString(data);

        return JsonUtility.FromJson<T>(jsonData);
    }
    public void InitSaveSlot()
    {
        if (File.Exists(Application.persistentDataPath + "_SlotData.json"))
        {

            var loadedData = LoadJsonFile<SlotData>(Application.persistentDataPath, "SlotData");
            Slot0.GetComponent<Button>().GetComponentInChildren<Text>().text = loadedData.text0;
            Slot1.GetComponent<Button>().GetComponentInChildren<Text>().text = loadedData.text1;
            Slot2.GetComponent<Button>().GetComponentInChildren<Text>().text = loadedData.text2;
        }
    }


    public void SaveGameData()
    {
        gameButtonControl.ButtonsPanelSet();
        StartCoroutine("SaveSuccess");

        dataManager.SaveData(isSet, this, statusManager, this.slotIndex);
        var slot = GameObject.Find("SaveSlot" + this.slotIndex).GetComponent<Button>();
        slot.GetComponentInChildren<Text>().text = statusManager.monthText.text + "월 " + statusManager.dayText.text + "일";
        //slot.GetComponentInChildren<Text>().text = DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss");
        SlotManager.instance.SaveData(Slot0, Slot1, Slot2);
        InitSaveSlot();
    }
    public void QuikSave()
    {
        dataManager.SaveData(this.isSet, this, statusManager, 4);
    }
    public void SetSlotIndex(int index)
    {
        this.slotIndex = index;
    }

    public void SetConfirmActive()
    {
        ConfirmPanel.SetActive(true);
        //slotpanel interactible=false
        Slot0.interactable = false;
        Slot1.interactable = false;
        Slot2.interactable = false;
        SlotQuitBtn.interactable = false;
    }
    public void SetConfirmQuit()
    {
        ConfirmPanel.SetActive(false);
        //slotpanel interactible=true
        Slot0.interactable = true;
        Slot1.interactable = true;
        Slot2.interactable = true;
        SlotQuitBtn.interactable = true;
    }
    public void SetSaveSlotActive()
    {
        SaveSlotPanel.SetActive(true);
    }
    public void SetSaveSlotQuit()
    {
        SaveSlotPanel.SetActive(false);
    }
    #endregion

    public void SetTextSize(int tS)
    {
        foreach (GameObject objects in contentObjects)
        {
            objects.GetComponentInChildren<Text>().fontSize = tS;
        }

    }
    public void SetTextGap(int tG)
    {
        //mobileTalkManager = new MobileTalkManager();

        if (tG == 1)
        {

            foreach (GameObject objects in contentObjects)
            {
                objects.GetComponentInChildren<Text>().lineSpacing = 1;
                if (SettingManager.instance.textGap == 2)
                {
                    for (int i = objects.GetComponentInChildren<Text>().text.Length; i >= 0; i -= 2)
                    {
                        if (i - 3 >= 0)
                        {
                            objects.GetComponentInChildren<Text>().text = objects.GetComponentInChildren<Text>().text.Remove(i - 3, 1);
                        }
                    }
                }
                if (SettingManager.instance.textGap == 3)
                {
                    for (int i = objects.GetComponentInChildren<Text>().text.Length; i >= 0; i -= 3)
                    {
                        if (i - 5 >= 0)
                        {
                            objects.GetComponentInChildren<Text>().text = objects.GetComponentInChildren<Text>().text.Remove(i - 5, 2);
                        }
                    }
                }
            }
            scrollContents.spacing = 10;
            //gameManager.mobileContents.spacing = 10;
            //mobileTalkManager.SetSpacing(10); //텍스트 박스 크기 고려


            SettingManager.instance.textGap = 1;
        }
        else if (tG == 2)
        {
            foreach (GameObject objects in contentObjects)
            {
                objects.GetComponentInChildren<Text>().lineSpacing = 2;
                if (SettingManager.instance.textGap == 2) // text를 "좁게"상태로 한번 초기화 하는 과정
                {
                    for (int i = objects.GetComponentInChildren<Text>().text.Length; i >= 0; --i)
                    {
                        if (i - 3 >= 0)
                        {
                            objects.GetComponentInChildren<Text>().text = objects.GetComponentInChildren<Text>().text.Remove(i - 3, 1);

                        }
                        --i;
                    }
                }
                if (SettingManager.instance.textGap == 3) // text를 "좁게"상태로 한번 초기화 하는 과정
                {
                    for (int i = objects.GetComponentInChildren<Text>().text.Length; i >= 0; --i)
                    {
                        if (i - 5 >= 0)
                        {
                            objects.GetComponentInChildren<Text>().text = objects.GetComponentInChildren<Text>().text.Remove(i - 5, 2);

                        }
                        --i;
                        --i;
                    }
                }
                for (int i = 1; i <= objects.GetComponentInChildren<Text>().text.Length; ++i)
                {
                    objects.GetComponentInChildren<Text>().text = objects.GetComponentInChildren<Text>().text.Insert(i, " ");
                    ++i;
                }
            }
            scrollContents.spacing = 60;
            //gameManager.mobileContents.spacing = 60;
            //mobileTalkManager.SetSpacing(60);

            SettingManager.instance.textGap = 2;
        }
        else if (tG == 3)
        {
            foreach (GameObject objects in contentObjects)
            {
                objects.GetComponentInChildren<Text>().lineSpacing = 3;
                if (SettingManager.instance.textGap == 2)
                {
                    for (int i = objects.GetComponentInChildren<Text>().text.Length; i >= 0; --i)
                    {
                        if (i - 3 >= 0)
                        {
                            objects.GetComponentInChildren<Text>().text = objects.GetComponentInChildren<Text>().text.Remove(i - 3, 1);

                        }
                        --i;
                    }
                }
                if (SettingManager.instance.textGap == 3)
                {
                    for (int i = objects.GetComponentInChildren<Text>().text.Length; i >= 0; --i)
                    {
                        if (i - 5 >= 0)
                        {
                            objects.GetComponentInChildren<Text>().text = objects.GetComponentInChildren<Text>().text.Remove(i - 5, 2);

                        }
                        --i;
                        --i;
                    }
                }
                for (int i = 1; i <= objects.GetComponentInChildren<Text>().text.Length; ++i)
                {
                    objects.GetComponentInChildren<Text>().text = objects.GetComponentInChildren<Text>().text.Insert(i, "  ");
                    ++i;
                    ++i;
                }
            }
            scrollContents.spacing = 110;
            //gameManager.mobileContents.spacing = 110;
            //mobileTalkManager.SetSpacing(110);

            SettingManager.instance.textGap = 3;
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            bPaused = true;
            if (outCount == 0)
                firstPausedTime = DateTime.Now;
            outCount++; // 벨튀 업적 달성을 위함
            pausedTime = DateTime.Now;
        }
        else
        {
            if (bPaused)
            {
                bPaused = false;
                timeGap = DateTime.Now - pausedTime;


                timeGap = DateTime.Now - firstPausedTime;


                if (!isBellRun && timeGap.TotalSeconds < 10 && outCount >= 5) // 10초 안에 5번 밖에 나갔다 들어오면 벨튀업적 달성
                {
                    isBellRun = true;
                }
            }
        }
    }

}
