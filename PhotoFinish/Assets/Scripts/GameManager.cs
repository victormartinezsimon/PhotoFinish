using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

  public GameObject[] triggers;

  public Movement[] runners;
  private Vector3[] initialPositions;

  private DateTime _timeStart;
  private DateTime _timeClick;
  private DateTime _timeShouldBe;

  private enum States { INITIAL, RUN, FINISH_CROSSED, END_RACE};
  private States m_state;

  [Header("UI")]
  public Text _wrText;
  public Text _nowText;
  public Image _flash;

  private string PlayerPrefKey = "Record";

  [Header("Sound")]
  public AudioClip shotSound;
  public AudioClip cameraSound;
  public AudioSource audioSource;

  [Header("Camera")]
  public float timeCamera = 0.1f;
  public float timeLapse = 0.005f;
  public float maxValueCamera =  75f;


  void Awake()
  {
    initialPositions = new Vector3[runners.Length];
    for(int i = 0; i < runners.Length; i++)
    {
      initialPositions[i] = runners[i].transform.position;
    }
    m_state = States.INITIAL;
    ResetWRRecord();
    ShowRecord();
  }
  void Update()
  {
    switch (m_state)
    {
      case States.INITIAL:
        ManageInitialState();
        break;
      case States.RUN:
        ManageRun();
        break;
      case States.FINISH_CROSSED:
        ManageFinishCrossed();
        break;
      case States.END_RACE:
        ManageEndRace();
        break;
    }
  }

  #region callbacks
  public void SomeOneCrossLine(bool fromTrigger)
  {
    for (int i = 0; i < runners.Length; i++)
    {
      runners[i].SomeOneCrossLine();
    }
    m_state = States.FINISH_CROSSED;
    _timeShouldBe = DateTime.Now;
  }

  public void SomeoneReachEndTrack(bool fromTrigger)
  {
    for (int i = 0; i < runners.Length; i++)
    {
      runners[i].SomeoneReachEndTrack();
    }
    if(fromTrigger)
    {
      _timeClick = DateTime.Now;
      ShowResults();
      m_state = States.END_RACE;
    }
  }
  #endregion
  #region states
  private void ManageInitialState()
  {
    if(Input.touchCount == 1 || Input.GetMouseButtonDown(0))
    {
      m_state = States.RUN;
      PlayShotSound();
      RestartGame();
    }
  }

  private void ManageRun()
  {
    if (Input.touchCount == 1 || Input.GetMouseButtonDown(0))
    {
      _timeClick = DateTime.Now;
      PlayCameraSound();
      CalculateTimeToFinish();
      ShowResults();
      SomeoneReachEndTrack(false);
      m_state = States.END_RACE;
    }
  }
  
  private void ManageFinishCrossed()
  {
    if (Input.touchCount == 1 || Input.GetMouseButtonDown(0))
    {
      _timeClick = DateTime.Now;
      PlayCameraSound();
      ShowResults();
      SomeoneReachEndTrack(false);
      m_state = States.END_RACE;
    }
  }

  private void ManageEndRace()
  {
    if (Input.touchCount == 1 || Input.GetMouseButtonDown(0))
    {
      m_state = States.INITIAL;
      ShowRecord();
      ResetPositions();
    }
  }
  #endregion
  #region finish
  void ShowResults()
  {
    float diff = Mathf.Round(_timeShouldBe.Ticks - _timeClick.Ticks) / 1000000f;
    SetTextNow(formatText(diff));
    SaveWR(diff);
  }
  void CalculateTimeToFinish()
  {
    _timeShouldBe = DateTime.Now.AddSeconds(1);
  }
  #endregion
  #region restart
  private void RestartGame()
  {
    UnityEngine.Random.seed = (int)System.DateTime.Now.Ticks;
    _timeStart = DateTime.Now;
    _timeClick = _timeStart;
    _timeShouldBe = _timeStart;

    ResetPositions();
    _nowText.text = "";

    for (int i = 0; i < runners.Length; i++)
    {
      runners[i].StartRace();
    }

    for (int i = 0; i < triggers.Length; i++)
    {
      triggers[i].SetActive(true);
    }
  }
  private void ResetPositions()
  {
    for (int i = 0; i < runners.Length; i++)
    {
      runners[i].transform.position = initialPositions[i];
      runners[i].ResetVariables();
    }
  }
  #endregion
  #region text
  public string formatText(float value)
  {
    return value.ToString("F3");
  }

  private void ShowRecord()
  {
    float value = PlayerPrefs.GetFloat(PlayerPrefKey);
    SetTextWR(formatText(value));
  }
  private void SaveWR(float time)
  {
    float value = PlayerPrefs.GetFloat(PlayerPrefKey);
    if(Mathf.Abs(time) < Mathf.Abs(value))
    {
      PlayerPrefs.SetFloat(PlayerPrefKey, time);
      SetTextWR(formatText(time));
    }
  }

  private void ResetWRRecord()
  {
    if(!PlayerPrefs.HasKey(PlayerPrefKey))
    {
      PlayerPrefs.SetFloat(PlayerPrefKey, 10);
    }
  }

  private void SetTextWR(string txt)
  {
    _wrText.text = "WR: " +txt + " ms";
  }

  private void SetTextNow(string txt)
  {
    _nowText.text =  txt + " ms";
  }

  #endregion
  #region sounds
  private void PlayShotSound()
  {
    audioSource.PlayOneShot(shotSound);
  }
  private void PlayCameraSound()
  {
    audioSource.PlayOneShot(cameraSound);
    StartCoroutine(ShowFlash(timeCamera));
  }
  #endregion
  #region flash
  private IEnumerator ShowFlash(float timePingPong)
  {
    float timeAcum = 0;
    float timeStep =timeLapse;
    while(timeAcum < timePingPong)
    {
      Color c = _flash.color;
      c.a = Mathf.Lerp(0, maxValueCamera / 255f, (timeAcum / timePingPong));
      _flash.color = c;
      yield return new WaitForSeconds(timeStep);
      timeAcum += timeStep;
    }
    yield return new WaitForSeconds(timeStep);
    timeAcum = 0;
    while (timeAcum < timePingPong)
    {
      Color c = _flash.color;
      c.a = Mathf.Lerp(maxValueCamera / 255f, 0, (timeAcum / timePingPong));
      _flash.color = c;
      yield return new WaitForSeconds(timeStep);
      timeAcum += timeStep;
    }
  }
  #endregion
}
