using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour {

  public Movement[] runners;
  private bool _photoFinish = false;

  private DateTime _timeStart;
  private DateTime _timeClick;

  void Awake()
  {
    UnityEngine.Random.seed = (int)System.DateTime.Now.Ticks;
  }

  public void SetEndRace()
  {
    for(int i = 0; i < runners.Length; i++)
    {
      runners[i].SetEndRace();
    }

    if(_timeStart == _timeClick)
    {
      _timeClick = DateTime.Now;
    }
  }

  public void SetFinishRace()
  {
    for (int i = 0; i < runners.Length; i++)
    {
      runners[i].SetFinishRace();
    }
    _timeClick = DateTime.Now;
  }

  void Update()
  {
    if(_photoFinish)
    {
      return;
    }
    if(Input.touchCount == 1 || Input.GetMouseButtonDown(0))
    {
      SetEndRace();
      ShowResults();
      _photoFinish = true;
    }
  }

  void ShowResults()
  {

  }

  private void RestartGame()
  {
    _timeStart = DateTime.Now;
    _timeClick = _timeStart;
  }
}
