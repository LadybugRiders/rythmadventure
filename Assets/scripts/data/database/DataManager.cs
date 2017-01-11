﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DataManager : DatabaseLoader {

	private static DataManager _instance;
	private bool m_allLoaded = false;
	public string m_previousLevelName = "first_scene";

    //Battle Data
    [SerializeField] private BattleDataAsset m_battleData = null;
    
    //USER DATA
    //Global data
    JSONObject m_gameData;

	override protected void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        base.Awake();
		_instance = this;
		LoadUserData ();
		m_allLoaded = true;
	}

	public void LoadUserData(){
		string tempStr = null;
		//GLOBAL DATA
		tempStr = PlayerPrefs.GetString ("gameData");
		if (tempStr!= "" && tempStr !=null) {
			m_gameData = new JSONObject(tempStr);
		}
	}

	#region SAVE

	public void SaveAll(){
		//m_characterWrapper.Save ();
		SaveGameData ();
	}

	public void SaveGameData(){
		PlayerPrefs.SetString ("gameData", m_gameData.Print());
	}

	#endregion

	#region DATABASE
	
	protected override void LoadDatabase(){
        base.LoadDatabase();
	}

    #endregion

    #region CHARACTER_LOADING

    public GameObject CreateCharacter(ProfileManager.CharacterData _charaData)
    {
        var charManager = DataManager.instance.CharacterManager;
        //load prefab
        GameObject go = Instantiate(Resources.Load("prefabs/character/character_parts")) as GameObject;
        CharacterBuild build = go.GetComponent<CharacterBuild>();
        
        //compute all around stats from the database
        var stats = charManager.ComputeStats(_charaData);
        build.gameObject.SetActive(true);
        //load appearance
        build.Load(_charaData);
        return go;
    }

    #endregion
    #region PROPERTIES

    public static DataManager instance {
		get{
			if( _instance == null ){
                GameObject newGO = Instantiate(Resources.Load("prefabs/DataManager")) as GameObject; ;
                _instance = newGO.GetComponent<DataManager>();
                newGO.name = "DataManager";
			}
			return _instance;
		}
	}

    public BattleDataAsset BattleData
    {
        get { return m_battleData; }
        set { m_battleData = value; }
    }

	public JSONObject GameData {
		get {
			if( m_gameData == null ){			
				m_gameData = new JSONObject ();
			}
			return m_gameData;
		}
	}
    
	public bool IsLoaded {
		get {
			return m_allLoaded;
		}
	}

    public DataCharManager CharacterManager
    {
        get { return GetComponent<DataCharManager>(); }
    }

    public DataEnemiesManager EnemiesManager
    {
        get { return GetComponent<DataEnemiesManager>(); }
    }

    public DataInventoryManager InventoryManager
    {
        get { return GetComponent<DataInventoryManager>(); }
    }

    public DataGameManager GameDataManager
    {
        get { return GetComponent<DataGameManager>(); }
    }

    #endregion
}
