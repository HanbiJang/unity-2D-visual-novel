using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementSetting : MonoBehaviour
{
    //업적 XML 데이터들을 파싱하여 Contents 안에 뿌린다
    AchievementXmlManager achievementXMLData;
    string fileName = "AchievementData";

    //업적에 대한 정보 뿌릴 UI
    public GameObject AchvPrefab;
    public GameObject AchvContent;

    public GameObject AchieveSettingPanel;

    public string Done0Text = "???";//해금 안됐을 때 텍스트

    //자식 오브젝트
    List<GameObject> AllAchvContents;

    //최초 실행 시 1번만 실행함 (1번만 업적을 만든다)
    private void Start()
    {
        achievementXMLData = new AchievementXmlManager();
        AllAchvContents = new List<GameObject>();
        MakeAchvs();
    }

    //활성화가 될 때마다 업적 XML 데이터를 가져와서 텍스트를 바꾼다
    private void OnEnable()
    {
        if (AllAchvContents != null) {
            ChangeText();
        }     
    }

    //최초로 실행됨
    void MakeAchvs()
    {
        AchievementData itemList;
        itemList = achievementXMLData.xmlScriptParsing(fileName); //데이터 가져오기

        for (int i = 0; i < itemList.achivements.Count; i++)
        {
            GameObject newContent = Instantiate(AchvPrefab); //업적 프리팹으로 생성
            newContent.transform.SetParent(AchvContent.transform, false); //부모 설정
            AllAchvContents.Add(newContent); //자식 관리

            Text[] TitleSubText;
            TitleSubText = newContent.GetComponentsInChildren<Text>(); //타이틀,서브텍스트 순으로 가져옴

            if (itemList.achivements[i].Done == 1) //달성
            {
                TitleSubText[0].text = itemList.achivements[i].Text;
                TitleSubText[1].text = itemList.achivements[i].SubText;
            }
            else if(itemList.achivements[i].Done == 0)
            { //미달성 Done0Text
                //TitleSubText[0].text = itemList.achivements[i].Text;
                TitleSubText[0].text = Done0Text;
                //TitleSubText[1].text = Done0Text;
                TitleSubText[1].text = itemList.achivements[i].HintText;
            }
        }
    }

    void ChangeText() {
        //List<AchievementData> itemList;
        AchievementData itemList;
        achievementXMLData = new AchievementXmlManager();
        itemList = achievementXMLData.xmlScriptParsing(fileName); //데이터 가져오기

        for (int i = 0; i < itemList.achivements.Count; i++)
        {
            Text[] TitleSubText = AllAchvContents[i].GetComponentsInChildren<Text>();

            if (itemList.achivements[i].Done == 1) //달성
            {
                TitleSubText[0].text = itemList.achivements[i].Text;
                TitleSubText[1].text = itemList.achivements[i].SubText;
            }
            else if (itemList.achivements[i].Done == 0)
            { //미달성
                TitleSubText[0].text = Done0Text;
                //TitleSubText[0].text = itemList.achivements[i].Text;
                TitleSubText[1].text = itemList.achivements[i].HintText;
            }
        }
    }

    public void OpenAchieveSettingPanel() {
        AchieveSettingPanel.SetActive(true);
    }
    public void CloseAchieveSettingPanel()
    {
        AchieveSettingPanel.SetActive(false);
    }
}
