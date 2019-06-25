using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class PlayerGhost
{
    public string PlayerName = "UnknownPlayer";
    public string LevelName = "";
    public float Score = 0;
    public string Color = "Blue";
    public List<Vector3> Positions = new List<Vector3>();
};

public class BackendConnection : MonoBehaviour
{
    string ServerURL = "http://ggj.mansisaksson.com";
    int port = 80;

    string GetURL() { return ServerURL + ":" + port.ToString(); }

    void Start()
    {
        // Debug test UploadPlayerGhost

        //Debug.Log("UploadPlayerGhost...");
        //PlayerGhost FakePlayer = new PlayerGhost();
        //FakePlayer.PlayerName = "UnknownPlayer";
        //FakePlayer.LevelName = "Level1";
        //FakePlayer.Score = 0;
        //FakePlayer.Positions = new List<Vector3>() {
        //    new Vector3(),
        //    new Vector3(0, 100, 0),
        //    new Vector3(0, 100, 0),
        //    new Vector3(112, 100, 0),
        //    new Vector3(212, 100, 323),
        //    new Vector3(0, 200, 0),
        //    new Vector3(12, 300, -23),
        //    new Vector3(-12, 400, 241),
        //};
        //UploadPlayerGhost(FakePlayer, (bool success) =>
        //{
        //    if (success)
        //    {
        //        Debug.Log("Player Uploaded");

        //        Debug.Log("GetPlayerGhosts");

        //        GetGhostsParams Params = new GetGhostsParams();
        //        Params.Level = "Level1";
        //        Params.MaxAmount = 100;
        //        GetPlayerGhosts(Params, (List<PlayerGhost> Players) =>
        //        {
        //            Debug.Log("Got Players:");
        //            foreach (var player in Players)
        //            {
        //                Debug.Log(player.PlayerName);
        //            }
        //        });
        //    }
        //    else
        //    {
        //        Debug.LogError("Failed to uplad player");
        //    }
        //});


        /* Debug test GetPlayerGhosts */
        //Debug.Log("GetPlayerGhosts");
        //GetGhostsParams Params = new GetGhostsParams();
        //Params.Level = "Level1";
        //Params.MaxAmount = 100;
        //GetPlayerGhosts(Params, (List<PlayerGhost> Players) =>
        //{
        //    Debug.Log("Got Players:");
        //    foreach (var player in Players)
        //    {
        //        Debug.Log(player.PlayerName);
        //    }
        //});
    }

    [Serializable]
    public class GetPlayerGhostsResult
    {
        public PlayerGhost LastWinner;
        public PlayerGhost WinnerAllTime;
        public int ResettingIn;
        public List<PlayerGhost> Ghosts = new List<PlayerGhost>();
    };

    [Serializable]
    public class GetPlayerResponse
    {
        public bool success = false;
        public GetPlayerGhostsResult body = new GetPlayerGhostsResult();
    };

    [Serializable]
    public struct GetGhostsParams
    {
        public string Level;
        public int MaxAmount;
    };
    public void GetPlayerGhosts(GetGhostsParams Params, Action<GetPlayerGhostsResult> callback)
    {
        string URL = GetURL() + "/requests/get_all_player_ghosts.php";
        string json = JsonUtility.ToJson(Params, true);
        StartCoroutine(SendGETRequest(URL, json, (string Result) =>
        {
            GetPlayerResponse Response = JsonUtility.FromJson<GetPlayerResponse>(Result);
            callback(Response != null ? Response.body : new GetPlayerGhostsResult());
        }));
    }

    [Serializable]
    public class UploadPlayerGhostResponse
    {
        public bool success = false;
    };
    public void UploadPlayerGhost(PlayerGhost PlayerGhost, Action<bool> callback)
    {
        string URL = GetURL() + "/requests/upload_player_ghost.php";
        string json = JsonUtility.ToJson(PlayerGhost, true);
        StartCoroutine(SendPOSTRequest(URL, json, (string Result) =>
        {
            UploadPlayerGhostResponse Response = JsonUtility.FromJson<UploadPlayerGhostResponse>(Result);
            callback(Response != null ? Response.success : false);
        }));
    }

    private IEnumerator SendGETRequest(string URL, string body, Action<string> callback)
    {
        string address = string.Format(URL+"?body={0}", Uri.EscapeDataString(body));
        Debug.Log("Sending GET Request: " + address);
        UnityWebRequest www = UnityWebRequest.Get(address);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            //Debug.Log("GET Response: " + www.downloadHandler.text);
            callback(www.downloadHandler.text);
        }
    }

    private IEnumerator SendPOSTRequest(string URL, string body, Action<string> callback)
    {
        Debug.Log("Sending POST Request: " + URL);
        //Debug.Log(body);
        WWWForm form = new WWWForm();
        form.AddField("body", body);

        UnityWebRequest www = UnityWebRequest.Post(URL, form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            //Debug.Log("POST Response: " + www.downloadHandler.text);
            callback(www.downloadHandler.text);
        }
    }
}
