using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum ColorMode { Replace, Add, Subtract, Multiply, Divide, Hue, Saturation, Lightness }

public static class ExtensionMethods
{

	#region -| Vector3 |-

	#region - Round To -
	/// <summary>Round vector's values to nearest [interval] degrees.</summary>
	/// <param name="interval">Defaults to 90 degrees.</param>
	public static Vector3 RoundTo ( this Vector3 vector, int interval = 90 )
	{
		if ( interval <= 0f )
			throw new UnityException( "interval argument must be above 0" );

		float x = Mathf.Round( vector.x / interval ) * interval;
		float y = Mathf.Round( vector.y / interval ) * interval;
		float z = Mathf.Round( vector.z / interval ) * interval;

		return new Vector3( x, y, z );
	}
	#endregion

	public static Vector3Int FloorToVector3Int ( this Vector3 vector )
	{
		return new Vector3Int( Mathf.FloorToInt( vector.x ), Mathf.FloorToInt( vector.y ), Mathf.FloorToInt( vector.z ) );
	}

	public static Vector3Int Random ( this Vector3Int vector, int range, int z = 0 )
	{
		return vector.Random( -range, range, z );
	}
	public static Vector3Int Random ( this Vector3Int vector, int min, int max, int z )
	{
		int x = UnityEngine.Random.Range( min, max );
		int y = UnityEngine.Random.Range( min, max );
		return vector + new Vector3Int( x, y, z );
	}

	public static Vector3 Random ( this Vector3 vector, float range )
	{
		float x = UnityEngine.Random.Range( -range, range );
		float y = UnityEngine.Random.Range( -range, range );
		float z = UnityEngine.Random.Range( -range, range );

		return vector + new Vector3( x, y, z );
	}

	public static Vector3 Random ( this Vector3 vector, float xRange, float yRange, float zRange )
	{
		float x = UnityEngine.Random.Range( -xRange, xRange );
		float y = UnityEngine.Random.Range( -yRange, yRange );
		float z = UnityEngine.Random.Range( -zRange, zRange );

		return vector + new Vector3( x, y, z );
	}

	public static Vector3 Random ( this Vector3 vector, float min, float max )
	{
		float x = UnityEngine.Random.Range( min, max );
		float y = UnityEngine.Random.Range( min, max );
		float z = UnityEngine.Random.Range( min, max );

		return vector + new Vector3( x, y, z );
	}

	public static List<Vector3> RandomPoints ( this Vector3 vector, int qty, float range, float spacing, float height, List<Vector3> exclude = null )
	{
		List<Vector3> points = new List<Vector3>( qty );
		if ( exclude != null )
		{ 
			for ( int i = 0; i < exclude.Count; i++ )
			{
				points.Add( exclude[i] );
			}
		}
		for ( int i = 0 + ( exclude != null ? exclude.Count : 0 ); i < qty; i++ )
		{
			Vector3 point = vector.Random( range );
			point.y = height;
			if ( i == 0 ) points.Add( point );
			else
			{
				bool farEnough = false;
				int iterations = 0;
				while ( !farEnough && iterations++ < 1000 )
				{
					points.Sort( ( p1, p2 ) => Vector3.Distance( point, p1 ).CompareTo( Vector3.Distance( point, p2 ) ) );
					if ( Vector3.Distance( point, points[0] ) > spacing ) farEnough = true;
				}
				points.Add( point );
			}
		}
		if ( exclude != null )
		{
			for ( int i = 0; i < exclude.Count; i++ )
			{
				points.Remove( exclude[i] );
			}
		}

		if ( points.Count < qty )
		{
			for ( int p = 0; p < qty - points.Count; p++ )
			{
				points.Add( vector.Random( range ) );
			}
		}
		return points;
	}

	public static Vector3 Lerp3 ( this Vector3 vectorA, Vector3 vectorB, Vector3 weights )
	{
		Vector3 output = vectorA;
		output.x = Mathf.Lerp( vectorA.x, vectorB.x, weights.x );
		output.y = Mathf.Lerp( vectorA.y, vectorB.y, weights.y );
		output.z = Mathf.Lerp( vectorA.z, vectorB.z, weights.z );

		return output;
	}

	#region - (Rounded) Dot Product -
	public static Vector3 DotProduct ( this Transform operand, Vector3 facing )
	{
		var x = Mathf.Round( Vector3.Dot( facing, operand.right ) );
		var y = Mathf.Round( Vector3.Dot( facing, operand.up ) );
		var z = Mathf.Round( Vector3.Dot( facing, operand.forward ) );

		if ( x < 0 )
			x = -1;
		else if ( x > 0 )
			x = 1;
		else
			x = 0;

		if ( y < 0 )
			y = -1;
		else if ( y > 0 )
			y = 1;
		else
			y = 0;

		if ( z < 0 )
			z = -1;
		else if ( z > 0 )
			z = 1;
		else
			z = 0;

		return new Vector3( x, y, z );
	}
	#endregion


	public static Vector3 ToVector3 ( this Vector2Int vector, int gridCols, int gridRows, float height, float halfUnit = 0.5f )
	{
		return new Vector3( gridCols * -halfUnit + vector.x - halfUnit, height, gridRows * -halfUnit + vector.y - halfUnit );
	}

	public static Vector3 ToVector3 ( this Vector3Int vector )
	{
		return new Vector3( vector.x, vector.y, vector.z );
	}

	#endregion


	#region -| Collections |-

	public static bool Contains<T> ( this System.Array array, T item )
	{
		for ( int i = 0; i < array.Length; i++ )
		{
			if ( array.GetValue(i).Equals( item ) )
				return true;
		}

		return false;
	}

	public static T Random<T> ( this List<T> list )
	{
		if ( list == null || list.Count <= 0 ) return default( T );
		return list[UnityEngine.Random.Range( 0, list.Count )];
	}

	public static T Random<T> ( this T[] array )
	{
		if ( array == null || array.Length <= 0 ) return default( T );
		return array[UnityEngine.Random.Range( 0, array.Length )];
	}

	public static T[] Random<T> ( this T[] array, int qty )
	{
		if ( qty > array.Length || array == null || array.Length <= 0 ) return default( T[] );
		T[] a = new T[qty];
		for ( int i = 0; i < qty; i++ )
		{
			T b = array.Random();
			while ( a.Contains( b ) ) b = array.Random();
			a[i] = b;
		}
		return a;
	}

	public static T Random<T> ( this List<T> list, System.Random prng )
	{
		return list[prng.Next( list.Count )];
	}

	public static T[] SafeConcat<T> ( this T[] arrayA, T[] arrayB )
	{
		if ( arrayB == null )
			return arrayA;
		return arrayA.Concat( arrayB ).ToArray();
	}

	public static void Shuffle<T> ( this IList<T> list )
	{
		int n = list.Count;
		while ( n > 1 )
		{
			n--;
			int k = UnityEngine.Random.Range( 0, n + 1 );
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public static int Add ( params int[] integers )
	{
		int a = 0;
		for ( int i = 0; i < integers.Length; i++ )
		{
			a += integers[i];
		}
		return a;
	}

	public static byte Add ( params byte[] bytes )
	{
		byte a = 0;
		for ( int i = 0; i < bytes.Length; i++ )
		{
			a += bytes[i];
		}
		return a;
	}

	#endregion


	#region -| Color |-

	public static Color Mix ( this Color color, params Color[] operands )
	{
		Color output = color;
		for ( int i = 0; i < operands.Length; i++ )
		{
			output += operands[i];
		}

		output /= operands.Length + 1;
		return output;
	}

	public static Texture2D ToTex ( this Color color )
	{
		Texture2D tex = new Texture2D( 2, 2 );
		tex.SetPixels( new Color[] { color, color, color, color } );
		tex.Apply();
		return tex;
	}

	public static Color ToAlpha ( this Color color, float alpha = 0.5f )
	{
		return new Color( color.r, color.g, color.b, alpha );
	}

	public static Color ProcessColor ( this Color baseColor, Color opColor, ColorMode mode )
	{
		float baseH, baseS, baseL, opH, opS, opL;
		Color.RGBToHSV( baseColor, out baseH, out baseS, out baseL );
		Color.RGBToHSV( opColor, out opH, out opS, out opL );
		Color processedColor = baseColor;
		switch ( mode )
		{
			case ColorMode.Replace:
				processedColor = opColor;
				break;
			case ColorMode.Add:
				processedColor += opColor;
				break;
			case ColorMode.Subtract:
				processedColor -= opColor;
				break;
			case ColorMode.Multiply:
				processedColor *= opColor;
				break;
			case ColorMode.Divide:
				processedColor.r /= opColor.r;
				processedColor.g /= opColor.g;
				processedColor.b /= opColor.b;
				processedColor.a /= opColor.a;
				break;
			case ColorMode.Hue:
				processedColor = Color.HSVToRGB( opH, baseS, baseL );
				break;
			case ColorMode.Saturation:
				processedColor = Color.HSVToRGB( baseH, opS, baseL );
				break;
			case ColorMode.Lightness:
				processedColor = Color.HSVToRGB( baseH, baseS, opL );
				break;
		}
		return processedColor;
	}

	#endregion


	#region -| LayerMask |-

	public static int ToLayer ( this LayerMask layerMask )
	{
		int layerNumber = 0;
		int layer = layerMask.value;
		while ( layer > 0 )
		{
			layer = layer >> 1;
			layerNumber++;
		}
		return layerNumber - 1;
	}

	public static void SetLayer ( this GameObject parent, int layer, bool includeChildren = true )
	{
		parent.layer = layer;
		if ( includeChildren )
		{
			foreach ( Transform trans in parent.transform.GetComponentsInChildren<Transform>( true ) )
			{
				trans.gameObject.layer = layer;
			}
		}
	}

	#endregion


	#region -| String |-

	public static string FirstUpper ( this string str )
	{
		return string.Format( "{0}{1}", str[0].ToString().ToUpper(), str.Substring( 1 ) );
	}

	public static string Random ( this string str )
	{
		return str[UnityEngine.Random.Range( 0, str.Length )].ToString();
	}

	#endregion


	#region -| System.Random |-

	public static float Next ( this System.Random prng, float minValue, float maxValue, int precision = 3 )
	{
		float precOp = Mathf.Pow( 10, precision );
		return prng.Next( (int)( minValue * precOp ), (int)( maxValue * precOp ) ) / precOp;
	}

	#endregion


	#region -| Transform |-

	public static void DestroyChildren ( this Transform transform )
	{
		var children = new List<GameObject>();
		foreach ( Transform child in transform ) children.Add( child.gameObject );
		if ( Application.isPlaying )
			children.ForEach( child => GameObject.Destroy( child ) );
		else
			children.ForEach( child => GameObject.DestroyImmediate( child ) );
	}

	#endregion


	public static bool HasByte ( this int number, byte byteToCheck )
	{
		return ( number & byteToCheck ) == byteToCheck;
	}

	public static bool HasByte ( this byte number, byte byteToCheck )
	{
		return ( number & byteToCheck ) == byteToCheck;
	}


	#region -| EDITOR METHODS |-
#if UNITY_EDITOR



#endif
	#endregion

}