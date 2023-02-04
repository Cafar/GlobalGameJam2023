using DG.Tweening;
using DG.Tweening.Plugins.Options;
using Rewired;
using SOA;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("PlayerConfig")]
    [SerializeField]
    private FloatVariable stepDistance;
    [SerializeField]
    private GameObject water;

    [SerializeField]
    private FloatVariable timeLineBar;
    [SerializeField]
    private FloatVariable timeLineBeat;
    [SerializeField]
    private List<Transform> roots;

    private float lastTimeLineBar;
    private float timeElapsed;
    private Player player; // The Rewired Player

    private Vector3 dir;
    private bool moved;
    private float distanceToCenter;
    private int currentRoot;

    void Awake()
    {
        water.SetActive(false);
        currentRoot = -1;
        lastTimeLineBar = 0;
        SetId(0);
        ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(true, "Gameplay");
        var dir = Vector3.zero - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void FixedUpdate()
    {
        distanceToCenter = Vector2.Distance(transform.position, Vector2.zero);
        timeElapsed += Time.deltaTime;
        if (timeLineBar % 2 == 0 && timeLineBeat.value == 2)
        {
            if (player.GetButton("Hit"))
            {
                if (!moved && distanceToCenter > .45f)
                {
                    currentRoot++;
                    roots[currentRoot].gameObject.SetActive(true);
                    dir = (transform.right * stepDistance.value) + transform.position;
                    transform.DOMove(dir, 0.1f);
                    moved = true;
                }
            }
            else
            {
                if (!moved && distanceToCenter < 5.25f)
                {
                    roots[currentRoot].gameObject.SetActive(false);
                    dir = (-transform.right * stepDistance.value) + transform.position;
                    transform.DOMove(dir, 0.1f);
                    moved = true;
                    currentRoot--;
                }
            }
        }

        if (timeLineBar % 2 != 0)
        {
            moved = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("HOA");
        if(collision.CompareTag("Core"))
        {
            water.SetActive(true);
        }
    }

    public void SetId(int id)
    {
        player = ReInput.players.GetPlayer(id);
    }
}
