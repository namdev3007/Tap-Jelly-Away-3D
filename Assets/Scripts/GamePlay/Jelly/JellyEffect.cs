using UnityEngine;

public class JellyEffect : MonoBehaviour
{
    [Header("Cài đặt hiệu ứng")]
    public float bounceSpeed = 10f;     // tốc độ rung
    public float bounceAmount = 0.1f;   // biên độ rung
    public float returnSpeed = 5f;      // tốc độ trở về kích thước gốc

    private Vector3 originalScale;
    private Vector3 targetScale;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
        Pulse();
    }

    void Update()
    {
        // Áp dụng dao động nhỏ kiểu "cục thạch"
        float scaleX = originalScale.x + Mathf.Sin(Time.time * bounceSpeed) * bounceAmount;
        float scaleY = originalScale.y + Mathf.Cos(Time.time * bounceSpeed) * bounceAmount;
        float scaleZ = originalScale.z + Mathf.Sin(Time.time * bounceSpeed * 0.8f) * bounceAmount;

        targetScale = new Vector3(scaleX, scaleY, scaleZ);

        // Smooth scale về vị trí dao động
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * returnSpeed);
    }

    // Gọi hàm này khi va chạm hoặc click để tạo “nảy”
    public void Pulse(float power = 0.2f)
    {
        StartCoroutine(PulseRoutine(power));
    }

    System.Collections.IEnumerator PulseRoutine(float power)
    {
        Vector3 pulseScale = originalScale * (1 + power);
        float t = 0;

        while (t < 1)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, pulseScale, t);
            t += Time.deltaTime * 8f;
            yield return null;
        }

        t = 0;
        while (t < 1)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, t);
            t += Time.deltaTime * 8f;
            yield return null;
        }
    }
}
