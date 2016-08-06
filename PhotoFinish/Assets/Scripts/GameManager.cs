using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour {

  public GameObject[] triggers;

  public Movement[] runners;
  private Vector3[] initialPositions;
  private bool _photoFinish = false;

  private DateTime _timeStart;
  private DateTime _timeClick;
  private DateTime _timeShouldBe;

  private enum States { INITIAL, RUN, FINISH_CROSSED, END_RACE};
  private States m_state;

  void Awake()
  {
    initialPositions = new Vector3[runners.Length];
    for(int i = 0; i < runners.Length; i++)
    {
      initialPositions[i] = runners[i].transform.position;
    }
    m_state = States.INITIAL;
  }

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

  void Update()
  {
    switch(m_state)
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

  private void ManageInitialState()
  {
    if(Input.touchCount == 1 || Input.GetMouseButtonDown(0))
    {
      m_state = States.RUN;
      RestartGame();
    }
  }

  private void ManageRun()
  {
    if (Input.touchCount == 1 || Input.GetMouseButtonDown(0))
    {
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
      ResetPositions();
    }
  }


  void ShowResults()
  {
    Debug.Log("show results");
  }

  void CalculateTimeToFinish()
  {
    Debug.Log("calculate time to finsh");
  }

  private void RestartGame()
  {
    UnityEngine.Random.seed = (int)System.DateTime.Now.Ticks;
    _timeStart = DateTime.Now;
    _timeClick = _timeStart;
    _timeShouldBe = _timeStart;

    ResetPositions();

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
}
