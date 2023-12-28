using GoogleSheetsToUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public string sheetID = "";
    public string sheetName = "";

    // Google Sheet ������ �ҷ�����
    public void OnClickGetData()
    {
        SpreadsheetManager.Read(new GSTU_Search(sheetID, sheetName), PrintDatas);
    }

    void PrintDatas(GstuSpreadSheet gstuSpreadSheet)
    {
        Debug.Log(gstuSpreadSheet["1", "Name"].value);

        List<GSTU_Cell> scoreList = new List<GSTU_Cell>();
        scoreList = gstuSpreadSheet.columns["Name"];

        foreach (var cell in scoreList)
        {
            Debug.Log(cell.value);
        }
    }

    // Google Sheet ������ �߰��ϱ�
    public void OnClickUpdateData()
    {
        List<string> datas = new List<string>()
        {
            "15", "Boss", "500", "10", "40", "5"
        };
        SpreadsheetManager.Write(new GSTU_Search(sheetID, sheetName, "A16"), new ValueRange(datas), UpdateCompleted);
    }

    void UpdateCompleted()
    {
        Debug.Log("::: Update Completed :::");
    }

    // Google Sheet ������ �а� ������Ʈ�ϱ�
    public void OnClickReadAndUpdate()
    {
        SpreadsheetManager.Read(new GSTU_Search(sheetID, sheetName), ReadAndUpdate);
    }

    public void ReadAndUpdate(GstuSpreadSheet sheetRef)
    {
        sheetRef["1", "Hp"].UpdateCellValue(sheetID, sheetName, "100", UpdateCompleted);
    }
}
