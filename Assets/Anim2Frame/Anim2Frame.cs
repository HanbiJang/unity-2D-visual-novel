using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Anim2Frame : MonoBehaviour
{
    /// <summary>
    /// 2개 이상의 이미지를 연결하여 보여줌
    /// </summary>
    public void StartAnim2Frame(GameObject newContent, int scriptIndex, List<ScriptData> scriptList) {
        StopCoroutine("StartAnim2Frame");
        //이미지 1개를 1초뒤에 순서대로 교체한다
        //gameManager.illustManager.EnableImage(); //이미지 활성화
        Image image;
        image = newContent.GetComponentInChildren<Image>(); 

        StartCoroutine(ChangeImageEffect(image, scriptIndex, scriptList));
    }

    IEnumerator ChangeImageEffect(Image image, int scriptIndex, List<ScriptData> scriptList)
    {
        int i = 0;
        while (true) {
            //gameManager.illustManager.LoadImage(scriptList[scriptIndex].path[i]); //이미지 설정
            image.sprite = Resources.Load(scriptList[scriptIndex].path[i], typeof(Sprite)) as Sprite;
            if (i != scriptList[scriptIndex].path.Length-1) i++;
            else i = 0;

            yield return new WaitForSeconds(0.5f);
        }
    }
    
}
