using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class IllustManager : MonoBehaviour
{
    public Image image; //게임매니저에서 스프라이트 보관
    public Sprite[] sprites;
    public Sprite none; //이미지 없애기

    public void SetImagePannel(Image imagePannel)
    {
        image = imagePannel;
    }

    public void LoadImage(string path)
    {
        image.sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
    }
    public void DisableImage()
    {
        //image.sprite = none;
        image.gameObject.SetActive(false);
    }

    public void EnableImage()
    {
        image.gameObject.SetActive(true);
    }
    public void SetPath(string path)
    {
        sprites = Resources.LoadAll<Sprite>(path);
    }

}
