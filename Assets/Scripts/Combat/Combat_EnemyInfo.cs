using TMPro;
using UnityEngine;

public class Combat_EnemyInfo : Combat_CharacterInfos
{
    [Header("Enemy")]
    public EnemyData m_EnemyLinked;
    public TextMeshProUGUI m_Txt_HealtTitle;
    public TextMeshProUGUI m_Txt_ArmorTitle;

    [Header("Explications Supplementaire")]
    public GameObject m_ExplicationSupplementaires;
    public TextMeshProUGUI m_TextExplicationSupplementaires;
    public GameObject m_Startegie;
    public TextMeshProUGUI m_TextStrategie;

    [Header("UI")]
    public GameObject m_DeathVector;

    [Header("Private")]
    private bool _isDark;
    public bool _isDead = false;
    private bool _showStrategie = false;

    [Header("Corruption")]
    public int _corruptionToursRestant = 0;
    public GameObject m_CorruptionIcon;
    public TextMeshProUGUI m_CorruptionText;

    /// <summary>
    /// Initializes the component by setting up UI elements and initial states.
    /// </summary>
    public void Start()
    {
        m_UI = this.gameObject;
        _gm = FindFirstObjectByType<GameManager>();
        m_ExplicationSupplementaires.SetActive(false);
        _isDark = false;
        m_DeathVector.SetActive(false);
    }

    /// <summary>
    /// Resets the enemy information and states.
    /// </summary>
    public void Reset()
    {
        m_EnemyLinked = null;
        m_IsSelectable = false;
        m_IsSelected = false;
        m_IsPlaying = false;
        _isDead = false;
        _corruptionToursRestant = 0;
        _showStrategie = false;
        m_CorruptionIcon.SetActive(false);
        m_DeathVector.SetActive(false);
    }

    /// <summary>
    /// Shows or hides the shadow effect on the enemy based on the given boolean.
    /// </summary>
    /// <param name="tf">True to show the shadow, false to hide it.</param>
    public override void ShowShadow(bool tf)
    {
        if (_isDead) { return; }
        if (tf) // Darken
        {
            m_SpriteCharacter.color = new Color(0.2f, 0.2f, 0.2f, 1);
            m_Cadre.color = new Color(m_Color.r, m_Color.g, m_Color.b, 0.3f);

            m_Text_Value_Name.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 0.3f);
            m_Text_Value_Sante.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 0.3f);
            m_Text_Value_Armure.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 0.3f);
            m_Txt_HealtTitle.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 0.3f);
            m_Txt_ArmorTitle.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 0.3f);
        }
        else // Reset to normal
        {
            m_SpriteCharacter.color = Color.white;
            m_Cadre.color = m_Color;
            m_Text_Value_Name.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 1);
            m_Text_Value_Sante.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 1);
            m_Text_Value_Armure.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 1);
            m_Txt_HealtTitle.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 1);
            m_Txt_ArmorTitle.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 1);
        }
        _isDark = tf;
    }

    /// <summary>
    /// Updates the displayed information for the enemy.
    /// </summary>
    public override void UpdateInformation()
    {
        m_Text_Value_Name.text = m_EnemyLinked._name;
        m_Text_Value_Sante.text = m_EnemyLinked._health.ToString();
        m_Text_Value_Armure.text = m_EnemyLinked._armor.ToString();
        m_SpriteCharacter.sprite = m_EnemyLinked._sprite;
        m_TextExplicationSupplementaires.text = m_EnemyLinked._description;
        _showStrategie = false;

        if (m_EnemyLinked._isDead)
        {
            m_DeathVector.SetActive(true);
            ShowShadow(true);
        }
    }

    /// <summary>
    /// Updates the strategy text for the enemy.
    /// </summary>
    /// <param name="tf">True to show the strategy, false to hide it.</param>
    /// <param name="_txt">The strategy text to display.</param>
    public void UpdateStrategie(bool tf, string _txt = "")
    {
        if (tf)
        {
            _showStrategie = tf;
            m_ExplicationSupplementaires.SetActive(false);
            m_Startegie.SetActive(true);
            m_TextStrategie.text = _txt;
        }
        else
        {
            _showStrategie = tf;
            m_Startegie.SetActive(false);
        }
    }

    /// <summary>
    /// Handles the trigger enter event for selection. Displays additional explanations and handles selection logic.
    /// </summary>
    public void OnTriggerEnterSelection()
    {
        if (_isDead) { return; }
        if (!_showStrategie) { m_ExplicationSupplementaires.SetActive(true); }

        // If it's selectable but darkened, when hovered over, it returns to color
        // If it's selectable and in color, it darkens
        if (m_IsSelectable && !m_IsSelected && !_gm._combatManager._isCorruption)
        {
            if (_gm._combatManager._selected == null) // If no enemy is selected, we will darken the enemy, otherwise we make it clear
            {
                ShowShadow(true);
            }
            else
            {
                ShowShadow(false);
            }
        }
        else if (_gm._combatManager._isCorruption)
        {
            ShowShadow(true);
        }
    }

    /// <summary>
    /// Handles the trigger exit event for selection. Hides additional explanations and handles deselection logic.
    /// </summary>
    public void OnTriggerExitSelection()
    {
        if (_isDead) { return; }
        m_ExplicationSupplementaires.SetActive(false);

        // If it's selectable but darkened, when hovered over, it returns to color
        // If it's selectable and in color, it darkens
        if (m_IsSelectable && !m_IsSelected && !_gm._combatManager._isCorruption)
        {
            if (_gm._combatManager._selected == null) // If no enemy is selected, we will make the enemy clear, otherwise we darken it
            {
                ShowShadow(false);
            }
            else
            {
                ShowShadow(true);
            }
        }
        else if (_gm._combatManager._isCorruption)
        {
            ShowShadow(false);
        }
    }

    /// <summary>
    /// Handles the click event for selecting the enemy. Updates the selection state and UI.
    /// </summary>
    public void OnClickToSelect()
    {
        if ((m_IsSelected && !_gm._combatManager._isCorruption) || _isDead) { return; } // If already selected, do nothing

        if (m_IsSelectable && !_gm._combatManager._isCorruption)
        {
            // If another is selected, deselect it and darken it
            if (_gm._combatManager._selected != null)
            {
                _gm._combatManager._selected.ShowShadow(true);
                _gm._combatManager._selected.m_IsSelected = false;
            }
            else
            {
                // Darken all other enemies
                foreach (EnemyData enemy in _gm._combatManager._enemies)
                {
                    Combat_EnemyInfo _e = enemy._ui.GetComponent<Combat_EnemyInfo>();
                    if (_e != this)
                    {
                        _e.ShowShadow(true);
                    }
                }
            }

            // Select this one instead
            m_IsSelected = true;
            ShowShadow(false);
            _gm._combatManager._selected = this;

            // We can now send the prompt if there is a selection
            _gm._combatManager.TurnSendPromptButton(true);
        }
        else if (_gm._combatManager._isCorruption)
        {
            _corruptionToursRestant += 1;
            m_CorruptionIcon.SetActive(true);
            m_CorruptionText.text = $"Corrompu pendant : {_corruptionToursRestant} tours.";
            _gm._combatManager.Validate_Enemy_Corruption();
        }
    }

    /// <summary>
    /// Updates the corruption state of the enemy.
    /// </summary>
    public void UpdateCorruption()
    {
        _corruptionToursRestant--;
        string _tour = _corruptionToursRestant > 1 ? "tours" : "tour";
        m_CorruptionText.text = $"Corrompu pendant : {_corruptionToursRestant} {_tour}.";
    }
}
