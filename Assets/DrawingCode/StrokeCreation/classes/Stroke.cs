/* 
 * mayra b, 2017
 * base class of a stroke, which consist of sections that are beautified
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stroke  {

    //unique identifier
    private int _id;

    //stroke gameobject
    private GameObject _stroke;
    private List<RibbonVertex> _myPositions;

    //visual features
    private Color  _ribbonColor;
    private float _ribbonWidth;

    //geometrical features
    private int _ringVertex;

    //new stroke (gameobject + references) from Drawing Canvas
    public Stroke(int id, float ribbonWidth, Color ribbonColor, int ringVertex=6)
    {
        _id = id;

        _myPositions = new List<RibbonVertex>();

        _ribbonWidth = ribbonWidth / 2;
        _ribbonColor = ribbonColor;

        _ringVertex = ringVertex;
    }

    //store the stroke gameobject reference from Beautification Module
    public void SaveStrokeGameobject(GameObject gameobject)
    {
        _stroke = gameobject;
    }

    //add stroke positions for snapping
    public void AddStrokePosition(Vector3 newPosition, Vector3 surfaceNormal)
    {
        RibbonVertex newVertex = new RibbonVertex(newPosition, Vector3.zero, surfaceNormal);
        _myPositions.Add(newVertex);
    }

    public RibbonVertex GetStrokePosition(int index)
    {
        return _myPositions[index];
    }

    public int LastIndex()
    {
        return _myPositions.Count;
    }

    //if stroke don't have enough points destroy it
    public void DestroyStroke()
    {
        _stroke = null;
    }

    #region accesors

    public int id
    {
        get { return _id; }
    }

    public GameObject stroke
    {
        get { return _stroke; }
    }

    public float ribbonWidth
    {
        get { return _ribbonWidth; }
    }

    public Color ribbonColor
    {
        get { return _ribbonColor; }
    }

    public int ringVertex
    {
        get { return _ringVertex; }
    }
    #endregion

}
