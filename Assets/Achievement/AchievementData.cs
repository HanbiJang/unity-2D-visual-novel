using System.Collections.Generic;
using System.Xml.Serialization;

//직,역직렬화 내용 포함 코드
[System.Serializable]
[XmlType("Achievement")]
public class Achievement
{
    [XmlAttribute("Index")]
    public int Index;

    [XmlAttribute("Text")]
    public string Text;

    [XmlAttribute("HintText")]
    public string HintText;

    [XmlAttribute("SubText")]
    public string SubText;

    [XmlAttribute("Done")]
    public int Done;

    [XmlAttribute("Cond")]
    public string Cond;
}

public class AchievementData
{
    public List<Achievement> achivements;
}
