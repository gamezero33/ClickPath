using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenSkyboxRotate : Tweener
{

	[SerializeField] private Material m_SkyboxMaterial;

	[SerializeField]
	private float m_StartAngle = 0;

	[SerializeField]
	private float m_EndAngle = 0;


	protected override void UpdateTween ()
	{
		m_SkyboxMaterial.SetFloat( "_Rotation", Mathf.LerpUnclamped( m_StartAngle, m_EndAngle, Factor ) );
	}

}