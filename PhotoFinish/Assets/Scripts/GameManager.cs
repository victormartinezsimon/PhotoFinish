using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

  public GameObject[] triggers;

  public Movement[] runners;
  private Vector3[] initialPositions;

  public float timeBetweenTouch = 0.5f;
  private float timeAcum = 0;

  private long _timeStart;
  private long _timeClick;
  private long _timeShouldBe;

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

  [Header("Camera Photos")]
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
    ChangeState(States.INITIAL);
    ResetWRRecord();
    ShowRecord();
  }
  void Update()
  {
    timeAcum += Time.deltaTime;
    if ( Input.GetMouseButtonDown(0) && timeAcum >= timeBetweenTouch)
    {
      Debug.Log("click in => " + timeAcum);
      timeAcum = 0;
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

   
  }

  #region callbacks
  public void SomeOneCrossLine(bool fromTrigger)
  {
    for (int i = 0; i < runners.Length; i++)
    {
      runners[i].SomeOneCrossLine();
    }
    ChangeState(States.FINISH_CROSSED);
    _timeShouldBe = DateTime.Now.Ticks;
  }

  public void SomeoneReachEndTrack(bool fromTrigger)
  {
    for (int i = 0; i < runners.Length; i++)
    {
      runners[i].SomeoneReachEndTrack();
    }
    if(fromTrigger)
    {
      _timeClick = DateTime.Now.Ticks;
      ShowResults();
      ChangeState(States.END_RACE);
    }
  }
  #endregion
  #region states
  private void ManageInitialState()
  {
    ChangeState(States.RUN);
    PlayShotSound();
    RestartGame();
  }

  private void ManageRun()
  {
    _timeClick = DateTime.Now.Ticks;
    PlayCameraSound();
    CalculateTimeToFinish();
    ShowResults();
    SomeoneReachEndTrack(false);
    ChangeState(States.END_RACE);
  }
  
  private void ManageFinishCrossed()
  {
    _timeClick = DateTime.Now.Ticks;
    PlayCameraSound();
    ShowResults();
    SomeoneReachEndTrack(false);
    ChangeState(States.END_RACE);
  }

  private void ManageEndRace()
  {
    ChangeState(States.INITIAL);
    ShowRecord();
    ResetPositions();
  }

  private void ChangeState(States newState)
  {
    switch(newState)
    {
      case States.END_RACE: AllToIdle(m_state); break;
      case States.INITIAL: AllToIdle(m_state); break;
      case States.RUN: AllToRun(m_state); break;
    }

    m_state = newState;

  }
  #endregion
  #region finish
  void ShowResults()
  {
    float diff = Mathf.Round(_timeShouldBe - _timeClick) / 1000000f;
    SetTextNow(formatText(diff));
    SaveWR(diff);
  }
  void CalculateTimeToFinish()
  {
    float min =  199;
    for(int i = 0; i < runners.Length; i++)
    {
      float res = runners[i].TimeToFinish();
      min  = Mathf.Min(min, runners[i].TimeToFinish());
    }
    _timeShouldBe = DateTime.Now.AddSeconds(min).Ticks;
  }
  #endregion
  #region restart
  private void RestartGame()
  {
    UnityEngine.Random.seed = (int)System.DateTime.Now.Ticks;
    _timeStart = DateTime.Now.Ticks;
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

  public void DeleteWR()
  {
    PlayerPrefs.SetFloat(PlayerPrefKey, 10);
    ShowRecord();
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
  #region animations
  private void AllToRun(States oldState)
  {
    if(oldState == States.RUN || oldState == States.FINISH_CROSSED)
    {
      return;
    }

    for (int i = 0; i < runners.Length; i++)
    {
      runners[i].GetComponent<Animator>().SetTrigger("RUN");
    }
  }

  private void AllToIdle(States oldState)
  {
    if (oldState == States.INITIAL || oldState == States.END_RACE)
    {
      return;
    }

    for (int i = 0; i < runners.Length; i++)
    {
      runners[i].GetComponent<Animator>().SetTrigger("STOP");
    }
  }
  #endregion
}
