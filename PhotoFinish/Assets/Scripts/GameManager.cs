using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

  public Movement[] runners;
  private bool _photoFinish = false;

  void Awake()
  {
    Random.seed = (int)System.DateTime.Now.Ticks;
  }

  public void SetEndRace()
  {
    for(int i = 0; i < runners.Length; i++)
    {
      runners[i].SetEndRace();
    }
  }

  public void SetFinishRace()
  {
    for (int i = 0; i < runners.Length; i++)
    {
      runners[i].SetFinishRace();
    }
  }

  void Update()
  {
    if(_photoFinish)
    {
      return;
    }
    if(Input.touchCount == 1 || Input.GetMouseButtonDown(0))
    {
      _photoFinish = true;
      SetEndRace();
      ShowResults();
    }
  }

  void ShowResults()
  {

  }
}
