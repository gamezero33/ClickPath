using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

	[SerializeField] private HexTileGrid m_Grid = default;
	[SerializeField] private Camera m_Camera = default;
	[SerializeField] private Vector2 m_DeadZone = Vector2.one * 0.1f;
	[SerializeField] private Vector3Int m_Coords = Vector3Int.zero;
	[SerializeField] private float m_JumpSpeed = 1;
	[SerializeField] private AnimationCurve m_JumpSpeedCurve = AnimationCurve.EaseInOut( 0, 0, 1, 1 );
	[SerializeField] private Animator m_Animator;
	[SerializeField] private GameObject m_ConfettiObject;
	[SerializeField] private GameObject m_EmoteObject;
	[SerializeField] private float m_SwipeThreshold = 0.1f;

	private Vector3 m_SwipeStart = Vector3.zero;
	private bool m_Moving = false;

	private bool m_Active = true;
	private bool m_SwipeStarted = false;
	private bool m_Swiping = false;
	public bool Swiping { get { return m_Swiping; } }

	private void Update ()
	{
		if ( !m_Active ) return;
		if ( Input.GetMouseButtonDown( 0 ) )
		{
			m_SwipeStart = m_Camera.ScreenToViewportPoint( Input.mousePosition );
			m_SwipeStarted = true;
		}

		if ( Input.GetMouseButtonUp( 0 ) )
		{
			m_Swiping = false;
			m_SwipeStarted = false;
		}

		if ( m_SwipeStarted )
		{
			Vector3 pos = m_Camera.ScreenToViewportPoint( Input.mousePosition );
			if ( Vector3.Distance( m_SwipeStart, pos ) > 0.1f )
			{
				m_Swiping = true;
				m_SwipeStarted = false;
			}
		}

		if ( m_Swiping )
		{
			Vector3 pos = m_Camera.ScreenToViewportPoint( Input.mousePosition );
			int swipeDir = 0;
			if ( m_SwipeStart.y < pos.y - m_DeadZone.y )
			{
				swipeDir = 1;
				if ( m_SwipeStart.x < pos.x - m_DeadZone.x )
				{
					swipeDir = 2;
				}
				else if ( pos.x + m_DeadZone.x < m_SwipeStart.x )
				{
					swipeDir = 6;
				}
			}
			else if ( pos.y + m_DeadZone.y < m_SwipeStart.y )
			{
				swipeDir = 4;
				if ( m_SwipeStart.x < pos.x - m_DeadZone.x )
				{
					swipeDir = 3;
				}
				else if ( pos.x + m_DeadZone.x < m_SwipeStart.x )
				{
					swipeDir = 5;
				}
			}
			if ( swipeDir > 0 )
			{
				MoveInDirection( swipeDir - 1 );
			}
		}
	}

	private void MoveInDirection ( int direction )
	{
		if ( m_Moving ) return;
		HexPathTile nbor = m_Grid.GetNeighbor( m_Coords, direction );
		if ( nbor && nbor.PathCode.HasByte( HexPathTile.PathDirs[(int)Mathf.Repeat( direction + 3, 6 )] ) )
			StartCoroutine( MoveTo( nbor.transform.position, direction ) );
	}

	private IEnumerator MoveTo ( Vector3 target, int direction )
	{
		m_Moving = true;
		m_Animator.Play( 0, -1, 0 );

		Vector3 start = transform.position;
		float t = 0;
		while ( t < 1 )
		{
			t += Time.deltaTime * m_JumpSpeed;
			transform.position = Vector3.Lerp( start, target, m_JumpSpeedCurve.Evaluate( t ) );
			yield return null;
		}

		m_Coords = HexTileGrid.oddq_offset_neighbor( m_Coords, direction );
		m_Grid.MovedToCoords( m_Coords, (byte)Mathf.Repeat( direction + 3, 6 ) );
		m_Moving = false;
	}



	public void CompleteLevel ()
	{
		m_ConfettiObject.SetActive( true );
		m_Animator.SetBool( "Success", true );
		m_Active = false;
	}

	public void FailLevel ()
	{
		m_Animator.SetBool( "Fail", true );
		m_EmoteObject.SetActive( true );
		m_Active = false;
	}

	public void ResetPlayer ()
	{
		m_Animator.SetBool( "Success", false );
		m_Animator.SetBool( "Fail", false );
		m_ConfettiObject.SetActive( false );
		m_EmoteObject.SetActive( false );
		transform.position = Vector3.zero;
		m_Coords = Vector3Int.zero;
		m_SwipeStart = Vector3.zero;
		m_Moving = false;
		m_Active = true;
	}

}
