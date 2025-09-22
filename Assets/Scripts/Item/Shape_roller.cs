using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Passive/Shape_roller")]
public class Shape_roller : Item
{ 
    private class ShapeRollerData
    {
        public Shape shape;
        public ShapeData original_data;
        public ShapeData random_data;
        public bool isUsingOriginal = true;
    }

    public float reroll_interval = 2f;

    private Shape mid_shape;
    private Shape left_shape;
    private Shape right_shape;

    public override void ApplyToPlayer(Player player)
    {
        player.StartCoroutine(ItemLogic());
    }

    private IEnumerator ItemLogic()
    {
        Shape[] shapes = GetCurrentShapes();
        List<ShapeRollerData> rollers = new List<ShapeRollerData>();

        foreach(var shape in shapes)
        {
            if (shape == null) continue; 
            var roller = new ShapeRollerData
            {
                shape = shape,
                original_data = shape.currentShapeData,
                random_data = ShapeStorage.instance.GetRandomShapeData()
            };
            rollers.Add(roller);
        }

        while (true)
        {
            foreach(var roller in rollers)
            {
                if(roller.shape == null) continue;
                if(!roller.shape.IsOnStartPosition()) continue;
                if(!roller.shape.IsAnyOfShapeSquareActive()) continue;

                roller.isUsingOriginal = !roller.isUsingOriginal;

                ShapeData next_data = roller.isUsingOriginal ? roller.original_data : roller.random_data;

                roller.shape.RequestNewShape(next_data);
                AnimateShapeAppear(roller.shape);
                
            }

            yield return new WaitForSeconds(reroll_interval);
        }
    }

    private Shape[] GetCurrentShapes()
    {
        Shape[] shapes =
        {
            GameObject.Find("MidShape").GetComponent<Shape>(),
            GameObject.Find("LeftShape").GetComponent<Shape>(),
            GameObject.Find("RightShape").GetComponent<Shape>()
        };

        //for(int i = 0; i < shapes.Length; i++)
        //{
        //    Debug.Log(shapes[i].name);
        //}

        return shapes;
    }

    private void AnimateShapeAppear(Shape shape)
    {
        Transform t = shape.transform;

        t.DOKill();

        var orig_size = t.localScale;

        //t.localScale = orig_size * 0.9f;
        t.localRotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f)); 

        Sequence seq = DOTween.Sequence();
        //seq.Append(t.DOScale(orig_size, 0.1f).SetEase(Ease.OutBack)); 
        seq.Join(t.DORotate(orig_size, 0.3f).SetEase(Ease.OutElastic)); 
    }
}
