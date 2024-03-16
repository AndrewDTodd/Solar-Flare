using UnityEngine;
using System.Collections;
using System.Xml.Schema;
using System.Threading.Tasks;
using System;
using System.Threading;

public class mysteryInvader : MonoBehaviour 
{

	[SerializeField]
	private GameObject forceField;
    private SphereCollider forceFieldCollider;
	[SerializeField]
	private int points;
    [SerializeField]
    private GameObject mysteryProjectilePrefab;

    [SerializeField]
    private float speed = 3.0f;

	private bool projectileLive = false;

    private GameObject mysteryInvaderGO;
    private MysteryProjectile instance;

    private int xMin = -100;
    private int xMax = 100;

    private SceneManager_gamescene _mySceneManager = null;

    private static CancellationTokenSource cancelSource = new();

    private bool moving = false;

    // Use this for initialization
    void Start () 
	{
        points = UnityEngine.Random.Range(50, 301);

        mysteryInvaderGO = Instantiate(mysteryProjectilePrefab);
        instance = mysteryInvaderGO.GetComponent<MysteryProjectile>();

        _mySceneManager = SceneManager_gamescene.instance;

        forceFieldCollider = forceField.GetComponent<SphereCollider>();
    }
	
	// Update is called once per frame
	async void Update () 
	{
        if (!moving)
        {
            moving = true;
            int xPos = UnityEngine.Random.Range(xMin, xMax);

            Vector3 destination = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
            destination.x = xPos;

            await Move(destination, cancelSource.Token);
            await Fire(cancelSource.Token);
            moving = false;
        }
    }

    void OnDisable()
    {
        if (_mySceneManager != null)
        {
            _mySceneManager.RegisterMysteryHit(points, this.transform.position);
        }

        cancelSource.Cancel();
        cancelSource.Dispose();
    }

    public void RegisterMysteryProjectile()
    {
        projectileLive = false;
    }

    private async Task Move(Vector3 destination, CancellationToken cancelToken)
    {
        cancelToken.ThrowIfCancellationRequested();

        while (this.transform.position != destination)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, destination, speed * Time.deltaTime);
            await Task.Yield();
        }
    }

    private async Task Fire(CancellationToken cancelToken)
    {
        if (!projectileLive && UnityEngine.Random.value <= .8f)
        {
            forceFieldCollider.enabled = false;
            forceField.SetActive(false);

            projectileLive = true;

            if (instance != null && !instance.isActive)
            {
                instance.Fire(this.transform.position);
            }

            while(projectileLive)
            {
                cancelToken.ThrowIfCancellationRequested();

                await Task.Yield();
            }
        }

        forceFieldCollider.enabled = true;
        forceField.SetActive(true);
    }
}
