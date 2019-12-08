using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class Sort : Singleton<Sort>
{
    public int index;
    public List<GraphView> graphViews = new List<GraphView>();

    [System.Serializable]
    public class DataSet
    {
        public string name;
        public List<int> data = new List<int>();
    }
    public List<DataSet> dataSets = new List<DataSet>();

    private void OnValidate()
    {
        MakeDataList();
        GetComponentsInChildren(includeInactive: true, result: graphViews);
    }
    public void SetIndex(int index)
    {
        this.index = index;
        UpdateGraph();
    }
    public void StartSort()
    {
        UpdateGraph();
        foreach (GraphView graphView in graphViews)
        {
            graphView.StartSort();
        }
    }

    public void UpdateGraph()
    {
        MakeDataList();
        foreach (GraphView graphView in graphViews)
        {
            graphView.UpdateGraphView(dataSets[index].data);
        }
    }
    private void MakeDataList()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Input/");
        FileInfo[] fileInfo = dir.GetFiles("*.*", SearchOption.AllDirectories);

        dataSets.Clear();
        for (int i = 0; i < fileInfo.Length; i++)
        {
            // file extension check
            if (fileInfo[i].Extension == ".txt")
            {
                DataSet dataSet = new DataSet();
                dataSet.name = fileInfo[i].Name;
                dataSet.data = ParseFile(fileInfo[i].Name);
                dataSets.Add(dataSet);
            }
        }
    }
    private List<int> ParseFile(string name)
    {
        string text = string.Empty;
        try
        {
            text = File.ReadAllText(Application.dataPath + "/Input/" + name);
        }
        catch
        {
            return null;
        }

        char[] separators = { ',', ';', '|' };
        string[] strValues = text.Split(separators);

        List<int> intValues = new List<int>();
        foreach (string str in strValues)
        {
            int val = 0;
            if (int.TryParse(str, out val))
                intValues.Add(val);
        }
        return intValues;
    }
}
