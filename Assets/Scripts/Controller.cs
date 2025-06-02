using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Контроллер управления юнитом, обрабатывающий ввод игрока и управляющий действиями юнита.
/// Реализует систему очереди действий, визуализацию пути и специальные способности.
/// </summary>
public class Controller : MonoBehaviour
{
    // Состояния контроллера
    private bool isChosen = false;    // Флаг выбора юнита
    private bool isOff = false;       // Флаг отключения контроллера

    // Ссылки на компоненты
    [SerializeField] private Model myModel;   // Модель юнита
    [SerializeField] private Unit myUnit;     // Данные юнита
    private SuperHitBar hitbar;               // Шкала супер-удара

    // Система действий
    private List<Action> actions = new List<Action>();  // Очередь действий юнита

    // Визуализация пути
    private List<Vector2> pathPoints = new List<Vector2>();  // Точки пути
    private GameObject pathLineObject;                       // Объект линии пути
    private LineRenderer pathLineRenderer;                   // Рендерер линии пути

    // События
    Action actionChange;      // Событие изменения действия
    Action onEndActions;      // Событие завершения всех действий

    private void Awake()
    {
        // Инициализация компонентов
        myModel = GetComponent<Model>();
        myUnit = GetComponent<Unit>();

        // Настройка события изменения действия
        actionChange = () => myUnit.ChangeFreeActions();

        // Создание визуализации пути
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
    /// Обрабатывает клик по юниту, выбирая его и начина визуализацию пути
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
    /// Переключает состояние активности контроллера
    /// </summary>
    public void ChangeControllerStatus()
    {
        isOff = !isOff;
    }

    private void Update()
    {
        if (isChosen && !isOff)
        {
            // Обработка движения (ПКМ)
            if (Input.GetMouseButtonDown(1))
            {
                Vector2 touch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                actions.Add(() => myModel.SetTarget(touch));
                pathPoints.Add(touch);
                UpdatePathLine();
                actionChange?.Invoke();
            }

            // Обработка выстрела (клавиша S)
            if (Input.GetKeyDown(KeyCode.S))
            {
                Vector2 touch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                actions.Add(() => myModel.SetBulletTarget(touch));
                DrawTemporaryLine(touch, Color.red, 1f);
                actionChange?.Invoke();
            }

            // Обработка супер-удара (клавиша Space)
            if (Input.GetKeyDown(KeyCode.Space) && SuperHitBar.IsHavePoints(myUnit.IsEnemy))
            {
                actions.Add(() => myModel.SetSuperHit());
                DrawCircleAroundObject(1f, Color.yellow, 1f);
                actionChange?.Invoke();
            }

            // Отмена выбора (клавиша Escape)
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isChosen = false;
                ClearPath();
            }
        }
    }

    /// <summary>
    /// Обновляет визуализацию пути
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
    /// Рисует временную линию к цели
    /// </summary>
    /// <param name="target">Конечная точка линии</param>
    /// <param name="color">Цвет линии</param>
    /// <param name="duration">Время отображения в секундах</param>
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
    /// Рисует временную окружность вокруг объекта
    /// </summary>
    /// <param name="radius">Радиус окружности</param>
    /// <param name="color">Цвет окружности</param>
    /// <param name="duration">Время отображения в секундах</param>
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
    /// Очищает текущий путь и его визуализацию
    /// </summary>
    private void ClearPath()
    {
        pathPoints.Clear();
        pathLineRenderer.positionCount = 0;
        pathLineObject.SetActive(false);
    }

    /// <summary>
    /// Корутина выполнения очереди действий
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
    /// Запускает выполнение очереди действий
    /// </summary>
    public void InitActions()
    {
        myModel.StartActions();
        StartCoroutine(InitActionsCoroutine());
    }

    private void OnDisable()
    {
        // Отписываемся от событий при отключении
        actionChange -= myUnit.ChangeFreeActions;
        ClearPath();
    }
}