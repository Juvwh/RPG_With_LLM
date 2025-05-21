using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UI_Manager;
using static Tags;
using UnityEngine.SocialPlatforms.Impl;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using System;
using UnityEngine.Windows;

public class PlayerData : MonoBehaviour
{
    #region Player Basic Information
    // Basic information about the player character
    public string playerName;
    public string playerAge;
    public string playerRace;
    public string playerClass;
    public string playerAdvantage;
    public string playerDisadvantage;
    public string playerDescription;
    public string playerResume;
    public Sprite playerSprite;
    #endregion

    #region Combat Attributes
    // Attributes related to combat
    public string playerArme = "";
    public int playerArmePuissance;
    public int playerArmorClass; // An enemy must roll >= playerArmorClass to hit the hero
    public int playerAimBonus; // +2, 0, -2
    public int nbrOfAttackBonus = 0; // 1 if there is 1 active potion -> 1d6 becomes 1d8
    #endregion

    #region Position and UI
    // Position and UI-related attributes
    public int playerpositionOnUI;
    public int playerIndex; // The player's index in the group
    public Coord playerPosition;
    public Coord playerPreviousCoord;
    #endregion

    #region Health and Status
    // Health and status-related attributes
    public string playerHealth;
    public int playerHealthNbr;
    public bool playerIsReady = false;
    public bool playerIsAlive = true;
    public bool CanBeSelectedToAttack = true;
    public bool hasAttacked = false;
    #endregion

    #region History and Actions
    // History and actions-related attributes
    public List<string> playerActions; // A list of all actions taken by the character during the adventure
    public string playerLastAction; // The last event performed by the character
    public List<History> playerAllHistory = new List<History>(); // The entire history of this character during the adventure
    public List<HistoryResume> playerAllHistoryResume = new List<HistoryResume>(); // The entire history of this character during the adventure but summarized
    public string playerLastPhase; // The last phase of the character's history
    public string playerLastHistoryResume; // The last summary of the character's history
    public List<string> playerLastNews; // The latest news of the character received from other characters
    #endregion

    #region Tags
    // Tags related to the character's history
    public List<Tag> playerTags; // A list of all the history of this character during the adventure
    public Tag playerLastTag; // The last tag added to the character's history
    #endregion

    #region Objectives
    [Header("Objectif")]
    public string playerObjectif;
    #endregion

    #region Map
    [Header("Map")]
    public bool _hasMoved = false;
    public int _specialMove = 0;
    public int playerVision;
    #endregion

    #region Group
    [Header("Group")]
    public string groupSignature;
    public string previousGroupSignature;
    public string _groupID;
    public string _previousGroupID;
    public Group _playerGroup;
    #endregion

    #region Inventory
    [Header("Inventaire")]
    public List<string> playerInventaire;
    public int playerInventory_HealthPotions = 0;
    public int playerInventory_AttackPotions = 0;
    public int playerInventory_ArmorPotions = 0;
    public int playerInventory_Coins = 0;
    public int playerInventory_Shoes = 0;
    public int playerInventory_Glasses = 0;
    public int playerInventory_Compass = 0;
    public int playerInventory_Keys = 0;
    #endregion

    #region Player Choices
    public string[] playerChoices;
    #endregion

    #region Methods
    public void Start()
    {
        LocalizationManager _language = FindFirstObjectByType<LocalizationManager>();
        playerHealth = _language.GetText("_player_forme");
        playerHealthNbr = 20;
        playerArmorClass = 10;
        playerVision = 2;
        changeToArmPuissance();
    }
    public void CopyFrom(PlayerData other)
    {
        this.playerName = other.playerName;
        this.playerAge = other.playerAge;
        this.playerRace = other.playerRace;
        this.playerClass = other.playerClass;
        this.playerAdvantage = other.playerAdvantage;
        this.playerDisadvantage = other.playerDisadvantage;
        this.playerDescription = other.playerDescription;
        this.playerResume = other.playerResume;
        this.playerArmorClass = other.playerArmorClass;
        this.playerArmePuissance = other.playerArmePuissance;
    }
    public void AddNews(string news)
    {
        playerLastNews.Add(news);
    }
    public void AddAction(string action)
    {
        playerActions.Add(action);
    }
    public void AddHistory(TextType type, History _history)
    {
        playerAllHistory.Add(_history);
        if (type == TextType.Histoire)
        {

            playerLastPhase = _history.GetText();
        }
        else
        {
            playerAllHistory.Add(_history);
            playerLastAction = _history.GetText();
            //playerAllHistory += " \n \n <color=#00FF00>" + txt + "</color>";
        }

        //playerTextCanvas.text = playerAllHistory;
    }
    /*public void SetTextCanvas(UnityEngine.UI.Text text)
    {
        playerTextCanvas = text;
    }*/
    public void AddTag(Tag tag)
    {
        playerTags.Add(tag);
        playerLastTag = tag;
    }
    public void AddInventory(string item)
    {
        playerInventaire.Add(item);
    }
    public void RemoveInventory(string item)
    {
        playerInventaire.Remove(item);
    }
    public void SetHealth(string health)
    {
        playerHealth = health;
    }
    public void SetHealthNbr(int health)
    {
        playerHealthNbr = health;
    }
    public void SetLastAction(string action)
    {
        playerLastAction = action;
    }
    public void SetLastTag(Tag tag)
    {
        playerLastTag = tag;
    }
    public void SetName(string name)
    {
        playerName = name;
    }
    public void SetAge(string age)
    {
        playerAge = age;
    }
    public void SetAdvantages(string adv)
    {
        playerAdvantage = adv;
    }
    public void SetDisadvantages(string dis)
    {
        playerDisadvantage = dis;
    }
    public void SetDescription(string desc)
    {
        playerDescription = desc;
    }
    public void SetReady(bool ready)
    {
        playerIsReady = ready;
    }
    public void SetSprite(Sprite sprite)
    {
        playerSprite = sprite;
    }
    public void SetplayerLastHistoryResume(string txt)
    {
        playerLastHistoryResume = txt;
    }
    public void SetObjectif(string objectif)
    {
        playerObjectif = objectif;
    }
    public string GetLastHistoryResume()
    {
        return playerLastHistoryResume;
    }
    public string GetResume()
    {
        return playerResume;
    }
    public List<HistoryResume> GetAllHistoryResume()
    {
        return playerAllHistoryResume;
    }
    public void AddAllHistoryResume(HistoryResume resume)
    {
        playerAllHistoryResume.Add(resume);
    }
    public void SetResume(string resume)
    {
        playerResume = resume;
    }
    public Tag GetLastTag()
    {
        return playerLastTag;
    }
    public string GetLastAction()
    {
        return playerLastAction;
    }
    public List<string> GetAllInventaire()
    {
        return playerInventaire;
    }
    public string GetAllInventaireString()
    {
        string toReturn = "";
        if(playerInventaire.Count == 0)
        {
            return "Aucun objet dans l'inventaire";
        }
        foreach (string item in playerInventaire)
        {
            toReturn += item + "; ";
        }
        return toReturn;
    }
    public List<string> GetAllActions()
    {
        return playerActions;
    }
    public List<History> GetAllHistory()
    {
        return playerAllHistory;
    }
    public List<Tag> GetAllTags()
    {
        return playerTags;
    }
    public List<string> GetAllNews()
    {
        return playerLastNews;
    }
    public void SetChoices(string[] choices)
    {
        playerChoices = choices;
    }
    public string GetChoices(int index)
    {
        return playerChoices[index];
    }
    public Coord GetCoord()
    {
        return playerPosition;
    }
    public Coord GetPreviousCoord()
    {
        return playerPreviousCoord;
    }
    public void SetLastCoord(Coord coord)
    {
        playerPreviousCoord = coord;
    }
    public void SetCoord(Coord coord)
    {
        playerPosition = coord;
    }
    public int GetVision()
    {
        return playerVision;
    }
    public void SetVision(int vision)
    {
        playerVision = vision;
    }
    public void AddHistoryResume(HistoryResume history)
    {
        playerAllHistoryResume.Add(history);
        playerLastHistoryResume = history.GetResume();
    }
    public override string ToString()
    {
        return $"Name: {playerName}";
    }
    public void SetGroupID(string id)
    {
        _groupID = id;
    }
    public string GetAllHistoryResumeString()
    {
        string toReturn = "";
        foreach (HistoryResume hr in playerAllHistoryResume)
        {
            string txt = hr.GetResume();
            toReturn += hr + "; ";
        }
        return toReturn;

    }
    public int GetDiceWeapon()
    {
        changeToArmPuissance();
        return playerArmePuissance;
    }
    public void changeToArmPuissance()
    {
        switch (playerArmePuissance)
        {
            case 0:
                playerAimBonus = 2;
                playerArmePuissance = 4;
                break;
            case 1:
                playerAimBonus = 0;
                playerArmePuissance = 6;
                break;
            case 2:
                playerAimBonus = -2;
                playerArmePuissance = 8;
                break;
            default:
                if (playerArmePuissance == 4)
                {
                    playerAimBonus = 2;
                }else if (playerArmePuissance == 6)
                {
                    playerAimBonus = 0;
                }
                else if (playerArmePuissance == 8)
                {
                    playerAimBonus = -2;
                }
                break;
        }
    }
    public int GetInventoryHealthPotionsCount()
    {
        return playerInventory_HealthPotions;
    }
    public int GetInventoryAttackPotionsCount()
    {
        return playerInventory_AttackPotions;
    }
    public int GetInventoryArmorPotionsCount()
    {
        return playerInventory_ArmorPotions;
    }
    public int GetInventoryCoinsCount()
    {
        return playerInventory_Coins;
    }
    public int GetInventoryShoesCount()
    {
        return playerInventory_Shoes;
    }
    public int GetInventoryGlassesCount()
    {
        return playerInventory_Glasses;
    }
    public int GetInventoryCompassCount()
    {
        return playerInventory_Compass;
    }
    public int GetInventoryKeysCount()
    {
        return playerInventory_Keys;
    }
    public void AddInventoryHealthPotion(int count)
    {
        playerInventory_HealthPotions += count;
        _playerGroup.UpdateCanvasInventaire(this);
    }
    public void AddInventoryAttackPotion(int count)
    {
        playerInventory_AttackPotions += count;
        _playerGroup.UpdateCanvasInventaire(this);
    }
    public void AddInventoryArmorPotion(int count)
    {
        playerInventory_ArmorPotions += count;
        _playerGroup.UpdateCanvasInventaire(this);
    }
    public void AddInventoryCoins(int count)
    {
        playerInventory_Coins += count;
        _playerGroup.UpdateCanvasInventaire(this);
    }
    public void AddInventoryShoes(int count)
    {
        playerInventory_Shoes += count;
        _playerGroup.UpdateCanvasInventaire(this);
    }
    public void AddInventoryGlasses(int count)
    {
        playerInventory_Glasses += count;
        _playerGroup.UpdateCanvasInventaire(this);
    }
    public void AddInventoryCompass(int count)
    {
        playerInventory_Compass += count;
        _playerGroup.UpdateCanvasInventaire(this);
    }
    public void AddInventoryKeys(int count)
    {
        playerInventory_Keys += count;
        _playerGroup.UpdateCanvasInventaire(this);
    }
    public int[] GetAllItemsCount()
    {
        int[] toReturn = new int[8];
        toReturn[0] = playerInventory_HealthPotions;
        toReturn[1] = playerInventory_AttackPotions;
        toReturn[2] = playerInventory_ArmorPotions;
        toReturn[3] = playerInventory_Coins;
        toReturn[4] = playerInventory_Shoes;
        toReturn[5] = playerInventory_Glasses;
        toReturn[6] = playerInventory_Compass;
        toReturn[7] = playerInventory_Keys;
        return toReturn;
    }
    public void SetDead()
    {
        playerIsAlive = false;
        playerPosition.RemoveHero();
        playerPosition = new Coord(1000, 1000, Events.EventCoord.Nothing); // la position du mort.
    }
    #endregion
}
