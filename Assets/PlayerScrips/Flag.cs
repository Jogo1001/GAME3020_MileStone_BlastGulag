using UnityEngine;
using System.Collections;

public class Flag : MonoBehaviour
{
    public float captureTime = 1f;
    public Transform holdOffset;

    private bool isBeingCaptured = false;
    private Coroutine captureRoutine;
    private MonoBehaviour capturingPlayer; // PlayerController or PlayerTwoController

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerOne"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player == null) return;

            bool fHeld = Input.GetKey(KeyCode.F);

            TryStartCapture(player, fHeld);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("PlayerTwo"))
        {
            PlayerTwoController player = other.GetComponent<PlayerTwoController>();
            if (player == null) return;

            bool fHeld = Input.GetKey(KeyCode.Keypad0);

            TryStartCapture(player, fHeld);
        }
    }

    private void TryStartCapture(MonoBehaviour player, bool fHeld)
    {
        bool isHoldingFlag = false;

        if (player is PlayerController pc)
            isHoldingFlag = pc.isHoldingFlag;
        else if (player is PlayerTwoController pc2)
            isHoldingFlag = pc2.isHoldingFlag;

        if (!isHoldingFlag)
        {
            if (fHeld && !isBeingCaptured)
            {
                captureRoutine = StartCoroutine(CaptureFlag(player));
            }
            else if (!fHeld && isBeingCaptured)
            {
                StopCoroutine(captureRoutine);
                isBeingCaptured = false;
            }
        }
    }

    private IEnumerator CaptureFlag(MonoBehaviour player)
    {
        isBeingCaptured = true;
        capturingPlayer = player;

        float timer = 0f;
        while (timer < captureTime)
        {
            bool fHeld = false;

            if (capturingPlayer is PlayerController pc)
                fHeld = Input.GetKey(KeyCode.F);
            else if (capturingPlayer is PlayerTwoController pc2)
                fHeld = Input.GetKey(KeyCode.Keypad0);

            if (capturingPlayer == null || !capturingPlayer.isActiveAndEnabled || !fHeld)
            {
                isBeingCaptured = false;
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // finished capture
        isBeingCaptured = false;

        if (capturingPlayer is PlayerController player1)
            player1.PickUpFlag(this);
        else if (capturingPlayer is PlayerTwoController player2)
            player2.PickUpFlag(this);
    }

    public void AttachToPlayer(Transform attachPoint)
    {
        transform.SetParent(attachPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        GetComponent<Collider2D>().enabled = false;
    }

    public void Drop(Vector3 position)
    {
        transform.SetParent(null);
        transform.position = position;
        GetComponent<Collider2D>().enabled = true;
    }
}
