/* 
 * mayra barrera, 2017
 * variables accessible everywhere
 * most of them are static variables,
 * others are me being lazy
 */


using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Xml;

public class GlobalVars {

    private static GlobalVars _instance;
    public static GlobalVars Instance
    {
        get
        {
            if (_instance == null)
                _instance = new GlobalVars();
            return _instance;
        }
    }

    public string currentParticipant;

    public Dictionary<string, List<LogPosition>> userHeadPositions = new Dictionary<string, List<LogPosition>>();
    public Dictionary<string, List<LogPosition>> userRightHandPositions = new Dictionary<string, List<LogPosition>>();
    public Dictionary<string, List<LogPosition>> userLeftHandPositions = new Dictionary<string, List<LogPosition>>();

    public ObjectShape thisObjectShape;
    public VisualGuide thisVisualGuide;
    public DrawnDirection thisDrawnDirection;
    public DrawnSize thisDrawnSize;

    //public int phase
    public ExperimentPhase thisExperiment;

    #region experimentalVariables

    public enum ObjectShape
    {
        none,
        circle,
        curvedCircle,
        cube,
        curvedCurbe,
    }

    public enum VisualGuide
    {
        none,
        noGuide,
        dottedGuide,
        fullGuide
    }

    public enum DrawnDirection
    {
        lateral,
        depth
    }

    public enum DrawnSize
    {
        none,
        small,
        large
    }

    #endregion

    public enum ExperimentPhase
    {
        seeModel,
        drawing,
        finish
    }

    #region user interaction

    public Color currentStrokeColor;
    public float currentStrokeSize;

    #endregion

    #region log

    public XmlWriterSettings fragmentSetting;
    public FileStream logFile;

    public int widgetCount;

    public enum WidgetType
    {
        ray,
        sphere,
        text
    }

    public enum LogCategory
    {
        none,
        controlPosition,
        controlRotation,
        leftControllerPose,
        rightControllerPose,
        userPosition,
        userRotation,
        userPose,
        newStroke,
        endStroke,
        startTry,
        endTry,
        changeView,
        deviationAngleX,
        deviationAngleY,
        deviationAngleZ,
        newRotation,
        templateCreated,
        templateSet,
        templateDelete
    }
  
    #endregion

    #region legacyStuff

    public static float snappingDistanceThreshold = 0.005f;
    public static float segmentChangeDistance = 0.03f;//TUNE THIS WHEN READY!!!

    public enum geometricRelation
    {
        none,
        parallelism,
        perpendicularity,
        acute45
    }

    public enum shapeType
    {
        none,
        line,
        circle
    }

    #endregion
}
