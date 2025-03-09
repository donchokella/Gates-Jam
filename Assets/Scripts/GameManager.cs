using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;
using System;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    [Header("UI Butonları")]
    public Button[] herbButtons;
    public Button[] miscButtons;
    public Button[] usageButtons;

    [Header("Hasta Yönetimi")]
    public GameObject[] patientPrefabs;
    [Header("Hasta Spawn Noktaları")]
    public Transform spawn1;
    public Transform spawn2;

    [Header("İksir ve Seçimler")]
    public GameObject potionPrefab;
    public Transform potionSpawnPoint;

    [Header("Puanlama Sistemi")]
    public int score = 0;
    public TMP_Text scoreText;

    [Header("Ses Efektleri")]
    public AudioSource correctSound;
    public AudioSource wrongSound;
    public AudioSource symptomSound;
    public AudioSource buttonClickSound;

    [Header("İksir Efektleri")]
    public GameObject potionSmokePrefab; // Duman efekti için prefab

    private GameObject currentPotion; // Spawn olan iksiri saklamak için

    private string selectedHerb;
    private string selectedMisc;
    private string selectedUsage;

    private bool herbSelected = false;
    private bool miscSelected = false;
    private bool usageSelected = false;

    private GameObject currentPatient;

    // Yeni eklenen açılış ve kapanış cümleleri dizileri (rastgele seçim için)
    private string[] openingSentences = new string[]
    {
        "İnanmayacaksınız ama",
        "Şaka gibi ama",
        "Doktor, halime bakın:",
        "Of of, başıma gelenler:",
        "Bu nasıl bir talih, anlamadım:",
        "Başıma gelen pişmiş tavuğun başına gelmedi:",
        "Yok artık, durum şöyle:",
        "Komik gelecek ama",
        "Durum vahim doktor,",
        "Bir garip haldeyim:"
    };

    private string[] closingSentences = new string[]
    {
        "Gülsem mi ağlasam bilemiyorum.",
        "Resmen perişan oldum.",
        "Kendimi mutant gibi hissediyorum.",
        "Bu gidişle Mars'a ışınlanacağım sanırım.",
        "Durumum evlere şenlik, doğrusu.",
        "İnanın ne yapacağımı şaşırdım.",
        "Böyle hastalık görülmedi!",
        "Galiba süper kahramana dönüşüyorum.",
        "Lütfen bana bir çare bulun.",
        "Böyle komedi olamaz!"
    };

    // symptomDictionary, excel dosyanızdaki veriler doğrultusunda oluşturulmuştur.
    // Her hastalık için: (requiredHerb, requiredMisc, requiredUsage, narrativeText)
    // narrativeText: İki semptom için toplam 4 varyant içermeli.
    // Örneğin, Dizanteri için:
    // "(iktidarsızlık_semptom_1_variant1). (iktidarsızlık_semptom_1_variant2). (bas_agrisi_semptom_2_variant1). (bas_agrisi_semptom_2_variant2)."
    private Dictionary<string, (string, string, string, string)> symptomDictionary = new Dictionary<string, (string, string, string, string)>
    {

    {"Varicella", ("Rose", "HagiaSophia", "Shot", "(varicella_semptom_1_variant1). (varicella_semptom_1_variant2). (varicella_semptom_2_variant1). (varicella_semptom_2_variant2).")},
    {"Dizanteri", ("Chili", "HagiaSophia", "Shot", "(iktidarsızlık_semptom_1_variant1). (iktidarsızlık_semptom_1_variant2). (bas_agrisi_semptom_2_variant1). (bas_agrisi_semptom_2_variant2).")},
    {"Kusmalı zatürre", ("Garlic", "HagiaSophia", "Inject", "(kusma_semptom_1_variant1). (kusma_semptom_1_variant2). (zatürre_semptom_2_variant1). (zatürre_semptom_2_variant2).")},
    {"Hepatit Y", ("Rosemary", "OldSweat", "3TimesADay", "(hepatitY_semptom_1_variant1). (hepatitY_semptom_1_variant2). (hepatitY_semptom_2_variant1). (hepatitY_semptom_2_variant2).")},
    {"Tetanoz", ("Rosemary", "OldSweat", "PutYourHair", "(tetanoz_semptom_1_variant1). (tetanoz_semptom_1_variant2). (tetanoz_semptom_2_variant1). (tetanoz_semptom_2_variant2).")},
    {"Amipli Dizanteri", ("TiptonWeed", "OldSweat", "Inject", "(amipli_dizanteri_semptom_1_variant1). (amipli_dizanteri_semptom_1_variant2). (amipli_dizanteri_semptom_2_variant1). (amipli_dizanteri_semptom_2_variant2).")},
    {"Hepatit X", ("Chili", "OldSweat", "PutYourHair", "(hepatitX_semptom_1_variant1). (hepatitX_semptom_1_variant2). (hepatitX_semptom_2_variant1). (hepatitX_semptom_2_variant2).")},
    {"Influenza Plus", ("Rose", "FrogLeg", "PutYourHair", "(influenzaPlus_semptom_1_variant1). (influenzaPlus_semptom_1_variant2). (influenzaPlus_semptom_2_variant1). (influenzaPlus_semptom_2_variant2).")},
    {"Ishalli Grip", ("TiptonWeed", "FrogLeg", "Shot", "(ishalliGrip_semptom_1_variant1). (ishalliGrip_semptom_1_variant2). (ishalliGrip_semptom_2_variant1). (ishalliGrip_semptom_2_variant2).")},
    {"Ishalli Varicella", ("Garlic", "FrogLeg", "Inject", "(ishalliVaricella_semptom_1_variant1). (ishalliVaricella_semptom_1_variant2). (ishalliVaricella_semptom_2_variant1). (ishalliVaricella_semptom_2_variant2).")},
    {"Influenza", ("Rose", "Leech", "GiveYourMama", "(influenza_semptom_1_variant1). (influenza_semptom_1_variant2). (influenza_semptom_2_variant1). (influenza_semptom_2_variant2).")},
    {"Grip", ("TiptonWeed", "Leech", "3TimesADay", "(grip_semptom_1_variant1). (grip_semptom_1_variant2). (grip_semptom_2_variant1). (grip_semptom_2_variant2).")},
    {"Akut Difteri", ("Garlic", "Leech", "GiveYourMama", "(akutDifteri_semptom_1_variant1). (akutDifteri_semptom_1_variant2). (akutDifteri_semptom_2_variant1). (akutDifteri_semptom_2_variant2).")},
    {"Difteri", ("Rosemary", "Widow_Tear", "Inject", "(difteri_semptom_1_variant1). (difteri_semptom_1_variant2). (difteri_semptom_2_variant1). (difteri_semptom_2_variant2).")},
    {"Zatürre", ("TiptonWeed", "Widow_Tear", "GiveYourMama", "(zaturre_semptom_1_variant1). (zaturre_semptom_1_variant2). (zaturre_semptom_2_variant1). (zaturre_semptom_2_variant2).")},
    {"Sarbiral Tetanoz", ("Chili", "Widow_Tear", "Shot", "(sarbiralTetanoz_semptom_1_variant1). (sarbiralTetanoz_semptom_1_variant2). (sarbiralTetanoz_semptom_2_variant1). (sarbiralTetanoz_semptom_2_variant2).")}
    };

    void Start()
    {
        UpdateScoreUI();
        SpawnPatient();

        foreach (Button btn in herbButtons)
        {
            btn.onClick.AddListener(() => { SelectHerb(btn.GetComponentInChildren<TMP_Text>().text); PlayButtonClickSound(); });
        }
        foreach (Button btn in miscButtons)
        {
            btn.onClick.AddListener(() => { SelectMisc(btn.GetComponentInChildren<TMP_Text>().text); PlayButtonClickSound();
});
        }
        foreach (Button btn in usageButtons)
{
    btn.onClick.AddListener(() => { SelectUsage(btn.GetComponentInChildren<TMP_Text>().text); PlayButtonClickSound(); });
}
    }

    void SpawnPatient()
{
    if (patientPrefabs.Length > 0)
    {
        // Eski hasta nesnesini yok edip yenisini oluşturuyoruz.
        Destroy(currentPatient);

        int randomIndex = UnityEngine.Random.Range(0, patientPrefabs.Length);
        currentPatient = Instantiate(patientPrefabs[randomIndex], spawn1.position, Quaternion.identity);
        Debug.Log("Yeni hasta Spawn1'de belirdi.");
        AssignRandomSymptom(currentPatient);

        // Hasta geldiğinde semptom sesi çal
        if (symptomSound != null) symptomSound.Play();
    }
}

void AssignRandomSymptom(GameObject patient)
{
    Patient2D patientScript = patient.GetComponent<Patient2D>();
    if (patientScript != null)
    {
        List<string> diseases = new List<string>(symptomDictionary.Keys);
        string randomDisease = diseases[UnityEngine.Random.Range(0, diseases.Count)];
        var entry = symptomDictionary[randomDisease];

        // narrativeText içerisindeki 4 cümleyi ayırıyoruz.
        string narrative = entry.Item4;
        string[] parts = narrative.Split(new char[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries);

        string symptom1 = "";
        string symptom2 = "";
        if (parts.Length >= 4)
        {
            // İlk iki cümle, semptom1 varyantları
            string[] symptom1Variants = new string[] { parts[0].Trim(), parts[1].Trim() };
            // Son iki cümle, semptom2 varyantları
            string[] symptom2Variants = new string[] { parts[2].Trim(), parts[3].Trim() };

            symptom1 = symptom1Variants[UnityEngine.Random.Range(0, symptom1Variants.Length)];
            symptom2 = symptom2Variants[UnityEngine.Random.Range(0, symptom2Variants.Length)];
        }
        else if (parts.Length >= 2)
        {
            symptom1 = parts[0].Trim();
            symptom2 = parts[1].Trim();
        }

        // Rastgele açılış ve kapanış cümleleri seç
        string opener = openingSentences[UnityEngine.Random.Range(0, openingSentences.Length)];
        string closer = closingSentences[UnityEngine.Random.Range(0, closingSentences.Length)];

        // Hasta cümlesi: açılış + semptom1 + semptom2 + kapanış
        string patientSentence = opener + " " + symptom1 + ". " + symptom2 + ". " + closer;

        // Hasta bilgilerini güncelle (tedavi kontrolü için required değerler excel dosyanızdaki verilere göre)
        patientScript.symptomText = patientSentence;
        patientScript.requiredHerb = entry.Item1;
        patientScript.requiredMisc = entry.Item2;
        patientScript.requiredUsage = entry.Item3;

        patientScript.UpdateSymptomUI();
    }
}

public void SelectHerb(string herbName)
{
    if (herbSelected) return;
    herbSelected = true;
    selectedHerb = herbName;
    DisableButtonList(herbButtons);
    TryCreatePotion();
}

public void SelectMisc(string miscName)
{
    if (miscSelected) return;
    miscSelected = true;
    selectedMisc = miscName;
    DisableButtonList(miscButtons);
    TryCreatePotion();
}

public void SelectUsage(string usageName)
{
    if (usageSelected) return;
    usageSelected = true;
    selectedUsage = usageName;
    DisableButtonList(usageButtons);
    TryCreatePotion();
}

private void DisableButtonList(Button[] buttons)
{
    foreach (Button btn in buttons)
    {
        btn.interactable = false;
    }
}

private void TryCreatePotion()
{
    if (herbSelected && miscSelected && usageSelected)
    {
        CreatePotion();
        // Tedavi kontrolü: Patient2D içindeki CheckPotion metodu, excel dosyanızdaki verilerle uyumlu olarak
        // seçilen iksir bileşenleri (selectedHerb, selectedMisc, selectedUsage) ile doğru tedavi kombinasyonunu karşılaştırır.
        CheckPotion(selectedHerb, selectedMisc, selectedUsage);
    }
}

private void CreatePotion()
{
    if (potionPrefab != null && potionSpawnPoint != null)
    {
        // Duman efektini hemen başlat
        if (potionSmokePrefab != null)
        {
            potionSmokePrefab.transform.position = potionSpawnPoint.position;
            potionSmokePrefab.SetActive(true);

            ParticleSystem ps = potionSmokePrefab.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
        }

        // 0.5 saniye bekleyip potion'u instantiate et
        StartCoroutine(SpawnPotionWithDelay(0.5f));
    }
}

private IEnumerator SpawnPotionWithDelay(float delay)
{
    yield return new WaitForSeconds(delay);

    Vector3 spawnPosition = potionSpawnPoint.position + new Vector3(0, 4f, 0);
    currentPotion = Instantiate(potionPrefab, spawnPosition, Quaternion.identity);
    Debug.Log($"İksir hazırlandı: {selectedHerb} + {selectedMisc} + {selectedUsage}");

    Rigidbody rb = currentPotion.GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.useGravity = true;
        rb.linearVelocity = Vector3.down * 2f;
    }

    StartCoroutine(DisableSmokeEffect(2f, 1f));
}

private IEnumerator DisableSmokeEffect(float delay, float delay2)
{
    yield return new WaitForSeconds(delay);

    if (potionSmokePrefab != null)
    {
        potionSmokePrefab.SetActive(false);
    }

    yield return new WaitForSeconds(delay2);

    if (currentPotion != null)
    {
        Destroy(currentPotion);
    }
}

public void CheckPotion(string herb, string misc, string usage)
{
    Debug.Log("Tedavi kontrolü başlatıldı.");
    if (currentPatient != null)
    {
        Patient2D patientScript = currentPatient.GetComponent<Patient2D>();
        if (patientScript != null)
        {
            bool isCured = patientScript.CheckPotion(herb, misc, usage);
            if (isCured)
            {
                score += 10;
                if (correctSound != null) correctSound.Play();
            }
            else
            {
                score -= 5;
                if (wrongSound != null) wrongSound.Play();
            }
            UpdateScoreUI();
            StartCoroutine(HandlePatientOutcome(isCured));
        }
    }
}

IEnumerator HandlePatientOutcome(bool isCured)
{
    yield return new WaitForSeconds(2f);

    Destroy(currentPatient);
    ResetGame();
    SpawnPatient();
}

private void ResetGame()
{
    selectedHerb = null;
    selectedMisc = null;
    selectedUsage = null;
    herbSelected = false;
    miscSelected = false;
    usageSelected = false;
    EnableButtonList(herbButtons);
    EnableButtonList(miscButtons);
    EnableButtonList(usageButtons);
}

private void EnableButtonList(Button[] buttons)
{
    foreach (Button btn in buttons)
    {
        btn.interactable = true;
    }
}

void UpdateScoreUI()
{
    if (scoreText != null)
    {
        scoreText.text = "Puan: " + score;
    }
}

void PlayButtonClickSound()
{
    if (buttonClickSound != null)
        buttonClickSound.Play();
}
}