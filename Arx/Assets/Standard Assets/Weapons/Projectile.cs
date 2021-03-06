﻿using CommonInterfaces.Controllers;
using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Standard_Assets.Weapons
{
    public class Projectile : MonoBehaviour
    {
        private Vector3 _direction;
        public float speed;
        public float lifetime = 4;
        [SerializeField]
        private LayerMask _colliderLayer;
        [SerializeField]
        private LayerMask _enemyLayer;
        [SerializeField]
        private int _damage;

        public LayerMask EnemyLayer
        {
            get
            {
                return _enemyLayer;
            }
            set
            {
                _enemyLayer = value;
            }
        }
        public int Damage
        {
            get
            {
                return _damage;
            }
            set
            {
                _damage = value;
            }
        }
        public GameObject Attacker { get; set; }
        public Vector3 Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
                var localScale = transform.localScale;
                transform.localScale = new Vector3(Mathf.Sign(_direction.x), localScale.y, localScale.z);
            }
        }

        void Update()
        {
            this.transform.position += Direction * (speed * Time.deltaTime);
            lifetime -= Time.deltaTime;
            if(lifetime <= 0)
            {
                Destroy(this.gameObject);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.isTrigger)
            {
                return;
            }
            if (_colliderLayer.IsInAnyLayer(other.gameObject))
            {
                Destroy(this.gameObject);
                return;
            }
            if (!EnemyLayer.IsInAnyLayer(other.gameObject))
            {
                return;
            }
            var character = other.gameObject.GetComponent<ICharacter>();
            if(character == null)
            {
                return;
            }
            character.Attacked(Attacker, Damage, this.transform.position, DamageType.Shoot);
            Destroy(this.gameObject);
        }
    }
}
