using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class HexPathTileBase : Tile
{

	// Property Sets

	#region - Tile -
	private HexPathTile m_Tile = null;
	public HexPathTile Tile
	{
		get
		{
			if ( !m_Tile && m_GameObject ) m_Tile = m_GameObject.GetComponent<HexPathTile>();
			return m_Tile;
		}
	}
	#endregion

	#region - PathCode -
	private byte m_PathCode;
	public byte PathCode
	{
		get
		{
			if ( Tile ) return Tile.PathCode;
			return m_PathCode;
		}
		set
		{
			if ( Tile ) Tile.PathCode = value;
			else m_PathCode = value;
		}
	}
	#endregion

	#region - Selected -
	private bool m_Selected;
	public bool Selected
	{
		get
		{
			if ( Tile ) return Tile.Selected;
			return m_Selected;
		}
		set
		{
			if ( Tile ) Tile.Selected = value;
			else m_Selected = value;
		}
	}
	#endregion

	#region - Visited -
	private bool m_Visited;
	public bool Visited
	{
		get
		{
			if ( Tile ) return Tile.Visited;
			return m_Visited;
		}
		set
		{
			if ( Tile ) Tile.Visited = value;
			else m_Visited = value;
		}
	}
	#endregion

	#region - Goal -
	private bool m_Goal;
	public bool Goal
	{
		get
		{
			if ( Tile ) return Tile.Goal;
			return m_Goal;
		}
		set
		{
			if ( Tile ) Tile.Goal = value;
			else m_Goal = value;
		}
	}
	#endregion

	#region - Grid -
	private HexTileGrid m_Grid;
	public HexTileGrid Grid
	{
		get
		{
			if ( Tile ) return Tile.grid;
			return m_Grid;
		}
		set
		{
			if ( Tile ) Tile.grid = value;
			else m_Grid = value;
		}
	}
	#endregion

	#region - Coords -
	private Vector3Int m_Coords;
	public Vector3Int Coords
	{
		get
		{
			if ( Tile ) return Tile.coords;
			return m_Coords;
		}
		set
		{
			if ( Tile ) Tile.coords = value;
			else m_Coords = value;
		}
	}
	#endregion


	// Private Members

	private GameObject m_GameObject;


	// Initializers

	private void InitializeProperties ()
	{
		Grid = m_Grid;
		PathCode = m_PathCode;
		Selected = m_Selected;
		Visited = m_Visited;
		Goal = m_Goal;
		Coords = m_Coords;
		Tile.CheckNeighborPaths( 1 );
	}


	// TileBase Overrides

	public override bool StartUp ( Vector3Int position, ITilemap tilemap, GameObject go )
	{
		if ( go ) m_GameObject = go;
		InitializeProperties();
		return base.StartUp( position, tilemap, go );
	}


}
