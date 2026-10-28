using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class Click : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI chanceText;
    [SerializeField] private TextMeshProUGUI fireRateText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI upgradeAcuracyBtnText;
    [SerializeField] private TextMeshProUGUI upgradeFireRateBtnText;
    [SerializeField] private TextMeshProUGUI upgradeDamageBtnText;

    [Header("Balance")]
    [SerializeField] private int money = 0;
    [SerializeField] [Range(0f, 1f)] private float successHit = 0.2f;
    [SerializeField] private float fireRate = 2.001f;
    [SerializeField] private int damage = 30;
    [SerializeField] private int upgradeAccuracyCost = 50;
    [SerializeField] private int upgradeFireRateCost = 30;
    [SerializeField] private int upgradeDamageCost = 10;
    [SerializeField] private float chanceStep = 0.008f;
    [SerializeField] private float fireRateStep = 0.02f;

    private int _fireRateCap = 100;
    private int _fireRateNow = 0;

    private int _damageCap = 100;
    private int _damageNow = 0;

    private float nextFireTimer = 0;

    private bool shot = false;

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            PerformRayCast (mousePosition);
        }
    }

    private void PerformRayCast(Vector3 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (Time.time < nextFireTimer)
                {
                    Debug.Log("Слишком быстро стреляешь, давай помедленнее!");
                    return;
                }
                ExecutePlayerLogic(hit.collider.gameObject);
                nextFireTimer = Time.time + fireRate;
                shot = false;
            }
        }
    }

    void ExecutePlayerLogic(GameObject playerObject)
    {
        shot = true;
        if (Random.value < successHit)
        {
            money += damage;
            Debug.Log("Есть пробитие");
        }
        else
        {
            Debug.Log("Не пробил");
        }

        UpdateUI();
    }
#region Damage
    public void BuyDamageUpgrade()
    {
        if (money >= upgradeDamageCost && _damageNow < _damageCap)
        {
            money -= upgradeDamageCost;
            damage += 5;

            if (_damageNow < _damageCap) _fireRateNow++;

            upgradeDamageCost += 10;

            UpdateUI();
            Debug.Log("Урон повышен!");
        }
        else
        {
            Debug.Log("Не хватает нефти! Ну или у вас максимальный урон");
        }
    }
#endregion
#region FireRate
    public void BuyFireRateUpgrade()
    {
        if (money >= upgradeFireRateCost && _fireRateNow < _fireRateCap)
        {
            money -= upgradeFireRateCost;
            fireRate -= fireRateStep;

            if (_fireRateNow < _fireRateCap) _fireRateNow++;

            upgradeFireRateCost = Mathf.RoundToInt(upgradeFireRateCost * 1.1f);

            UpdateUI();
            Debug.Log("Скорострельность повышена!");
        }
        else
        {
            Debug.Log("Не хватает нефти! Ну или у вас максимальная скорострельность");
        }
    }
    #endregion
#region Accuracy
    public void BuyAccuracyUpgrade()
    {
        if (money >= upgradeAccuracyCost)
        {
            money -= upgradeAccuracyCost;
            successHit += chanceStep;

            if (successHit > 1f) successHit = 1f;

            upgradeAccuracyCost = Mathf.RoundToInt(upgradeAccuracyCost * 1.5f);

            UpdateUI();
            Debug.Log("Шанс попадания повышен!");
        }
        else
        {
            Debug.Log("Не хватает нефти! Ну или у вас максимальная точность");
        }
    }
    #endregion
#region UpdateUI
    void UpdateUI()
    {
        moneyText.text = $"Доллары: {money}";
        chanceText.text = $"Шанс попадания: {successHit}";
        if (successHit >= 1f) upgradeAcuracyBtnText.text = "Максимум";
        else upgradeAcuracyBtnText.text = $"увеличить шанс (-{upgradeAccuracyCost} долларов)";

        if (_fireRateNow == _fireRateCap) upgradeFireRateBtnText.text = "Максимум";
        else upgradeFireRateBtnText.text = $"увеличить скорострельность (-{upgradeFireRateCost} долларов)";

        if (_damageNow == _damageCap) upgradeDamageBtnText.text = "Максимум";
        else upgradeDamageBtnText.text = $"увеличить урон (-{upgradeDamageCost} долларов)";
    }
    #endregion
}
