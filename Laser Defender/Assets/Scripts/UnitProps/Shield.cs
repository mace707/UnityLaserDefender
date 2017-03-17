using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IShield
{
	void Activate(Transform parent);
	void Deactivate();
	bool IsActive();
	void OnTriggerEnter2D(Collider2D col);
}
