using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenSizeDelta : Tweener
{
	
	[SerializeField]
	private Vector2 m_StartSize;

	[SerializeField]
	private Vector2 m_EndSize;


	protected override void UpdateTween ()
	{
		Vector2 sizeDelta = ( transform as RectTransform ).sizeDelta;
		sizeDelta = Vector2.LerpUnclamped( m_StartSize, m_EndSize, Factor );
		( transform as RectTransform ).sizeDelta = sizeDelta;
	}

}
