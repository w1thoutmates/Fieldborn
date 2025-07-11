using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Unity.Multiplayer.Center.Common;
using System;

public class Grid : MonoBehaviour
{
    public static Grid instance;
    public ShapeStorage shapeStorage;
    public int columns = 8;
    public int rows = 8;
    public float cellSize = 1f;
    public GameObject cellPrefab;

    private List<GameObject> cells = new List<GameObject>();
    private LineIndicator lineIndicator;
    public float additionalYOffset = 1f;
    public GameObject choice_menu;

    private Canvas canvas;

    private Queue<GameObject> choice_queue = new Queue<GameObject>();
    private bool isChoosing = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
    }

    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
    }

    private void Start()
    {
        lineIndicator = GetComponent<LineIndicator>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        CreateGrid();
    }

    private void CreateGrid()
    {
        SpawnGridSquares();
    }

    private void SpawnGridSquares()
    {
        int square_index = 0;

        float offsetX = (columns - 1) * cellSize / 2f;
        float offsetY = (rows - 1) * cellSize / 2f;

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < columns; col++)
            {
                Vector3 position = new Vector3(col * cellSize - offsetX, -row * cellSize + offsetY + additionalYOffset, 0f);
                cells.Add(Instantiate(cellPrefab, position, Quaternion.identity, transform) as GameObject);
                cells[cells.Count - 1].GetComponent<GridSquare>().SquareIndex = square_index;
                cells[cells.Count - 1].transform.SetParent(this.transform);
                cells[cells.Count - 1].transform.localScale = new Vector3(1f, 1f, 1f);
                square_index++;
            }
        }
    }

    private void CheckIfShapeCanBePlaced()
    {
        var squareIndexes = new List<int>();
        foreach (var square in cells)
        {
            var gridSquare = square.GetComponent<GridSquare>();
            if (gridSquare.Selected && !gridSquare.SquareOccupied)
            {
                squareIndexes.Add(gridSquare.SquareIndex);
                gridSquare.Selected = false;
            }
        }

        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();
        if (currentSelectedShape == null) return;

        if (currentSelectedShape.TotalSquareNumber == squareIndexes.Count &&
            DoesShapeMatchGrid(currentSelectedShape, squareIndexes))
        {
            Color shapeColor = currentSelectedShape.ShapeColor;
            foreach (var squareIndex in squareIndexes)
            {
                cells[squareIndex].GetComponent<GridSquare>().PlaceShapeOnBoard(shapeColor);
            }

            Player.instance.OnShapePlaced();

            var shapeLeft = 0;
            foreach (var shape in shapeStorage.shapeList)
            {
                if (shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
                {
                    shapeLeft++;
                }
            }

            if (shapeLeft == 0 && Player.instance.CanPlaceShape())
            {
                GameEvents.RequestNewShapes();
            }
            else
            {
                GameEvents.SetShapeInactive();
            }

            CheckIfAnyLineIsCompleted();
        }
        else
        {
            GameEvents.MoveShapeToStartPosition();
        }
    }

    void CheckIfAnyLineIsCompleted()
    {
        List<int[]> lines = new List<int[]>();

        // for columns
        foreach (var column in lineIndicator.columnIndexes)
        {
            lines.Add(lineIndicator.GetVerticalLine(column));
        }

        // for rows
        for (var row = 0; row < 8; row++)
        {
            List<int> data = new List<int>(8);
            for (var index = 0; index < 8; index++)
            {
                data.Add(lineIndicator.line_data[row, index]);
            }

            lines.Add(data.ToArray());
        }

        var completedLines = CheckIfSquaresAreCompleted(lines);

        if (completedLines > 2)
        {
            // any bonus
        }

        // add scores
    }

    private int CheckIfSquaresAreCompleted(List<int[]> data)
    {
        List<int[]> completedLines = new List<int[]>();

        var linesCompleted = 0;

        foreach (var line in data)
        {
            var lineCompleted = true;

            int redCounter = 0;
            int greenCounter = 0;
            int blueCounter = 0;

            foreach (var squareIndex in line)
            {
                var comp = cells[squareIndex].GetComponent<GridSquare>();
                if (comp.SquareOccupied == false)
                {
                    lineCompleted = false;
                }

                Color squareColor = comp.activeImage.color;
                if (squareColor == Color.red)
                    redCounter++;
                if (squareColor == Color.green)
                    greenCounter++;
                if (squareColor == Color.blue)
                    blueCounter++;
            }

            if (lineCompleted)
            {
                Vector3 targetPosition = Vector3.zero;

                if (redCounter > greenCounter && redCounter > blueCounter)
                {
                    Debug.Log("<color=red> –¿—Õ€… ƒŒÃ»Õ»–”≈“!!!</color>");
                    targetPosition = Player.instance.damage_counter_text.transform.position;
                    StartCoroutine(AnimateSquaresToCounter(line, Color.red, targetPosition));
                    Player.instance.damage_counter++;
                    Player.instance.UpdateUI();
                    // Red-Dominated color bonus
                }
                else if (greenCounter > redCounter && greenCounter > blueCounter)
                {
                    Debug.Log("<color=green>«≈À≈Õ€… ƒŒÃ»Õ»–”≈“!!!</color>");
                    targetPosition = Player.instance.heal_counter_text.transform.position;
                    StartCoroutine(AnimateSquaresToCounter(line, Color.green, targetPosition));
                    Player.instance.heal_counter++;
                    Player.instance.UpdateUI();
                    // Green-Dominated color bonus
                }
                else if (blueCounter > redCounter && blueCounter > greenCounter)
                {
                    Debug.Log("<color=blue>C»Õ»… ƒŒÃ»Õ»–”≈“!!!</color>");
                    targetPosition = Player.instance.shield_counter_text.transform.position;
                    StartCoroutine(AnimateSquaresToCounter(line, Color.blue, targetPosition));
                    Player.instance.shield_counter++;
                    Player.instance.UpdateUI();
                    // Blue-Dominated color bonus
                }
                else
                {
                    GameObject choice_menu_instance = Instantiate(choice_menu, transform.position, Quaternion.identity, canvas.transform);
                    choice_menu_instance.SetActive(false);
                    choice_queue.Enqueue(choice_menu_instance);

                    if (!isChoosing)
                        StartCoroutine(ChoiceQueue());

                    Debug.Log("ÕË◊Â√Ó ÕÂ ƒÓÃËÕË–Û≈Ú");
                    // Give possible for player to select the counter++
                }

                completedLines.Add(line);
            }
        }

        foreach (var line in completedLines)
        {
            var completed = false;
            foreach (var squareIndex in line)
            {
                var comp = cells[squareIndex].GetComponent<GridSquare>();
                comp.Deactivate();
                completed = true;
            }

            foreach (var squareIndex in line)
            {
                var comp = cells[squareIndex].GetComponent<GridSquare>();
                comp.ClearOccupied();
            }

            if (completed)
            {
                linesCompleted++;
            }
        }
        return linesCompleted;
    }

    private System.Collections.IEnumerator ChoiceQueue()
    {
        isChoosing = true;

        while (choice_queue.Count > 0)
        {
            GameObject current_choice = choice_queue.Dequeue();
            current_choice.SetActive(true);

            bool choice_made = false;

            ChoiceMenu menu = current_choice.GetComponent<ChoiceMenu>();
            menu.OnChoiceMade += () => { choice_made = true; };

            yield return new WaitUntil(() => choice_made);

            Destroy(current_choice);
            yield return new WaitForSeconds(0.4f);
        }
        isChoosing = false;
    }

    private bool DoesShapeMatchGrid(Shape shape, List<int> selectedSquares)
    {
        var lineIndicator = GetComponent<LineIndicator>();
        var selectedPositions = new List<(int, int)>();
        foreach (var index in selectedSquares)
        {
            selectedPositions.Add(lineIndicator.GetSquarePosition(index));
        }

        var shapeData = shape.currentShapeData;
        var shapePositions = new List<(int, int)>();
        for (int row = 0; row < shapeData.rows; row++)
        {
            for (int col = 0; col < shapeData.columns; col++)
            {
                if (shapeData.board[row].column[col])
                {
                    shapePositions.Add((row, col));
                }
            }
        }

        var normalizedShape = NormalizePositions(shapePositions);
        var normalizedSelected = NormalizePositions(selectedPositions);

        return ArePositionsEqual(normalizedShape, normalizedSelected);
    }

    private List<(int, int)> NormalizePositions(List<(int, int)> positions)
    {
        if (positions.Count == 0) return positions;

        int minRow = positions.Min(p => p.Item1);
        int minCol = positions.Min(p => p.Item2);

        return positions.Select(p => (p.Item1 - minRow, p.Item2 - minCol)).ToList();
    }

    private bool ArePositionsEqual(List<(int, int)> a, List<(int, int)> b)
    {
        if (a.Count != b.Count) return false;

        var setA = new HashSet<(int, int)>(a);
        var setB = new HashSet<(int, int)>(b);

        return setA.SetEquals(setB);
    }

    private IEnumerator AnimateSquaresToCounter(int[] squareIndexes, Color dominantColor, Vector3 targetPosition)
    {
        List<GameObject> animatedSquares = new List<GameObject>();

        foreach (var index in squareIndexes)
        {
            var originalSquare = cells[index].GetComponent<GridSquare>();
            if (originalSquare.activeImage.color == dominantColor)
            {
                GameObject animatedSquare = new GameObject("FlyingSquare");
                Image img = animatedSquare.AddComponent<Image>();
                img.color = dominantColor;
                img.sprite = originalSquare.activeImage.sprite;
                animatedSquare.transform.SetParent(canvas.transform, false);
                animatedSquare.transform.position = cells[index].transform.position;
                animatedSquare.transform.localScale = Vector3.one;
                animatedSquares.Add(animatedSquare);

                originalSquare.Deactivate();
                originalSquare.ClearOccupied();
            }
        }

        float fallDuration = 0.05f;
        float elapsedTime = 0f;
        Vector3[] startPositions = animatedSquares.Select(s => s.transform.position).ToArray();
        Vector3[] fallPositions = animatedSquares.Select(s => s.transform.position + Vector3.down * 0.5f).ToArray();

        while (elapsedTime < fallDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fallDuration;
            for (int i = 0; i < animatedSquares.Count; i++)
            {
                animatedSquares[i].transform.position = Vector3.Lerp(startPositions[i], fallPositions[i], t);
            }
            yield return null;
        }

        float suctionDuration = 0.25f;
        elapsedTime = 0f;
        Vector3[] suctionStartPositions = animatedSquares.Select(s => s.transform.position).ToArray();

        while (elapsedTime < suctionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / suctionDuration;
            for (int i = 0; i < animatedSquares.Count; i++)
            {
                animatedSquares[i].transform.position = Vector3.Lerp(suctionStartPositions[i], targetPosition, t);
                animatedSquares[i].transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.05f, t);
            }
            yield return null;
        }

        foreach (var square in animatedSquares)
        {
            Destroy(square);
        }
    }

    public void ClearBoard()
    {
        StartCoroutine(ClearBoardWithDelay());
    }

    private IEnumerator ClearBoardWithDelay()
    {
        float delay_btw_fall = 0.008f;

        foreach (var cell in cells)
        {
            var grid_square = cell.GetComponent<GridSquare>();

            if (grid_square.SquareOccupied)
            {
                grid_square.Deactivate();
                grid_square.ClearOccupied();

                StartCoroutine(FallSquare(grid_square));

                yield return new WaitForSeconds(delay_btw_fall);
            }
        }
    }

    private IEnumerator FallSquare(GridSquare grid_square)
    {
        GameObject animated_square = new GameObject("FlyingSquare");
        Image img = animated_square.AddComponent<Image>();
        img.color = grid_square.activeImage.color;
        img.sprite = grid_square.activeImage.sprite;
        animated_square.transform.SetParent(canvas.transform, false);
        animated_square.transform.position = grid_square.transform.position;
        animated_square.transform.localScale = Vector3.one;

        Vector3 start_position = animated_square.transform.position;
        Vector3 end_position = start_position + Vector3.down * 8f;

        float duration = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            animated_square.transform.position = Vector3.Lerp(start_position, end_position, t);
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1f - t);
            yield return null;
        }

        Destroy(animated_square);
    }

}