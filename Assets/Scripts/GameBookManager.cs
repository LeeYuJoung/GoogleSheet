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
        yield return StartCoroutine(RemoveSelectList());

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

        Debug.Log($"scriptList 갯수는 {dataList.Count}");

        int nodeNum = 0;

        for(int i = 0; i<dataList.Count;i++)
        {
            switch (dataList[i].type)
            {
                case "TEXT":
                    MakeScript(dataList[i].naration, i);
                    MakeScript(dataList[i].scriptA, i);
                    MakeScript(dataList[i].scriptB, i);

                    break;
                case "SELECT":
                    nodeNum++;
                    string n = "N" + nodeNum;
                    MakeSelect(dataList[i].naration, int.Parse(nodeData[sceneIDX - 1][n].ToString()));
                    MakeSelect(dataList[i].scriptA, int.Parse(nodeData[sceneIDX - 1][n].ToString()));
                    MakeSelect(dataList[i].scriptB, int.Parse(nodeData[sceneIDX - 1][n].ToString()));

                    break;
                case "RESULT":
                    //TODO 결과창 출력...
                    break;
                default:
                    break;
            }
        }
    }

    public void MakeScript(string str, int n)
    {
        if (str == "0")
            return;

        GameObject obj = Instantiate(scriptOBJPrefab);
        obj.transform.SetParent(parentCanvas);
        obj.transform.localPosition = Vector3.one;
        obj.name = "TEXT";
        obj.tag = "TEXT";
        obj.GetComponentInChildren<Text>().text = str;
        textQueue.Enqueue(obj);
        obj.SetActive(false);
    }

    public void MakeSelect(string str, int n)
    {
        if(str == "0") 
            return;

        GameObject obj = Instantiate(selectOBJPrefab);
        obj.transform.SetParent(parentCanvas);
        obj.transform.localPosition = Vector3.one;
        obj.name = "SELECT";
        obj.tag = "SELECT";

        obj.GetComponentInChildren<Text>().text = str;
        obj.GetComponent<SelectButtonOnClick>().nodeNum = n;
        obj.GetComponent<SelectButtonOnClick>().gameBookManager = this;

        selectList.Add(obj);
        obj.SetActive(false);
    }

    public void NextButton()
    {
        if(textQueue.Count > 0)
        {
            GameObject target = textQueue.Dequeue();
            target.SetActive(true);
            string targetText = target.GetComponentInChildren<Text>().text;
            target.GetComponentInChildren<Text>().text = "";
            target.GetComponentInChildren<Text>().DOText(targetText, targetText.Length * 0.25f, true).OnComplete(() => Destroy(target, 1.0f));
        }
        else
        {
            foreach (GameObject item in selectList)
            {
                item.SetActive(true);
            }
        }
    }

    IEnumerator RemoveSelectList()
    {
        for(int i = 0; i < selectList.Count; i++)
        {
            Destroy(selectList[i]);
            selectList.RemoveAt(i);
        }
        GameObject[] texts;
        yield return texts = GameObject.FindGameObjectsWithTag("TEXT");

        GameObject[] selects;
        yield return selects = GameObject.FindGameObjectsWithTag("SELECT");

        for(int i = 0; i < texts.Length; i++)
        {
            Destroy(texts[i]);
        }

        for(int i = 0; i < selects.Length; i++)
        {
            Destroy(selects[i]);
        }

        selectList.Clear();
        dataList.Clear();
    }
}
