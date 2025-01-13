using System;
using System.Collections;
using System.Collections.Generic;
using Milease.Core;
using Milease.Core.Animator;
using Milease.DSL;
using Milease.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Minity.SceneRouter
{
    public class BlackFade : LoadingAnimator
    {
        public Image Panel;
        
        public override void AboutToLoad()
        {
            MilInstantAnimator.Start(
                    0.5f / Panel.MQuad(x => x.color, Color.clear, Color.black)
                )
                .Then(
                    new Action(ReadyToLoad).AsMileaseKeyEvent()
                )
                .UsingResetMode(RuntimeAnimationPart.AnimationResetMode.ResetToInitialState)
                .PlayImmediately();
        }

        public override void OnLoaded()
        {
            MilInstantAnimator.Start(
                    0.5f / Panel.MQuad(x => x.color, Color.black, Color.clear)
                )
                .Then(
                    new Action(FinishLoading).AsMileaseKeyEvent()
                )
                .UsingResetMode(RuntimeAnimationPart.AnimationResetMode.ResetToInitialState)
                .PlayImmediately();
        }
    }
}
