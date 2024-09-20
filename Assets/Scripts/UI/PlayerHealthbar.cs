using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthbar : MonoBehaviour
{
    Slider _healthSlider;
    public TextMeshProUGUI healthText;

    void Start() {
        _healthSlider = GetComponent<Slider>();
    }

    void Update() {
        healthText.text = GameManager.gameManager._playerHealth.Health.ToString() + " / " + GameManager.gameManager._playerHealth.MaxHealth.ToString();
        _healthSlider.value = GameManager.gameManager._playerHealth.Health;
    }

    public void SetMaxHealth(int maxHealth) {
        _healthSlider.maxValue = maxHealth;
        _healthSlider.value = maxHealth;
    }

        public void SetHealth(int health) {
        _healthSlider.value = health;
    }
}
