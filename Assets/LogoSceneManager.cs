using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LogoSceneManager : MonoBehaviour
{
    public GameObject fadePanel;
    
    private Image fadePanelImage;
    private bool isDark = true, isClicked = false;

    // Start is called before the first frame update
    void Awake()
    {
        fadePanelImage = fadePanel.GetComponent<Image>();

    }

    // Update is called once per frame
    void Update()
    {
        if(isDark && !isClicked)
            StartCoroutine("FadeIn");

        if (Input.GetMouseButtonDown(0) && !isDark)
            isClicked = true;

        if (!isDark && isClicked)
        {
            fadePanel.SetActive(true);
            StartCoroutine("FadeOut");
        }

        
    }

    IEnumerator FadeIn()
    {
        Color color = fadePanelImage.color;
        int i = 0;
        for (i = 100; i >= 0; --i)
        {
            color.a -= Time.deltaTime * 0.01f;

            fadePanelImage.color = color;

            if (fadePanelImage.color.a <= 0)
            {
                isDark = false;
                fadePanel.SetActive(false);
            }
        }
        yield return null;
    }
    IEnumerator FadeOut()
    {
        Color color = fadePanelImage.color;
        int i = 0;
        for (i = 0; i <= 100; ++i)
        {
            color.a += Time.deltaTime * 0.01f;

            fadePanelImage.color = color;

            if (fadePanelImage.color.a >= 1f)
            {
                isDark = true;
                SceneManager.LoadScene("LobbyScene");
            }
        }
        yield return null;
    }
}
