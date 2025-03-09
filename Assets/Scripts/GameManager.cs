using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    // Sözlük artık hastalık adını (key) ve dört öğe içeren tuple'ı içeriyor.
    // Tuple sırası: (requiredHerb, requiredMisc, requiredUsage, narrativeText)
    private Dictionary<string, (string, string, string, string)> symptomDictionary = new Dictionary<string, (string, string, string, string)>
    {
        {"Varicella", ("Rose", "OldSweat", "Shot", "Cildimde renkli döküntüler beliriyor. Kaşıntı sürekli devam ediyor. Kendimi adeta bir sanat eserine benzetiyorum.")},
{"Dizanteri", ("Chili", "FrogLeg", "3TimesADay", "Karnım fırtınalı bir şekilde hareket ediyor. Yemek sonrası zorlanıyorum. Bu durum bazen komik, bazen de oldukça acı veriyor.")},
{"Kusmalı zatürre", ("Garlic", "Leech", "PutYourHair", "Göğsümde şiddetli öksürükler var ve kusma refleksi neredeyse sürekli. Her nefes alışımda rahatsızlık duyuyorum. Bu durum beni gerçekten yıpratıyor.")},
{"Hepatit Y", ("Rosemary", "Widow_Tear", "Inject", "Karaciğerimde yanma hissi var. İçimde sıcaklık artıyor ve bu durumun asla durmayacağını hissediyorum. Doktorlar şaşkınlıkla bakıyor.")},
{"Tetanoz", ("TiptonWeed", "HagiaSophia", "GiveYourMama", "Kaslarım istemsizce kasılıyor ve vücudum sürekli kasılmalar yaşıyor. Sanki çekiçle dövülüyormuşum gibi hissediyorum. Bu durum hem korkutucu hem de ironik.")},
{"Amipli Dizanteri", ("Rose", "FrogLeg", "Shot", "Karnımda kontrolsüz hareketler var. Yemek sonrası sürekli tuvalete koşmak zorunda kalıyorum. Durum sanki küçük amipler istilası gibi, hem sinir bozucu hem komik.")},
{"Hepatit X", ("Chili", "OldSweat", "3TimesADay", "Vücudumun derinliklerinde sürekli yanma hissediyorum. İçimde kavruluyormuş gibi bir sıcaklık var. Bu durum gerçekten dayanılmaz oluyor.")},
{"Influenza Plus", ("Garlic", "Leech", "PutYourHair", "Yüksek ateş ve titreme yüzünden yatağa düşmekten kurtulamıyorum. Nefes almak zorlaşıyor. Bazen bu durum bana adeta bir dram filmi yaşatıyor.")},
{"Ishalli Grip", ("Rosemary", "Widow_Tear", "Inject", "Başımdan geçen grip belirtileri dayanılmaz. Sürekli terliyor, titriyorum. Bazen komik bir hal alsa da genel olarak moral bozucu bir durum.")},
{"Ishalli Varicella", ("TiptonWeed", "HagiaSophia", "GiveYourMama", "Cildimde hem döküntüler hem de ishal belirtileri var. Hem kaşıntı var hem de sürekli tuvalete yetişmeye çalışıyorum. Bu durum hem utanç verici hem de komik anlar yaşatıyor.")},
{"Influenza", ("Rose", "OldSweat", "Shot", "Yüksek ateş ve şiddetli öksürük beni adeta yıpratıyor. Her nefeste kendimi bitkin hissediyorum. Gün boyu dinlenmek zorundayım.")},
{"Grip", ("Chili", "FrogLeg", "3TimesADay", "Ateşim var, vücudum titriyor ve öksürüklerim sürekli. Her şey çok halsizleştirici. Sanki bedenim bozulmuş gibi hissediyorum.")},
{"Akut Difteri", ("Garlic", "Leech", "PutYourHair", "Boğazım şişiyor ve nefes almak neredeyse imkansız hale geliyor. Sesim kısılıyor. Bu dramatik durum hem korkutucu hem de komik görünebiliyor.")},
{"Difteri", ("Rosemary", "Widow_Tear", "Inject", "Boğazımda sürekli bir tıkanıklık var. Konuşurken boğuk çıkıyorum. İnsanlar bana gülse de, ben durumumdan rahatsızım.")},
{"Zatürre", ("TiptonWeed", "HagiaSophia", "GiveYourMama", "Göğsümde derin bir ağrı var ve nefes almak bile güçleşiyor. Her soluk alışımda acı hissediyorum. Kendimi çaresiz ve yorgun hissediyorum.")},
{"Sarbiral Tetanoz", ("Rose", "FrogLeg", "Shot", "Kaslarım aniden kasılıyor ve sürekli sertleşiyor. Her adımım acı veriyor. Bu durum bazen sinir bozucu bazen de komik olabiliyor.")},
{"Varicella_2", ("Chili", "Leech", "3TimesADay", "Cildimde yeniden döküntüler beliriyor ve kaşıntı şiddetleniyor. Her gün artan belirtilerle mücadele ediyorum. Durum bana tuhaf bir sanat eseri gibi geliyor.")},
{"Dizanteri_2", ("Garlic", "OldSweat", "PutYourHair", "Karnımda fırtına kopuyor. Yemek sonrası tuvalet kaçamakları artık günlük rutinim haline geldi. Hem sinir bozucu hem de komik anlar yaşıyorum.")},
{"Grip_2", ("Rosemary", "FrogLeg", "Inject", "Yüksek ateş, titreme ve halsizlik beni sarmış durumda. Her nefeste yorgunluk hissediyorum. Bazen bu durum komik bir trajediye dönüşüyor.")},
{"Influenza_Plus", ("TiptonWeed", "HagiaSophia", "GiveYourMama", "Ateşim çok yüksek, vücudum titriyor ve öksürüklerim neredeyse sinema sahnesi gibi dramatik. Bu durum hem yıpratıcı hem de bazen güldürücü oluyor.")}


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
            btn.onClick.AddListener(() => { SelectMisc(btn.GetComponentInChildren<TMP_Text>().text); PlayButtonClickSound(); });
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
            if (currentPatient != null)
            {
                Destroy(currentPatient);
            }

            int randomIndex = Random.Range(0, patientPrefabs.Length);
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
            string randomDisease = diseases[Random.Range(0, diseases.Count)];
            var entry = symptomDictionary[randomDisease];

            // Tam anlatım cümlesini symptomText olarak atıyoruz.
            patientScript.symptomText = entry.Item4;
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
        }
    }
    private void CreatePotion()
    {
        if (potionPrefab != null && potionSpawnPoint != null)
        {
            // **Duman efektini hemen başlat**
            if (potionSmokePrefab != null)
            {
                potionSmokePrefab.transform.position = potionSpawnPoint.position; // Dumanın pozisyonunu belirle
                potionSmokePrefab.SetActive(true); // Dumanı aç

                ParticleSystem ps = potionSmokePrefab.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Play(); // Duman efektini başlat
                }
            }

            // **0.5 saniye bekleyip potion'u instantiate et**
            StartCoroutine(SpawnPotionWithDelay(0.5f));
        }
    }

    // **Potion spawn işlemini gecikmeli yapacak coroutine**
    private IEnumerator SpawnPotionWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 0.5 saniye bekle

        Vector3 spawnPosition = potionSpawnPoint.position + new Vector3(0, 4f, 0);
        currentPotion = Instantiate(potionPrefab, spawnPosition, Quaternion.identity);
        Debug.Log($"İksir hazırlandı: {selectedHerb} + {selectedMisc} + {selectedUsage}");

        // **Potion aşağı düşsün**
        Rigidbody rb = currentPotion.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true; // Yerçekimini aç
            rb.linearVelocity = Vector3.down * 2f; // Hafif bir düşüş efekti ver
        }

        // **Dumanı belirli bir süre sonra kapat ve potion'u yok et**
        StartCoroutine(DisableSmokeEffect(2f, 1f));
    }

    // **Duman efektini kapatma ve potion'u yok etme**
    private IEnumerator DisableSmokeEffect(float delay, float delay2)
    {
        yield return new WaitForSeconds(delay); // İlk bekleme süresi (Duman devam edecek)

        if (potionSmokePrefab != null)
        {
            potionSmokePrefab.SetActive(false); // Dumanı kapat
        }

        yield return new WaitForSeconds(delay2); // İksirin kaybolma süresi

        if (currentPotion != null)
        {
            Destroy(currentPotion); // Potion'u yok et
        }
    }


    public void CheckPotion(string herb, string misc, string usage)
    {
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
