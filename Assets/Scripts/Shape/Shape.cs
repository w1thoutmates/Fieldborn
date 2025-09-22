using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler,
    IPointerDownHandler
{
    public GameObject shapePrefab;
    public Vector3 shapeSelectedScale;
    public Vector2 offset = new Vector2 (0, 700f);
    public Color ShapeColor {  get; private set; }

    [HideInInspector]
    public ShapeData currentShapeData;
    
    public int TotalSquareNumber {  get; set; }

    private List<GameObject> currentShape = new List<GameObject>();
    private Vector3 shapeStartScale;
    private RectTransform transform;
    private bool shapeDraggable = false;
    private Canvas canvas;
    private Vector3 startPosition;
    private bool shapeActive = true;
    private int lastColorIndex = -1;

    private bool isDragging = false;
    private const float StartPositionEpsilon = 1f;

    public void Awake()
    {
        shapeStartScale = this.GetComponent<RectTransform>().localScale;
        transform = this.GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        shapeDraggable = true;
        shapeActive = true;
        startPosition = transform.localPosition;
    }

    private void Start()
    {
        AssignRandomColor();
        ApplyColorToShape();
    }

    public bool IsBeingDragged()
    {
        return isDragging;
    }

    public bool IsOnStartPosition()
    {
        return Vector2.Distance(transform.localPosition, startPosition) <= StartPositionEpsilon;
        //return transform.localPosition == startPosition;
    }

    public bool IsAnyOfShapeSquareActive()
    {
        foreach (var shape in currentShape)
        {
            if(shape.gameObject.activeSelf)
            {
                return true;
            }            
        }
        return false;
    }

    public void DeactivateShape()
    {
        if (shapeActive)
        {
            foreach (var shape in currentShape)
            {
                shape?.GetComponent<ShapeSquare>().DeactivateShape();
            }
        }

        shapeActive = false;
    }

    private void SetShapeInactive()
    {
        if(IsOnStartPosition() == false && IsAnyOfShapeSquareActive())
        {
            foreach(var shape in currentShape)
            {
                shape.gameObject.SetActive(false);
            }
        }
    }

    public void ActiveShape()
    {
        if(!shapeActive)
        {
            foreach(var shape in currentShape)
            {
                shape?.GetComponent<ShapeSquare>().ActivateShape();
            }
        }
        shapeActive = true;
    }

    public void RequestNewShape(ShapeData shapeData)
    {
        transform.localPosition = startPosition;
        AssignRandomColor();
        CreateShape(shapeData);
    }

    public void CreateShape(ShapeData shapeData)
    {
        currentShapeData = shapeData;
        TotalSquareNumber = GetNumberOfShapes(shapeData);

        while (currentShape.Count <= TotalSquareNumber)
        {
            currentShape.Add(Instantiate(shapePrefab, transform) as GameObject);
        }

        foreach (var shape in currentShape)
        {
            shape.gameObject.transform.position = Vector3.zero;
            shape.gameObject.SetActive(false);
        }

        var squareRect = shapePrefab.GetComponent<RectTransform>();
        var moveDistance = new Vector2(squareRect.rect.width * squareRect.localScale.x,
            squareRect.rect.height * squareRect.localScale.y);

        int currentIndexInList = 0;

        for(var row = 0; row < shapeData.rows; row++)
        {
            for(var col = 0; col < shapeData.columns; col++)
            {
                if (shapeData.board[row].column[col])
                {
                    currentShape[currentIndexInList].SetActive(true);
                    currentShape[currentIndexInList].GetComponent<RectTransform>().localPosition =
                       new Vector2(GetXPositionForShape(shapeData, col, moveDistance), 
                       GetYPositionForShape(shapeData, row, moveDistance));

                    currentIndexInList++;
                }
            }
        }
        ApplyColorToShape();
    }

    private void OnEnable()
    {
        GameEvents.MoveShapeToStartPosition += MoveShapeToStartPosition;
        GameEvents.SetShapeInactive += SetShapeInactive;
    }

    private void OnDisable()
    {
        GameEvents.MoveShapeToStartPosition -= MoveShapeToStartPosition;
        GameEvents.SetShapeInactive -= SetShapeInactive;
    }

    private float GetYPositionForShape(ShapeData shapeData, int row, Vector2 moveDistance)
    {
        float shiftOnY = 0;

        if(shapeData.rows > 1 )
        {
            if(shapeData.rows % 2 != 0)
            {
                var middleSquareIndex = (shapeData.rows - 1) / 2;
                var multiplier = (shapeData.rows - 1) / 2;

                if(row < middleSquareIndex)
                {
                    shiftOnY = moveDistance.y * 1;
                    shiftOnY *= multiplier;
                }
                if (row > middleSquareIndex)
                {
                    shiftOnY = moveDistance.y * -1;
                    shiftOnY *= multiplier;
                }
            }

            else
            {
                var middleSquareIndex2 = (shapeData.rows == 2) ? 1 : (shapeData.rows / 2);
                var middleSquareIndex1 = (shapeData.rows == 2) ? 0 : (shapeData.rows - 2);
                var multiplier = shapeData.rows / 2;

                if(row == middleSquareIndex1 || row == middleSquareIndex2)
                {
                    if(row == middleSquareIndex2)
                    {
                        shiftOnY = (moveDistance.y / 2) * -1;
                    }
                    if(row == middleSquareIndex1)
                    {
                        shiftOnY = moveDistance.y / 2;
                    }
                }

                if(row < middleSquareIndex1 && row < middleSquareIndex2)
                {
                    shiftOnY = moveDistance.y * 1;
                    shiftOnY *= multiplier;
                }
                else if(row > middleSquareIndex1 && row > middleSquareIndex2)
                {
                    shiftOnY = moveDistance.y * -1;
                    shiftOnY *= multiplier;
                }
            }
        }
        return shiftOnY;
    }

    private float GetXPositionForShape(ShapeData shapeData, int column, Vector2 moveDistance)
    {
        float shiftOnX = 0;

        if(shapeData.columns > 1)
        {
            if(shapeData.columns % 2 != 0)
            {
                var middleSquareIndex = (shapeData.columns - 1) / 2;
                var multiplier = (shapeData.columns - 1) / 2;

                if (column < middleSquareIndex)
                {
                    shiftOnX = moveDistance.x * -1;
                    shiftOnX *= multiplier;
                }
                else if(column > middleSquareIndex)
                {
                    shiftOnX = moveDistance.x * 1;
                    shiftOnX *= multiplier;
                }
            }
            else
            {
                var middleSquareIndex2 = (shapeData.columns == 2) ? 1 : (shapeData.columns / 2);

                var middleSquareIndex1 = (shapeData.columns == 2) ? 0 : (shapeData.columns - 1);

                var multiplier = (shapeData.columns / 2);

                if(column == middleSquareIndex1 || column == middleSquareIndex2)
                {
                    if (column == middleSquareIndex2)
                    {
                        shiftOnX = moveDistance.x / 2;
                    }
                    if(column == middleSquareIndex1)
                    {
                        shiftOnX = (moveDistance.x / 2) * -1;
                    }
                }

                if(column < middleSquareIndex1 && column < middleSquareIndex2)
                {
                    shiftOnX = moveDistance.x * -1;
                    shiftOnX *= multiplier;
                }
                else if(column > middleSquareIndex1 && column > middleSquareIndex2)
                {
                    shiftOnX = moveDistance.x * 1;
                    shiftOnX *= multiplier;
                }
            }
        }
        return shiftOnX;
    }

    private int GetNumberOfShapes(ShapeData shapeData)
    {
        int number = 0;

        foreach(var rowData in shapeData.board)
        {
            foreach(var active in rowData.column)
            {
                if(active)
                {
                    number++;
                }
            }
        }

        return number;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (PauseManager.isPaused) return;
        SoundEffectStorage.instance.audio_source.PlayOneShot(SoundEffectStorage.instance.pop_sound);
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (PauseManager.isPaused) return;

        isDragging = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (PauseManager.isPaused)
        {
            GameEvents.MoveShapeToStartPosition();
            return;
        }

        if (!TurnManager.instance.IsPlayerTurn()) return;
        if (!Player.instance.CanPlaceShape()) return;

        isDragging = true;

        SoundEffectStorage.instance.audio_source.PlayOneShot(SoundEffectStorage.instance.pop_sound);

        this.GetComponent<RectTransform>().localScale = shapeSelectedScale;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (PauseManager.isPaused)
        {
            GameEvents.MoveShapeToStartPosition();
            return;
        }

        if (!TurnManager.instance.IsPlayerTurn()) return;
        if (!Player.instance.CanPlaceShape()) return;

        transform.anchorMin = new Vector2(0, 0);
        transform.anchorMax = new Vector2(0, 0);
        transform.pivot = new Vector2(0, 0);

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
            eventData.position, Camera.main, out pos);
        transform.localPosition = pos + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (PauseManager.isPaused) return;

        isDragging = false;
        this.GetComponent<RectTransform>().localScale = shapeStartScale;

        //GameEvents.MoveShapeToStartPosition();

        if (!TurnManager.instance.IsPlayerTurn()) return;
        if (!Player.instance.CanPlaceShape()) return;

        GameEvents.CheckIfShapeCanBePlaced();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (PauseManager.isPaused) return;
    }

    private void MoveShapeToStartPosition()
    {
        transform.transform.localPosition = startPosition;
        SoundEffectStorage.instance.audio_source.PlayOneShot(SoundEffectStorage.instance.deny_sound);
    }

    private void AssignRandomColor()
    {
        int newColorIndex;
        do
        {
            newColorIndex = Random.Range(0, ShapeColors.AvailableColors.Length);
        }
        while (newColorIndex == lastColorIndex && ShapeColors.AvailableColors.Length > 1);

        lastColorIndex = newColorIndex;
        ShapeColor = ShapeColors.AvailableColors[newColorIndex];
    }
    
    private void ApplyColorToShape()
    {
        foreach (var square in GetComponentsInChildren<ShapeSquare>())
        {
            square.SetColor(ShapeColor);
        }
    }
}

public static class ShapeColors
{
    public static readonly Color[] AvailableColors = new Color[]
    {
        new Color(229f/255f, 115f/255f, 115f/255f), // r
        new Color(129f/255f, 199f/255f, 132f/255f), // g
        new Color(100f/255f, 181f/255f, 246f/255f) // b
    };
}
