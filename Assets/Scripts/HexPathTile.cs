using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexPathTile : MonoBehaviour
{
	// Helper Constant LUT
	public static readonly byte[] PathDirs = { 1, 2, 4, 8, 16, 32 };


	// Published Fields

	[SerializeField] private byte m_PathCode = 0;
	[SerializeField] private Renderer[] m_PathRenderers;
	[SerializeField] private Renderer m_PathHubRenderer;
	[SerializeField] private Renderer m_TileRenderer;
	[SerializeField] private GameObject m_SelectFX;

	[SerializeField] private float m_PopupHeight = 0.5f;
	[SerializeField] private float m_PopupSpeed = 3;
	[SerializeField] private AnimationCurve m_PopupCurve = AnimationCurve.EaseInOut( 0, 0, 1, 1 );
	[SerializeField] private float m_SpinSpeed = 5;
	[SerializeField] private AnimationCurve m_SpinCurve = AnimationCurve.EaseInOut( 0, 0, 1, 1 );


	// Public Fields

	public HexTileGrid grid;
	public Vector3Int coords;


	// Property Sets

	#region - Selected -
	private bool m_Selected;
	public bool Selected
	{
		get
		{
			return m_Selected;
		}
		set
		{
			m_Selected = value;
			m_SelectFX.SetActive( m_Selected );
		}
	}
	#endregion

	#region - Visited -
	private bool m_Visited;
	public bool Visited
	{
		get
		{
			return m_Visited;
		}
		set
		{
			m_Visited = value;
			if ( grid )
			{
				m_TileRenderer.material.color = m_Visited ? grid.VisitedTileColor : grid.UnvisitedTileColor;
				CheckNeighborPaths( 0 );
			}
		}
	}
	#endregion

	#region - Goal -
	private bool m_Goal;
	public bool Goal
	{
		get
		{
			return m_Goal;
		}
		set
		{
			m_Goal = value;
			if ( grid )
			{
				m_TileRenderer.material.color = m_Goal ? grid.GoalTileColor : m_Visited ? grid.VisitedTileColor : grid.UnvisitedTileColor;
				CheckNeighborPaths( 0 );
			}
		}
	}
	#endregion

	#region - PathCode -
	public byte PathCode
	{
		get
		{
			return m_PathCode;
		}
		set
		{
			m_PathCode = (byte)Mathf.Repeat( value, 63 );
			UpdatePath();
		}
	}
	#endregion


	// Private Memnbers

	private bool m_Rotating = false;
	private float m_TouchTimer = -1;


	// Mono Callbacks

#if UNITY_EDITOR
	private void OnValidate ()
	{
		if ( !Application.IsPlaying( this ) ) UpdatePath();
	}
#endif

	private void OnMouseDown ()
	{
		if ( !m_Selected ) return;
		m_TouchTimer = 0;
	}

	private void Update ()
	{
		if ( m_TouchTimer >= 0 && m_TouchTimer < 0.2f )
		{
			m_TouchTimer += Time.deltaTime;
		}
		else if ( m_TouchTimer > 0.2f && !grid.Player.Swiping )
		{
			if ( !m_Rotating ) StartCoroutine( Rotate() );
			m_TouchTimer = -1;
		}
		else
		{
			m_TouchTimer = -1;
		}
	}


	// Update Methods

	private void UpdatePath ()
	{
		if ( m_PathRenderers == null || !gameObject.activeInHierarchy ) return;
		for ( int i = 0; i < m_PathRenderers.Length; i++ )
		{
			if ( m_PathRenderers[i] )
			{
				m_PathRenderers[i].gameObject.SetActive( m_PathCode.HasByte( PathDirs[i] ) );
			}
		}
	}


	// Transform Coroutines

	private IEnumerator Rotate ()
	{
		m_Rotating = true;
		Vector3 start = transform.position;
		Vector3 end = Vector3.up * m_PopupHeight + start;
		float t = 0;
		while ( t < 1 )
		{
			t += Time.deltaTime * m_PopupSpeed;
			transform.position = Vector3.LerpUnclamped( start, end, m_PopupCurve.Evaluate( t ) );
			yield return null;
		}

		Quaternion startRot = transform.rotation;
		Quaternion endRot = transform.rotation * Quaternion.Euler( Vector3.up * 60 );
		t = 0;
		while ( t < 1 )
		{
			t += Time.deltaTime * m_SpinSpeed;
			transform.rotation = Quaternion.LerpUnclamped( startRot, endRot, m_SpinCurve.Evaluate( t ) );
			yield return null;
		}

		end = start;
		start = transform.position;
		t = 0;
		while ( t < 1 )
		{
			t += Time.deltaTime * m_PopupSpeed;
			transform.position = Vector3.LerpUnclamped( start, end, m_PopupCurve.Evaluate( t ) );
			yield return null;
		}

		transform.rotation = Quaternion.identity;
		RotatePathCode();
		CheckNeighborPaths( 1 );

		yield return null;

		m_Rotating = false;
	}

	private void RotatePathCode ()
	{
		PathCode <<= 1;
	}

	public void CheckNeighborPaths ( int recursion )
	{
		bool hasValidPath = false;
		for ( int i = 0; i < PathDirs.Length; i++ )
		{
			if ( PathCode.HasByte( PathDirs[i] ) )
			{
				if ( m_PathRenderers[i] )
				{
					bool valid = grid.CheckNeighborPaths( coords, i );
					m_PathRenderers[i].material.color = valid ? grid.ValidPathColor : m_Visited ? grid.InvalidPathVistedColor : m_Goal ? grid.InvalidPathGoalColor : grid.InvalidPathColor;
					if ( valid )
					{
						hasValidPath = true;
						HexPathTile nbor = grid.GetNeighbor( coords, i );
						if ( nbor && recursion > 0 )
						{
							nbor.CheckNeighborPaths( recursion - 1 );
						}
					}
				}
			}
			else
			{
				HexPathTile nbor = grid.GetNeighbor( coords, i );
				if ( nbor && recursion > 0 )
				{
					nbor.CheckNeighborPaths( recursion - 1 );
				}
			}
		}

		m_PathHubRenderer.material.color = hasValidPath ? grid.ValidPathColor : m_Visited ? grid.InvalidPathVistedColor : m_Goal ? grid.InvalidPathGoalColor : grid.InvalidPathColor;
	}
	
}
