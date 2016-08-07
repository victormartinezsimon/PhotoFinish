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

  public enum States { INITIAL, RACE, FINISH, STOP}
  private States m_state;

  public Transform finishPoint;

	// Use this for initialization
	void Start ()
  {
    ResetVariables();
    m_state = States.INITIAL;
	}

  public void ResetVariables()
  {
    _max_velocity = Random.Range(max_velocity[0], max_velocity[1]);
    _min_velocity = Random.Range(min_velocity[0], min_velocity[1]);
    aceleration = Random.Range(acelerationRange[0], acelerationRange[1]);
    velocity = Random.Range(_min_velocity, _max_velocity);
  }
	
	// Update is called once per frame
	void Update () {
    switch(m_state)
    {
      case States.RACE:
        Move();
        UpdateData();
        break;
      case States.FINISH:
        Move();
        break;
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

  public void SomeOneCrossLine()
  {
    m_state = States.FINISH;
  }

  public void SomeoneReachEndTrack()
  {
    m_state = States.STOP;
  }

  public void StartRace()
  {
    m_state = States.RACE;
  }

  public float TimeToFinish()
  {
    float a = aceleration / 2;
    float b = velocity;
    float c = -(finishPoint.position.x - transform.position.x);

    float res1 = (-b + Mathf.Sqrt((b * b)- (4 * a * c))) / (2 * a);
    float res2 = (-b - Mathf.Sqrt((b * b) - (4 * a * c))) / (2 * a);

    return Mathf.Max(res1,res2);
  }

}
