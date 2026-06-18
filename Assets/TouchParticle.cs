using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchParticle : MonoBehaviour
{
    public GameObject touchParticle;
    public GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckMousePos();
    }
    public void CheckMousePos()
    {
        Vector2 mousePos;
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Input.mousePosition;
            SoundManager.instance.PlaySE("클릭");
            var touchEffect = Instantiate(touchParticle, new Vector3(mousePos.x, mousePos.y, 1f), Quaternion.identity, canvas.transform);

            Destroy(touchEffect, 0.5f);
        }
    }
}
