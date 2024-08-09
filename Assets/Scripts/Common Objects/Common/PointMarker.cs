using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PointMarker : CommonActivableStatelessObject
{
    public float Width = 1.4f;
    public float Height = 5f;
    public int PointMarkerID = 2;
    public float DimensionType = 0f;
    public float StageType = 0f;

    public Animator pointMarkerLeft;
    public Animator pointMarkerRight;

    public GameObject laser;

    public AudioClip checkpoint;

    private MeshRenderer laserRenderer;
    private Color laserColor;
    private BoxCollider boxCollider;
    private AudioSource audioSource;

    private SkinnedMeshRenderer skinnedMeshRendererLeft;
    private SkinnedMeshRenderer skinnedMeshRendererRight;

    public override void OnValidate()
    {
        base.OnValidate();
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector3(Width, Height, 0.1f);
        boxCollider.center = new Vector3(0, Height * 0.5f, 0);

        laser.transform.localScale = new Vector3(Width, 1, 1);

        pointMarkerLeft.transform.localPosition = new Vector3(Width * 0.5f, 0, 0);
        pointMarkerRight.transform.localPosition = new Vector3(-Width * 0.5f, 0, 0);
    }

    public void Start()
    {
        laserRenderer = laser.GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        audioSource = GetComponent<AudioSource>();

        skinnedMeshRendererLeft = pointMarkerLeft.GetComponentInChildren<SkinnedMeshRenderer>();
        skinnedMeshRendererRight = pointMarkerRight.GetComponentInChildren<SkinnedMeshRenderer>();

        pointMarkerLeft.SetBool("Active", active);
        pointMarkerRight.SetBool("Active", active);

        OnPlayerTriggerEnter.AddListener(Checkpoint);

        if (!active)
        {
            Deactivate();
        }
    }

    private void Update()
    {
        if (active)
        {
            laserColor = Color.Lerp(Color.white, Color.clear, Mathf.PingPong(Time.time * 25, 1));
            laserRenderer.sharedMaterial.color = laserColor;
        }
    }

    private void Checkpoint()
    {
        if (active)
        {
            pointMarkerLeft.SetTrigger("Checkpoint");
            pointMarkerRight.SetTrigger("Checkpoint");
            audioSource.PlayOneShot(checkpoint);

            Deactivate();
            GameManager.instance.lastCheckpoint = (int)PointMarkerID;
            GameManager.instance.activeCheckpoints.Add(PointMarkerID);
            if (!GameManager.instance.saveData.achievements.Contains(Achievements.getCheckpoint.ToString()))
            {
                GameManager.instance.achievementUI.Show(Achievements.achievements[Achievements.getCheckpoint], GameManager.instance.settings.languageType);
                GameManager.instance.saveData.achievements += Achievements.getCheckpoint + ",";
            }
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();
        skinnedMeshRendererLeft.materials[4].color = Color.yellow;
        skinnedMeshRendererRight.materials[4].color = Color.yellow;
        laser.gameObject.SetActive(false);
        boxCollider.enabled = false;
    }
}