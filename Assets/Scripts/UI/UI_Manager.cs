using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using static Tags;
using UnityEngine.EventSystems;
using static Unity.Collections.AllocatorManager;
using System.ComponentModel;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using static Jobs;
using static AgentManager;
// Ce script est le UI_Manager, il permet de g�rer l'interface utilisateur du jeu.
// Il permet d'afficher ou de cacher des �l�ments, de changer du texte, de changer des images, de d�placer des �l�ments � l'�cran.
public class UI_Manager : MonoBehaviour
{
    [Header("Ecran de chargement")]
    public GameObject m_LoadingIcone;  // L'�cran de chargement
    [Header("M_GoldLines")]
    public GameObject m_GoldLines;
    [Header("Boutons")]
    public GameObject m_Bouton_Generer;  // Le bouton sur lequel on va cliquer pour g�n�rer la campagne
    public GameObject m_Bouton_Recommencer;  // Le bouton sur lequel on va cliquer pour recommencer la campagne
    public GameObject m_Bouton_Commencer;  // Le bouton sur lequel on va cliquer pour Commencer la campagne
    public GameObject m_Bouton_Continuer;  // Le bouton sur lequel on va cliquer pour continuer la campagne
    public GameObject m_Bouton_Poursuivre;  // Le bouton sur lequel on va cliquer pour poursuivre dans une nouvelle salle

    [Header("Images IA")]
    public GameObject m_ImageIA_Object;
    public RawImage m_ImageIA;  // Le composant RawImage o� l'image g�n�r�e sera affich�e

    [Header("Texte IA")]
    public GameObject m_TextIA_Object;  // La zone o� s'affichera la r�ponse
    public GameObject m_TextIA_Contexte;  // La zone o� s'affichera la r�ponse
    public GameObject m_TextIA_TextIntro;  // La zone o� s'affichera la r�ponse
    public Text m_TextIA;
    public string m_AllText;  // Tous les textes g�n�r�s
    public Vector2 m_PositionTexte_Initial;
    public Vector2 m_PositionTexte_PresentationSynopsis;
    public Vector2 m_PositionTexte_AvecImage;

    [Header("Input")]
    public GameObject m_Input_Object;
    public TMP_InputField m_Input_Object_Glob_;
    public GameObject m_Input_NonCoherent;
    public GameObject m_ChargementInput;
    public TextMeshProUGUI m_InputText;
    public Button m_ButtonEnvoie;
    public TextMeshProUGUI m_IncoherenceInfo;
    public GameObject m_ExplicationSupplementairesBouttonSend_Exploration;
    public GameObject m_ButtonSendPrompt_Exploration;

    [Header("Canvas Map")]
    public GameObject mapCanvas;

    [Header("Canvas Character")]
    public GameObject m_AllCanvas;
    public GameObject Canvas_InventaireA;
    public GameObject Canvas_InventaireB;
    public GameObject Canvas_InventaireC;
    public GameObject Canvas_InventaireD;

    //public GameObject[] m_CanvasCharacters = new GameObject[4];
    //public GameObject[] m_ColorCharacters = new GameObject[4];
    public Button[] m_ButtonsCharacters = new Button[4];
    public Text[] m_TextObjectCharacters = new Text[4];
    public string[] m_TextCharacter;
    public TextMeshProUGUI m_TextAuTourDe;
    private bool isCheckingCoherence = false;

    [Header("Groups")]
    public GameObject[] m_CanvasCharacter = new GameObject[4];
    public GameObject m_CurrentCanvasCharacter;
    public GameObject m_CurrentCanvasComposition;
    public int m_IndexCurrentGroupOnDisplay = 0;

    [Header("Combat")]
    public GameObject m_ExplicationDice;
    public GameObject m_Canvas_ChargementCombat;
    public GameObject m_TexteEnnemisReperent;
    public GameObject m_TexteHerosReperent;
    [Header("Combat")]
    public GameObject m_CanvasCombat;
    public GameObject m_CanvasCombatHero;
    public GameObject m_CanvasCombatEnnemi;
    public GameObject[] m_CanvasCombatHeroIndividual;
    public GameObject[] m_CanvasCombatEnnemiIndividual;
    public GameObject m_ButtonChangerSoinAttack;
    public GameObject m_ButtonContinuerNarration;
    public GameObject m_ButtonContinuerCombatFini;
    public GameObject m_ButtonContinuerCloseCombat;
    public GameObject m_ButtonSendPrompt;
    public GameObject m_AreaInputPrompt;
    public GameObject m_InputPromptCombat;
    public TMP_InputField m_InputPromptTextGlob_;
    public GameObject m_TextExplication;
    public GameObject m_TextAuTourDeCombat;
    public GameObject m_TextNarration;
    public GameObject m_ExplicationSupplementairesBouttonSend;
    public TextMeshProUGUI m_Text_ExplicationsSupplementairesBouttonSend;
    public GameObject m_IconeChargementCombat;
    public GameObject m_Incoherence;
    public TextMeshProUGUI m_TextIncoherence;
    public GameObject m_ChargementEcritureCombat;
    public GameObject m_MoreExplicationDice;
    public GameObject m_Txt_Action;
    public TextMeshProUGUI m_PlaceHolderPrompt;

    public GameObject m_CanvasCorruption;



    [Header("Options")]
    public Animator m_PannelOptionAnimation;
    [Header("Je sais pas")]
    public GameObject ConfirmUseItem;
    public GameObject FinDeTest;
    [Header("Option")]
    public GameObject m_CanvasOption;
    public TMP_Dropdown m_DropdownTextLLM; // On modifiera l'option séléctionnée si jamais on change en cours de partie de llm.
    public Vector3[] m_PositionCanvasOption;

    [Header("Tutoriel")]
    public GameObject m_Canvas_Tutoriel;
    public GameObject m_Canvas_Tutoriel_part1;
    public GameObject m_Canvas_Tutoriel_part2;
    public GameObject m_Canvas_Tutoriel_part3;
    public GameObject m_Canvas_Tutoriel_part4;
    public GameObject m_Canvas_Tutoriel_part5;

    [Header("Fin")]
    public GameObject m_Canvas_Fin;
    public TextMeshProUGUI m_txt_TitreFin;
    public TextMeshProUGUI m_txt_SousTitreFin;
    public TextMeshProUGUI m_txt_NarrationFin;
    public Image m_ImageFin;
    public GameObject m_Canvas_Formulaire;
    public Sprite m_ImageMauvaiseFinParDefault;
    public Sprite m_ImageBonneFinParDefault;

    [Header("Supplication")]
    public TextMeshProUGUI m_TextSupplie;
    public GameObject Canvas_BouttonSupplieMoi;

    public enum TextType
    {
        Histoire,
        Event
    }


    private GameManager _gameManager;
    private CharactersManagers _charactersManagers;
    private HistoryManager _historyManager;
    private GroupsManager _groupsManager;
    public void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _charactersManagers = _gameManager._charactersManagers;
        _groupsManager = _gameManager._groupsManager;


        _historyManager = _gameManager._historyManager;
        //m_TextIA_Object.SetActive(false);
        //m_TextIA_Contexte.SetActive(false);
        m_Bouton_Commencer.SetActive(false);
        m_Bouton_Continuer.SetActive(false);
        m_Bouton_Recommencer.SetActive(false);
        m_ButtonEnvoie.gameObject.SetActive(false);
        m_Bouton_Poursuivre.SetActive(false);
        m_Input_Object.SetActive(false);
        m_Input_NonCoherent.SetActive(false);
        m_ChargementInput.SetActive(false);
        m_AllCanvas.SetActive(false);
        mapCanvas.SetActive(false);
        int nbrHero = _charactersManagers.GetNumberOfHeroes();
        m_TextCharacter = new string[nbrHero];
        m_TextAuTourDe.gameObject.SetActive(false);
        m_Canvas_Fin.SetActive(false);
        FinDeTest.SetActive(false);
        //StartCoroutine(generateSupplication());
    }

    /// <summary>
    /// Changer le texte de l'IA qui se trouve � l'�cran
    /// </summary>
    /// <param name="new_text"></param>
    public void Changer_Texte_Contexte(string new_text)
    {
        m_TextIA_Contexte.GetComponent<Text>().text = new_text;
    }

    /// <summary>
    /// Afficher l'écran de chargement et cacher les autres éléments
    /// </summary>
    public void Show_Loading_Screen()
    {

        m_TextIA_Contexte.SetActive(false); 
        m_TextIA_TextIntro.SetActive(false);
        m_ImageIA_Object.SetActive(false); 
        m_Bouton_Generer.SetActive(false); 
        m_Bouton_Recommencer.SetActive(false); 
        m_Bouton_Commencer.SetActive(false); 
        m_Bouton_Continuer.SetActive(false);
        m_LoadingIcone.SetActive(true);
        m_Bouton_Poursuivre.SetActive(false);
        m_Input_Object.SetActive(false);
        m_Input_NonCoherent.SetActive(false);
        m_ButtonEnvoie.gameObject.SetActive(false);
        m_TextAuTourDe.gameObject.SetActive(false);
        m_TextAuTourDe.gameObject.SetActive(false);
        m_ChargementInput.SetActive(false);
        m_AllCanvas.SetActive(false);
        m_GoldLines.SetActive(false);
        mapCanvas.SetActive(false);
        ConfirmUseItem.SetActive(false);
        m_CanvasOption.SetActive(false);
    }

    public void SetCheckCoherenceInput(bool tf)
    {
        isCheckingCoherence = tf;
    }

    /// <summary>
    /// Cacher l'�cran de chargement
    /// </summary>
    public void Hide_Loading_Screen()
    {
        m_LoadingIcone.SetActive(false);
        m_CanvasOption.SetActive(true);
    }
    /// <summary>
    /// Afficher le texte de l'IA si true, cacher le texte si false
    /// </summary>
    /// <param name="tf"></param>
    /// 
    public void Afficher_Texte_Contexte(bool tf)
    {
        m_TextIA_Contexte.SetActive(tf);
    }

    public void Afficher_InputNonCoherent(bool tf)
    {
        m_Input_NonCoherent.SetActive(tf);
    }

    public void Afficher_BoutonEnvoie(bool tf)
    {
        m_ButtonEnvoie.gameObject.SetActive(tf);
    }
    public void Afficher_InputManager(bool tf)
    {
        m_Input_Object.SetActive(tf);
    }

    public void Afficher_ConfirmUseItem(bool tf)
    {
        ConfirmUseItem.SetActive(tf);
    }
    /// <summary>
    /// Permet d'afficher ou de cacher le texte de l'IA
    /// </summary>
    /// <param name="tf"></param>
    public void Afficher_Texte_IA(bool tf)
    {
        m_TextIA_Object.SetActive(tf);
    }
    /// <summary>
    /// Afficher l'image de l'IA si true, cacher l'image si false
    /// </summary>
    /// <param name="tf"></param>
    public void Afficher_Image_IA(bool tf)
    {
        m_ImageIA_Object.SetActive(tf);
    }
    /// <summary>
    /// Afficher le bouton g�n�rer si true, cacher le bouton g�n�rer si false
    /// </summary>
    /// <param name="tf"></param>
    public void Afficher_Bouton_Generer(bool tf)
    {
        m_Bouton_Generer.SetActive(tf);
    }
    /// <summary>
    /// Afficher le bouton commencer si true, cacher le bouton commencer si false
    /// </summary>
    /// <param name="tf"></param>
    public void Afficher_Bouton_Commencer(bool tf)
    {
        m_Bouton_Commencer.SetActive(tf);
    }
    /// <summary>
    /// Afficher le bouton recommencer si true, cacher le bouton recommencer si false
    /// </summary>
    /// <param name="tf"></param>
    public void Afficher_Bouton_Recommencer(bool tf)
    {
        m_Bouton_Recommencer.SetActive(tf);
    }

    /// <summary>
    /// Afficher le bouton continuer si true, cacher le bouton continuer si false
    /// </summary>
    /// <param name="tf"></param>
    public void Afficher_Bouton_Continuer(bool tf)
    {
        m_Bouton_Continuer.SetActive(tf);
    }
    public void Afficher_Bouton_Poursuivre(bool tf)
    {
        m_Bouton_Poursuivre.SetActive(tf);
    }

    public void SetIncoherenceInfo(string txt)
    {
        m_IncoherenceInfo.text = txt;
    }

    public void HideInventaire(string inventaire)
    {
        foreach (var character in inventaire)
        {
            switch (character)
            {
                case 'A' or 'a':
                    Canvas_InventaireA.SetActive(false);
                    break;
                case 'B' or 'b':
                    Canvas_InventaireB.SetActive(false);
                    break;
                case 'C' or 'c':
                    Canvas_InventaireC.SetActive(false);
                    break;
                case 'D'or'd':
                    Canvas_InventaireD.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Permet de justifier le texte vers le bas de l'�cran
    /// </summary>
    public void JustifierLeCodeVersLeBas()
    {
        m_CurrentCanvasComposition.transform.GetChild(m_IndexCurrentGroupOnDisplay).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        m_CurrentCanvasComposition.transform.GetChild(m_IndexCurrentGroupOnDisplay).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        m_CurrentCanvasComposition.transform.GetChild(m_IndexCurrentGroupOnDisplay).transform.GetChild(1).GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
        //m_contentSizeFitter[nbr].verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        //m_contentSizeFitter[nbr].verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        Canvas.ForceUpdateCanvases();
        //m_scrollRect[nbr].verticalNormalizedPosition = 0f;


    }


    public void AfficherMap(bool tf)
    {
        mapCanvas.SetActive(tf);
    }

    public void SwitchGroupCanvas(int indexGroupWeWantWatch)
    {
        hideAll();
        AfficherMap(true);
        m_AllCanvas.SetActive(true);
        m_GoldLines.SetActive(true);
        m_CurrentCanvasCharacter.SetActive(true); // Le canvas p�re
        m_CurrentCanvasComposition.SetActive(true); // Le canvas enfant ( = le canvas de la composition)
        int currentGroupToPlay = _groupsManager.m_CurrentGroupIndexOnDisplay;
        m_IndexCurrentGroupOnDisplay = indexGroupWeWantWatch;
        _gameManager._mapManager.rebuildVector(new Vector2Int(0, 0));
        GroupsManager _gm = _gameManager._groupsManager;
        _gm.m_CurrentGroupIndexOnDisplay = indexGroupWeWantWatch;
        for (int i = 0; i < _gm.m_GroupsCount; i++)
        {
            _gm.m_Groups[i].AfficherText(i == indexGroupWeWantWatch);
            //On va donn� une apparence "s�lectionn�" au bouton du personnage s�lectionn� et une apparence non-s�lectionn� aux autres
            _gm.m_Groups[i].showSelectionned(i == indexGroupWeWantWatch);
            //_groupsManager.m_Groups[i].m_Canvas.SetActive(true);
            for (int j = 0; j < _gm.m_Groups[i].m_PlayerCount; j++)
            {
                Color color = new Color(0.29f, 0.29f, 0.29f,1f);
                _gm.m_Groups[i].m_Canvas.transform.GetChild(0).transform.GetChild(j).transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().color = i == indexGroupWeWantWatch ? Color.white : color;
            }

        }
        if (_gm.m_Groups[indexGroupWeWantWatch].isReady()) // Si le group s�lectionn� est ready
        {
            
            m_TextAuTourDe.gameObject.SetActive(false);
            if (isCheckingCoherence)
            {
                Afficher_InputNonCoherent(false);
                Afficher_BoutonEnvoie(false);
                m_ChargementInput.SetActive(true);
                        
            }
            else
            {
                Afficher_InputManager(true);
                Afficher_BoutonEnvoie(true);
                
                m_ChargementInput.SetActive(false);
                //Comment Afficher CanSendPromptButton?
                CanSendPromptButton(_gm.m_Groups[indexGroupWeWantWatch].SomeoneHasMoved());
            }
        }
        else
        {
            if (_groupsManager.m_Groups[indexGroupWeWantWatch].m_PlayerCount == 1) { m_TextAuTourDe.text = $"{_groupsManager.m_Groups[indexGroupWeWantWatch].GetAllNames()} accomplit sa tâche."; }
            else { m_TextAuTourDe.text = _groupsManager.m_Groups[indexGroupWeWantWatch].GetAllNames() + " accomplissent leur tâche."; }
            m_TextAuTourDe.gameObject.SetActive(true);
        }

        mapCanvas.SetActive(true);
        _gameManager._mapManager.ChangeColorHeroTile();
        JustifierLeCodeVersLeBas();
        JustifierLeCodeVersLeBas();
    }


    private void hideAll()
    {
        Afficher_InputManager(false);
        m_TextAuTourDe.gameObject.SetActive(false);
        m_ChargementInput.SetActive(false);
        Afficher_BoutonEnvoie(false);
        Afficher_Bouton_Poursuivre(false);
        m_TextAuTourDe.gameObject.SetActive(false);
        for(int i = 0;i < m_CanvasCharacter.Length; i++)
        {
            m_CanvasCharacter[i].SetActive(false);
        }
    }


    public void SetTriggerAnimation(bool needToUp)
    {
        //GameObject blocker = GameObject.Find("Blocker");
        //if (blocker == null)
        //{
            m_PannelOptionAnimation.SetBool("needToUp", needToUp);
        //}
    }

    public void CanSendPromptButton(bool canSend)
    {
        if (canSend)
        {
            m_ButtonSendPrompt_Exploration.GetComponent<Image>().color = new Color32(199, 127, 60, 255);
        }
        else
        {
            m_ButtonSendPrompt_Exploration.GetComponent<Image>().color = new Color32(46, 23, 0, 255);
        }
    }

    public void OnTriggerButtonSend(bool enter)
    {
        if(m_ButtonSendPrompt_Exploration.GetComponent<Image>().color == new Color32(46, 23, 0, 255))
        {
            m_ExplicationSupplementairesBouttonSend_Exploration.SetActive(enter);
        }
    }


    #region Combat
    public void UpdateNarrationText(string txt)
    {
        m_TextNarration.GetComponent<TextMeshProUGUI>().text = txt;
    }
    public void AfficherNarrationText(bool tf)
    {
        m_TextNarration.SetActive(tf);
    }
    public void AfficherButtonChangerSoinAttack(bool tf)
    {
        m_ButtonChangerSoinAttack.SetActive(tf);
    }
    public void UpdateAuTourde(string txt)
    {
        m_TextAuTourDeCombat.GetComponent<TextMeshProUGUI>().text = txt;
    }
    public void UpdateExplicationText(string txt)
    {
        m_TextExplication.GetComponent<TextMeshProUGUI>().text = txt;
    }
    public void AfficherButtonContinuerNarration(bool tf)
    {
        m_ButtonContinuerNarration.SetActive(tf);
    }
    public void AfficherButtonContinuerCombatFini(bool tf)
    {
        m_ButtonContinuerCombatFini.SetActive(tf);
    }
    public void AfficherButtonContinuerCloseCombat(bool tf)
    {
        m_ButtonContinuerCloseCombat.SetActive(tf);
    }
    public void AfficherSendPrompt(bool tf)
    {
        m_ButtonSendPrompt.SetActive(tf);
        m_AreaInputPrompt.SetActive(tf);
    }
    public void AfficherAuTourDeExplication(bool tf)
    {
        m_TextAuTourDeCombat.SetActive(tf);
        m_TextExplication.SetActive(tf);
    }   
    public void Show_More_Explication_SenddingButton(bool tf)
    {
        m_ExplicationSupplementairesBouttonSend.SetActive(tf);
    }
    public void UpdateTextExplicationsSupplementaireBouttonSend(string txt)
    {
        m_Text_ExplicationsSupplementairesBouttonSend.text = txt;
    }
    public void AfficherIconeChargementCombat(bool tf)
    {
        m_IconeChargementCombat.SetActive(tf);
    }
    public void AfficherIncoherence(bool tf)
    {
        m_Incoherence.SetActive(tf);
    }
    public void UpdateTextIncoherenceCombat(string txt)
    {
        m_TextIncoherence.text = txt;
    }
    public void Show_Combat_Loading_Screen(bool tf)
    {
        m_ChargementEcritureCombat.SetActive(tf);
    }
    public void Show_More_Explication_Dice(bool tf)
    {
        m_MoreExplicationDice.SetActive(tf);
    }
    public void Update_MoreExplicationDice(string txt)
    {
        m_MoreExplicationDice.GetComponent<TextMeshProUGUI>().text = txt;
    }
    public void Show_Action_Text(bool tf)
    {
        m_Txt_Action.SetActive(tf);
    }
    public void Update_Txt_Action(string txt)
    {
        m_Txt_Action.GetComponent<TextMeshProUGUI>().text = txt;
    }
    public void Update_Txt_PlaceHolderPrompt(string txt)
    {
        m_PlaceHolderPrompt.text = txt;
    }
    public void Show_Corruption_Canvas(bool tf)
    {
        m_CanvasCorruption.SetActive(tf);
    }

    public void ChangePositionOptionCanvas(bool isCombat)
    {
        if(isCombat)
        {
            m_CanvasOption.transform.position = m_PositionCanvasOption[1];
            m_CanvasOption.GetComponent<Canvas>().sortingOrder = 50;
        }
        else
        {
            m_CanvasOption.GetComponent<RectTransform>().anchoredPosition = m_PositionCanvasOption[0];
            m_CanvasOption.GetComponent<Canvas>().sortingOrder = 0;
        }
    }
    #endregion

    #region Qurstionnaire Supplication

    IEnumerator generateSupplication()
    {
        string[] prompt = { "Supplie-moi, mais avec un ton plus grave.",
                            "Supplie-moi, mais comme si tu étais le Parrain de la mafia, et que tu faisais comprendre qu'il faut remplir ce formulaire.",
                            "Supplie-moi, mais en étant plus convaincant, en me promettant que tu m'offriras plein de choses en échange.",
                            "Supplie-moi, sur un ton très comique, en me faisant rire.",
                            "Supplie-moi, mais en étant très sérieux, comme si ta vie en dépendait.",
                            "Supplie-moi, mais en étant très poli, comme si tu étais un gentleman.",
                            "Supplie-moi, mais en étant très triste, comme si tu avais perdu tout espoir.",
                            "Supplie-moi, mais en étant très heureux, comme si tu venais de gagner à la loterie.",
                            "Supplie-moi, mais en étant très mystérieux, comme si tu cachais un secret.",
                            "Supplie-moi, mais en étant très convaincant, comme si tu avais vraiment besoin de moi.",
                            "Supplie-moi, mais en étant très sincère, comme si tu parlais avec ton cœur.",
                            "Supplie-moi, mais en étant très poétique, comme si tu écrivais un poème.",
                            "Supplie-moi, mais en étant très philosophique, comme si tu voulais me faire réfléchir.",

        };
        int randomIndex = Random.Range(0, prompt.Length);
        string selectedPrompt = prompt[randomIndex];
        bool done = false;
        string _textGenerer = "";
        string _textParser = "";
        while (!done)
        {
            yield return StartCoroutine(_gameManager._agent.Generate_Text(Models.Model.gemma2_9b_it, Job.Supplication, selectedPrompt, (response) =>
            {
                _textGenerer = response;
            }));

            done = Parser.BEGGING(_textGenerer, out _textParser);
            if (!done)
            {
                yield return new WaitForSeconds(1f);
            }
        }
        m_TextSupplie.text = _textParser;
        yield return new WaitForSeconds(1f);
        Canvas_BouttonSupplieMoi.SetActive(true);
    }

    public void _OnClicSupplieMoiEncore()
    {
        Canvas_BouttonSupplieMoi.SetActive(false);
        StartCoroutine(generateSupplication());
    }

    public void _OnClicQuestionnairePrincipal()
    {
        Application.OpenURL("https://forms.office.com/e/cw4wtMW27w");
    }
    public void _OnClicQuestionnaireNarration()
    {
        Application.OpenURL("https://forms.office.com/e/eex83GgeCK");
    }
    public void _OnClicQuestionnaireCombat()
    {
        Application.OpenURL("https://forms.office.com/e/NZr3zK3dyB");
    }
    #endregion
    #region Tutoriel
    public void ExitTutoriel()
    {
        m_Canvas_Tutoriel.SetActive(false);
    }
    public void ContinuerTutorial(int i)
    {
        switch (i)
        {
            case 0:
                m_Canvas_Tutoriel_part1.SetActive(false);
                m_Canvas_Tutoriel_part2.SetActive(true);
                break;
            case 1:
                m_Canvas_Tutoriel_part2.SetActive(false);
                m_Canvas_Tutoriel_part3.SetActive(true);
                break;
            case 2:
                m_Canvas_Tutoriel_part3.SetActive(false);
                m_Canvas_Tutoriel_part4.SetActive(true);
                break;
            case 3:
                m_Canvas_Tutoriel_part4.SetActive(false);
                m_Canvas_Tutoriel_part5.SetActive(true);
                break;
            case 4:
                m_Canvas_Tutoriel_part5.SetActive(false);
                m_Canvas_Tutoriel.SetActive(false);
                break;
            default:
                break;
        }   
    }
    #endregion


}
