using UnityEngine;
using System.Collections;

public class SetColor : MonoBehaviour {

  public Color _color;

	// Use this for initialization
	void Start () {
    GetComponent<Renderer>().material.color = _color;
	}
}
