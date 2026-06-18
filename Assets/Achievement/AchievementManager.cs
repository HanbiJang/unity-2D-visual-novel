using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 업적 달성 조건 체크, 업적 달성 시 해당 업적 XML 파일 Done 내용 수정, 저장 하는 클래스
/// </summary>
public class AchievementManager : MonoBehaviour
{


    /*
     
    XML과 이 클래스에서 쓰는 스탯값
    rel : 관계 수치
    dre : 꿈 수치
    fam : 가족 수치
    mo : 돈 (moPoint와 같음) - XML에서는 mo로 표기
    me : 정신력 (mePoint와 같음) - XML에서는 me로 표기
    **) XML에서 스크립트 특정 지점 표시 :
    * AchievementData.XML 파일 상 : Point -> (예 - 5번째 업적에서 특정 스크립트 지점에 대한 조건이 필요하다 : <Cond>point == 5</Cond>)
    * 시나리오 스크립트 상 : Point -> 어떤 지점인지 EV태그에 표시한다 ( <Ev>point,1</Ev> )


    =====================

    XML 표기 : Cond 태그에 공백과 쉼표로 구분한다. 
    만약에 꿈 수치 10 이상 이라고 하면 <Cond>dre >= 10</Cond> 라고 왼편에는 수치, 오른편에는 값으로 구분하고
    공백으로 각각의 연산자들을 구분한다. rel dre fam mo me 와 dreSel 은 같으나
    '3월 종료'<=같이 어느 특정 스크립트 지점을 나타내는 곳은 point를 쓴다. 얘는 <Cond>point == 업적의Index값</Cond> 으로 비교연산자 ==만 가능하다

    ======================

    ***
    업적이 더 복잡해지고 비교해야 하는 값이 추가된다면, 
    StatusManager에 변수를 추가하게 된다면, GetStatus를 수정한 다음 
    이 클래스에서 CompareCond() 안의 switch문을 수정합니다.

     */


    string fileaName = "AchievementData"; //업적 XML 파일 이름
    public GameManager gameManager; //인스펙터에서 할당
    public GameObject UpPanel; //인스펙터에서 할당
    AchievementXmlManager achievementXmlManager;

    public int moPoint, mePoint, rel, dre, fam, dreSel1, countTalk, countDrawing;

    //업적 UI 조작
    GameObject AchievePanel; //업적 패널
    Animation AchieveAnim; //업적 달성 시 애니메이션 재생
    Text AchieveText; //업적패널 텍스트 변경

    Queue<Achievement> waitingDoneAchv; //대기중인 업적들 큐 (AchievementData가 아니라 Achievement)

    int meetCnt; //만족해야 하는 조건 개수
    int doneCnt; //실제로 만족한 조건 개수

    AchievementData achvList; //업적 리스트 (대화 진행마다 파일을 다시 읽지 않도록 메모리에 캐싱)


    private void Start()
    {
        achievementXmlManager = new AchievementXmlManager();
        AchievePanel = GameObject.Find("AchievePanel");
        AchieveAnim = AchievePanel.GetComponent<Animation>();
        AchieveText = (AchievePanel.GetComponentsInChildren<Text>())[1];
        waitingDoneAchv = new Queue<Achievement>();

        //업적 XML 파일에서 리스트화된 업적들을 한 번만 꺼내서 캐싱해둔다
        achvList = achievementXmlManager.xmlScriptParsing(fileaName);
    }

    /// <summary>
    /// 업적 내용이 담긴 XML 파일에서 조건을 따진 후, 업적을 Done = 1처리하여 저장함
    /// </summary>
    public void OnNotify()
    {
        //statusManager의 스탯들을 가져오기
        GetStatus();

        //Done이 0인 값에 대해서 Cond를 statusManager의 조건들과 검사한다
        if (achvList.achivements != null) {
            foreach (Achievement achv in achvList.achivements)
            { //원소 각각에 대해 반복적으로 조건 달성 여부를 확인한다
                if (achv.Done == 0)
                { //미달성
                    int DoneIndex = -1;
                    DoneIndex = CompareCond(achv);
                    if (DoneIndex > -1)
                    { //비교해봤더니 달성

                        //캐싱된 리스트에도 즉시 반영하여 다음 OnNotify부터는 다시 검사하지 않도록 함
                        achv.Done = 1;

                        //대기중 큐에 넣기
                        waitingDoneAchv.Enqueue(achv);

                        //업적 XML파일에 업적달성 처리, 저장
                        achievementXmlManager.xmlScriptSave(fileaName, DoneIndex, 1);
                    }
                }
            }

            //대기중인 큐에 대해서 업적 시스템 UI 택스트 변경 & 애니메이션 재생
            StartCoroutine(CoroutineWithMultipleParameters(waitingDoneAchv));
        }

    }

    IEnumerator CoroutineWithMultipleParameters(Queue<Achievement> waitingDoneAchv) //인덱스
    {
        while (waitingDoneAchv.Count > 0)
        {
            AchieveText.text = waitingDoneAchv.Dequeue().Text;
            AchieveAnim.Play();
            yield return new WaitForSeconds(3.6f);
        }
    }

    /// <summary>
    /// 업적 조건을 판단함
    /// </summary>
    /// <returns>조건달성한 업적 인덱스</returns>
    int CompareCond(Achievement achv)
    {
        int DoneIndex = -1; //미달성 시 -1
        string[] newCondArr = achv.Cond.Split(' ', ',');//쉼표와 공백으로 구분됨 [예: me <= 1 , mo >= 2]

        meetCnt = (int)(newCondArr.Length / 3); //만족해야 하는 조건 개수
        doneCnt = 0; //실제로 만족한 조건 개수
        int i = 0;
        while (i < newCondArr.Length)
        { //여러 조건들을 비교함 :rel, dre, fam, dreSel1, countTalk, countDrawing

            switch (newCondArr[i])
            {
                case "mo": //돈
                    IncDoneCnt(moPoint, newCondArr[i + 2], newCondArr[i + 1]);
                    break;
                case "me": //정신력
                    IncDoneCnt(mePoint, newCondArr[i + 2], newCondArr[i + 1]);
                    break;
                case "rel": //관계
                    IncDoneCnt(rel, newCondArr[i + 2], newCondArr[i + 1]);
                    break;
                case "dre": //꿈
                    IncDoneCnt(dre, newCondArr[i + 2], newCondArr[i + 1]);
                    break;
                case "fam": //가족
                    IncDoneCnt(fam, newCondArr[i + 2], newCondArr[i + 1]);
                    break;
                case "dreSel1":
                    IncDoneCnt(dreSel1, newCondArr[i + 2], newCondArr[i + 1]);
                    break;
                case "countTalk": //반복 선택
                    IncDoneCnt(countTalk, newCondArr[i + 2], newCondArr[i + 1]);
                    break;
                case "countDrawing":
                    IncDoneCnt(countDrawing, newCondArr[i + 2], newCondArr[i + 1]);
                    break;
                case "point": //현재 스크립트의 EV 태그에 적힌 point가 업적의 인덱스와 같은지 확인한다
                    if (CheckScriptEvPoint(newCondArr[i + 2])) doneCnt++;
                    break;
                default:
                    break;
            }

            i += 3;
        }

        if (meetCnt == doneCnt) { DoneIndex = achv.Index; }
        return DoneIndex;
    }

    /// <summary>
    /// 현재 스크립트의 EV 태그에 적힌 point가 업적의 인덱스와 같은지 확인한다
    /// </summary>
    /// <returns>같으면 참, 아니면 거짓 반환</returns>
    bool CheckScriptEvPoint(string value)
    {
        for (int i = 0; i < gameManager.scriptList[gameManager.scriptIndex].ev.Length; i += 3)
        {
            if (gameManager.scriptList[gameManager.scriptIndex].ev[i] == "point")
            {
                if (gameManager.scriptList[gameManager.scriptIndex].ev[i + 1] == value) doneCnt++;
                break;
            }
        }

        return false;

    }

    void IncDoneCnt(int status, string value, string oper)
    { //스탯값, 비교값, 비교연산자(문자열)

        switch (oper)
        {
            case "==":
                if (status == System.Convert.ToInt32(value)) doneCnt++;
                break;
            case "<=":
                if (status <= System.Convert.ToInt32(value)) doneCnt++;
                break;
            case ">=":
                if (status >= System.Convert.ToInt32(value)) doneCnt++;
                break;
            case ">":
                if (status > System.Convert.ToInt32(value)) doneCnt++;
                break;
            case "<":
                if (status < System.Convert.ToInt32(value)) doneCnt++;
                break;
        }
    }

    /// <summary>
    /// statusManager의 스탯내용을 가져옴
    /// </summary>
    public void GetStatus()
    {//moPoint, mePoint, rel, dre, fam, dreSel1, countTalk, countDrawing;
       StatusManager statusManager = UpPanel.GetComponent<StatusManager>();

        moPoint = statusManager.moPoint;
        mePoint = statusManager.mePoint;
        //rel = statusManager.rel;
        dre = statusManager.dre;
        fam = statusManager.fam;
    }
    /// <summary>
    /// 모든 업적을 Done=1로 초기화를 위해 버튼에 할당하는 함수
    /// </summary>
    public void BtnInitTest() {
        for (int i=0; i< achvList.achivements.Count; i++) {
            //캐싱된 리스트와 업적 XML파일을 모두 초기화 상태로 되돌림
            achvList.achivements[i].Done = 0;
            achievementXmlManager.xmlScriptSave(fileaName, i, 0);
        }
    }

    /// <summary>
    /// 애니메이션의 이벤트로 들어감 업적 패널에 있는 텍스트를 초기화함
    /// </summary>
    public void InitAchieveText() {
        AchieveText.text = " ";
    }
}




