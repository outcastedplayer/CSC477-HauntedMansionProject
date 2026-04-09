using UnityEngine;

public class HouseSinkerController : MonoBehaviour
{
    public float sinkSpeed = 1.5f;
    public float targetY = -15f; // How deep it goes
    private bool shouldSink = false;

    void Update()
    {
        if (shouldSink)
        {
            Vector3 targetPos = new Vector3(transform.position.x, targetY, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, sinkSpeed * Time.deltaTime);

            // Optional: Add a slight shake while sinking
            transform.position += new Vector3(Mathf.Sin(Time.time * 20) * 0.02f, 0, 0);
        }
    }

    public void StartSinking()
    {
        shouldSink = true;
    }
}

//Attach to house parent object