using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SerializablePlayerData
{
    public string playerName;
    public string playerAge;
    public string playerRace;
    public string playerClass;
    public string playerAdvantage;
    public string playerDisadvantage;
    public string playerDescription;
    public string playerResume;
    public string playerArme;
    public string playerArmePuissance;
    public Sprite playerSprite;
    public List<string> playerInventaire;
    public Coord playerPosition;
    public string playerHealth;
    public int playerHealthNbr;
    public List<string> playerActions;
    public List<Tags.Tag> playerTags;
    public int playerVision;
}
