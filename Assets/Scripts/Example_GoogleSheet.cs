using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheetsToUnity;

public class Example_GoogleSheet : MonoBehaviour
{
    public string sheetID = "1pL8aflCQS5IQNItF16ObFiGXqrHetvB4X4xi20MznY4";
    public string sheetName = "UserInfo";

    void Start()
    {
        OnClickGetData();
    }

    public void OnClickGetData()
    {
        SpreadsheetManager.Read(new GSTU_Search(sheetID, sheetName), PrintDatas);
    }

    void PrintDatas(GstuSpreadSheet gstuSpreadSheet)
    {
        Debug.Log(gstuSpreadSheet["1", "Score"].value);

        List<GSTU_Cell> scoreList = new List<GSTU_Cell>();
        scoreList = gstuSpreadSheet.columns["Score"];

        foreach(var cell in scoreList)
        {
            Debug.Log(cell.value);
        }
    }
}
