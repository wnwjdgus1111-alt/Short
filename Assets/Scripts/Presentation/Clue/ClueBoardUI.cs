using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// 구조
//   Canvas (화면에 UI를 그리는 캔버스)
//   └── Panel (배경/컨테이너)
//       ├── Image (이미지 표시)
//       ├── Text (텍스트 표시)
//       └── ScrollView (스크롤 가능한 영역)

// RectTransform: 일반 Transform의 UI 버전.
//   - anchorMin/anchorMax: 부모 기준 위치 비율 (0~1)
//   - offsetMin/offsetMax: 앵커 기준 오프셋 (픽셀)
//   - pivot: 자기 자신의 기준점 (0~1)

public class ClueBoardUI : MonoBehaviour
{
    // ── UI 요소 참조 ──────────────────────────────────────
    private GameObject boardRoot;           // 보드 전체 루트 (활성/비활성용)
    private Transform listContent;          // 좌측 리스트 Content 영역
    private Image detailImage;              // 우측 상세 - 단서 이미지
    private Text detailName;                // 우측 상세 - 단서 이름
    private Text detailDescription;         // 우측 상세 - 단서 설명
    private Text detailLocation;            // 우측 상세 - 발견 장소
    private GameObject lockIcon;            // 우측 상세 - 자물쇠 (미발견 시)
    private Text headerCountText;           // 상단 - 발견 진행도

    // ── 상태 ──────────────────────────────────────────────
    private ClueService clueService;
    private List<ClueListItemUI> listItems = new List<ClueListItemUI>();
    private int selectedIndex = 0;

    // ── 노트패드 스타일 색상 정의 ──────────────────────────

    
    private static readonly Color COLOR_OVERLAY = new Color(0f, 0f, 0f, 0.75f);         // 반투명 검정 배경
    private static readonly Color COLOR_PAPER = new Color(1f, 0.98f, 0.89f, 1f);        // 크림색 노트 용지
    private static readonly Color COLOR_LINE = new Color(0.7f, 0.85f, 0.95f, 0.5f);     // 연한 파란 줄
    private static readonly Color COLOR_MARGIN = new Color(0.9f, 0.3f, 0.3f, 0.4f);     // 빨간 마진선
    private static readonly Color COLOR_HIGHLIGHT = new Color(1f, 0.95f, 0.3f, 0.35f);  // 형광펜 하이라이트
    private static readonly Color COLOR_TEXT = new Color(0.2f, 0.2f, 0.25f, 1f);        // 짙은 글씨
    private static readonly Color COLOR_TEXT_DIM = new Color(0.5f, 0.5f, 0.55f, 1f);    // 흐린 글씨 (???)
    private static readonly Color COLOR_DIVIDER = new Color(0.75f, 0.73f, 0.68f, 1f);   // 구분선

    // ── 초기화 ────────────────────────────────────────────


    public void Initialize(ClueService clueService, Sprite bgImage = null)
    {
        this.clueService = clueService;
        BuildUI(bgImage);
        boardRoot.SetActive(false); 
    }

    public void OpenBoard()
    {
        PopulateList();
        
        // 첫 번째 항목 선택
        selectedIndex = 0;
        UpdateSelection();

        boardRoot.SetActive(true);
    }

    public void CloseBoard()
    {
        boardRoot.SetActive(false);
    }

    void Update()
    {
        if (!ClueBoardManager.IsBoardOpen) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveSelection(-1);  // 위로 이동
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveSelection(1);   // 아래로 이동
        }
    }


    private void MoveSelection(int direction)
    {
        if (listItems.Count == 0) return;

        selectedIndex += direction;

        selectedIndex = Mathf.Clamp(selectedIndex, 0, listItems.Count - 1);

        UpdateSelection();
    }

    private void UpdateSelection()
    {
        if (listItems.Count == 0) return;

        // 모든 항목의 하이라이트 해제
        for (int i = 0; i < listItems.Count; i++)
        {
            listItems[i].SetSelected(i == selectedIndex);
        }

        // 우측 상세 패널 갱신
        ClueData clue = clueService.GetDatabase().allClues[selectedIndex];
        bool isDiscovered = clueService.IsClueDiscovered(clue.clueId);

        ShowClueDetail(clue, isDiscovered);
    }


    private void ShowClueDetail(ClueData clue, bool isDiscovered)
    {
        if (isDiscovered)
        {
            //  발견된 단서: 이미지 + 이름 + 설명 표시
            lockIcon.SetActive(false);

            detailImage.gameObject.SetActive(true);
            if (clue.clueImage != null)
            {
                detailImage.sprite = clue.clueImage;
                detailImage.color = Color.white;
            }
            else
            {
                // 이미지가 없으면 회색 플레이스홀더
                detailImage.sprite = null;
                detailImage.color = new Color(0.85f, 0.83f, 0.78f);
            }

            detailName.text = clue.clueName;
            detailName.color = COLOR_TEXT;

            detailDescription.text = clue.description;
            detailDescription.gameObject.SetActive(true);

            detailLocation.text = $"📍 {clue.discoveryHint}";
            detailLocation.gameObject.SetActive(true);
        }
        else
        {
            //  미발견 단서: 자물쇠 아이콘만 표시
            detailImage.gameObject.SetActive(false);
            detailName.text = "???";
            detailName.color = COLOR_TEXT_DIM;
            detailDescription.gameObject.SetActive(false);
            detailLocation.gameObject.SetActive(false);
            lockIcon.SetActive(true);
        }
    }


    private void PopulateList()
    {

        foreach (var item in listItems)
        {
            Destroy(item.gameObject);
        }
        listItems.Clear();

        headerCountText.text = $"단서 ({clueService.GetDiscoveredClueCount()}/{clueService.GetTotalClueCount()})";

        // 각 단서에 대해 리스트 아이템 생성
        var database = clueService.GetDatabase();
        
        if (database == null || database.allClues == null)
        {
            Debug.LogError("[ClueBoardUI] ❌ 단서 데이터베이스(ClueDatabase)가 연결되지 않았습니다! ClueBoardManager 인스펙터에 ClueDatabase 에셋을 할당해주세요.");
            return;
        }

        for (int i = 0; i < database.allClues.Count; i++)
        {
            ClueData clue = database.allClues[i];
            bool isDiscovered = clueService.IsClueDiscovered(clue.clueId);

            // 리스트 아이템 생성
            GameObject itemGO = CreateListItem(listContent, i, clue, isDiscovered);
            ClueListItemUI itemUI = itemGO.GetComponent<ClueListItemUI>();
            listItems.Add(itemUI);
        }

        // 레이아웃 강제 재계산 (Content 높이가 0에서 시작하므로 즉시 갱신 필요)
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(listContent as RectTransform);
    }

    private void BuildUI(Sprite bgImage)
    {
        //  Canvas 생성 ─────────────────────────────────
        //    sortingOrder가 높을수록 다른 Canvas보다 위에 그려집니다.
        
        boardRoot = new GameObject("ClueBoardCanvas");
        boardRoot.transform.SetParent(transform);
        
        Canvas canvas = boardRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        // 📘 CanvasScaler: 다양한 해상도에서 UI가 올바르게 표시되도록 합니다.
        //    ScaleWithScreenSize: 기준 해상도를 설정하고, 실제 해상도에 맞게 자동 스케일링
        CanvasScaler scaler = boardRoot.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f; // 가로/세로 균형 맞춤

        boardRoot.AddComponent<GraphicRaycaster>();

        // ── 2. 어두운 배경 오버레이 ──────────────────────────
        GameObject overlay = CreateUIElement("Overlay", boardRoot.transform);
        Image overlayImg = overlay.AddComponent<Image>();
        overlayImg.color = COLOR_OVERLAY;
        StretchToFill(overlay); // 화면 전체를 덮음

        // ── 3. 노트패드 메인 패널 ────────────────────────────
        // 화면 중앙에 80% x 85% 크기의 노트패드
        GameObject notebook = CreateUIElement("NotebookPanel", overlay.transform);
        Image notebookImg = notebook.AddComponent<Image>();
        SetAnchors(notebook, 0.10f, 0.075f, 0.90f, 0.925f);

        if (bgImage != null)
        {
            // 사용자가 이미지를 지정한 경우
            notebookImg.sprite = bgImage;
            notebookImg.color = Color.white; // 원본 이미지 색상
            notebookImg.type = Image.Type.Sliced; // 9-Slice 이미지를 지원하기 위함
        }
        else
        {
            // 기본 노트패드 테마
            notebookImg.color = COLOR_PAPER;
            
            // 노트패드 줄무늬 효과 (수평 줄) - 기본 테마에서만 추가
            CreateNotebookLines(notebook.transform, 20);
        }

        // ── 4. 헤더 (제목 + 발견 진행도) ──────────────────
        GameObject header = CreateUIElement("Header", notebook.transform);
        SetAnchors(header, 0.02f, 0.9f, 0.98f, 0.98f);
        
        // 제목
        Text titleText = CreateText(header.transform, "Title", "📒  단서 노트", 24, FontStyle.Bold, TextAnchor.MiddleLeft);
        SetAnchors(titleText.gameObject, 0f, 0f, 0.5f, 1f);
        titleText.color = COLOR_TEXT;

        // 진행도
        headerCountText = CreateText(header.transform, "Count", "단서 (0/0)", 18, FontStyle.Normal, TextAnchor.MiddleRight);
        SetAnchors(headerCountText.gameObject, 0.5f, 0f, 1f, 1f);
        headerCountText.color = COLOR_TEXT_DIM;

        // 헤더 아래 구분선
        GameObject headerLine = CreateUIElement("HeaderLine", notebook.transform);
        Image headerLineImg = headerLine.AddComponent<Image>();
        headerLineImg.color = COLOR_DIVIDER;
        SetAnchors(headerLine, 0.02f, 0.89f, 0.98f, 0.895f);

        // ── 5. 좌측 패널 (단서 리스트) ──────────────────────
        // 📘 ScrollRect: 콘텐츠가 영역을 넘어가면 스크롤할 수 있게 해줍니다.
        //    Content 오브젝트 안에 리스트 아이템들을 추가합니다.
        
        GameObject leftPanel = CreateUIElement("LeftPanel", notebook.transform);
        SetAnchors(leftPanel, 0.02f, 0.03f, 0.38f, 0.88f);

        // 빨간 마진선 (노트패드 왼쪽의 빨간 줄!)
        GameObject marginLine = CreateUIElement("MarginLine", leftPanel.transform);
        Image marginImg = marginLine.AddComponent<Image>();
        marginImg.color = COLOR_MARGIN;
        RectTransform marginRect = marginLine.GetComponent<RectTransform>();
        marginRect.anchorMin = new Vector2(0.08f, 0f);
        marginRect.anchorMax = new Vector2(0.085f, 1f);
        marginRect.offsetMin = Vector2.zero;
        marginRect.offsetMax = Vector2.zero;

        // 스크롤뷰
        GameObject scrollView = CreateScrollView(leftPanel.transform, "ClueScrollView");
        SetAnchors(scrollView, 0.1f, 0f, 1f, 1f);
        
        // Content는 ScrollView 안의 실제 콘텐츠 영역
        listContent = scrollView.transform.Find("Viewport/Content");

        // ── 6. 세로 구분선 ────────────────────────────────
        GameObject divider = CreateUIElement("Divider", notebook.transform);
        Image dividerImg = divider.AddComponent<Image>();
        dividerImg.color = COLOR_DIVIDER;
        SetAnchors(divider, 0.39f, 0.05f, 0.395f, 0.87f);

        // ── 7. 우측 패널 (단서 상세) ───────────────────────
        GameObject rightPanel = CreateUIElement("RightPanel", notebook.transform);
        SetAnchors(rightPanel, 0.41f, 0.03f, 0.98f, 0.88f);

        // 단서 이미지 (테이프로 붙인 느낌)
        GameObject imageArea = CreateUIElement("ImageArea", rightPanel.transform);
        SetAnchors(imageArea, 0.1f, 0.35f, 0.9f, 0.95f);
        
        detailImage = imageArea.AddComponent<Image>();
        detailImage.color = new Color(0.85f, 0.83f, 0.78f);
        detailImage.preserveAspect = true;

        // 이미지 테두리 (테이프 느낌)
        Outline imageOutline = imageArea.AddComponent<Outline>();
        imageOutline.effectColor = new Color(0.7f, 0.68f, 0.62f, 0.5f);
        imageOutline.effectDistance = new Vector2(2, 2);

        // 단서 이름
        detailName = CreateText(rightPanel.transform, "DetailName", "", 22, FontStyle.Bold, TextAnchor.MiddleCenter);
        SetAnchors(detailName.gameObject, 0.05f, 0.25f, 0.95f, 0.33f);
        detailName.color = COLOR_TEXT;

        // 단서 설명
        detailDescription = CreateText(rightPanel.transform, "DetailDesc", "", 16, FontStyle.Normal, TextAnchor.UpperLeft);
        SetAnchors(detailDescription.gameObject, 0.08f, 0.08f, 0.92f, 0.24f);
        detailDescription.color = COLOR_TEXT;

        // 발견 장소
        detailLocation = CreateText(rightPanel.transform, "DetailLocation", "", 14, FontStyle.Italic, TextAnchor.MiddleLeft);
        SetAnchors(detailLocation.gameObject, 0.08f, 0.01f, 0.92f, 0.07f);
        detailLocation.color = COLOR_TEXT_DIM;

        // 자물쇠 아이콘 (미발견 시)
        lockIcon = CreateUIElement("LockIcon", rightPanel.transform);
        SetAnchors(lockIcon, 0.3f, 0.4f, 0.7f, 0.8f);
        Text lockText = lockIcon.AddComponent<Text>();
        lockText.text = "🔒";
        lockText.fontSize = 72;
        lockText.alignment = TextAnchor.MiddleCenter;
        lockText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // 조작 안내
        Text helpText = CreateText(notebook.transform, "HelpText", "↑↓ 이동  |  M 닫기", 14, FontStyle.Normal, TextAnchor.MiddleCenter);
        SetAnchors(helpText.gameObject, 0.3f, 0.005f, 0.7f, 0.028f);
        helpText.color = COLOR_TEXT_DIM;
    }

    // ========================================================================
    //  UI 유틸리티 메서드들
    // ========================================================================
    // 📘 아래는 UI 요소를 쉽게 만들기 위한 헬퍼(Helper) 메서드들입니다.
    //    같은 코드를 반복하지 않도록 재사용 가능한 함수로 분리했습니다.
    //    이것을 "DRY 원칙" (Don't Repeat Yourself)이라고 합니다!
    // ========================================================================

    /// <summary>
    /// 기본 UI 요소(RectTransform 포함)를 생성합니다.
    /// </summary>
    private GameObject CreateUIElement(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false); // false: 월드 좌표가 아닌 로컬 좌표 유지
        go.AddComponent<RectTransform>();
        return go;
    }

    /// <summary>
    /// UI 요소를 부모 전체에 꽉 채웁니다.
    /// </summary>
    private void StretchToFill(GameObject go)
    {
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;      // 좌하단 (0, 0)
        rect.anchorMax = Vector2.one;       // 우상단 (1, 1)
        rect.offsetMin = Vector2.zero;      // 오프셋 없음
        rect.offsetMax = Vector2.zero;
    }

    /// <summary>
    /// 앵커를 비율로 설정합니다. (0~1 범위)
    /// 예: SetAnchors(go, 0.1, 0.1, 0.9, 0.9) → 부모의 10%~90% 영역
    /// </summary>
    private void SetAnchors(GameObject go, float minX, float minY, float maxX, float maxY)
    {
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(minX, minY);
        rect.anchorMax = new Vector2(maxX, maxY);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    /// <summary>
    /// Text UI 요소를 생성합니다.
    /// </summary>
    private Text CreateText(Transform parent, string name, string content, int fontSize, FontStyle style, TextAnchor alignment)
    {
        GameObject go = CreateUIElement(name, parent);
        Text text = go.AddComponent<Text>();
        text.text = content;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.alignment = alignment;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.supportRichText = true;
        return text;
    }

    /// <summary>
    /// 노트패드 수평 줄 무늬를 생성합니다.
    /// </summary>
    private void CreateNotebookLines(Transform parent, int lineCount)
    {
        for (int i = 1; i <= lineCount; i++)
        {
            float yPos = 1f - (float)i / (lineCount + 1);
            GameObject line = CreateUIElement($"Line_{i}", parent);
            Image lineImg = line.AddComponent<Image>();
            lineImg.color = COLOR_LINE;
            lineImg.raycastTarget = false; // 클릭을 방해하지 않음

            RectTransform rect = line.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.02f, yPos);
            rect.anchorMax = new Vector2(0.98f, yPos + 0.002f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }

    /// <summary>
    /// 스크롤뷰를 생성합니다.
    /// </summary>
    private GameObject CreateScrollView(Transform parent, string name)
    {
        // ScrollView 루트
        GameObject scrollView = CreateUIElement(name, parent);
        ScrollRect scrollRect = scrollView.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;  // 수평 스크롤 비활성화
        scrollRect.vertical = true;     // 수직 스크롤만

        // Viewport (보이는 영역)
        GameObject viewport = CreateUIElement("Viewport", scrollView.transform);
        StretchToFill(viewport);
        Image viewportImg = viewport.AddComponent<Image>();
        viewportImg.color = new Color(0, 0, 0, 0); // 투명
        Mask mask = viewport.AddComponent<Mask>();
        mask.showMaskGraphic = false;

        // Content (실제 콘텐츠가 들어가는 곳)
        GameObject content = CreateUIElement("Content", viewport.transform);
        RectTransform contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);  // 상단 고정
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);   // 위에서 아래로 채움
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;

        // 📘 VerticalLayoutGroup: 자식 요소들을 자동으로 세로로 정렬합니다.
        //    Spacing: 아이템 간 간격
        //    Padding: 전체 여백
        VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 4;
        layout.padding = new RectOffset(15, 10, 8, 8);
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        // 📘 ContentSizeFitter: Content의 크기를 자식 요소에 맞게 자동 조정합니다.
        //    이래야 아이템이 많아지면 스크롤이 가능해집니다!
        ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // ScrollRect에 연결
        scrollRect.viewport = viewport.GetComponent<RectTransform>();
        scrollRect.content = contentRect;

        return scrollView;
    }

    /// <summary>
    /// 개별 단서 리스트 아이템을 생성합니다.
    /// </summary>
    private GameObject CreateListItem(Transform parent, int index, ClueData clue, bool isDiscovered)
    {
        GameObject itemGO = CreateUIElement($"ClueItem_{index}", parent);

        // 높이 설정
        LayoutElement layoutElement = itemGO.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 36;
        layoutElement.minHeight = 36;

        // 배경 (선택 시 하이라이트용)
        Image bg = itemGO.AddComponent<Image>();
        bg.color = Color.clear;

        // 텍스트
        string displayText = isDiscovered
            ? $"  {index + 1}. {clue.clueName}"
            : $"  {index + 1}. ???";

        Text itemText = CreateText(itemGO.transform, "Text", displayText, 17, FontStyle.Normal, TextAnchor.MiddleLeft);
        StretchToFill(itemText.gameObject);
        itemText.color = isDiscovered ? COLOR_TEXT : COLOR_TEXT_DIM;

        // ClueListItemUI 컴포넌트 추가
        ClueListItemUI itemUI = itemGO.AddComponent<ClueListItemUI>();
        itemUI.Setup(bg, itemText, isDiscovered);

        return itemGO;
    }
}
