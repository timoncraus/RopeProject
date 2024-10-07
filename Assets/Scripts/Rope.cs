using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public LayerMask collMask;

    private bool stop;
    public bool getStop() { return stop; }
    private LineRenderer lineRend;
    private List<Vector2> ropePositions = new List<Vector2>();
    private const float deltaPath = 0.01f;
    private const float hitVectorCut = 0.1f;
    private const float minDistBetwPoints = 0.1f;

    private void Start()
    {
        lineRend = GetComponent<LineRenderer>();
        ropePositions.Add(transform.position);
        ropePositions.Add(transform.position);
        GetComponent<Renderer>().enabled = false;
    }

    private bool proccessHit(RaycastHit2D hit)
    {
        if (hit.transform.gameObject.tag == "Blade")
        {
            ropePositions.Clear();
            ropePositions.Add(Vector2.zero);
            lineRend.positionCount = 1;
            lineRend.SetPosition(0, Vector2.zero);
            stop = true;
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (!stop)
        {
            Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(screenPosition);
            Vector2 mouseDirecrion = (mousePos - (Vector2)transform.position).normalized;
            float mouseDistance = System.Math.Abs(Vector2.Distance(transform.position, mousePos));
            for (int i = 0; i < mouseDistance / deltaPath; i++)
            {
                Vector2 intermPos = (Vector2)transform.position + mouseDirecrion * i * deltaPath;
                
                RaycastHit2D hitInside = Physics2D.Raycast(
                    intermPos,
                    Vector2.zero,
                    0,
                    collMask
                );
                
                if (hitInside)
                {
                    proccessHit(hitInside);
                    mousePos = (Vector2)transform.position + mouseDirecrion * (i - 1) * deltaPath;
                    break;
                }
                Vector2 last2 = ropePositions[ropePositions.Count - 2];
                RaycastHit2D hitWith = Physics2D.Raycast(
                    intermPos,
                    (last2 - intermPos).normalized,
                    Vector2.Distance(last2, intermPos) - hitVectorCut,
                    collMask
                );
                if (hitWith && !proccessHit(hitWith) && System.Math.Abs(Vector2.Distance(last2, hitWith.point)) > minDistBetwPoints){
                    mousePos = intermPos;
                    ropePositions.RemoveAt(ropePositions.Count - 1);
                    ropePositions.Add(hitWith.point);
                    ropePositions.Add(mousePos);
                }

                if (ropePositions.Count > 2)
                {
                    Vector2 last = ropePositions[ropePositions.Count - 1];
                    last2 = ropePositions[ropePositions.Count - 2];
                    Vector2 last3 = ropePositions[ropePositions.Count - 3];
                    float angle = Vector3.Angle(last2 - last3, last - last3);
                    if(angle >= 90)
                    {
                        ropePositions.RemoveAt(ropePositions.Count - 2);
                    }
                    else
                    {
                        RaycastHit2D hitNewWay = Physics2D.Raycast(
                            last,
                            (last3 - last).normalized,
                            Vector2.Distance(last, last3) - hitVectorCut,
                            collMask
                        );

                        if (!hitNewWay)
                        {
                            Vector2 newWay = last3 - last;
                            Vector2 oldWay = last2 - last;
                            Vector2 force = (Vector2)Vector3.Project(oldWay, newWay);
                            Vector2 pointOnNewWay = last + force;
                            Vector2 vector = pointOnNewWay - last2;
                            Vector2 checkPoint = last2 + vector.normalized * 0.5f;

                            RaycastHit2D hitInsideCheck = Physics2D.Raycast(
                                checkPoint,
                                Vector2.zero,
                                0,
                                collMask
                            );
                            if (!hitInsideCheck || hitInsideCheck.collider == null)
                            {
                                ropePositions.RemoveAt(ropePositions.Count - 2);
                            }
                        }
                    }
                }
            }

            transform.position = mousePos;
            ropePositions.RemoveAt(ropePositions.Count - 1);
            ropePositions.Add(transform.position);

            lineRend.positionCount = ropePositions.Count;
            for (int i = lineRend.positionCount - 1; i >= 0; i--)
            {
                lineRend.SetPosition(i, ropePositions[i]);
            }
        }
    }
}
