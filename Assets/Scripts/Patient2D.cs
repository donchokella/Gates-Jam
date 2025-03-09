using UnityEngine;
using TMPro;
using System.Collections; // Coroutine için gerekli

public class Patient2D : MonoBehaviour
{
    [Header("Semptom Bilgileri")]
    public string symptomText;
    public string requiredHerb;
    public string requiredMisc;
    public string requiredUsage;

    [Header("UI Referansları")]
    public TextMeshProUGUI symptomUIText;
    public SpriteRenderer spriteRenderer;

    [Header("Animasyon ve Ses")]
    public Animator animator;
    public AudioSource symptomSound;

    private bool isCured = false;
    private Transform spawn2; // Spawn2'nin konumunu alacağız
    private float moveDuration = 1f; // Hareket süresi

    void Start()
    {
        if (symptomUIText != null)
        {
            symptomUIText.text = symptomText; // TMP Text güncelleniyor
        }

        if (symptomSound != null)
            symptomSound.Play();

        // **Spawn2’yi GameManager üzerinden al**
        GameObject spawn2Object = GameObject.Find("Spawn2");

        if (spawn2Object != null)
        {
            spawn2 = spawn2Object.transform;
            Debug.Log("Spawn2 bulundu, hareket başlıyor...");

            // **Spawn2’ye hareket etmeyi başlat**
            StartCoroutine(MoveToTarget(spawn2.position, moveDuration));
        }
        else
        {
            Debug.LogError("Spawn2 bulunamadı! Unity'de doğru isimle oluşturduğunuzdan emin olun.");
        }
    }

    IEnumerator MoveToTarget(Vector3 target, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, target, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = target; // Son konuma tam olarak oturt
        Debug.Log("Hasta hedefe ulaştı!");
    }

    public void UpdateSymptomUI()
    {
        if (symptomUIText != null)
        {
            symptomUIText.text = symptomText; // Semptom yazısını güncelle
        }
    }

    public bool CheckPotion(string herb, string misc, string usage)
    {
        Debug.Log("11111");
        if (herb == requiredHerb && misc == requiredMisc && usage == requiredUsage)
        {
            isCured = true;
            GetCured();
            return true;
        }
        else
        {
            GetWorse();
            return false;
        }
    }

    void GetCured()
    {
        if (animator != null)
            animator.SetBool("isCured", true);
        Destroy(gameObject, 2f);
    }

    void GetWorse()
    {
        if (animator != null)
            animator.SetBool("isDead", true);
        Destroy(gameObject, 2f);
    }
}
