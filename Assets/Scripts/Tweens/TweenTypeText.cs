using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweenTypeText : Tweener
{

	[SerializeField, TextArea] private string m_Text = string.Empty;

	private TMPro.TextMeshProUGUI m_TextComponent;

	protected override void UpdateTween ()
	{
		if ( !m_TextComponent ) m_TextComponent = GetComponent<TMPro.TextMeshProUGUI>();

		m_TextComponent.text = m_Text.Substring( 0, Mathf.CeilToInt( m_Text.Length * Factor ) );
	}

}
