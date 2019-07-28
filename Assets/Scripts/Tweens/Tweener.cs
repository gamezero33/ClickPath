using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tweener : MonoBehaviour
{

	#region - Events -

	public delegate void OnStartedHandler ();
	public delegate void OnLoopHandler ();
	public delegate void OnChangeDirectionHandler ();
	public delegate void OnFinishedHandler ();

	public event OnStartedHandler OnStarted;
	public event OnLoopHandler OnLoop;
	public event OnChangeDirectionHandler OnChangeDirection;
	public event OnFinishedHandler OnFinished;

	#endregion


	#region - Enums -

	public enum PlayMode
	{
		Once,
		Loop,
		PingPongOnce,
		PingPongLoop
	}

	private enum State
	{
		Off,
		Idle,
		IdleR,
		Forward,
		Reverse
	}

	#endregion


	#region - Published Fields -

	[SerializeField]
	private PlayMode m_PlayMode = PlayMode.Once;
	public PlayMode playMode { get { return m_PlayMode; } set { m_PlayMode = value; } }

	[SerializeField]
	private bool m_PlayOnEnable = true;
	
	[SerializeField]
	private float m_Delay = 0.0f;
	public float delay { get { return m_Delay; } set { m_Delay = value; } }

	[SerializeField]
	protected float m_Duration = 1.0f;
    public float Duration { get { return m_Duration; } }

    [SerializeField]
	private AnimationCurve m_Curve = AnimationCurve.EaseInOut( 0.0f, 0.0f, 1.0f, 1.0f );

	[SerializeField, Range( 0.0f, 1.0f )]
	private float m_Factor;

	[SerializeField]
	private bool m_RandomStartFactor;

	[SerializeField]
	private UnityEvent m_OnFinished;

	#endregion


	#region - Properties -

	public float Factor
	{
		get
		{
			return m_Curve.Evaluate( m_Factor );
		}
	}

	#endregion


	#region - Private Members -

	private State m_State = State.Off;
	private float m_Time = 0.0f;

	protected float m_RandomTime;

	#endregion


	#region - Mono Methods -

	private void OnValidate ()
	{
		if ( enabled )
			UpdateTween();
	}

	private void OnEnable ()
	{
		if ( m_PlayOnEnable )
		{
			Play();
		}
		else
		{
			ResetToStart();
		}
	}

	private void Update ()
	{
		UpdateTime();
		UpdateTween();
	}

	#endregion


	#region - Update Methods -

	// ---------------------------------------
	protected virtual void UpdateTween () { }
	// ---------------------------------------


	private void UpdateTime ()
	{
		if ( m_State != State.Off )
		{
			m_Time += Time.deltaTime;
		}

		switch ( m_State )
		{
			// -------------------------------
			case State.Off:
				return;
			// -------------------------------

			// -------------------------------
			case State.Idle:
				if ( m_Time >= m_Delay )
				{
					m_State = State.Forward;
					m_Time = 0.0f;
				}
				break;
			// -------------------------------

			// -------------------------------
			case State.IdleR:
				if ( m_Time >= m_Delay )
				{
					m_State = State.Reverse;
					m_Time = 0.0f;
				}
				break;
			// -------------------------------

			// -------------------------------
			case State.Forward:
				if ( m_Duration > 0.0f )
				{
					m_Factor = m_Time / m_Duration;
				}

				if ( m_Time >= m_Duration )
				{
					if ( m_PlayMode == PlayMode.Once )
					{
						End();
					}
					else if ( m_PlayMode == PlayMode.Loop )
					{
						if ( OnLoop != null )
							OnLoop();
						ResetToStart();
						m_State = State.Forward;
					}
					else
					{
						ChangeDirection();
					}
				}
				break;
			// -------------------------------

			// -------------------------------
			case State.Reverse:
				if ( m_Duration > 0.0f )
				{
					m_Factor = 1.0f - ( m_Time / m_Duration );
				}
				else
				{
					m_Factor = 1.0f;
				}

				if ( m_Time >= m_Duration )
				{
					if ( m_PlayMode == PlayMode.PingPongOnce )
					{
						End();
					}
					else if ( m_PlayMode == PlayMode.PingPongLoop )
					{
						ChangeDirection();
					}
				}
				break;
				// -------------------------------
		}
	}

	#endregion


	#region - Private Methods -

	private void ChangeDirection ()
	{
		m_Time = 0.0f;
		if ( m_State == State.Forward )
		{
			m_State = State.Reverse;
		}
		else
		{
			m_State = State.Forward;
		}
		if ( OnChangeDirection != null )
			OnChangeDirection();
	}

	private void End ()
	{
		if ( m_State == State.Forward )
		{
			m_Factor = 1.0f;
		}
		else
		{
			m_Factor = 0.0f;
		}
		m_State = State.Off;
		m_OnFinished.Invoke();
		if ( OnFinished != null )
			OnFinished();
	}

	#endregion


	#region - Public Methods -

	[RuntimeButton]
	public void Play ()
	{
		ResetToStart();
		m_State = State.Idle;
		if ( OnStarted != null )
			OnStarted();
		if ( m_RandomStartFactor ) StartCoroutine( RandomiseStart() );
	}

	[RuntimeButton]
	public void PlayReverse ()
	{
		m_State = State.IdleR;
		if ( OnStarted != null )
			OnStarted();
	}

	public void ResetToStart ()
	{
		m_Factor = 0.0f;
		m_Time = 0.0f;
		UpdateTween();
		m_State = State.Off;
	}

	#endregion

	/// Bit of a hack, but it gets the job done :P
	private IEnumerator RandomiseStart ()
	{
		yield return new WaitUntil( () => m_State == State.Forward );
		//m_Time = m_RandomTime;
		m_Time = Random.Range( 0.0f, m_Duration );
	}

	public void SetDuration ( float newDuration )
	{
		m_Duration = newDuration;
	}

}