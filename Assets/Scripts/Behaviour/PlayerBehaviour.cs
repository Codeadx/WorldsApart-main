using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] PlayerHealthbar _healthBar;
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
    }

    private void PlayerTakeDmg(int dmg) {
        GameManager.gameManager._playerHealth.DmgUnit(dmg);
        _healthBar.SetHealth(GameManager.gameManager._playerHealth.Health);
    }

    private void PlayerHeal(int healing) {
        GameManager.gameManager._playerHealth.HealUnit(healing);
        _healthBar.SetHealth(GameManager.gameManager._playerHealth.Health);
    }

    private void EnemyTakeDmg(int dmg) {
        GameManager.gameManager._enemyHealth.DmgUnit(dmg);
        _healthBar.SetHealth(GameManager.gameManager._enemyHealth.Health);
    }
}
