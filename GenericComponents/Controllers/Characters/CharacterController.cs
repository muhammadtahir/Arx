﻿using CommonInterfaces.Controllers;
using GenericComponents.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GenericComponents.Controllers.Characters
{
    [RequireComponent(typeof(CharacterStatus))]
    public class CharacterController : MonoBehaviour, ICharacter
    {
        private CharacterStatus _status;

        public virtual bool CanBeAttacked
        {
            get
            {
                return true;
            }
        }

        public virtual bool IsEnemy
        {
            get
            {
                return false;
            }
        }

        public int LifePoints
        {
            get
            {
                return _status.health.lifePoints;
            }
        }

        public int MaxLifePoints
        {
            get
            {
                return _status.health.maxLifePoints;
            }
        }

        public virtual float Attacked(GameObject attacker, int damage, Vector3? hitPoint)
        {
            _status.Damage(damage);
            if (_status.HealthDepleted)
            {
                Kill();
            }
            return LifePoints;
        }

        public virtual void Kill()
        {
            Destroy(this.gameObject);
        }

        protected virtual void Awake()
        {
            _status = GetComponent<CharacterStatus>();
        }
    }
}