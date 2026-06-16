using UnityEngine;


// 사용 방법:
// 1. 씬에서 단서가 될 오브젝트를 선택
// 2. Inspector에서 Add Component → ClueObject를 추가
// 3. clueData 필드에 해당 ClueData ScriptableObject를 드래그

public class ClueObject : MonoBehaviour
{
    [Header("이 오브젝트가 나타내는 단서")]
    [Tooltip("ClueData ScriptableObject를 드래그")]
    public ClueData clueData;

    // 에디터에서만 보이는 시각적 표시.
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }

    void OnDrawGizmosSelected()
    {
        // 선택했을 때 더 크게 표시
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}
