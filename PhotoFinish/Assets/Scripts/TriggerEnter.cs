using UnityEngine;
using System.Collections;

public class TriggerEnter : MonoBehaviour {

  public GameManager _manager;
  public string nameMethod;

	void OnTriggerEnter(Collider other)
  {
    this.gameObject.SetActive(false);
    _manager.SendMessage(nameMethod);
  }
}
