using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonClick : MonoBehaviour
{
    public BallPrefab ballPrefab;

    void Update()
    {
        if (Touchscreen.current.press.wasPressedThisFrame)
        {
            if (!GameManager.Instance.IsGameActive())
            {
                GameManager.Instance.RestartGame();
                return;
            }

            BallPrefab ball = Instantiate<BallPrefab>(ballPrefab);
            ball.transform.localPosition = transform.position;

            Rigidbody rb = ball.GetComponent<Rigidbody>();
            rb.linearVelocity = Camera.main.transform.forward * 60f;

            GameManager.Instance.RegisterShot();
            Destroy(ball.gameObject, 5f);
        }
    }
}