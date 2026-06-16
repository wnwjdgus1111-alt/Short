using System.Collections.Generic;

/// 단서 발견 상태를 저장/불러오기 위한 인터페이스.

public interface IClueRepository
{
    /// 발견된 단서 ID 목록을 저장
    void SaveDiscoveredClues(HashSet<string> discoveredClueIds);

    /// 저장된 발견 단서 ID 목록을 불러옴
    HashSet<string> LoadDiscoveredClues();
}
