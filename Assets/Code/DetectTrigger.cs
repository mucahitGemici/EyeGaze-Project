using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectTrigger : MonoBehaviour
{

    public GameObject cursor;
    public LineRenderer lineRenderer;

    private int lineIncrease=0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ButtonPressed()
    {
        Debug.Log("Buton bas");
    }


    public void Draw()
    {
        lineRenderer.SetPosition(lineIncrease, cursor.transform.position);
        lineIncrease++;
        
    }

}
