using System;
using UnityEngine;

namespace HellSpawn
{
    public class Health : MonoBehaviour
    {
        public int Current;
        public int Total;
        public Action<bool> OnDepleted;
        public Action<int> OnDamaged;
        public Action<int> OnHealed;
        public bool DestroyOnDepleted;

        void Awake()
        {
            Current = Total;
        }

        public void Damage(int amount)
        {
            Current -= amount;
            OnDamaged?.Invoke(amount);
            if (Current <= 0 && DestroyOnDepleted) Destroy(gameObject);
        }

        public void Heal(int amount)
        {
            Current = System.Math.Min(Current + amount, Total);
            OnHealed?.Invoke(amount);
        }
    }
}
