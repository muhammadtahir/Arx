﻿using GenericComponents.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Standard_Assets._2D.Scripts.Characters.Arx.StateMachine
{
    public class PlatformerCharacterAction
    {
        public float Move { get; private set; }
        public float Vertical { get; private set; }
        public bool Jump { get; private set; }
        public bool JumpOnLedge { get; private set; }
        public bool Roll { get; private set; }
        public AttackType AttackType { get; private set; }
        public bool ReleaseRope { get; private set; }
        public bool Shoot { get; private set; }
        public bool Aiming { get; private set; }
        public bool Throw { get; private set; }
        public bool GrabLadder { get; private set; }
        public bool RollAfterAttack { get; private set; }

        public PlatformerCharacterAction(
            float move,
            float vertical,
            bool jump,
            bool roll,
            AttackType attackType,
            bool releaseRope,
            bool aiming,
            bool shoot,
            bool @throw,
            bool grabLadder,
            bool jumpOnLedge,
            bool rollAfterAttack)
        {
            Move = move;
            Vertical = vertical;
            Jump = jump;
            Roll = roll;
            AttackType = attackType;
            ReleaseRope = releaseRope;
            Aiming = aiming;
            Shoot = shoot;
            Throw = @throw;
            GrabLadder = grabLadder;
            JumpOnLedge = jumpOnLedge;
            RollAfterAttack = rollAfterAttack;
        }
    }
}