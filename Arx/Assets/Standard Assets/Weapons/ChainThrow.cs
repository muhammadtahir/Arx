﻿using CommonInterfaces.Controllers;
using CommonInterfaces.Enums;
using CommonInterfaces.Weapons;
using Extensions;
using GenericComponents.Behaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Standard_Assets.Weapons
{
    [CreateAssetMenu(fileName = "ChainThrow", menuName = "Weapons/Create Chain Throw Weapon", order = 1)]
    public class ChainThrow : BaseWeapon, IChainThrowWeapon
    {
        private float _focusTime;
        private List<ICharacter> _attackedEnemies;
        private ChainedProjectile _instantiatedHeldProjectile;
        private LayerMask _enemyLayer;
        private LayerMask _wallLayer;     

        [SerializeField]
        private int _damage;

        private ChainedProjectile InstantiatedHeldProjectile
        {
            get
            {
                if(_instantiatedHeldProjectile == null)
                {
                    _instantiatedHeldProjectile = Instantiate(projectile);
                    _instantiatedHeldProjectile.Origin = this.RightHandSocket;
                    _instantiatedHeldProjectile.transform.parent = null;
                    _instantiatedHeldProjectile.OnAttackFinish += OnAttackFinishHandler;
                    _instantiatedHeldProjectile.OnTriggerEnter += OnTriggerEnterHandler;
                }
                return _instantiatedHeldProjectile;
            }
        }

        public event Action OnAttackFinish;
        public event Action<Collider2D, ChainedProjectile, Vector3> OnTriggerEnter;

        public ChainedProjectile projectile;

        public bool ReadyToThrow { get { return InstantiatedHeldProjectile.Status == ProjectileStatus.None; } }

        public ChainThrow()
        {
            WeaponType = WeaponType.ChainedProjectile;
        }

        public void FocusThrow()
        {
            _focusTime += Time.deltaTime;
        }

        public void Throw(float degrees)
        {
            if (!ReadyToThrow)
            {
                return;
            }
            StartAttack();
            InstantiatedHeldProjectile.Throw(degrees, _damage);
        }

        public void Reset()
        {
            if(_instantiatedHeldProjectile != null)
            {
                _instantiatedHeldProjectile.Reset();
                _instantiatedHeldProjectile.gameObject.SetActive(false);
            }
        }

        public override void Unequipped()
        {
            if(_instantiatedHeldProjectile != null)
            {
                Destroy(_instantiatedHeldProjectile.gameObject);
                _instantiatedHeldProjectile = null;
            }
        }

        void Awake()
        {
            _attackedEnemies = new List<ICharacter>();
        }

        void OnDestroy()
        {
            if(_instantiatedHeldProjectile != null)
            {
                _instantiatedHeldProjectile.OnAttackFinish -= OnAttackFinishHandler;
            }
        }

        private void OnAttackFinishHandler()
        {
            AttackFinished();
        }

        private void StartAttack()
        {
            InstantiatedHeldProjectile.gameObject.SetActive(true);
            InstantiatedHeldProjectile.Reset();
            _focusTime = 0;
        }

        private void AttackFinished()
        {
            _attackedEnemies.Clear();
            if(OnAttackFinish != null)
            {
                OnAttackFinish();
            }
            InstantiatedHeldProjectile.gameObject.SetActive(false);
        }        

        private void OnTriggerEnterHandler(Collider2D other, ChainedProjectile projectile, Vector3 position)
        {
            if(OnTriggerEnter != null)
            {
                OnTriggerEnter(other, projectile, position);
            }
        }
    }
}