using UnityEngine;

namespace dutpekmezi
{
    public abstract class AbilityBase : ScriptableObject
    {
        public virtual void UseAbility(Entity owner)
        {
            if (CanUse(owner))
            {
                ExecuteAbility(owner);
            }
        }

        protected abstract void ExecuteAbility(Entity owner);

        protected virtual bool CanUse(Entity owner)
        {
            return true;
        }
    }
}
