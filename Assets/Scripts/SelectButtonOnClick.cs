using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectButtonOnClick : MonoBehaviour
{
    public int nodeNum;
    public GameBookManager gameBookManager;

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(ButtonOnClick);
    }

    public void ButtonOnClick()
    {
        gameBookManager.sceneIDX = nodeNum;
        gameBookManager.ReadCoroutine();
    }
}
