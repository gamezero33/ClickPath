using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenRotate : Tweener
{

	[SerializeField]
	private Vector3 m_StartEulers = Vector3.zero;

	[SerializeField]
	private Vector3 m_EndEulers = Vector3.zero;


	protected override void UpdateTween ()
	{
		transform.localEulerAngles = Vector3.LerpUnclamped( m_StartEulers, m_EndEulers, Factor );
	}

}