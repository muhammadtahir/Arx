﻿using GenericComponents.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GenericComponents.Behaviours
{
    public class CharacterStatus : MonoBehaviour
    {
        [SerializeField]
        private bool _immortal;

        public Health health;

        public bool HealthDepleted
        {
            get
            {
                return health.lifePoints <= 0;
            }
        }

        public void Damage(int damagePoints)
        {
            if (_immortal)
            {
                return;
            }
            health.lifePoints -= damagePoints;
        }

        public void Heal(int healPoints)
        {
            health.lifePoints += healPoints;
            if (health.lifePoints > health.maxLifePoints)
            {
                health.lifePoints = health.maxLifePoints;
            }
        }
    }
}
