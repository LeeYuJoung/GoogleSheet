using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class Data
{
    public string type;
    public string naration;
    public string scriptA;
    public string scriptB;
}

public class GameBookManager : MonoBehaviour
{
    [Header("FilePath")]
    public string nodeDataPath;
    public string scriptDataPath;

    public List<Dictionary<string, object>> nodeData = new List<Dictionary<string, object>>();
    public List <Dictionary<string, object>> scriptData = new List<Dictionary<string, object>>();

    [SerializeField]
    public List<Data> dataList = new List<Data>();

    public List<GameObject> selectList = new List<GameObject>();

    [SerializeField]
    public Queue<GameObject> textQueue = new Queue<GameObject>();

    [Header("UI Elements")]
    public Transform parentCanvas;
    public Text sceneIDX_TXT;
    public GameObject scriptOBJPrefab;
    public GameObject selectOBJPrefab;
    public GameObject resultOBJPrefab;

    [Header("GameBook")]
    public int sceneIDX = 1;

    public enum SCENE_TYPE
    {
        CONTINUE,
        CHOICE,
        ENDING
    }

    public enum SCRIPT_TYPE
    {
        TEXT,
        SELECT,
        RESULT
    }

    [Header("SceneType")]
    public SCENE_TYPE SceneType;
    public SCRIPT_TYPE scriptType;

    void Start()
    {
        nodeData = CSVReader.Read(nodeDataPath);
        scriptData = CSVReader.Read(scriptDataPath);
        parentCanvas = GameObject.Find("Content").GetComponent<Transform>();

        ReadCoroutine();
    }

    public void ReadCoroutine()
    {
        sceneIDX_TXT.text = $"SceneIDX : {nodeData[sceneIDX - 1]["IDX"]}";
        StartCoroutine(ReadScriptData());
    }

    IEnumerator ReadScriptData()
    {
        yield return null;

        for(int i = 0; i < scriptData.Count; i++)
        {
            if (int.Parse(scriptData[i]["SceneNum"].ToString()) == sceneIDX)
            {
                Data data = new Data();
                data.type = scriptData[i]["Type"].ToString();
                data.naration = scriptData[i]["Naration"].ToString();
                data.scriptA = scriptData[i]["ScriptA"].ToString();
                data.scriptB = scriptData[i]["ScriptB"].ToString();
                dataList.Add(data);
            }
        }

        Debug.Log($"scriptList °¹¼ö´Â {dataList.Count}");
    }

    void Update()
    {

    }
}
