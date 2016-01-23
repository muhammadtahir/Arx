﻿using GenericComponents.Interfaces.States;
using GenericComponents.Interfaces.States.PlatformerCharacter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericComponents.Interfaces;

namespace GenericComponents.StateMachine.States.PlatformerCharacter.TemplateStates
{
    public class BasePlatformerCharacterState : IState<IPlatformerCharacterController, PlatformerCharacterAction>
    {
        public IPlatformerCharacterController StateController { get; set; }
        public float TimeInState { get; set; }

        public virtual void OnStateEnter(PlatformerCharacterAction action)
        {
        }

        public virtual void OnStateExit(PlatformerCharacterAction action)
        {
        }

        public virtual void Perform(PlatformerCharacterAction action)
        {
        }
    }
}
