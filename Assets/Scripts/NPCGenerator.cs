using UnityEngine;

public class NPCGenerator : MonoBehaviour
{
    // Her bileşen için SpriteRenderer referansları (Inspector'da atayın)
    public SpriteRenderer kafaRenderer;
    public SpriteRenderer gozRenderer;
    public SpriteRenderer burunRenderer;
    public SpriteRenderer agizRenderer;
    public SpriteRenderer bedenRenderer;
    public SpriteRenderer sacRenderer;
    public SpriteRenderer sakalRenderer;

    void Start()
    {
        GenerateNPC();
    }

    void GenerateNPC()
    {
        // Resources klasöründen ilgili sprite dizilerini yükle
        Sprite[] heads = Resources.LoadAll<Sprite>("Head");
        Sprite[] eyes = Resources.LoadAll<Sprite>("Eyes");
        Sprite[] noses = Resources.LoadAll<Sprite>("Nose");
        Sprite[] mouthes = Resources.LoadAll<Sprite>("Mouth");
        Sprite[] bodies = Resources.LoadAll<Sprite>("Body");
        Sprite[] hair = Resources.LoadAll<Sprite>("Hair");
        Sprite[] beard = Resources.LoadAll<Sprite>("Beard");

        // Rastgele seçim yapıp SpriteRenderer'lara atama
        if (heads.Length > 0)
            kafaRenderer.sprite = heads[Random.Range(0, heads.Length)];
        else
            Debug.LogWarning("Kafa klasöründe hiç sprite bulunamadı!");

        if (eyes.Length > 0)
            gozRenderer.sprite = eyes[Random.Range(0, eyes.Length)];
        else
            Debug.LogWarning("Goz klasöründe hiç sprite bulunamadı!");

        if (noses.Length > 0)
            burunRenderer.sprite = noses[Random.Range(0, noses.Length)];
        else
            Debug.LogWarning("Burun klasöründe hiç sprite bulunamadı!");

        if (mouthes.Length > 0)
            agizRenderer.sprite = mouthes[Random.Range(0, mouthes.Length)];
        else
            Debug.LogWarning("Agiz klasöründe hiç sprite bulunamadı!");

        if (bodies.Length > 0)
            bedenRenderer.sprite = bodies[Random.Range(0, bodies.Length)];
        else
            Debug.LogWarning("Beden klasöründe hiç sprite bulunamadı!");

        if (hair.Length > 0)
            sacRenderer.sprite = hair[Random.Range(0, hair.Length)];
        else
            Debug.LogWarning("Sac klasöründe hiç sprite bulunamadı!");

        if (beard.Length > 0)
            sakalRenderer.sprite = beard[Random.Range(0, beard.Length)];
        else
            Debug.LogWarning("Sakal klasöründe hiç sprite bulunamadı!");
    }
}

