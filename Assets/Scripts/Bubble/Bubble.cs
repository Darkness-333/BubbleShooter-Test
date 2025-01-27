using UnityEngine;

public class Bubble : MonoBehaviour {
    public int colorCode { get; private set; }

    public bool isMaxStretched { get; set; }

    private SpriteRenderer rend;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;

    private bool collisionWithBubble;
    private const string BubbleTag = "Bubble";
    private const string DownBorderTag = "DownBorder";

    public bool haveRb => rb != null;

    private void Awake() {
        rend = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        CheckDownBorder(collider.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collisionWithBubble) return;
        if (!haveRb) return;

        CheckDownBorder(collision.gameObject);

        if (collision.gameObject.CompareTag(BubbleTag)) {
            collisionWithBubble = true;
            ServiceLocator.Get<GameController>().HandleBubblePlacement(this, collision);
        }
    }

    public Rigidbody2D AddRb() {
        if (TryGetComponent(out Rigidbody2D rb2)) {
            if (rb2 != null) {
                rb = rb2;
            }
        }
        if (!haveRb) {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        return rb;
    }

    public void RemoveRb() {
        if (haveRb) {
            Destroy(rb);
            rb = null;
        }
    }

    public CircleCollider2D GetCollider() {
        return circleCollider;
    }

    public void ResetCollision() {
        collisionWithBubble = false;
    }

    public void SetColor(int colorCode, Color color) {
        this.colorCode = colorCode;
        rend.color = color;
    }

    private void CheckDownBorder(GameObject collision) {
        if (collision.CompareTag(DownBorderTag)) {
            ServiceLocator.Get<BubblePool>().ReturnBubble(this);

            EventBus.ScoreUpdated?.Invoke(1);
        }
    }

}
