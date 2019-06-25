using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int numberOfHitsLeft = 7;
    private GameObject fullscreenButton;
    public GameObject golfClub;
    public GameObject playerModel;
    public GameObject playerAnchor;
    private Rigidbody playerRigidBody;
    private GameObject home;
    private GameObject restartButton;
    private GameObject EndGUI;
    //private Text scoreTextBox;
    private GameMode gameMode;
    void Awake()
    {
        gameMode = GameObject.Find("GameMode").GetComponent<GameMode>();
        home = GameObject.Find("Home");
        fullscreenButton = GameObject.Find("fullscreenButton");
        restartButton = GameObject.Find("RestartButton");
        restartButton.SetActive(false);
        EndGUI = GameObject.Find("EndGUI");
      //  scoreTextBox = EndGUI.transform.Find("ScoreText").GetComponent<Text>();
        EndGUI.SetActive(false);

        playerRigidBody = playerModel.GetComponent<Rigidbody>();
        initEvents();
    }
    public void initEvents()
    {
        EventTrigger trigger = fullscreenButton.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();

        // default drag operations
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((data) => { PointerDown((PointerEventData)data); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drag;
        entry.callback.AddListener((data) => { PointerDrag((PointerEventData)data); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener((data) => { PointerUp((PointerEventData)data); });
        trigger.triggers.Add(entry);

    }

    private float bestDistanceToHome = float.MaxValue;

    void Update()
    {
        playerAnchor.transform.position = new Vector3(playerRigidBody.position.x, playerAnchor.transform.position.y, playerRigidBody.position.z);

        if (isDying == false && playerModel.transform.position.y < -1)
        {
            isDying = true;
            StartCoroutine(DeathTimer(0.5f));
        }

        if(startedMovingAfterHit == true && playerIsMoving == false && isDying == false)
        {
            float currentDistanceToHome = (home.transform.position - playerModel.transform.position).magnitude;
            if(currentDistanceToHome < bestDistanceToHome)
            {
                bestDistanceToHome = currentDistanceToHome;
                gameMode.Score = 200-bestDistanceToHome;
                gameMode.SetScoreBoardText();
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 planarVelocity = new Vector2(playerRigidBody.velocity.x, playerRigidBody.velocity.z);
        playerIsMoving = true;
        if (planarVelocity.magnitude < 0.01f)
        {
            playerIsMoving = false;


            var text = GameObject.Find("ShotsLeftText")?.GetComponent<Text>();
            if(text != null) text.text = "Shots Left\n" + numberOfHitsLeft;

            playerRigidBody.velocity = new Vector3(0, playerRigidBody.velocity.y, 0);
        }
        else if (planarVelocity.magnitude < 2)
        {
            startedMovingAfterHit = true;
            planarVelocity = planarVelocity * 0.92f;
            playerRigidBody.velocity = new Vector3(planarVelocity[0], playerRigidBody.velocity.y, planarVelocity[1]);

        }
        else if (planarVelocity.magnitude < 3.3f)
        {
            startedMovingAfterHit = true;
            planarVelocity = planarVelocity * 0.999f;
            playerRigidBody.velocity = new Vector3(planarVelocity[0], playerRigidBody.velocity.y, planarVelocity[1]);

        }

    }
    private bool startedMovingAfterHit = false;

    public void EndOfGame()
    {
        gameMode.UploadPlayerData();

        canMovePlayer = false;
        isDying = true;

        EndGUI.SetActive(true);
        var fade = EndGUI.transform.Find("Fade").GetComponent<Image>();
        //scoreTextBox.enabled = false;
        FadeColor(2f, fade.color * new Color(1, 1, 1, 0), fade.color, fade, () =>
        {
            restartButton.SetActive(true);
          //  scoreTextBox.enabled = true;
          //  scoreTextBox.text = $"Distance to home : {Mathf.FloorToInt(bestDistanceToHome)}";
        });
    }
    bool isDying = false;
    IEnumerator DeathTimer(float deathTime)
    {
        float timerElapsed = 0;

        while (timerElapsed < deathTime)
        {
            timerElapsed += Time.deltaTime;
            yield return null;
        }

        EndOfGame();
    }

    bool canMovePlayer = false;
    public void PointerDown(PointerEventData data)
    {
        if (isDying) return;

        Ray ray = Camera.main.ScreenPointToRay(data.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, 1 << LayerMask.NameToLayer("Player")))
        {
            canMovePlayer = (hit.transform.name == "PlayerModel");
            canMovePlayer &= playerRigidBody.velocity.magnitude < 0.1f;

            if (canMovePlayer)
            {
                if(numberOfHitsLeft == 1)
                {
                    // dostuff
                    golfClub.GetComponent<Renderer>().material.color = Color.red;
                }
                golfClub.GetComponent<Renderer>().enabled = true;
                golfClub.transform.position = playerModel.transform.position;
            }
        }
    }
    
    public bool playerIsMoving = false;
    public void PointerDrag(PointerEventData data)
    {
        if (isDying) return;
        if (canMovePlayer == false) return;

        Ray ray = Camera.main.ScreenPointToRay(data.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, 1 << LayerMask.NameToLayer("ScreenRay")))
        {
            Transform playerTransform = transform.Find("PlayerModel");
            golfClub.transform.position = hit.point;
            golfClub.transform.position = new Vector3(golfClub.transform.position.x, playerTransform.position.y, golfClub.transform.position.z);
            golfClub.transform.LookAt(playerTransform);
            golfClub.transform.Rotate(Vector3.right, 90.0f);
        }
    }
    public float  forcePower = 10f;
    public void PointerUp(PointerEventData data)
    {
        if (isDying) return;
        if (canMovePlayer)
        {
            canMovePlayer = false;
            numberOfHitsLeft--;
            startedMovingAfterHit = false;
            if (numberOfHitsLeft == 0)
            {
                    InvokeWhen(
                    () =>
                    {
                        return startedMovingAfterHit && playerIsMoving == false;
                    },
                    () =>
                    {
                        if(isDying == false)
                        EndOfGame();
                    }
                );
            }
            golfClub.GetComponent<Renderer>().enabled = false;
            playerRigidBody.AddForce((playerModel.transform.position - golfClub.transform.position) * forcePower);
        }
    }

    public void InvokeDelayed(float delayTime, Action func)
    {
        StartCoroutine(inner_InvokeDelayed(delayTime, func));
    }

    public void InvokeWhen(Func<bool> condition, Action func)
    {
        StartCoroutine(inner_InvokeWhen(condition, func));
    }

    public void FadeColor(float fadeTime, Color startColor, Color endColor, Material mat, Action onComplete = null)
    {
        StartCoroutine(inner_FadeColor(fadeTime, startColor, endColor, mat, onComplete));
    }

    public void FadeColor(float fadeTime, Color startColor, Color endColor, Image img, Action onComplete = null)
    {
        StartCoroutine(inner_FadeColor(fadeTime, startColor, endColor, img, onComplete));
    }

    private IEnumerator inner_FadeColor(float fadeTime, Color startColor, Color endColor, Material mat, Action onComplete = null)
    {
        float timerElapsed = 0;
        float lerpVal = 0;
        while (timerElapsed < fadeTime)
        {
            lerpVal = Mathf.InverseLerp(0, fadeTime, timerElapsed);
            if (lerpVal >= 1) break;

            timerElapsed += Time.deltaTime;
            mat.color = Color.Lerp(startColor, endColor, lerpVal);
            yield return null;
        }

        mat.color = endColor;
        onComplete?.Invoke();
    }
    private IEnumerator inner_FadeColor(float fadeTime, Color startColor, Color endColor, Image img, Action onComplete = null)
    {
        float timerElapsed = 0;
        float lerpVal = 0;
        while (timerElapsed < fadeTime)
        {
            lerpVal = Mathf.InverseLerp(0, fadeTime, timerElapsed);
            if (lerpVal >= 1) break;

            timerElapsed += Time.deltaTime;
            img.color = Color.Lerp(startColor, endColor, lerpVal);
            yield return null;
        }

        img.color = endColor;
        onComplete?.Invoke();
    }

    private IEnumerator inner_InvokeWhen(Func<bool> condition, Action func)
    {
        while (condition() == false)
        {
            yield return null;
        }

        func();
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
}
