using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenScale : Tweener
{

	[SerializeField]
	private Vector3 m_StartScale = Vector3.one;

	[SerializeField]
	private Vector3 m_EndScale = Vector3.one;


	private void Awake ()
	{
		m_RandomTime = Random.Range( 0.0f, m_Duration );
	}

	public Vector3 GetValue ()
	{
		return Vector3.LerpUnclamped( m_StartScale, m_EndScale, 1f / m_Duration * m_RandomTime );
	}

	protected override void UpdateTween ()
	{
		transform.localScale = Vector3.LerpUnclamped( m_StartScale, m_EndScale, Factor );
	}

}