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

    [Header("Game settings")]
    [SerializeField] private int money = 0;
    [SerializeField] [Range(0f, 1f)] private float successHit = 0.2f;
    [SerializeField] private int upgradeAccuracyCost = 50;
    [SerializeField] private float chanceStep = 0.1f;

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
                ExecutePlayerLogic(hit.collider.gameObject);
                shot = false;
            }
        }
    }

    void ExecutePlayerLogic(GameObject playerObject)
    {
        shot = true;
        if (Random.value < successHit)
        {
            money += 20;
            Debug.Log("Есть пробитие");
        }
        else
        {
            Debug.Log("Не пробил");
        }

        UpdateUI();
    }

    public void BuyUpgrade()
    {
        if (money >= upgradeAccuracyCost && successHit < 1f)
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
            Debug.Log("Не хватает золота! Ну или у вас максимальная точность");
        }
    }

    void UpdateUI()
    {
        if (moneyText != null) moneyText.text = $"Доллары: {money}";
        if (chanceText != null) chanceText.text = $"Шанс попадания: {successHit}";
        if (upgradeAcuracyBtnText != null)
        {
            if (successHit >= 1f) upgradeAcuracyBtnText.text = "Максимум";
            else upgradeAcuracyBtnText.text = $"увеличить шанс(-{upgradeAccuracyCost} долларов)";
        }
    }
}
