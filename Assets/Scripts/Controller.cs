using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ���������� ���������� ������, �������������� ���� ������ � ����������� ���������� �����.
/// ��������� ������� ������� ��������, ������������ ���� � ����������� �����������.
/// </summary>
public class Controller : MonoBehaviour
{
    // ��������� �����������
    private bool isChosen = false;    // ���� ������ �����
    private bool isOff = false;       // ���� ���������� �����������

    // ������ �� ����������
    [SerializeField] private Model myModel;   // ������ �����
    [SerializeField] private Unit myUnit;     // ������ �����
    private SuperHitBar hitbar;               // ����� �����-�����

    // ������� ��������
    private List<Action> actions = new List<Action>();  // ������� �������� �����

    // ������������ ����
    private List<Vector2> pathPoints = new List<Vector2>();  // ����� ����
    private GameObject pathLineObject;                       // ������ ����� ����
    private LineRenderer pathLineRenderer;                   // �������� ����� ����

    // �������
    Action actionChange;      // ������� ��������� ��������
    Action onEndActions;      // ������� ���������� ���� ��������

    private void Awake()
    {
        // ������������� �����������
        myModel = GetComponent<Model>();
        myUnit = GetComponent<Unit>();

        // ��������� ������� ��������� ��������
        actionChange = () => myUnit.ChangeFreeActions();

        // �������� ������������ ����
        pathLineObject = new GameObject("PathLine");
        pathLineRenderer = pathLineObject.AddComponent<LineRenderer>();
        pathLineRenderer.startWidth = 0.1f;
        pathLineRenderer.endWidth = 0.1f;
        pathLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        pathLineRenderer.startColor = Color.cyan;
        pathLineRenderer.endColor = Color.cyan;
        pathLineObject.SetActive(false);
    }

    /// <summary>
    /// ������������ ���� �� �����, ������� ��� � ������ ������������ ����
    /// </summary>
    private void OnMouseDown()
    {
        isChosen = true;
        ClearPath();
        pathPoints.Add(transform.position);
        UpdatePathLine();
        pathLineObject.SetActive(true);
    }

    /// <summary>
    /// ����������� ��������� ���������� �����������
    /// </summary>
    public void ChangeControllerStatus()
    {
        isOff = !isOff;
    }

    private void Update()
    {
        if (isChosen && !isOff)
        {
            // ��������� �������� (���)
            if (Input.GetMouseButtonDown(1))
            {
                Vector2 touch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                actions.Add(() => myModel.SetTarget(touch));
                pathPoints.Add(touch);
                UpdatePathLine();
                actionChange?.Invoke();
            }

            // ��������� �������� (������� S)
            if (Input.GetKeyDown(KeyCode.S))
            {
                Vector2 touch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                actions.Add(() => myModel.SetBulletTarget(touch));
                DrawTemporaryLine(touch, Color.red, 1f);
                actionChange?.Invoke();
            }

            // ��������� �����-����� (������� Space)
            if (Input.GetKeyDown(KeyCode.Space) && SuperHitBar.IsHavePoints(myUnit.IsEnemy))
            {
                actions.Add(() => myModel.SetSuperHit());
                DrawCircleAroundObject(1f, Color.yellow, 1f);
                actionChange?.Invoke();
            }

            // ������ ������ (������� Escape)
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isChosen = false;
                ClearPath();
            }
        }
    }

    /// <summary>
    /// ��������� ������������ ����
    /// </summary>
    private void UpdatePathLine()
    {
        pathLineRenderer.positionCount = pathPoints.Count;
        for (int i = 0; i < pathPoints.Count; i++)
        {
            pathLineRenderer.SetPosition(i, pathPoints[i]);
        }
    }

    /// <summary>
    /// ������ ��������� ����� � ����
    /// </summary>
    /// <param name="target">�������� ����� �����</param>
    /// <param name="color">���� �����</param>
    /// <param name="duration">����� ����������� � ��������</param>
    private void DrawTemporaryLine(Vector2 target, Color color, float duration)
    {
        GameObject lineObj = new GameObject("TempLine");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.positionCount = 2;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, target);
        Destroy(lineObj, duration);
    }

    /// <summary>
    /// ������ ��������� ���������� ������ �������
    /// </summary>
    /// <param name="radius">������ ����������</param>
    /// <param name="color">���� ����������</param>
    /// <param name="duration">����� ����������� � ��������</param>
    private void DrawCircleAroundObject(float radius, Color color, float duration)
    {
        GameObject circleObj = new GameObject("TempCircle");
        LineRenderer lr = circleObj.AddComponent<LineRenderer>();
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;

        int segments = 36;
        lr.positionCount = segments + 1;
        Vector3[] points = new Vector3[segments + 1];
        for (int i = 0; i < segments + 1; i++)
        {
            float angle = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = transform.position + new Vector3(Mathf.Sin(angle) * radius, Mathf.Cos(angle) * radius, 0);
        }
        lr.SetPositions(points);
        Destroy(circleObj, duration);
    }

    /// <summary>
    /// ������� ������� ���� � ��� ������������
    /// </summary>
    private void ClearPath()
    {
        pathPoints.Clear();
        pathLineRenderer.positionCount = 0;
        pathLineObject.SetActive(false);
    }

    /// <summary>
    /// �������� ���������� ������� ��������
    /// </summary>
    private IEnumerator InitActionsCoroutine()
    {
        bool exit = false;
        int i = 0;
        while (!exit && actions.Count != 0)
        {
            if (myModel.GetReadyStatus)
            {
                actions[i]?.Invoke();
                i++;
                if (i == actions.Count)
                {
                    exit = true;
                    onEndActions?.Invoke();
                    isChosen = false;
                    actions.Clear();
                    ClearPath();
                }
            }
            yield return new WaitForSeconds(1);
        }
    }

    /// <summary>
    /// ��������� ���������� ������� ��������
    /// </summary>
    public void InitActions()
    {
        myModel.StartActions();
        StartCoroutine(InitActionsCoroutine());
    }

    private void OnDisable()
    {
        // ������������ �� ������� ��� ����������
        actionChange -= myUnit.ChangeFreeActions;
        ClearPath();
    }
}