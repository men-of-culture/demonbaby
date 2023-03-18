
using UnityEngine;

public class projectileScript : MonoBehaviour
{
    public int projectileSpeed;
    public Vector3 startPoint;
    public GameObject playerOrigin;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = playerOrigin.transform.position;
        startPoint = playerOrigin.transform.position;
        transform.position += new Vector3(0, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z >= startPoint.z + 200)
        {
            Destroy(this.gameObject);
            
        }

        transform.position += transform.forward * Time.deltaTime * projectileSpeed;
    }
}
