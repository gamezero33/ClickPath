using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweenColor : Tweener
{

	[SerializeField]
	private Color m_StartColor = Color.white;
	public Color StartColor
	{
		get
		{
			return m_StartColor;
		}
		set
		{
			m_StartColor = value;
		}
	}

	[SerializeField]
	private Color m_EndColor = Color.black.ToAlpha( 0.5f );
	public Color EndColor
	{
		get
		{
			return m_EndColor;
		}
		set
		{
			m_EndColor = value;
		}
	}

    private TMPro.TextMeshPro m_TMP;
    private Image m_Image;
	private SpriteRenderer m_SpriteRenderer;
	private MeshRenderer m_MeshRenderer;
	private Text m_Text;
	private Light m_Light;

	private void Awake ()
	{
        m_TMP = GetComponent<TMPro.TextMeshPro>();
		m_Image = GetComponent<Image>();
		m_SpriteRenderer = GetComponent<SpriteRenderer>();
		m_MeshRenderer = GetComponent<MeshRenderer>();
		m_Text = GetComponent<Text>();
		m_Light = GetComponent<Light>();
	}

	protected override void UpdateTween ()
	{
        if ( !m_TMP )
            m_TMP = GetComponent<TMPro.TextMeshPro>();
		if ( !m_Image )
			m_Image = GetComponent<Image>();
		if ( !m_Text )
			m_Text = GetComponent<Text>();
		if ( !m_Light )
			m_Light = GetComponent<Light>();
		if ( !m_SpriteRenderer )
			m_SpriteRenderer = GetComponent<SpriteRenderer>();
		if ( !m_MeshRenderer )
			m_MeshRenderer = GetComponent<MeshRenderer>();

		Color color = Color.LerpUnclamped( m_StartColor, m_EndColor, Factor );
        if ( m_TMP )
            m_TMP.color = color;
		if ( m_Image )
			m_Image.color = color;
		if ( m_Text )
			m_Text.color = color;
		if ( m_Light )
			m_Light.color = color;
		if ( m_SpriteRenderer )
			m_SpriteRenderer.color = color;
		if ( m_MeshRenderer && Application.isPlaying )
			m_MeshRenderer.sharedMaterial.color = color;
	}

}