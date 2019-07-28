using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenPosition : Tweener
{

	[SerializeField]
	private Vector3 m_StartPosition;

	[SerializeField]
	private Vector3 m_EndPosition;

	[SerializeField]
	private bool m_LocalSpace;

	[SerializeField] private bool m_Anchored = false;


	protected override void UpdateTween ()
	{
		RectTransform rectTrans = transform as RectTransform;

		if ( rectTrans && m_Anchored )
		{
			rectTrans.anchoredPosition = Vector2.LerpUnclamped( m_StartPosition, m_EndPosition, Factor );
		}
		else
		{
			if ( m_LocalSpace )
				transform.localPosition = Vector3.LerpUnclamped( m_StartPosition, m_EndPosition, Factor );
			else
				transform.position = Vector3.LerpUnclamped( m_StartPosition, m_EndPosition, Factor );
		}
	}

	[ContextMenu( "Set Start from Current" )]
	private void SetStartFromCurrent ()
	{
		if ( m_LocalSpace )
			m_StartPosition = transform.localPosition;
		else
			m_StartPosition = transform.position;
	}

	[ContextMenu( "Set End from Current" )]
	private void SetEndFromCurrent ()
	{
		if ( m_LocalSpace )
			m_EndPosition = transform.localPosition;
		else
			m_EndPosition = transform.position;
	}



}