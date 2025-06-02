using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

/// <summary>
/// ����� �����, ����������� ������� ��������� �������� � �� ���������� � ������� ������
/// </summary>
public class Card : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Name;          // ��������� ���� ��� ����������� �������� �����
    [SerializeField] private TextMeshProUGUI Description;   // ��������� ���� ��� ����������� �������� �����
    private CardOptions card;                               // ������� ��������� �����
    private System.Random randint = new System.Random();    // ��������� ��������� �����

    /// <summary>
    /// �������, ���������� ��� ������ �����
    /// </summary>
    public static Action onCardSelected;

    /// <summary>
    /// ���������, ����������� ��������� �����
    /// </summary>
    public struct CardOptions
    {
        public string Name;             // �������� �����
        public string Description;      // �������� ������� �����
        public List<Action> Effects;     // ������ ��������, ����������� ������
    }

    /// <summary>
    /// ����������� �����, ���������� ��������� ������� ����
    /// </summary>
    public static class CardEffects
    {
        /// <summary>
        /// ������� ������ ������
        /// </summary>
        /// <param name="count">���������� ������������������ ��������</param>
        /// <param name="group">������ ������ ��� �������</param>
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
        /// ���������� ������ ������ ������
        /// </summary>
        /// <param name="defenseBoost">����� � ������</param>
        /// <param name="group">������ ������ ��� ��������</param>
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
        /// ���������� ����� ������ ������
        /// </summary>
        /// <param name="attackBoost">����� � �����</param>
        /// <param name="group">������ ������ ��� ��������</param>
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
    /// ������ ��������� ���� � �� ���������
    /// </summary>
    public CardOptions[] Options = new CardOptions[]
    {
        new CardOptions()
        {
            Name = "�������",
            Description = "����� ����� ������ �� 50 HP",
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
            Name = "�������� ������",
            Description = "+40 � ������ ����� ������",
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
            Name = "�������� �����",
            Description = "+30 � ����� ����� ������",
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
    /// ��� ��������� ����� ��������� ��������� ����
    /// </summary>
    private void OnEnable()
    {
        Name.text = card.Name;
        Description.text = card.Description;
    }

    /// <summary>
    /// ��� ����������� ����� �������� ��������� ����� ������
    /// </summary>
    private void OnDisable()
    {
        card = Options[randint.Next(3)];
    }

    /// <summary>
    /// ������������ �����
    /// </summary>
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ���������� �����
    /// </summary>
    public void Enable()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// ������������� ����� ��� ��������
    /// </summary>
    private void Awake()
    {
        onCardSelected += Disable;
        card = Options[randint.Next(3)];
        Name.text = card.Name;
        Description.text = card.Description;
    }

    /// <summary>
    /// ��������� ������� ������� �����
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