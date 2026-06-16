using System.Collections.Generic;
using UnityEngine;

// 단서 시스템의 핵심 비즈니스 로직을 담당하는 서비스.
// UI에서 이 클래스의 메서드만 호출하면 됨.
public class ClueService
{
    private readonly ClueDatabase database; 
    private readonly IClueRepository repository; 
    private HashSet<string> discoveredClueIds;      

    public ClueService(ClueDatabase database, IClueRepository repository)
    {
        this.database = database;
        this.repository = repository;

        this.discoveredClueIds = repository.LoadDiscoveredClues();
    }

    /// 단서를 발견 처리, 이미 발견된 단서면 무시
    public bool DiscoverClue(string clueId)
    {
        // 이미 발견한 단서인지 확인
        if (discoveredClueIds.Contains(clueId))
        {
            Debug.Log($"[ClueSystem] '{clueId}' 단서는 이미 발견됨. 무시.");
            return false;
        }

        // 데이터베이스에 존재하는 단서인지 확인
        ClueData clueData = database.FindById(clueId);
        if (clueData == null)
        {
            Debug.LogWarning($"[ClueSystem] '{clueId}' 단서가 데이터베이스에 없음");
            return false;
        }

        // 발견 처리
        discoveredClueIds.Add(clueId);

        // 자동 저장
        repository.SaveDiscoveredClues(discoveredClueIds);

        Debug.Log($"[ClueSystem] 새 단서 발견 '{clueData.clueName}'");
        return true;
    }

    //단서 목록 조회 
    public ClueDatabase GetDatabase()
    {
        return database;
    }

    //단서 발견 여부 확인
    public bool IsClueDiscovered(string clueId)
    {   
        return discoveredClueIds.Contains(clueId);
    }

    // 총 단서 수
    public int GetTotalClueCount()
    {
        return database != null ? database.Count : 0;
    }

    // 발견된 단서 수
    public int GetDiscoveredClueCount()
    {
        return discoveredClueIds.Count;
    }
}
