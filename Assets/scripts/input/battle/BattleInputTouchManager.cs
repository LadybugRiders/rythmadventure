﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleInputTouchManager : MonoBehaviour {

    public enum SLIDE
    {
        RIGHT,LEFT,DOWN,UP
    }
	
	[SerializeField] private BattleTracksManager m_tracksManager;

	[SerializeField] private BoxCollider2D m_attackCollider;
	[SerializeField] private BoxCollider2D m_defendCollider;

    [SerializeField] private float m_slideMinLength = 3.0f;
	[SerializeField] private float m_slideMaxTime = 0.1f;
	private float m_timePressed = 0;

	bool m_pressed = false;
    Vector2 m_positionPressed;

    Vector2 m_deltaSlide;
    bool m_sliding = false;

    /// <summary>
    /// The id of the button pressed
    /// </summary>
	int m_inputDown = -1;

	// Use this for initialization
	void Start () {
        m_tracksManager.noteEventHandler += OnNoteHit;
	}
	
	// Update is called once per frame
	void Update () {
		//Check press
		if (m_pressed==false && IsPressed() ){
            //compute position pressed
			Vector3 worldPoint = Camera.main.ScreenToWorldPoint( CustomGetTouchPosition() );
			Vector2 touchPos = new Vector2(worldPoint.x,worldPoint.y);
            m_positionPressed = touchPos;
			Collider2D coll = Physics2D.OverlapPoint(touchPos);
			m_timePressed = 0;
            //check attack 
            if ( m_attackCollider == coll ){				
				m_pressed = true;
				m_inputDown = 1;
				m_tracksManager.OnInputTriggered(1,BattleNote.HIT_METHOD.PRESS);
			}else if( m_defendCollider == coll){				
				m_pressed = true;
				m_inputDown = -1;
				m_tracksManager.OnInputTriggered(-1,BattleNote.HIT_METHOD.PRESS);
			}
		}

		// check slide and release
		if( m_pressed )
        {
			m_timePressed += Time.deltaTime;
            //compute position pressed
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(CustomGetTouchPosition());
            Vector2 touchPos = new Vector2(worldPoint.x, worldPoint.y);
            //compute delta vector from first press point to actual pressed point
            m_deltaSlide = touchPos - m_positionPressed ;
            //if just released
            if (IsReleased())
            {
                m_pressed = false;
                if( m_sliding)
                {
                    //slide end
                }
                m_sliding = false;
                m_tracksManager.OnInputTriggered(m_inputDown,BattleNote.HIT_METHOD.RELEASE);
            }else
            {
                //Still pressing => check slide
				if ( m_timePressed <= m_slideMaxTime && !m_sliding && m_deltaSlide.magnitude >= m_slideMinLength)
                {
                    m_sliding = true;
                    m_tracksManager.OnInputTriggered(m_inputDown, BattleNote.HIT_METHOD.SLIDE);
                }
            }
		}
	}

    void OnNoteHit(object sender, BattleTracksManager.NoteEventInfo eventInfo) {
        if (eventInfo.NextNote == null)
            return;
    }

	Vector2 CustomGetTouchPosition(){
#if UNITY_STANDALONE || UNITY_EDITOR
		return Input.mousePosition;
#else
		return Input.GetTouch(0).position;
#endif
	}

	bool IsPressed(){
#if UNITY_STANDALONE|| UNITY_EDITOR
			return Input.GetMouseButtonDown(0);
#else
			return Input.touchCount == 1;
#endif
	}

	bool IsReleased(){
#if UNITY_STANDALONE|| UNITY_EDITOR
	return Input.GetMouseButtonUp(0);
#else
	return Input.touchCount < 1 ;
#endif	
	}
}
