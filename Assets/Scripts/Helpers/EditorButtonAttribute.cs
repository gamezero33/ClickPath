#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;
using UnityEngine;
using System.Reflection;

[System.AttributeUsage( System.AttributeTargets.Method )]
public class EditorButtonAttribute : PropertyAttribute
{
}

[System.AttributeUsage( System.AttributeTargets.Method )]
public class RuntimeButtonAttribute : PropertyAttribute
{
}

#if UNITY_EDITOR
[CustomEditor( typeof( MonoBehaviour ), true )]
public class EditorButton : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();

		MonoBehaviour mono = target as MonoBehaviour;

		var editorMethods = mono.GetType()
			.GetMembers( BindingFlags.Instance | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
						BindingFlags.NonPublic )
			.Where( o => System.Attribute.IsDefined( o, typeof( EditorButtonAttribute ) ) );

		for ( int i = 0; i < editorMethods.Count(); i++ )
		{
			MethodInfo memberInfo = editorMethods.ElementAt( i ) as MethodInfo;
			if ( GUILayout.Button( memberInfo.Name ) )
				memberInfo.Invoke( mono, null );
		}

		if ( Application.isPlaying )
		{
			var runtimeMethods = mono.GetType()
				.GetMembers( BindingFlags.Instance | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
							BindingFlags.NonPublic )
				.Where( o => System.Attribute.IsDefined( o, typeof( RuntimeButtonAttribute ) ) );

			for ( int i = 0; i < runtimeMethods.Count(); i++ )
			{
				MethodInfo memberInfo = runtimeMethods.ElementAt( i ) as MethodInfo;
				if ( GUILayout.Button( memberInfo.Name ) )
					memberInfo.Invoke( mono, null );
			}
		}


	}
}
#endif