//
// Candy.cs
//
// Author:
//       Cosmos and paradox <cosmos.n.paradox@gmail.com>
//
// Copyright (c) 2016 Cosmos and paradox

using UnityEngine;
using System.Collections;

namespace Match3CS {
	
	public class Candy : MonoBehaviour
	{
		
		// Animator Parameters
		private struct AnimatorParamaters {
			public int onLanding;
			public int onDestroy;
			public int onShake;
			public int onGlow;
			public int isHint;
		}

		private AnimatorParamaters _animatorParameters;
		private Animator _animator;

		private void Awake() {
			
			_animator = GetComponent<Animator>();
			_animatorParameters = new AnimatorParamaters {
				onLanding = Animator.StringToHash("onLanding"),
				onDestroy = Animator.StringToHash("onDestroy"),
				onShake = Animator.StringToHash("onShake"),
				onGlow = Animator.StringToHash("onGlow"),
				isHint = Animator.StringToHash("isHint"),
			};
		}

		public void Landing() {
			_animator.SetTrigger(_animatorParameters.onLanding);
		}

		public void Destroy() {
			_animator.SetTrigger(_animatorParameters.onDestroy);
		}

		public void Shake() {
			_animator.SetTrigger(_animatorParameters.onShake);
		}

		public void Glow() {
			_animator.SetTrigger(_animatorParameters.onGlow);
		}

		public void Hint(bool state = true) {
			_animator.SetBool(_animatorParameters.isHint, state);
		}
	
	}
	
}
