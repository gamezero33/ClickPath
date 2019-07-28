using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweenTextMarquee : Tweener
{

	[SerializeField] private bool m_PadBeforeText = true;
	[SerializeField] private int m_RenderedCharacters = 20;
	[SerializeField, TextArea] private string m_Text = string.Empty;

	private TMPro.TextMeshProUGUI m_TextComponent;

	private void Start ()
	{
		ResetTextComponent();
	}

	protected override void UpdateTween ()
	{
		int start = Mathf.CeilToInt( m_Text.Length * Factor );
		int length = Mathf.Min( m_Text.Length - start, m_RenderedCharacters );
		if ( m_TextComponent )
		{
			m_TextComponent.alignment = start < m_RenderedCharacters ? TMPro.TextAlignmentOptions.CaplineRight : TMPro.TextAlignmentOptions.CaplineLeft;
			m_TextComponent.text = m_Text.Substring( start, length );
		}
	}

	[EditorButton]
	private void ResetTextComponent ()
	{
		m_TextComponent = null;
		m_Text = m_Text.TrimStart();
		m_TextComponent = GetComponent<TMPro.TextMeshProUGUI>();
		if ( m_PadBeforeText )
		{
			string padding = string.Empty;
			for ( int i = 0; i < m_RenderedCharacters; i++ ) padding += " ";
			m_Text = padding + m_Text;
		}
	}

}
