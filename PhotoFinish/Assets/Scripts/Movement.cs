using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

  public float[] max_velocity;
  private float _max_velocity;
  public float[] min_velocity;
  private float _min_velocity;
  public float[] acelerationRange;
  private float aceleration;
  private float velocity;

  public Vector3 direction;

  private bool _endRace = false;
  private bool _finishRace = false;

	// Use this for initialization
	void Start () {
   
    _max_velocity = Random.Range(max_velocity[0], max_velocity[1]);
    _min_velocity = Random.Range(min_velocity[0], min_velocity[1]);
    aceleration = Random.Range(acelerationRange[0], acelerationRange[1]);
    velocity = Random.Range(_min_velocity, _max_velocity);
	}
	
	// Update is called once per frame
	void Update () {
    if(!_endRace)
    {
      Move();
      if (!_finishRace)
      {
        UpdateData();
      }
    }
  }

  private void Move()
  {
    transform.position += direction * Time.deltaTime * velocity;
  }

  private void UpdateData()
  {
    velocity += aceleration * Time.deltaTime;
    velocity = Mathf.Max(velocity, _min_velocity);
    velocity = Mathf.Min(velocity, _max_velocity);
  }

  public void SetEndRace()
  {
    _endRace = true;
  }

  public void SetFinishRace()
  {
    _finishRace = true;
  }
}
