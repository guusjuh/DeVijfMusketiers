using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoUI : MonoBehaviour
{
    private HealthBar healthBar;
    private Text apText;

    private GameObject specialAttack;
    private GameObject specialAttackIcon;
    private GameObject disabledSpecialAttack;
    private Text cooldownText;

    private bool active = false;
    private Enemy selectedEnemy;

    public void Initialize()
    {
        healthBar = transform.Find("HealthBar").GetComponent<HealthBar>();
        healthBar.Initialize();

        apText = transform.Find("ActionPoints").GetComponentInChildren<Text>();

        specialAttack = transform.Find("Spell").gameObject;
        specialAttackIcon = specialAttack.transform.Find("SpellIcon").gameObject;
        disabledSpecialAttack = specialAttack.transform.Find("Disabled").gameObject;
        cooldownText = specialAttack.GetComponentInChildren<Text>();
    }

    public void Restart()
    {
        healthBar.SetHealthbar(null);
    }

    public void Clear()
    {
        active = false;
        selectedEnemy = null;
        gameObject.SetActive(false);
    }

    public void OnChange(Enemy selectedEnemy = null)
    {
        // deactivate code
        if (selectedEnemy == null)
        {
            if(!active) return;

            //TODO slide out of screen anim
            active = false;
            gameObject.SetActive(false);
        }
        else
        {
            //check for already being active
            if (active)
            {
                //check for already same enemy (update only)
                if (this.selectedEnemy == selectedEnemy) UpdateValues();
                else
                {
                    this.selectedEnemy = selectedEnemy;
                    SetValues();
                }
            }
            else
            {
                //TODO: fancy animation code
                gameObject.SetActive(true);
                this.selectedEnemy = selectedEnemy;
                SetValues();
                active = true;
            }
        }
    }

    private void SetValues()
    {
        healthBar.SetHealthbar(selectedEnemy);
        SetAPText(selectedEnemy.CurrentActionPoints);

        if (selectedEnemy.HasSpecial)
        {
            ActivateSpecialSpell(true);
            SetCooldownText(selectedEnemy.SpecialCooldown);
        }
        else
        {
            ActivateSpecialSpell(false);
        }
    }

    //TODO: has to animate!
    private void UpdateValues()
    {
        healthBar.SetHealthbar(selectedEnemy);
        SetAPText(selectedEnemy.CurrentActionPoints);

        if (selectedEnemy.HasSpecial)
        {
            SetCooldownText(selectedEnemy.SpecialCooldown);
        }
    }

    public void SetAPText(int value)
    {
        apText.text = "" + value;
    }

    public void ActivateSpecialSpell(bool on)
    {
        if (on)
        {
            if (!specialAttack.activeInHierarchy)
            {
                specialAttack.SetActive(true);
                specialAttackIcon.GetComponent<Image>().sprite = selectedEnemy.SpellIconSprite;
            }
        }
        else
        {
            if (specialAttack.activeInHierarchy) specialAttack.SetActive(false);
        }
    }

    public void SetCooldownText(int value)
    {
        if (value <= 0)
        {
            if (disabledSpecialAttack.activeInHierarchy) disabledSpecialAttack.SetActive(false);
            if (cooldownText.gameObject.activeInHierarchy) cooldownText.gameObject.SetActive(false);
        }
        else
        {
            if (!disabledSpecialAttack.activeInHierarchy) disabledSpecialAttack.SetActive(true);
            if (!cooldownText.gameObject.activeInHierarchy) cooldownText.gameObject.SetActive(true);
            cooldownText.text = "" + value;
        }
    }
}
