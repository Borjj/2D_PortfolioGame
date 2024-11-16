using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CooldownUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image cooldownImage;
    [SerializeField] private GameObject timer;
    [SerializeField] private TextMeshProUGUI cooldownText;
    
    [Header("Settings")]
    [SerializeField] private float cooldownDuration = 2f;
    
    private float currentCooldown;
    private bool isOnCooldown;


// -------------------------------------------------------------------------------- //

    private void Start()
    {
        ResetUI();
        timer.SetActive(false);
    }

    private void Update()
    {
        if (!isOnCooldown) return;

        currentCooldown -= Time.deltaTime;
        UpdateUI();

        if (currentCooldown <= 0)
        {
            ResetUI();
        }
    }


// -------------------------------------------------------------------------------- //


    public void StartCooldown()
    {
        isOnCooldown = true;
        currentCooldown = cooldownDuration;
        UpdateUI();
    }

    private void UpdateUI()
    {
        float fillAmount = currentCooldown / cooldownDuration;
        cooldownImage.fillAmount = fillAmount;
        cooldownText.text = currentCooldown.ToString("0.0");
        timer.SetActive(true);
    }

    private void ResetUI()
    {
        isOnCooldown = false;
        currentCooldown = 0f;
        cooldownImage.fillAmount = 0f;
        cooldownText.text = "";
        timer.SetActive(false);
    }
}