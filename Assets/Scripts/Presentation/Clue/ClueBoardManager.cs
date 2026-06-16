using UnityEngine;


/// 단서 보드의 열기/닫기를 관리 , 게임 상태(일시정지/재개)를 제어

public class ClueBoardManager : MonoBehaviour
{
    /// 단서 보드가 열려있는지 여부
    public static bool IsBoardOpen { get; private set; } = false;
    
    [Header("단서 데이터")]
    [Tooltip("ClueDatabase ScriptableObject를 드래그")]
    public ClueDatabase clueDatabase;
    
    [Header("UI 커스터마이징")]
    [Tooltip("사용자 지정 보드 배경 이미지 (없으면 기본 크림색 배경 사용)")]
    public Sprite customBackgroundImage;
    
    private ClueBoardUI boardUI;        
    private ClueService clueService;    

    void Awake()
    {
        // 저장소 생성 
        IClueRepository repository = new PlayerPrefsClueRepository();

        // 서비스 생성 
        clueService = new ClueService(clueDatabase, repository);

        // UI 컴포넌트 생성
        boardUI = gameObject.AddComponent<ClueBoardUI>();
        boardUI.Initialize(clueService, customBackgroundImage);

        // 시작 시 보드는 닫힌 상태
        IsBoardOpen = false;
    }

    void Update()
    {
        // M키 입력 감지

        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleBoard();
        }
    }

    // 단서 보드 토글 (열기/닫기) 
    private void ToggleBoard()
    {
        if (IsBoardOpen)
        {
            CloseBoard();
        }
        else
        {
            OpenBoard();
        }
    }

    // 단서 보드 열기 
    private void OpenBoard()
    {
        IsBoardOpen = true;

        // 게임 일시정지
        Time.timeScale = 0f;

        // 마우스 커서 표시 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // UI 표시
        boardUI.OpenBoard();

        Debug.Log("[ClueSystem] 📋 단서 보드 열림");
    }

    private void CloseBoard()
    {
        IsBoardOpen = false;

        // 게임 재개
        Time.timeScale = 1f;

        // 마우스 커서 다시 잠금 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // UI 숨기기
        boardUI.CloseBoard();

        Debug.Log("[ClueSystem] 📋 단서 보드 닫힘");
    }

    // 외부에서 호출
    public void DiscoverClue(string clueId)
    {
        bool isNew = clueService.DiscoverClue(clueId);
        if (isNew)
        {
            Debug.Log($"[ClueSystem] 새 단서를 발견했습니다!");
        }
    }
}
