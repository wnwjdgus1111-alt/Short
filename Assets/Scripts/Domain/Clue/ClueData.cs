using UnityEngine;

// ============================================================================
// [CreateAssetMenu] 어트리뷰트를 붙이면 
// Unity 에디터에서 우클릭 → Create → Game → Clue Data 로 에셋을 만들 수 있음.
// ============================================================================

[CreateAssetMenu(fileName = "NewClue", menuName = "Game/Clue Data")]
public class ClueData : ScriptableObject
{
    // ── 기본 정보 ──────────────────────────────────────────
    
    [Header("기본 정보")]
    [Tooltip("단서의 고유 식별자")]
    public string clueId;
    
    [Tooltip("단서 이름")]
    public string clueName;
    
    // ── 상세 정보 ──────────────────────────────────────────
    
    [Header("상세 정보")]
    [TextArea(3, 5)]  // Inspector에서 여러 줄 입력 가능한 텍스트 영역으로 표시
    [Tooltip("단서에 대한 상세 설명")]
    public string description;
    
    [Tooltip("어디서 발견했는지 한 줄 설명. 예: 2층 서재 책상 위에서 발견")]
    public string discoveryHint;
    
    // ── 비주얼 ─────────────────────────────────────────────
    
    [Header("비주얼")]
    [Tooltip("단서 이미지. 없으면 기본 아이콘이 표시됩니다")]
    public Sprite clueImage;
}

