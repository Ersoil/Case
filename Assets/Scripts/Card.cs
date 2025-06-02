using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

/// <summary>
/// Класс карты, реализующий систему карточных эффектов и их применение к группам юнитов
/// </summary>
public class Card : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Name;          // Текстовое поле для отображения названия карты
    [SerializeField] private TextMeshProUGUI Description;   // Текстовое поле для отображения описания карты
    private CardOptions card;                               // Текущие параметры карты
    private System.Random randint = new System.Random();    // Генератор случайных чисел

    /// <summary>
    /// Событие, вызываемое при выборе карты
    /// </summary>
    public static Action onCardSelected;

    /// <summary>
    /// Структура, описывающая параметры карты
    /// </summary>
    public struct CardOptions
    {
        public string Name;             // Название карты
        public string Description;      // Описание эффекта карты
        public List<Action> Effects;     // Список действий, выполняемых картой
    }

    /// <summary>
    /// Статический класс, содержащий возможные эффекты карт
    /// </summary>
    public static class CardEffects
    {
        /// <summary>
        /// Лечение группы юнитов
        /// </summary>
        /// <param name="count">Количество восстанавливаемого здоровья</param>
        /// <param name="group">Список юнитов для лечения</param>
        public static void ModifyHeal(float count, List<GameObject> group)
        {
            foreach (GameObject unit in group)
            {
                if (unit.TryGetComponent(out Model model))
                {
                    model.Heal(count);
                }
            }
        }

        /// <summary>
        /// Увеличение защиты группы юнитов
        /// </summary>
        /// <param name="defenseBoost">Бонус к защите</param>
        /// <param name="group">Список юнитов для усиления</param>
        public static void ModifyDefense(float defenseBoost, List<GameObject> group)
        {
            foreach (GameObject unit in group)
            {
                if (unit.TryGetComponent(out Model model))
                {
                    model.ModifyDefense(defenseBoost);
                }
            }
        }

        /// <summary>
        /// Увеличение атаки группы юнитов
        /// </summary>
        /// <param name="attackBoost">Бонус к атаке</param>
        /// <param name="group">Список юнитов для усиления</param>
        public static void ModifyAttack(float attackBoost, List<GameObject> group)
        {
            foreach (GameObject unit in group)
            {
                if (unit.TryGetComponent(out Model model))
                {
                    model.ModifyAttack(attackBoost);
                }
            }
        }
    }

    /// <summary>
    /// Массив возможных карт с их эффектами
    /// </summary>
    public CardOptions[] Options = new CardOptions[]
    {
        new CardOptions()
        {
            Name = "Лечение",
            Description = "Лечит ваших юнитов на 50 HP",
            Effects = new List<Action>
            {
                () =>
                {
                    var targetGroup = RoundSystem.GetQueue ? RoundSystem.GetEnemyGroup : RoundSystem.GetAllienGroup;
                    CardEffects.ModifyHeal(50, targetGroup);
                }
            }
        },
        new CardOptions()
        {
            Name = "Усиление защиты",
            Description = "+40 к защите ваших юнитов",
            Effects = new List<Action>
            {
                () =>
                {
                    var targetGroup = RoundSystem.GetQueue ? RoundSystem.GetEnemyGroup : RoundSystem.GetAllienGroup;
                    CardEffects.ModifyDefense(40, targetGroup);
                }
            }
        },
        new CardOptions()
        {
            Name = "Усиление атаки",
            Description = "+30 к атаке ваших юнитов",
            Effects = new List<Action>
            {
                () =>
                {
                    var targetGroup = RoundSystem.GetQueue ? RoundSystem.GetEnemyGroup : RoundSystem.GetAllienGroup;
                    CardEffects.ModifyAttack(30, targetGroup);
                }
            }
        }
    };

    /// <summary>
    /// При активации карты обновляет текстовые поля
    /// </summary>
    private void OnEnable()
    {
        Name.text = card.Name;
        Description.text = card.Description;
    }

    /// <summary>
    /// При деактивации карты выбирает случайный новый эффект
    /// </summary>
    private void OnDisable()
    {
        card = Options[randint.Next(3)];
    }

    /// <summary>
    /// Деактивирует карту
    /// </summary>
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Активирует карту
    /// </summary>
    public void Enable()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Инициализация карты при создании
    /// </summary>
    private void Awake()
    {
        onCardSelected += Disable;
        card = Options[randint.Next(3)];
        Name.text = card.Name;
        Description.text = card.Description;
    }

    /// <summary>
    /// Применяет эффекты текущей карты
    /// </summary>
    public void UseEffect()
    {
        foreach (var effect in card.Effects)
        {
            effect?.Invoke();
        }
        onCardSelected?.Invoke();
    }
}