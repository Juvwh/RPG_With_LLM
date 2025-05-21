using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UI_Manager;
using static Tags;

public class CharactersManagers : MonoBehaviour
{
    private static CharactersManagers instance;
    [SerializeField]
    public PlayerData[] _Characters = new PlayerData[4];//stocke les info des 4 héros pris pour l'aventure
    private GameManager _gameManager;
    public Models.Model m_currentModel = Models.Model.llama_3_3_70b_versatile;
    public int _nbrOfCharacters = 0;
    public string m_CurrentTheme;


    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject); // Rend l'objet persistant entre les scènes
    }
    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }
    public void SetGameManager(GameManager gameManager)
    {
        _gameManager = gameManager;
    }
    public int GetNumberOfHeroes()
    {
        return _Characters.Length;
    }
    public void SetCharacters(PlayerData characters, int nbr)
    {
        characters.playerpositionOnUI = nbr;
        _Characters[nbr] = characters;
    }
    public PlayerData GetPlayer(int nbr)
    {
        return _Characters[nbr];
    }
    public PlayerData[] GetAllPlayers()
    {
        return _Characters;
    }

    #region Method Get Components Of Characters
    public string GetName(int nbr)
    {
        return _Characters[nbr].playerName;
    }
    public string GetResume(int nbr)
    {
        return _Characters[nbr].playerResume;
    }  
    public void SetPosition(Coord coord, int nbr)
    {
        _Characters[nbr].playerPosition = coord;
    }
    public Coord GetPosition(int nbr)
    {
        return _Characters[nbr].playerPosition;
    }
    public Coord GetPreviousCoord(int nbr)
    {
        return _Characters[nbr].playerPreviousCoord;
    }
    public void SetPreviousCoord(Coord coord, int nbr)
    {
        _Characters[nbr].playerPreviousCoord = coord;
    }
    public void SetHasMoved(bool hasMoved, int nbr)
    {
        _Characters[nbr]._hasMoved = hasMoved;
    }
    public int GetVision(int nbr)
    {
        return _Characters[nbr].playerVision;
    }
    #endregion
    #region Useful Methods
    public string GetAllCharactersNameAndDescriptions()
    {
        string toReturn = "";
        for (int i = 0; i < _Characters.Length; i++)
        {
            string characterIntroTemplate = _gameManager._language.GetPrompt("__createPrompt__Generate_CharactersDescriptions_Introduction__", new Dictionary<string, string>
                {
                    { "0", (i + 1).ToString() },
                    { "1", GetName(i) },
                    { "2", Parser.CLEAN(GetResume(i)) }
                });

            toReturn += characterIntroTemplate;
        }
        return toReturn;
    }
    public void RemoveNullCharacters()
    {
        List<PlayerData> nonNullCharacters = new List<PlayerData>();
        foreach (var character in _Characters)
        {
            if (character != null)
            {
                nonNullCharacters.Add(character);
            }
        }
        _Characters = nonNullCharacters.ToArray();
        _nbrOfCharacters = _Characters.Length;
    }
    #endregion

}