using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class TalkData
{
    public string group;
    public string id;
    public string text;
    public string nextId;
    
    public TalkData(string group, string id, string text, string nextId)
    {
        this.group = group;
        this.id = id;
        this.text = text;
        this.nextId = nextId;
    }
}

public class TalkDataLoader : MonoBehaviour
{
    public static TalkDataLoader instance;
    
    [Header("CSV 파일 설정")]
    [SerializeField] private TextAsset csvFile;
    
    private Dictionary<string, TalkData> talkDataDict = new Dictionary<string, TalkData>();
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            LoadTalkData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void LoadTalkData()
    {
        if (csvFile == null)
        {
            Debug.LogError("CSV 파일이 인스펙터에 할당되지 않았습니다!");
            return;
        }
        
        ParseCSVData(csvFile.text);
        Debug.Log($"대화 데이터 {talkDataDict.Count}개를 로드했습니다.");
    }
    
    private void ParseCSVData(string csvContent)
    {
        string[] lines = csvContent.Split('\n');
        
        // 첫 번째 줄은 헤더이므로 건너뛰기
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i].Trim())) continue;
            
            string[] values = ParseCSVLine(lines[i]);
            
            if (values.Length >= 4)
            {
                string group = values[0].Trim();
                string id = values[1].Trim();
                string text = values[2].Trim().Trim('"'); // 따옴표 제거
                string nextId = values[3].Trim();
                
                TalkData talkData = new TalkData(group, id, text, nextId);
                talkDataDict[id] = talkData;
            }
        }
    }
    
    private string[] ParseCSVLine(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        string currentField = "";
        
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(currentField);
                currentField = "";
            }
            else
            {
                currentField += c;
            }
        }
        
        result.Add(currentField);
        return result.ToArray();
    }
    
    public TalkData GetTalkData(string id)
    {
        if (talkDataDict.ContainsKey(id))
        {
            return talkDataDict[id];
        }
        
        Debug.LogWarning($"대화 ID '{id}'를 찾을 수 없습니다.");
        return null;
    }
    
    public List<TalkData> GetTalkDataByGroup(string group)
    {
        List<TalkData> result = new List<TalkData>();
        
        foreach (var talkData in talkDataDict.Values)
        {
            if (talkData.group == group)
            {
                result.Add(talkData);
            }
        }
        
        return result;
    }
    
    public bool HasTalkData(string id)
    {
        return talkDataDict.ContainsKey(id);
    }
    
    [ContextMenu("CSV 데이터 다시 로드")]
    public void ReloadData()
    {
        talkDataDict.Clear();
        LoadTalkData();
    }
    
    private void OnValidate()
    {
        // 인스펙터에서 CSV 파일이 변경되면 자동으로 다시 로드
        if (csvFile != null && Application.isPlaying)
        {
            ReloadData();
        }
    }
}
