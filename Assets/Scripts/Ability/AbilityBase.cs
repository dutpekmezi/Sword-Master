using UnityEngine;

namespace dutpekmezi
{
    public abstract class AbilityBase<T> : ScriptableObject
    {
        public virtual void UseAbility(T owner)
        {
            if (CanUse(owner))
            {
                ExecuteAbility(owner);
            }
        }

        protected abstract void ExecuteAbility(T owner);

        protected virtual bool CanUse(T owner)
        {
            return true;
        }
    }
}