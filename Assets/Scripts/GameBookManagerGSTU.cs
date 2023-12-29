using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GoogleSheetsToUnity;
using System.Linq;

namespace GoogleSheet
{
    [System.Serializable]
    public class Data
    {
        public string type;
        public string naration;
        public string scriptA;
        public string scriptB;
    }


    public class GameBookManagerGSTU : MonoBehaviour
    {
        [Header("GoogleSheet")]
        public string sheetID = "1KTPY6Hz_r_sqdrD4VqZKMyLTmMy_fA8HgP4OQ6RHu-Y";
        public string sheetNode = "SceneNode";
        public string scriptScene = "SceneScript";

        //[Header("FilePath")]
        ////public string nodeDataPath;
        ////public string scriptDataPath;

        public List<Dictionary<string, object>> nodeData = new List<Dictionary<string, object>>();
        public List<Dictionary<string, object>> scriptData = new List<Dictionary<string, object>>();

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
        public GameObject gameoverPanel;

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
        [Header("ScriptType")]
        public SCRIPT_TYPE scriptType;

        void Start()
        {
            gameoverPanel.SetActive(false);
            parentCanvas = GameObject.Find("Content").GetComponent<Transform>();

            //nodeData = CSVReader.Read(nodeDataPath);
            //scriptData = CSVReader.Read(scriptDataPath);

            SpreadsheetManager.Read(new GSTU_Search(sheetID, sheetNode), PrintNodeDatas);
            SpreadsheetManager.Read(new GSTU_Search(sheetID, scriptScene), PrintScriptDatas);

            ReadCoroutine();
        }

        void PrintNodeDatas(GstuSpreadSheet gstuSpreadSheet)
        {
            for (int i = 1; i < gstuSpreadSheet.Cells.Count() / 7; i++)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("IDX", gstuSpreadSheet[i.ToString(), "IDX"].value);
                dic.Add("TYPE", gstuSpreadSheet[i.ToString(), "TYPE"].value);
                dic.Add("N1", gstuSpreadSheet[i.ToString(), "N1"].value);
                dic.Add("N2", gstuSpreadSheet[i.ToString(), "N2"].value);
                dic.Add("N3", gstuSpreadSheet[i.ToString(), "N3"].value);
                dic.Add("N4", gstuSpreadSheet[i.ToString(), "N4"].value);
                dic.Add("N5", gstuSpreadSheet[i.ToString(), "N5"].value);
                nodeData.Add(dic);
            }

            Debug.Log(nodeData[0]["TYPE"]);
        }

        void PrintScriptDatas(GstuSpreadSheet gstuSpreadSheet)
        {
            for (int i = 1; i < gstuSpreadSheet.Cells.Count() / 6; i++)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("IDX", gstuSpreadSheet[i.ToString(), "IDX"].value);
                dic.Add("SceneNum", gstuSpreadSheet[i.ToString(), "SceneNum"].value);
                dic.Add("Type", gstuSpreadSheet[i.ToString(), "Type"].value);
                dic.Add("Naration", gstuSpreadSheet[i.ToString(), "Naration"].value);
                dic.Add("ScriptA", gstuSpreadSheet[i.ToString(), "ScriptA"].value);
                dic.Add("ScriptB", gstuSpreadSheet[i.ToString(), "ScriptB"].value);
                scriptData.Add(dic);
            }

            Debug.Log(scriptData[0]["Type"]);
        }


        //void ReadNodeDatas(GstuSpreadSheet gstuSpreadSheet)
        //{
        //    Dictionary<string, object> _dic = new Dictionary<string, object>();

        //    for (int i = 1; i < gstuSpreadSheet.columns["IDX"].Count(); i++)
        //    {
        //        foreach (string col in nodeSheetCols)
        //        {
        //            _dic.Add(col, gstuSpreadSheet[i.ToString(), col].value);
        //        }
        //        nodeData.Add(_dic);

        //        _dic.Clear();
        //    }
        //}

        //void ReadScriptDatas(GstuSpreadSheet gstuSpreadSheet)
        //{
        //    Dictionary<string, object> _dic = new Dictionary<string, object>();

        //    for (int i = 1; i < gstuSpreadSheet.columns["IDX"].Count(); i++)
        //    {
        //        foreach (string col in scriptSheetCols)
        //        {
        //            _dic.Add(col, gstuSpreadSheet[i.ToString(), col].value);
        //        }
        //        scriptData.Add(_dic);

        //        _dic.Clear();
        //    }
        //}

        public void ReadCoroutine()
        {
            sceneIDX_TXT.text = $"SceneIDX : {nodeData[sceneIDX]["IDX"]}";
            StartCoroutine(ReadScriptData());
        }

        IEnumerator ReadScriptData()
        {
            yield return StartCoroutine(RemoveSelectList());

            for (int i = 0; i < scriptData.Count(); i++)
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

            for (int i = 0; i < dataList.Count; i++)
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
            if (str == "0")
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
            if (textQueue.Count > 0)
            {
                GameObject target = textQueue.Dequeue();
                target.SetActive(true);
                string targetText = target.GetComponentInChildren<Text>().text;
                target.GetComponentInChildren<Text>().text = "";
                target.GetComponentInChildren<Text>().DOText(targetText, targetText.Length * 0.2f, true).OnComplete(() => Destroy(target, 0.25f));
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
            for (int i = 0; i < selectList.Count; i++)
            {
                Destroy(selectList[i]);
                selectList.RemoveAt(i);
            }
            GameObject[] texts;
            yield return texts = GameObject.FindGameObjectsWithTag("TEXT");

            GameObject[] selects;
            yield return selects = GameObject.FindGameObjectsWithTag("SELECT");

            for (int i = 0; i < texts.Length; i++)
            {
                Destroy(texts[i]);
            }

            for (int i = 0; i < selects.Length; i++)
            {
                Destroy(selects[i]);
            }

            selectList.Clear();
            dataList.Clear();
        }
    }
}

