using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class HexTileGrid : MonoBehaviour
{


	public static readonly Vector3Int[][] oddq_directions =
	{
		new Vector3Int[]
		{
			new Vector3Int( 1, 0, 0 ),
			new Vector3Int( 0, 1, 0 ),
			new Vector3Int( -1, 1, 0 ),
			new Vector3Int( -1, 0, 0 ),
			new Vector3Int( -1, -1, 0 ),
			new Vector3Int( 0, -1, 0 )
		},
		new Vector3Int[]
		{
			new Vector3Int( 1, 0, 0 ),
			new Vector3Int( 1, 1, 0 ),
			new Vector3Int( 0, 1, 0 ),
			new Vector3Int( -1, 0, 0 ),
			new Vector3Int( 0, -1, 0 ),
			new Vector3Int( 1, -1, 0 )
		}
	};

	public static Vector3Int oddq_offset_neighbor ( Vector3Int hex, int direction )
	{
		var parity = hex.y & 1;
		var dir = oddq_directions[parity][direction];

		return new Vector3Int( hex.x + dir[0], hex.y + dir[1], 0 );
	}

	
	[SerializeField] private HexPathTileBase m_HexPathTileBase;
	[SerializeField] private HexPathTile m_HexPathTilePrefab;

	[SerializeField] private TMPro.TextMeshProUGUI m_TilesCounterTMPro;

	[SerializeField] private Color m_ValidPathColor = Color.white;
	public Color ValidPathColor { get { return m_ValidPathColor; } }

	[SerializeField] private Color m_UnvisitedTileColor = Color.blue.Mix( Color.white );
	public Color UnvisitedTileColor { get { return m_UnvisitedTileColor; } }
	[SerializeField] private Color m_InvalidPathColor = Color.blue.Mix( Color.white, Color.black );
	public Color InvalidPathColor { get { return m_InvalidPathColor; } }

	[SerializeField] private Color m_VisitedTileColor = Color.red.Mix( Color.blue );
	public Color VisitedTileColor { get { return m_VisitedTileColor; } }
	[SerializeField] private Color m_InvalidPathVistedColor = Color.red.Mix( Color.blue, Color.black );
	public Color InvalidPathVistedColor { get { return m_InvalidPathVistedColor; } }

	[SerializeField] private Color m_GoalTileColor = Color.green;
	public Color GoalTileColor { get { return m_GoalTileColor; } }
	[SerializeField] private Color m_InvalidPathGoalColor = Color.green.Mix( Color.black );
	public Color InvalidPathGoalColor { get { return m_InvalidPathGoalColor; } }


	[SerializeField] private Tilemap m_Tilemap;
	[SerializeField] private PlayerController m_Player;
	public PlayerController Player { get { return m_Player; } }

	private int m_TileCount = 0;


	private void Start ()
	{
		CreateStartingTiles();
	}

	private void CreateStartingTiles ()
	{
		m_TileCount = 0;
		Vector3Int point = Vector3Int.zero;
		CreateTile( point, 1, false, true, false );
		byte p1 = 8; while ( p1 == 8 ) p1 = HexPathTile.PathDirs.Random();
		byte p2 = 8; while ( p2 == 8 || p2 == p1 ) p2 = HexPathTile.PathDirs.Random();
		//byte p3 = 8; while ( p3 == 8 || p3 == p2 || p3 == p1 ) p3 = HexPathTile.PathDirs.Random();
		CreateTile( oddq_offset_neighbor( point, 0 ), (byte)( p1 + p2 ), true, false, false );

		p1 = 1; while ( p1 == 1 ) p1 = HexPathTile.PathDirs.Random();
		CreateTile( oddq_offset_neighbor( point, 3 ), p1, false, false, true );
		//for ( int i = 0; i < 6; i++ )
		//{
		//	if ( i != 3 )
		//		CreateTile( oddq_offset_neighbor( oddq_offset_neighbor( point, 0 ), i ), RandomPath( 2 ), false, false );
		//}
	}

	private byte RandomPath ( int paths )
	{
		byte[] codeBytes = HexPathTile.PathDirs.Random( paths );
		return ExtensionMethods.Add( codeBytes );
	}

	private HexPathTileBase CreateTile ( Vector3Int coords, byte pathcode, bool selected, bool visited, bool goal )
	{
		HexPathTileBase tile = m_Tilemap.GetTile<HexPathTileBase>( coords );
		//if ( tile != null && tile.Visited ) return false;
		if ( tile != null )
		{
			tile.Selected = !tile.Visited;
			return tile;
		}

		tile = Instantiate( m_HexPathTileBase );
		tile.PathCode = pathcode;
		tile.Selected = selected;
		tile.Visited = visited;
		tile.Goal = goal;
		tile.Coords = coords;
		tile.Grid = this;

		m_Tilemap.SetTile( coords, tile );
		m_Tilemap.RefreshTile( coords );

		if ( !goal && !visited )
			m_TileCount++;
		m_TilesCounterTMPro.text = string.Format( "{0}\nTILE{1}", m_TileCount, m_TileCount == 1 ? "" : "S" );
		return tile;
	}


	public HexPathTile GetNeighbor ( Vector3Int coords, int direction )
	{
		HexPathTileBase tile = m_Tilemap.GetTile<HexPathTileBase>( oddq_offset_neighbor( coords, direction ) );
		if ( tile )
		{
			return tile.Tile;
		}
		return null;
	}

	public bool CheckNeighborPaths ( Vector3Int coords, int direction )
	{
		HexPathTileBase tile = m_Tilemap.GetTile<HexPathTileBase>( oddq_offset_neighbor( coords, direction ) );
		if ( tile )
		{
			return tile.PathCode.HasByte( HexPathTile.PathDirs[(int)Mathf.Repeat( direction + 3, 6 )] );
		}
		return false;
	}

	public void MovedToCoords ( Vector3Int coords, byte fromDir )
	{
		HexPathTileBase tile = m_Tilemap.GetTile<HexPathTileBase>( coords );
		if ( tile.Goal )
		{
			CompleteLevel();
			return;
		}
		tile.Selected = false;
		tile.Visited = true;

		int dir = HexPathTile.PathDirs.ToList().IndexOf( (byte)( tile.PathCode - HexPathTile.PathDirs[fromDir] ) );
		if ( dir >= 0 )
		{
			byte d = HexPathTile.PathDirs[(int)Mathf.Repeat( dir + 3, 6 )];
			byte p1 = d; while ( p1 == d ) p1 = HexPathTile.PathDirs.Random();
			byte p2 = d; while ( p2 == d || p2 == p1 ) p2 = HexPathTile.PathDirs.Random();
			HexPathTileBase nbor = CreateTile( oddq_offset_neighbor( coords, dir ), (byte)( p1 + p2 ), true, false, false );
			if ( nbor.Visited && !nbor.PathCode.HasByte( d ) )
			{
				FailLevel();
			}
		}
	}

	private void CompleteLevel ()
	{
		Debug.Log( "Level Complete!" );
		m_Player.CompleteLevel();
		Invoke( "RestartLevel", 2 );
	}


	private void FailLevel ()
	{
		Debug.Log( "Level cannot be completed!" );

		m_Player.FailLevel();
		Invoke( "RestartLevel", 2 );
	}

	private void RestartLevel ()
	{
		ClearTilemap();
		m_Player.ResetPlayer();
		CreateStartingTiles();
	}

	private void ClearTilemap ()
	{
		foreach( Vector3Int pos in m_Tilemap.cellBounds.allPositionsWithin)
		{
			HexPathTileBase tile = m_Tilemap.GetTile<HexPathTileBase>( pos );
			if ( tile )
			{
				if ( tile.Tile.gameObject )
				{
					Destroy( tile.Tile.gameObject );
				}
			}
		}
		m_Tilemap.ClearAllTiles();
	}

}
