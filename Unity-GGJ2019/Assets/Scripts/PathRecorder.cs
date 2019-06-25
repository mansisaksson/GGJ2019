using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathRecorder : MonoBehaviour
{
    //Distance until we record a new location (if direction have changed)
    //will also record a location each time planar direction changes
    public float MaxDistance = 10;

    //Minimum distance between path points
    public float MinDistance = 1;

    public LineRenderer MyLineRenderer;
    public Toggle orange;
    public Toggle blue;

    public List<Vector3> Path { get; } = new List<Vector3>();
   

    // Start is called before the first frame update
    void Start()
    {
        Path.Add(gameObject.transform.position);
        //print(Path[Path.Count - 1].ToString("F5"));
        orange = GameObject.Find("Orange").GetComponent<Toggle>();
        blue = GameObject.Find("Blue").GetComponent<Toggle>();

        InvokeWhen(() => orange.gameObject.activeInHierarchy == false,
            () =>
            {
                Color color = orange.isOn ? orange.targetGraphic.color : blue.targetGraphic.color;
                MyLineRenderer.startColor = color;
                MyLineRenderer.endColor = color;
            });
       
    }


    void FixedUpdate()
    {

        if (Path.Count == 0) return;

        Vector3 PrevPos = Path[Path.Count-1];
        Vector3 Pos = gameObject.transform.position;
        Vector3 Dir = gameObject.GetComponent<Rigidbody>().velocity;
        Vector3 DistToLastPathPoint = Pos - PrevPos;


        if (gameObject.GetComponent<Rigidbody>().velocity.magnitude < 0.1f && DistToLastPathPoint.magnitude > 0.1f)
        {
            Path.Add(Pos);
            MyLineRenderer.positionCount = Path.Count;
            MyLineRenderer.SetPositions(Path.ToArray());
            //print(Path[Path.Count - 1].ToString("F5") + " Has stopped");
            return;
        }

      
        if (Pos.y < -1)
        {
            Path.Add(Pos);
            MyLineRenderer.positionCount = Path.Count;
            MyLineRenderer.SetPositions(Path.ToArray());
            enabled = false;
            return;
        }


        if (DistToLastPathPoint.magnitude < MinDistance) return;

        if (Path.Count == 1) //Always add 2nd point (if we have moved)
        {
            if (DistToLastPathPoint.magnitude > 0)
            {
                Path.Add(Pos);
                MyLineRenderer.positionCount = Path.Count;
                MyLineRenderer.SetPositions(Path.ToArray());
                //print(Path[Path.Count - 1].ToString("F5") + "Pos 2");
            }
            return;
        }

        Vector3 PrevDir = PrevPos - Path[Path.Count - 2];
        Vector3 PrevPlanarDir = PrevDir;
        Vector3 PlanarDir = Dir;
        PrevPlanarDir.y = 0;
        PlanarDir.y = 0;

        
        //print(Vector3.Angle(Vector3.one, PrevPlanarDir).ToString("F5") + " PrevPlanarDir.angle");


        //If we aren't pointing in the same planar direction (we bounced on a wall or something)
        if (Vector3.Angle(PlanarDir.normalized, PrevPlanarDir.normalized) > 2)
        {
            Path.Add(Pos);
            MyLineRenderer.positionCount = Path.Count;
            MyLineRenderer.SetPositions(Path.ToArray());
            //print(Vector3.Angle(PlanarDir.normalized, PrevPlanarDir.normalized).ToString("F5") + "New Planar Direction");
            return;
        }


        //If long enough distance between points and direction changed
        if (DistToLastPathPoint.magnitude > MaxDistance && Vector3.Angle(Dir.normalized, PrevDir.normalized) > 2)
        {
            Path.Add(Pos);
            MyLineRenderer.positionCount = Path.Count;
            MyLineRenderer.SetPositions(Path.ToArray());
            //print(Path[Path.Count - 1].ToString("F5") + "Flying?");
            return;
        }

       
    }


    public void ClearPath() { Path.Clear(); }

    public void InvokeWhen(Func<bool> condition, Action func)
    {
        StartCoroutine(inner_InvokeWhen(condition, func));
    }

    private IEnumerator inner_InvokeWhen(Func<bool> condition, Action func)
    {
        while (condition() == false)
        {
            yield return null;
        }

        func();
    }

}