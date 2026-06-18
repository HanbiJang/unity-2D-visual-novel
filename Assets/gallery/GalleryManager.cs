using System.Collections;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.UI;

public class GalleryManager :MonoBehaviour
{
    static string jsonData;
    const string fileName = "GalleryDataTest";
    public GameObject GalleryPanel;
    public GameObject GalleryContents; //스크롤뷰 안에 있는 contents : 인스펙터 할당
    public GameObject GalleryContentsPrefab;
    public Sprite none; //해금 안된 이미지
    public GalleryUI galleryUI; //인스펙터 할당

    private void Start()
    {
        if(GalleryPanel == null || GalleryContents == null)
        {
            GalleryPanel = GameObject.Find("GalleryPanel");
            GalleryContents = GameObject.Find("ZoomPanel");
        }

        string fileInfoName = Application.persistentDataPath + "/" + fileName;
        FileInfo file = new FileInfo(fileInfoName + ".json");
        if(!file.Exists) CreateIllustData(); //최초 한번 실행

    }

    //갤러리 애니메이션 재생
    public void PlayAnim() {
        GalleryPanel.GetComponent<Animation>().Play();
    }

    //갤러리를 비운다
    void ClearGallery() {
        for (int i = 0; i < GalleryContents.transform.childCount; i++) {
            Destroy(GalleryContents.transform.GetChild(i).gameObject);            
        }
        
    }

    //일러스트들을 해금 여부에 관해 해당 일러스트들을 출력한다
    public void ShowGalleryIllusts() {
        //내용물 비우기
        if (GalleryContents.transform.childCount != 0) {
            ClearGallery();
        }
        GalleryData galleryData = LoadIllustData();

        if (galleryData != null) {

            for (int i = 0; i < galleryData.IllustCnt; i++)
            {
                GameObject newContent = Instantiate<GameObject>(GalleryContentsPrefab, GalleryContents.transform, false);
                if (galleryData.SeeData[i])
                {
                    //이미지 설정
                    string path = (GalleryData.illustPath + "/" + galleryData.IllustsData[i]);

                    Sprite newSprite = Resources.Load(path, typeof(Sprite)) as Sprite;

                    newContent.GetComponent<ImgBtnChild>().image.GetComponent<Image>().sprite = newSprite;

                    //확대 기능 부여하기
                    newContent.GetComponent<Button>().onClick.AddListener(()=> galleryUI.ZoomPanelOpen(newSprite));
                }
                else
                {
                    newContent.GetComponent<ImgBtnChild>().image.GetComponent<Image>().sprite = none;
                }
            }
        }

    }

    //갤러리 파일 생성
    public static void CreateIllustData() {

        //파일 정보
        string createPath = Application.persistentDataPath;

        //GalleryData의 형태로 저장
        GalleryData gameData = new GalleryData();
        jsonData = JsonUtility.ToJson(gameData);

        //CreateJsonFile 
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", createPath, fileName), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }


    //일러스트 출력 시 값을 json data로 저장함
    public static void SaveIllustData(string illustPath) {

        //파일 정보
        string dataPath = Application.persistentDataPath;
        
        //GalleryData의 형태로 저장
        GalleryData gameData = LoadIllustData();

        if (gameData == null) {
            CreateIllustData();
            gameData = LoadIllustData();
        }

        //해당 일러스트(경로) 값을 true로 저장
        gameData.SetHIllustsData(illustPath, true);

        jsonData = JsonUtility.ToJson(gameData);

        //CreateJsonFile + save
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", dataPath, fileName), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();


    }

    //일러스트 출력 데이터(GalleryData)를 로드한다
    public static GalleryData LoadIllustData() {

        //파일 정보
        string dataPath = Application.persistentDataPath;
        string fileInfoName = Application.persistentDataPath + "/" + fileName;

        FileInfo file = new FileInfo(fileInfoName +".json");
        if (file.Exists)
        {
            FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", dataPath, fileName), FileMode.Open);
            byte[] data = new byte[fileStream.Length];
            fileStream.Read(data, 0, data.Length);
            fileStream.Close();

            string jsonData = Encoding.UTF8.GetString(data);
            return JsonUtility.FromJson<GalleryData>(jsonData);
        }
        else {
            return null;
        }
        
    }
}
