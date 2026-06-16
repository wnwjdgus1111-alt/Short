using UnityEngine;
using System.Collections.Generic;


// 우리는 발견한 단서 ID 목록을 JSON 문자열로 변환해서 저장

public class PlayerPrefsClueRepository : IClueRepository
{

    private const string SAVE_KEY = "DiscoveredClues";

    [System.Serializable]
    private class SaveData
    {
        public List<string> discoveredIds = new List<string>();
    }

    /// 발견된 단서 ID들을 PlayerPrefs에 JSON 형태로 저장
    public void SaveDiscoveredClues(HashSet<string> discoveredClueIds)
    {
        // 1. HashSet → List로 변환
        SaveData data = new SaveData();
        data.discoveredIds = new List<string>(discoveredClueIds);

        // 2. 객체 → JSON 문자열로 변환
        string json = JsonUtility.ToJson(data);

        // 3. PlayerPrefs에 저장
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log($"[ClueSystem] 단서 저장 완료! 발견된 단서 수: {discoveredClueIds.Count}");
    }

    /// PlayerPrefs에서 발견된 단서 ID 목록을 불러옵니다.
    public HashSet<string> LoadDiscoveredClues()
    {
        // 1. 저장된 데이터가 있는지 확인
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("[ClueSystem] 저장된 단서 데이터 없음. 빈 목록 반환.");
            return new HashSet<string>();
        }

        // 2. JSON 문자열 불러오기
        string json = PlayerPrefs.GetString(SAVE_KEY);

        // 3. JSON → 객체로 변환
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // 4. List → HashSet으로 변환하여 반환
        HashSet<string> result = new HashSet<string>(data.discoveredIds);
        Debug.Log($"[ClueSystem] 단서 로드 완료! 발견된 단서 수: {result.Count}");

        return result;
    }
}