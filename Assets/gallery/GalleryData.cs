using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
public class GalleryData
{   
    //일러스트 이름과 illustName - 출력여부
    public List<string> IllustsData;
    public List<bool> SeeData; //IllustsData와 같은 순서로 봤는지에 대한 여부가 저장됨

    public int IllustCnt = 0; //일러스트 총 개수
    public const string illustPath = "Images/Illusts/GameIllusts";


    public GalleryData() {
        InitIllustData();
    }

    //모든 images/Illusts 폴더 안의 png 이미지들의 경로를 illustsData에 가져온다 + 초기화
    void InitIllustData()
    {
/*        string[] illustsRoot = Directory.GetFiles(Application.dataPath + "/Resources/" + illustPath, "*.png");//폴더 내의 png파일 탐색
*/
        Sprite[] spritetmp = Resources.LoadAll<Sprite>(illustPath);

        /*        //테스트 구문들
                GameObject testText = GameObject.Find("TestText");
                testText.GetComponent<Text>().text += "CreatePath: " + illustsRoot[0] + "\n";*/

        /*        IllustCnt = illustsRoot.Length;*/
        IllustCnt = spritetmp.Length;

        IllustsData = new List<string>();
        SeeData = new List<bool>();

        for (int i = 0; i < IllustCnt; i++) {
            /*IllustsData.Add(getIllustName(illustsRoot[i]));*/
            IllustsData.Add(spritetmp[i].name);
            SeeData.Add(false);
        }
    }

    public void SetHIllustsData(string XmliIlustPath, bool value) { //xml 상에 표기된 일러스트 경로
        //HIllustsData.Add(getIllustName(XmliIlustPath), value);
        int targetIdx = IllustsData.FindIndex(a => a.Contains(getIllustName(XmliIlustPath)));
        if(targetIdx >= 0) {
            SeeData[targetIdx] = value;
        }
        
    }

    //경로에서 이미지 파일 이름만(.png삭제) 가져오기
    string getIllustName(string XmliIlustPath) {
        string[] paths = XmliIlustPath.Split('/', '.');
        string result = paths[paths.Length - 1]; 
        return paths[paths.Length - 1];
    }
}