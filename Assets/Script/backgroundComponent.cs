using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgroundComponent : MonoBehaviour
{
    [SerializeField] public float speed = 5f;

    private void Start()
    {

    }

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (transform.name == "Graveyard")
            if (transform.position.x <= -17.95f)
                transform.position = new Vector3(transform.position.x + 17.95f * 2, transform.position.y, transform.position.z);

        if (transform.name == "Mountains")
            if (transform.position.x <= -13.43f)
                transform.position = new Vector3(transform.position.x + 13.43f * 2, transform.position.y, transform.position.z);

        if (transform.name == "Ground")
            if (transform.position.x <= -19.90f)
                transform.position = new Vector3(transform.position.x + 19.90f * 2, transform.position.y, transform.position.z);
    }
}
