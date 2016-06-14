﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ProfileManager : MonoBehaviour {

    private static ProfileManager _instance;

    public Profile profile;
    
    public static ProfileManager instance{
        get{
            if(_instance == null)
            {
                GameObject go = Instantiate(Resources.Load("prefabs/Profile") as GameObject);
                _instance = go.GetComponent<ProfileManager>();
            }
            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        _instance = this;
        PlayerPrefs.DeleteAll();
        LoadProfile();
    }

    void Update()
    {
    }

    #region PROFILE_SAVE_LOAD
    public void ResetProfile()
    {
        LoadDefaultProfile();
    }

    void LoadProfile()
    {
        string json = PlayerPrefs.GetString("profile");
        if (string.IsNullOrEmpty(json))
        {
            //Load profile by default
            LoadDefaultProfile();
            return;
        }
        //PArse JSON
        profile = JsonUtility.FromJson<Profile>(json);
    }

    public void SaveProfile()
    {
        string json = JsonUtility.ToJson(profile);
        PlayerPrefs.SetString("profile", json);
        Debug.Log("[Saved Profile] " + json);
    }

#if UNITY_EDITOR
    public void SaveDefaultProfile()
    {
        string path = Application.dataPath + "/Resources/database/defaultProfile.json";
        string text = JsonUtility.ToJson(profile);

        File.WriteAllText(path, text);
        AssetDatabase.Refresh();
    }
#endif

    void LoadDefaultProfile()
    {
        TextAsset json = Resources.Load("database/defaultProfile") as TextAsset;
        profile = JsonUtility.FromJson(json.text, typeof(Profile)) as Profile;
        Resources.UnloadAsset(json);
    }

    #endregion

    public Profile GetProfile()
    {
        return profile;
    }

    #region CHARACTERS

    public CharacterData GetCharacter(string _id)
    {
        foreach (var chara in profile.Characters)
        {
            if (chara.Id == _id)
                return chara;
        }
        return null;
    }

    public void AddCharacterXp(string _id, int _xp)
    {
        var charaSave = GetCharacter(_id);
        if( charaSave != null)
        {
            charaSave.Xp += _xp;
        }
    }

    public List<CharacterData> GetCurrentTeam()
    {
        List<CharacterData> chars = new List<CharacterData>();
        foreach (string teamMateName in profile.CurrentTeam)
        {
            var charaSave = GetCharacter(teamMateName);
            if (charaSave != null)
                chars.Add(charaSave);
        }
        return chars;
    }

    #endregion

    [System.Serializable]
    public class Profile
    {
        [SerializeField] public string PlayerName = "player";
        [SerializeField] public int Xp = 0;
        [SerializeField] public bool Initialized = false;

        [SerializeField] public List<CharacterData> Characters = new List<CharacterData>();

        [SerializeField] public List<string> CurrentTeam;

        public Profile()
        {
            Characters = new List<CharacterData>()
            {
                new CharacterData("player1"),
                new CharacterData("player2"),
                new CharacterData("player3")
            };

            CurrentTeam = new List<string>() { "player1", "player3", "player1" };
        }

    }

    [System.Serializable]
    public class CharacterData
    {
        public string Id;
        public string Name = "temp";
        public int Xp = 0;
        public Stats baseStats = new Stats();
        public string Category = "warrior";
        //public CharacterBuild Build; // skins
        //Equipment
        public CharacterData(string _id)
        {
            Id = _id;
        }
    }
}