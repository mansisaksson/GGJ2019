using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMode : MonoBehaviour
{
    public PathRecorder PlayerPath;
    public BackendConnection Backend;
    public GameObject LineRenderObject;

    public InputField PlayerName;
    public float Score = 0;
    public Toggle orange;
    public Toggle blue;

    void Start()
    {
        DrawLinesForCurrentScene();
        PlayerName = GameObject.Find("InputField").GetComponent<InputField>();
        orange = GameObject.Find("Orange").GetComponent<Toggle>();
        blue = GameObject.Find("Blue").GetComponent<Toggle>();
    }
    
    public void UploadPlayerData()
    {
        PlayerGhost PlayerData = new PlayerGhost();
        PlayerData.PlayerName = PlayerName.text;
        PlayerData.LevelName = SceneManager.GetActiveScene().name;
        PlayerData.Score = Score;
        PlayerData.Color = orange.isOn ? "Red" : "Blue";
        PlayerData.Positions = PlayerPath.Path;

        Backend.UploadPlayerGhost(PlayerData, (bool Success) => {
            //Stuff
        });
    }

    public void DrawLinesForCurrentScene()
    {
        var Params = new BackendConnection.GetGhostsParams()
        {
            Level = SceneManager.GetActiveScene().name,
            MaxAmount = 50
        };

        Backend.GetPlayerGhosts(Params, (BackendConnection.GetPlayerGhostsResult result) => 
        {
            //Clear previous LineRender Components
            foreach (Transform child in transform)
            {
                if (child.gameObject.name != "LineRenderer")
                {
                    GameObject.Destroy(child.gameObject);
                }
            }

            for (int i = 0; i < result.Ghosts.Count; i++)
            {
                List<Vector3> Positions = result.Ghosts[i].Positions;
                //Add new LineRender Components
                for (int j = 0; j < Positions.Count; j++)
                {
                    GameObject obj = Instantiate(LineRenderObject, transform);

                    LineRenderer lr = obj.GetComponent<LineRenderer>();

                    Color color = result.Ghosts[i].Color == "Blue" ? blue.targetGraphic.color : orange.targetGraphic.color;
                    lr.startColor = color;
                    lr.endColor = color;
                    lr.startWidth = 0.1f;
                    lr.endWidth = 0.1f;

                    lr.positionCount = Positions.Count;
                    lr.SetPositions(Positions.ToArray());
                }
            }
            InvokeWhen(() => orange.gameObject.activeInHierarchy == false,
             () =>
             {
                 SetScoreBoardText(result);
             }); 
        });
    }

    BackendConnection.GetPlayerGhostsResult cachedResults;
    public void SetScoreBoardText(BackendConnection.GetPlayerGhostsResult result = null)
    {
        if(result != null)
            cachedResults = result;

        if (cachedResults == null) return;

        Text text = GameObject.Find("Scores").GetComponent<Text>();

        string str = string.Empty;

        int index = 0;
        string colorCode;

        foreach (var ghost in cachedResults.Ghosts.Take(3))
        {
            index++;
            colorCode = ghost.Color == "Red" ? "#FC910D" : "#239CD3";
            str += $"<color={colorCode}>{index}. {ghost.PlayerName} {ghost.Score.ToString("F0")} </color>\n";
        }
        colorCode = orange.isOn ? "#FC910D" : "#239CD3";
        str += $"<color={colorCode}>X. {PlayerName.text} {Score.ToString("F0")} </color>\n\n";

        colorCode = cachedResults.LastWinner.Color == "Red" ? "#FC910D" : "#239CD3";
        str += $"Last Winner\n<color={colorCode}>{cachedResults.LastWinner.PlayerName} {cachedResults.LastWinner.Score.ToString("F0")} </color>\n";

        colorCode = cachedResults.WinnerAllTime.Color == "Red" ? "#FC910D" : "#239CD3";
        str += $"Winner All Time\n<color={colorCode}>{cachedResults.WinnerAllTime.PlayerName} {cachedResults.WinnerAllTime.Score.ToString("F0")} </color>\n";
        text.text = str;

        Text timerText = GameObject.Find("Timer").GetComponent<Text>();
        timerText.text = $"Resetting in\n{Mathf.Max(1, cachedResults.ResettingIn / 60).ToString()} minutes";
    }

    void SetLinesToDraw(List<PlayerGhost> Ghosts)
    {
       

        
    }

    public void InvokeDelayed(float delayTime, Action func)
    {
        StartCoroutine(inner_InvokeDelayed(delayTime, func));
    }

    private IEnumerator inner_InvokeDelayed(float delayTime, Action func)
    {
        float timerElapsed = 0;
        while (timerElapsed < delayTime)
        {
            timerElapsed += Time.deltaTime;
            yield return null;
        }

        func();
    }

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
