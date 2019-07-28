using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPFX : MonoBehaviour
{
	
	[SerializeField] private ParticleSystem m_ParticleSystem;


	public void Trigger ()
	{
		m_ParticleSystem.Play();
	}


}
