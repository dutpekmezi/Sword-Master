using UnityEngine;

namespace dutpekmezi
{
    public abstract class AbiliyBase
    {
        public virtual void Use(CharacterBase character)
        {
        }

        public void CanUse(CharacterBase character)
        {

        }
    }
}