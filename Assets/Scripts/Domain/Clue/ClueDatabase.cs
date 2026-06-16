using UnityEngine;
using System.Collections.Generic;

// ============================================================================
// ClueData = 개별 단서 1개
// ClueDatabase = 게임에 존재하는 모든 단서의 목록
//
// 이것도 ScriptableObject이므로 에디터에서 에셋으로 만들어서
// 단서들을 드래그 앤 드롭으로 등록할 수 있음.
//
// 사용 방법:
// 1. Unity 에디터에서 우클릭 → Create → Game → Clue Database
// 2. 생성된 에셋을 선택하면 Inspector에 리스트가 보임
// 3. 리스트에 ClueData 에셋들을 드래그해서 등록
// ============================================================================

[CreateAssetMenu(fileName = "ClueDatabase", menuName = "Game/Clue Database")]
public class ClueDatabase : ScriptableObject
{
    [Header("게임 내 모든 단서 목록")]
    [Tooltip("여기에 모든 ClueData 에셋을 등록. 순서가 보드에서의 표시 순서가 됨")]
    public List<ClueData> allClues = new List<ClueData>();


    public ClueData FindById(string clueId)
    {
        return allClues.Find(clue => clue.clueId == clueId);
    }

    public int Count => allClues.Count;

}
