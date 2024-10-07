using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float speed = 1;
    public Rope rope;

    private LineRenderer line;
    private int pointNum = 0;
    private Vector2 direction;
    private bool stop = false;
    private const float distBetwConst = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        direction = (line.GetPosition(pointNum) - transform.position).normalized;
        line.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (rope.getStop())
        {
            stop = true;
        }
        if(!stop)
        {
            Vector2 move = direction * speed * Time.deltaTime;
            transform.position += new Vector3(move.x, move.y, 0);
        }
        if(Vector2.Distance(line.GetPosition(pointNum), transform.position) < distBetwConst)
        {
            if (pointNum == line.positionCount - 1)
            {
                stop = true;
            }
            else
            {
                pointNum += 1;
                direction = (line.GetPosition(pointNum) - transform.position).normalized;
            }
        }
    }
}
