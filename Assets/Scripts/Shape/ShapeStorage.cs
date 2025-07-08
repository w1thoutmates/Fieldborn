using UnityEngine;
using System.Collections.Generic;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> shapeData;
    public List<Shape> shapeList;

    private int lastShapeIndex = -1;

    private void Start()
    {
        foreach (var shape in shapeList)
        {
            var shapeIndex = UnityEngine.Random.Range(0, shapeData.Count);
            shape.CreateShape(shapeData[shapeIndex]);
        }
    }

    public Shape GetCurrentSelectedShape()
    {
        foreach(var shape in shapeList)
        {
            if (shape.IsOnStartPosition() == false && shape.IsAnyOfShapeSquareActive())
                return shape;    
        }
        Debug.Log("There is no shape selected.");
        return null;
    }

    private void OnEnable()
    {
        GameEvents.RequestNewShapes += RequestNewShapes;
    }

    private void OnDisable()
    {
        GameEvents.RequestNewShapes -= RequestNewShapes;
    }

    public void RequestNewShapes()
    {
        if (!Player.instance.CanPlaceShape()) return;

        foreach(var shape in shapeList)
        {
            var shapeIndex = GetNonRepeatingIndex();
            shape.RequestNewShape(shapeData[shapeIndex]);
        }
    }

    private int GetNonRepeatingIndex()
    {
        if(shapeData.Count == 0)
        {
            Debug.Log("ShapeData list is empty");
            return 0;
        }

        int index;
        do
        {
            index = Random.Range(0, shapeData.Count);
        }
        while (index == lastShapeIndex && shapeData.Count > 1);

        lastShapeIndex = index;

        return index;
    }
}
