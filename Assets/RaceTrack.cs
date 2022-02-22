using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
[System.Serializable]
public class Cart
{
    [HideInInspector]
    public RaceTrack m_race_track;
    public float spline_t;
    public float t;
    public int index;
    public Transform cart;
    public float speed = 0.5f;
    public Pokemon m_pokemon;

    private bool isScoring = false;
    public void Initialize()
    {
        if (cart != null)
        {
            cart.position = m_race_track.curves[index].GetPoint(t);
        }
    }

    public void UpdatePod()
    {
        spline_t += (Time.deltaTime * GameManager.Instance.Base_Speed)/ m_race_track.curves[index].length;
        spline_t = spline_t % (m_race_track.curves.Count);
        index = Mathf.Abs((int)spline_t);

        t = spline_t % 1f;
        if(cart != null)
        {
            cart.position = m_race_track.curves[index].GetPoint(t);
            CheckIfScoring();

        }
    }

    void CheckIfScoring()
    {
        Vector3 scoringPosition = m_race_track.curves[m_race_track.scoring_field_curve_index].GetPoint(0.5f);
        if(Vector3.Distance(cart.position, scoringPosition) < 0.1f && isScoring == false)
        {
            isScoring = true;
            GameManager.Instance.AddScore(m_pokemon.pokemon_score);
            //Score
        }
        if(Vector3.Distance(cart.position, scoringPosition) > 1f && isScoring == true)
        {
            isScoring = false;
        }
    }

}

public class RaceTrack : MonoBehaviour
{
    public static RaceTrack Instance;


    public List<Transform> points;
    public List<BezierCurve> curves;
    public List<Cart> carts;
    public Transform target;

    [Range(0, 7)]
    public int landing_strip_curve_index;
    [Range(0, 7)]
    public int scoring_field_curve_index;

    public float track_placement_range = 0.5f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (BezierCurve curve in curves)
        {
            curve.Initialize();
        }
    }
    void Update()
    {
        if (curves.Count < 1)
            return;


        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int index = FindClosestCurve(mousePos);
        Color trackPlacementColor = IsPointOnTrack(mousePos) ? Color.green : Color.red;
        Debug.DrawLine(mousePos, curves[index].GetPoint(0.5f), trackPlacementColor);


        foreach (Cart cart in carts)
        {
            if (cart.m_race_track == null)
                cart.m_race_track = this;
            cart.UpdatePod();
        }
    }

    public bool IsPointOnTrack(Vector2 point)
    {
        float distance;
        FindClosestCurve(point, out distance);
        return distance < track_placement_range;
    }

    [ContextMenu("Spawn Random Cart")]
    public void AddRandomCart()
    {
        AddCartAtT(Random.Range(0, curves.Count));
    }

    public Cart AddCartAtT(float t)
    {
        Cart cart = new Cart();
        cart.spline_t = t + 0.5f;
        cart.cart = Instantiate(target);
        Debug.Log("Instantiated object Cart", cart.cart);
        cart.m_race_track = this;
        cart.Initialize();
        carts.Add(cart);
        return cart;
    }

    public void RemoveCart(Cart cart)
    {
        carts.Remove(cart);
        cart.m_pokemon = null;
    }

    public void SetCartAtPosition(Cart cart, Vector3 point)
    {
        int index = FindClosestCurve(point);
        cart.spline_t = index + 0.5f;
        cart.m_race_track = this;
        cart.Initialize();
        if(!carts.Contains(cart))
            carts.Add(cart);
    }

    public Cart AddCartAtPosition(Vector3 point)
    {
        int index = FindClosestCurve(point);
        return AddCartAtT(index);
    }

    public int FindClosestCurve(Vector3 point)
    {
        float spline_max_length = curves.Count;
        float t = 0.5f;
        int index = 0;
        int selection_index = 0;
        float current_distance = spline_max_length;
        while (index < spline_max_length)
        {
            if(Vector3.Distance(point, curves[index].GetPoint(t)) < current_distance)
            {
                current_distance = Vector3.Distance(point, curves[index].GetPoint(t));
                selection_index = index;
            }
            index += 1;
        }

        return selection_index;
    }

    public int FindClosestCurve(Vector3 point, out float distance)
    {
        float spline_max_length = curves.Count;
        float t = 0.5f;
        int index = 0;
        int selection_index = 0;
        float current_distance = spline_max_length;
        while (index < spline_max_length)
        {
            if (Vector3.Distance(point, curves[index].GetPoint(t)) < current_distance)
            {
                current_distance = Vector3.Distance(point, curves[index].GetPoint(t));
                selection_index = index;
            }
            index += 1;
        }
        distance = current_distance;
        return selection_index;
    }

    public int FindClosestCurve(Vector3 point, out float distance, out bool within_racetrack_threshold)
    {
        float spline_max_length = curves.Count;
        float t = 0.5f;
        int index = 0;
        int selection_index = 0;
        float current_distance = spline_max_length;
        while (index < spline_max_length)
        {
            if (Vector3.Distance(point, curves[index].GetPoint(t)) < current_distance)
            {
                current_distance = Vector3.Distance(point, curves[index].GetPoint(t));
                selection_index = index;
            }
            index += 1;
        }
        within_racetrack_threshold = current_distance < track_placement_range;
        distance = current_distance;
        return selection_index;
    }

    public int lineSteps = 10;
    private void OnDrawGizmos()
    {
        if (points.Count < 1)
            return;
        if (curves.Count < 1)
            return;
        Gizmos.color = Color.green;
 

        foreach (BezierCurve curve in curves)
        {
            Vector3 lineStart = curve.GetPoint(0f);
            for (int i = 1; i <= lineSteps; i++)
            {
                Gizmos.color = Color.cyan;
                Vector3 velocity = curve.GetVelocity(i / (float)lineSteps).normalized;
                Gizmos.DrawRay(lineStart, velocity);

                Gizmos.color = Color.green;
                Vector3 lineEnd = curve.GetPoint(i / (float)lineSteps);
                Gizmos.DrawLine(lineStart, lineEnd);
                lineStart = lineEnd;

                
            }
        }
        DrawLandingStrip();
        DrawScoringField();
    }

    [ContextMenu("Reverse Racetrack")]
    public void ReverseRaceTrack()
    {
        foreach (BezierCurve curve in curves)
        {
            Vector3 start = curve.points[0];
            Vector3 end = curve.points[2];
            curve.points[0] = end;
            curve.points[2] = start;
        }

        curves.Reverse();
    }

    void DrawLandingStrip()
    {
        BezierCurve curve = curves[landing_strip_curve_index];
        Vector3 lineStart = curve.GetPoint(0f);
        for (int i = 1; i <= lineSteps; i++)
        {
            Gizmos.color = Color.blue;
            Vector3 lineLeft = lineStart - Vector3.left * 0.1f;
            Vector3 lineRight = lineStart + Vector3.left * 0.1f;
            Vector3 lineEnd = curve.GetPoint(i / (float)lineSteps);
            Gizmos.DrawLine(lineLeft, lineEnd);
            Gizmos.DrawLine(lineRight, lineEnd);
            lineStart = lineEnd;
        }
    }

    void DrawScoringField()
    {
        BezierCurve curve = curves[scoring_field_curve_index];
        Vector3 ScoringPosition = curve.GetPoint(0.5f);
        Gizmos.DrawCube(ScoringPosition, Vector3.one * 0.25f);
    }

    internal float GetScore()
    {
        float sum = 0;
        foreach (Cart cart in carts)
        {
            if(cart.m_pokemon != null)
                sum += cart.m_pokemon.GetScore();
        }

        return sum;
    }
}
