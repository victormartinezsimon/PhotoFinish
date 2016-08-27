using UnityEngine;
using System.Collections;

public class AdjustCamera : MonoBehaviour {

  public GameObject track;

	// Use this for initialization
	void Start () {
    Camera c = Camera.main;
    float size = c.orthographicSize;
    Vector3 bottomLeftCamera = Camera.main.ScreenToWorldPoint(new Vector3(0,0, c.nearClipPlane));
    float leftTrack = track.transform.position.x - track.GetComponent<Renderer>().bounds.size.x/2;
    float newSize = leftTrack * size / bottomLeftCamera.x;
    c.orthographicSize = newSize;
	}
	

}
