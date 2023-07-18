using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class GameManager : MonoBehaviour
{
    public Transform cameraTransform;
    public GameObject piecesHolder;
    public GameObject BlockHolder;
    public GameObject r;
    public GameObject n;
    public GameObject b;
    public GameObject q;
    public GameObject k;
    public GameObject p;
    public GameObject R;
    public GameObject N;
    public GameObject B;
    public GameObject Q;
    public GameObject K;
    public GameObject P;
    public GameObject emptyBlock;

    static Dictionary<char, GameObject> piecePrefabs = new Dictionary<char, GameObject>();
    static HashSet<char> whitePiece = new HashSet<char>() { 'R', 'N', 'B', 'Q', 'K', 'P' };
    static List<GameObject> capturedPiece = new List<GameObject>();
    static Vector3 capturedWhite = new Vector3(13, -2, 9);
    static Vector3 capturedBlack = new Vector3(-13, -2, -9);

    int floorLayer = 1 << Layers.Floor_Int;
    int pieceLayer = 1 << Layers.Piece_Int;
    int floorAndPieceLayer = (1 << Layers.Floor_Int) | (1 << Layers.Piece_Int);

    public static bool playerIsWhite = true;
    public static bool sandBox = false;
    public static bool whiteTurn = true;

    public static GameObject PiecesHolder;
    public static Board board = new Board();
    public static PieceController selectedPiece = null;
    public static HashSet<PieceController> animating = new HashSet<PieceController>();

    public GameObject DiedPieceGO;
    static DiedPiece diedPiece;

    MainCameraController mainCamera;
    StockFish ai;
    bool aiProcessing = false;

    public TMP_Text alertText;
    public GameObject alertGO;
    public PromotionPanel promotionPanel;
    Block promotingBlock = null;

    void Awake()
    {
        mainCamera = cameraTransform.GetComponent<MainCameraController>();
        PiecesHolder = piecesHolder;
        LoadPrefabs();
        CreateEmptyBoard();
    }

    void Start()
    {
        diedPiece = DiedPieceGO.GetComponent<DiedPiece>();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (!sandBox && whiteTurn != playerIsWhite && !aiProcessing) StartCoroutine(GetMove());
        if (animating.Count > 0 || !board.IsPresent()) return;
        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            if (promotingBlock != null) return;
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition), out hit, Mathf.Infinity, floorAndPieceLayer))
            {
                var piece = hit.transform.GetComponent<ISelectable>().GetPiece();
                if (piece != null && (selectedPiece == null || selectedPiece.isWhite == piece.isWhite) && (!sandBox || whiteTurn == piece.isWhite) && (sandBox || whiteTurn == playerIsWhite)) Select(piece);
                else if (selectedPiece != null)
                {
                    var block = hit.transform.GetComponent<ISelectable>().GetBlock();
                    if (block != null && board.LegalTarget(block))
                    {
                        if ((selectedPiece.notation == 'p' && block.position.y == 0) || (selectedPiece.notation == 'P' && block.position.y == 7))
                        {
                            promotionPanel.Open(selectedPiece.isWhite, block.position.ToVector());
                            promotingBlock = block;
                        }
                        else
                        {
                            board.MovePiece(selectedPiece, block);
                            whiteTurn = !whiteTurn;
                            UnSelect();
                        }
                    }
                }
            }
        }
        else if (UnityEngine.Input.GetMouseButtonDown(1))
        {
            UnSelect();
        }
    }

    void Select(PieceController piece)
    {
        if (piece.isWhite != playerIsWhite && !sandBox) return;
        if (selectedPiece != null) UnSelect();

        selectedPiece = piece;
        var moves = selectedPiece.Select();
        if (moves != null) board.HighLightBlocks(moves);
    }

    void UnSelect()
    {
        board.TurnOffHightLight();
        promotionPanel.Close();
        promotingBlock = null;
        if (selectedPiece != null)
        {
            selectedPiece.UnSelect();
            selectedPiece = null;
        }
    }

    public void CloseAlert()
    {
        alertGO.SetActive(false);
    }

    public void NewGame()
    {
        if (aiProcessing) return;
        if (ai == null && !CheckStockFishAvailable())
        {
            alertText.text = Application.persistentDataPath;
            alertGO.SetActive(true);
            return;
        }
        else if (ai == null)
        {
            ai = new StockFish(Application.persistentDataPath + "/stockfish/stockfish.exe", "go depth 5");
        }

        CreateGame(false, Random.Range(0, 2) == 0 ? false : true, Constants.startingBoard);
    }

    public bool CheckStockFishAvailable()
    {
        return File.Exists(Application.persistentDataPath + "/stockfish/stockfish.exe");
    }

    public void SandBox()
    {
        if (aiProcessing) return;
        CreateGame(true, true, Constants.startingBoard);
    }

    public void RandomPuzzle()
    {
        if (aiProcessing) return;
        CreateGame(false, true, Puzzles.TakeRandomPuzzle());
    }

    public void CreateGame(bool ifSandBox, bool ifPlayerWhite, string board)
    {
        whiteTurn = true;
        sandBox = ifSandBox;
        playerIsWhite = ifPlayerWhite;
        if (playerIsWhite != UIBoardController.isWhite) UIBoardController.FlipBoard();
        mainCamera.ResetCamera();
        ResetBoard();
        BuildBoard(Utils.NotationConverter(board));
    }

    void BuildBoard(List<string> notation)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var p = notation[i][j];
                var piece = p == '-' ? null : CreatePiece(p, j, i);
                Board.RegisterPiece(j, i, piece);
            }
        }
        UIBoardController.UpdateBoard();
    }

    public static PieceController CreatePiece(char p, int x, int y)
    {
        var isWhite = whitePiece.Contains(p);
        var rot = isWhite ? Constants.front : Constants.back;
        var go = GameObject.Instantiate(piecePrefabs[p], Position.PositionToVector(x, y), rot, PiecesHolder.transform);
        var controller = go.GetComponent<PieceController>();
        controller.Init(isWhite, x, y);

        return controller;
    }

    void ResetBoard()
    {
        board.Reset();
        capturedWhite = new Vector3(13, -2, 9);
        capturedBlack = new Vector3(-13, -2, -9);
        foreach (Transform child in piecesHolder.transform) Destroy(child.gameObject);
        foreach (var c in capturedPiece) Destroy(c);
        capturedPiece.Clear();
    }

    void CreateEmptyBoard()
    {
        board.CreateEmptyBoard(emptyBlock, BlockHolder);
    }

    void LoadPrefabs()
    {
        piecePrefabs['r'] = r;
        piecePrefabs['n'] = n;
        piecePrefabs['b'] = b;
        piecePrefabs['q'] = q;
        piecePrefabs['k'] = k;
        piecePrefabs['p'] = p;

        piecePrefabs['R'] = R;
        piecePrefabs['N'] = N;
        piecePrefabs['B'] = B;
        piecePrefabs['Q'] = Q;
        piecePrefabs['K'] = K;
        piecePrefabs['P'] = P;
    }

    public void Backward()
    {
        if (animating.Count > 0 || aiProcessing) return;
        board.Backward();
    }

    public void Forward()
    {
        if (animating.Count > 0 || aiProcessing) return;
        board.Forward();
    }

    public void ToPresent()
    {
        if (animating.Count > 0 || aiProcessing) return;
        board.ToPresent();
    }

    public static void AddCapturedPiece(PieceController piece)
    {
        var p = piece.notation;
        var isWhite = piece.isWhite;
        var position = isWhite ? capturedWhite : capturedBlack;
        var offSet = isWhite ? new Vector3(0, 0, -1.5f) : new Vector3(0, 0, 1.5f);
        var rot = isWhite ? Constants.left : Constants.right;
        var go = GameObject.Instantiate(diedPiece.piecePrefabs[p], position, rot, DiedPiece.holder);
        capturedPiece.Add(go);

        if (isWhite) capturedWhite += offSet;
        else capturedBlack += offSet;
    }

    IEnumerator GetMove()
    {
        aiProcessing = true;
        ai.Input(board.GetNotation());
        yield return new WaitForSeconds(2);
        while (animating.Count > 0) yield return new WaitForSeconds(0.5f);
        aiProcessing = false;
        var move = ai.GetMove();
        board.AIMove(move);
        whiteTurn = !whiteTurn;
    }

    public void PromoteToRook()
    {
        Promote(selectedPiece.isWhite ? 'R' : 'r');
    }

    public void PromoteToKnight()
    {
        Promote(selectedPiece.isWhite ? 'N' : 'n');
    }

    public void PromoteToBishop()
    {
        Promote(selectedPiece.isWhite ? 'B' : 'b');
    }

    public void PromoteToQueen()
    {
        Promote(selectedPiece.isWhite ? 'Q' : 'q');
    }

    void Promote(char p)
    {
        whiteTurn = !whiteTurn;
        board.PromotionMove(selectedPiece, promotingBlock, p);
        UnSelect();
    }
}