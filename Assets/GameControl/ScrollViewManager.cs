using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;

[System.Serializable]

public class ScriptData
{
    //public int index;
    public int id;
    public int tg;
  //  public int illustIndex;// 일러스트 인덱스 포함
    public string script;
    public int mode; //모바일 팝업, 일반 텍스트 진행모드 구분 인자
    public string[] ev;
    public string[] path;
    public string[] cond;
    public int rand;
}
public class ScrollViewManager : MonoBehaviour
{

    static public string month;
    static public string day;
    public ScrollRect scrollViewRect;
    public Scrollbar scrollbar;

    private void Awake()
    {
        scrollbar = GetComponentInChildren<Scrollbar>();
    }

    public List<ScriptData> xmlScriptParsing(string fileName)
    {
        month = fileName.Substring(4, 2);
        day = fileName.Substring(6,2);
        List<ScriptData> itemList = new List<ScriptData>();

        TextAsset scriptXml = (TextAsset)Resources.Load(fileName, typeof(TextAsset)); //텍스트를 Resources 폴더 안의 xml 파일을 부름
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(scriptXml.text);

        XmlNodeList nodes = xml.SelectNodes("Scenario/Script");

        foreach (XmlElement node in nodes)
        {
            ScriptData item = new ScriptData();
            //item.index = System.Convert.ToInt32(node.SelectSingleNode("Index").InnerText);

/*            //=== 기존 코드 (XPath 방식) ===
            item.id = System.Convert.ToInt32(node.SelectSingleNode("Id").InnerText);
            item.tg = System.Convert.ToInt32(node.SelectSingleNode("Tg").InnerText);
            item.script = node.SelectSingleNode("Text").InnerText;
            item.mode = System.Convert.ToInt32(node.SelectSingleNode("Mode").InnerText); //UI 모드전환 인자
            item.ev = node.SelectSingleNode("Ev").InnerText.Split(',');
            item.path = node.SelectSingleNode("Path").InnerText.Split(new string[] { ","," " }, System.StringSplitOptions.RemoveEmptyEntries);
            item.cond = node.SelectSingleNode("Cond").InnerText.Split(',');
            item.rand = System.Convert.ToInt32(node.SelectSingleNode("Rand").InnerText); // 랜덤분기인자*/

            //=== 변경 코드 (인덱서 방식 - XPath 해석 없이 자식 노드를 이름으로 바로 조회, 결과는 동일) ===
            item.id = System.Convert.ToInt32(node["Id"].InnerText);
            item.tg = System.Convert.ToInt32(node["Tg"].InnerText);
            item.script = node["Text"].InnerText;
            item.mode = System.Convert.ToInt32(node["Mode"].InnerText); //UI 모드전환 인자
            item.ev = node["Ev"].InnerText.Split(',');
            item.path = node["Path"].InnerText.Split(new string[] { ",", " " }, System.StringSplitOptions.RemoveEmptyEntries);
            item.cond = node["Cond"].InnerText.Split(',');
            item.rand = System.Convert.ToInt32(node["Rand"].InnerText); // 랜덤분기인자

            itemList.Add(item);
        }
        return itemList;
    }

}
