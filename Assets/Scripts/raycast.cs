using UnityEngine;

public class raycast : MonoBehaviour
{
    public float maxDistance = 1f; // 상호작용 가능 사거리
    public Renderer targetObject_wire; // 외곽선 쉐이더
    public GameObject text; // 상호작용 문구

    [Header("단서 시스템")]
    [Tooltip("ClueBoardManager 컴포넌트가 있는 오브젝트를 드래그")]
    public ClueBoardManager clueBoardManager;

    int layerMask;

    void Awake()
    {
        layerMask = LayerMask.GetMask("Interactable");
    }

    void Update()
    {
        
        if (ClueBoardManager.IsBoardOpen) return;

        // 레이캐스트 충돌 정보
        RaycastHit hit;

        // 카메라에서 정면으로 maxDistance까지 켄버스를 무시하고 레이 발사
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance))
        {
            
            ClueObject clueObject = hit.collider.GetComponent<ClueObject>();
            
            if (clueObject != null)
            {
                
                // 외곽선 표시
                if (targetObject_wire != null)
                {
                    targetObject_wire.enabled = true;
                    text.SetActive(true);
                }

                // E키로 단서 발견
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (clueBoardManager != null && clueObject.clueData != null)
                    {
                        clueBoardManager.DiscoverClue(clueObject.clueData.clueId);
                    }
                    else
                    {
                        Debug.LogWarning("[ClueSystem] ClueBoardManager 또는 ClueData가 연결되지 않았습니다!");
                    }
                }
                
                return; 
            }

            // 레이케스트가 Cube에 닿은 경우
            if (hit.collider.gameObject.name == "Cube")
            {
                // 상호작용
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("hello world!");
                }

                // 외각선 쉐이더 켜기
                if (targetObject_wire != null)
                {
                    targetObject_wire.enabled = true;
                    text.SetActive(true);
                }
            }
                
            else
            {
                // 외각선 쉐이더 끄기
                if (targetObject_wire != null)
                {
                    targetObject_wire.enabled = false;
                    text.SetActive(false);
                }
            }
        }
        else
        {
            if (targetObject_wire != null)
            {
                targetObject_wire.enabled = false;
                text.SetActive(false);
            }
        }
    }
}
