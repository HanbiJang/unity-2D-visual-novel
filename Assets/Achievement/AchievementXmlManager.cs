using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using System.Xml;

/// <summary>
/// 업적 달성 시 XML 파일 관리에 관한 클래스
/// </summary>
public class AchievementXmlManager
{
    //XmlSerializer 생성자는 호출될 때마다 리플렉션으로 직렬화 어셈블리를 새로 만들어 비용이 크므로, 타입당 1회만 생성해 재사용한다
    static readonly XmlSerializer serializer = new XmlSerializer(typeof(List<Achievement>), new XmlRootAttribute("AchievementData"));

    //Resource 폴더에서 찾아서 읽은 후 persistentDataPath 경로에 생성한다
    public void CreateAchieveXml() {
        //최초 실행
        //Resource에서 읽고 역직렬화하여 데이터 가져옴.
        string SFileName = "XML/NewAchievementData";
        AchievementData itemList = new AchievementData();

        TextAsset scriptXml = (TextAsset)Resources.Load(SFileName, typeof(TextAsset));

        using (var stream = new MemoryStream(scriptXml.bytes))
        {
            itemList.achivements = (List<Achievement>)serializer.Deserialize(stream);
        }

        //가져온 데이터 바탕으로 persistant 루트에 직렬화하여 xml파일 생성하기
        string userAchieveDataName = "AchievementData";

        string path = Application.persistentDataPath + "/" + userAchieveDataName + ".xml";

        FileStream fileStream = new FileStream(path, FileMode.Create);
        serializer.Serialize(fileStream, itemList.achivements);
        fileStream.Close();
    }

    //XML 파일을 읽어서 업적 리스트를 반환한다
    public AchievementData xmlScriptParsing(string fileaName) {
        AchievementData achievement = LoadAchieveXml(fileaName);
        return achievement;
    }



    //업적 달성 시 Done 태그 값을 수정한다
    public void xmlScriptSave(string fileName, int Index, int newValue) {

        AchievementData achievementData = LoadAchieveXml(fileName); //로드
        achievementData.achivements[Index].Done = newValue;

        string path = Application.persistentDataPath + "/" + fileName + ".xml";
        FileStream fileStream = new FileStream(path, FileMode.Create);
        serializer.Serialize(fileStream, achievementData.achivements);
        fileStream.Close();

    }

    //xml 데이터를 역직렬화하여 클래스 객체로 받기
    AchievementData LoadAchieveXml(string fileName) {
        string fileInfoName = Application.persistentDataPath + "/" + fileName;
        FileInfo file = new FileInfo(fileInfoName + ".xml");
        if (!file.Exists)
        {
            CreateAchieveXml();
        }

        AchievementData achievementData = new AchievementData();

        FileStream fileStream = new FileStream(fileInfoName + ".xml", FileMode.Open);
        achievementData.achivements = (List<Achievement>)serializer.Deserialize(fileStream);
        fileStream.Close();

        return achievementData;
    }
}
