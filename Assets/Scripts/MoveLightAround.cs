using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLightAround : MonoBehaviour
{
    public float moveSpeed = 1;
    public List<GameObject> points;

    Coroutine coroutine;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (coroutine == null)
        {
            coroutine = StartCoroutine(MoveCycle());
        }
    }

    IEnumerator MoveCycle()
    {
        List<GameObject> path = new List<GameObject>();
        path.AddRange(points);

        while(true)
        {
            if (path.Count == 0)
                break;

            Vector3 end = path[0].transform.position;
            transform.position = Vector3.MoveTowards(transform.position, end, moveSpeed);
            if(transform.position == end)
            {
                path.RemoveAt(0);
            }
            yield return new WaitForEndOfFrame();
        }
        coroutine = null;
    }
}
