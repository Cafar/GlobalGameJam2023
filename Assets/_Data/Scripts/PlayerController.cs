using DG.Tweening;
using DG.Tweening.Plugins.Options;
using Rewired;
using SOA;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("PlayerConfig")]
    [SerializeField]
    private FloatVariable stepDistance;
    [SerializeField]
    private FloatVariable threshold;

    [SerializeField]
    private FloatVariable timeLineBar;
    [SerializeField]
    private FloatVariable timeLineBeat;

    private float lastTimeLineBar;
    private float timeElapsed;
    private Player player; // The Rewired Player

    private Vector3 dir;
    private bool moved;
    private float distanceToCenter;

    void Awake()
    {
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
        if (timeLineBar % 2 == 0 && timeLineBeat.value == 1)
        {
            if (player.GetButton("Hit"))
            {
                if (!moved && distanceToCenter >= .45f)
                {
                    dir = (transform.right * stepDistance.value) + transform.position;
                    transform.DOMove(dir, 0.2f);
                    moved = true;
                }
            }
            else
            {
                if (!moved && distanceToCenter < 5.25f)
                {
                    dir = (-transform.right * stepDistance.value) + transform.position;
                    transform.DOMove(dir, 0.2f);
                    moved = true;
                }
            }
        }

        if (timeLineBar % 2 != 0)
        {
            moved = false;
        }
    }

    private void BeatTracker_upBeatUpdate()
    {
        Debug.Log("HOAL");
    }

    public void SetId(int id)
    {
        player = ReInput.players.GetPlayer(id);
    }
}
