using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class TalkData
{
    public string group_id;
    public string talk_id;
    public string start;
    public string talk;
    public string next_talk_id;
    
    public TalkData(string group_id, string talk_id, string start, string talk, string next_talk_id)
    {
        this.group_id = group_id;
        this.talk_id = talk_id;
        this.start = start;
        this.talk = talk;
        this.next_talk_id = next_talk_id;
    }
}

public class TalkDataLoader : MonoBehaviour
{
    public static TalkDataLoader instance;
    
    [Header("CSV 파일 설정")]
    [SerializeField] private TextAsset csvFile;
    
    private List<TalkData> talkDataList = new List<TalkData>();
    
    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        // 데이터 로드
        LoadTalkData();
    }
    
    private void LoadTalkData()
    {
        if (csvFile == null)
        {
            Debug.LogError("CSV 파일이 인스펙터에 할당되지 않았습니다!");
            return;
        }
        
        ParseCSVData(csvFile.text);
        Debug.Log($"대화 데이터 {talkDataList.Count}개를 로드했습니다.");
    }
    
    private void ParseCSVData(string csvContent) {
        string[] lines = csvContent.Split('\n');
        
        for (int i = 1; i < lines.Length; i++) {
            if (string.IsNullOrEmpty(lines[i].Trim())) continue;
            
            string[] values = ParseCSVLine(lines[i]);            
            if (values.Length >= 5) {
                string group_id = values[0].Trim();
                string talk_id = values[1].Trim();
                string start = values[2].Trim().Trim('"');
                string talk = values[3].Trim().Trim('"');
                string next_talk_id = values[4].Trim();
                
                TalkData talkData = new TalkData(group_id, talk_id, start, talk, next_talk_id);
                talkDataList.Add(talkData);
            }
        }
    }    

    private string[] ParseCSVLine(string line) {
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
    
    // 그룹 ID를 받아서, 첫 번째 대화 데이터를 제공
    public TalkData GetTalkDataByGroup(string group_id)
    {
        foreach (var talkData in talkDataList) {
            if (talkData.group_id == group_id) {
                return talkData;
            }
        }

        Debug.LogWarning($"그룹 ID '{group_id}'를 찾을 수 없습니다.");
        return null;
    }

    // 대화 ID를 받아서, 대화 데이터를 제공
    public TalkData GetTalkData(string talk_id)
    {
        foreach (var talkData in talkDataList) {
            if (talkData.talk_id == talk_id) {
                return talkData;
            }
        }
        
        Debug.LogWarning($"대화 ID '{talk_id}'를 찾을 수 없습니다.");
        return null;
    }
}
