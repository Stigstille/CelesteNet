﻿using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.CelesteNet.Client.Entities {
    public class GhostNameTag : Entity {

        public Entity Tracking;
        public string Name;

        protected Camera Camera;

        public float Alpha = 1f;

        public GhostNameTag(Entity tracking, string name)
            : base(Vector2.Zero) {
            Tracking = tracking;
            Name = name;

            Tag = TagsExt.SubHUD | Tags.Persistent | Tags.PauseUpdate | Tags.TransitionUpdate;
        }

        public override void Update() {
            base.Update();

            if (Tracking != null && Tracking.Scene == null)
                RemoveSelf();
        }

        public override void Render() {
            base.Render();

            if (string.IsNullOrWhiteSpace(Name))
                return;

            Level level = SceneAs<Level>();
            if (level == null)
                return;

            float scale = level.GetScreenScale();

            Vector2 marginSize = CelesteNetClientFont.Measure(Name) * scale;
            marginSize.X *= 0.25f;
            marginSize.Y *= 0.5f;

            float screenMargins = CelesteNetClientModule.Settings.InGameHUD.ScreenMargins * 8f;

            bool isOnScreen = CelesteNetClientUtils.GetClampedScreenPos(
                Tracking?.Position ?? Position,
                level,
                out Vector2 pos,
                marginX: screenMargins + marginSize.X,
                marginY: screenMargins + marginSize.Y,
                offsetY: -16f
            );

            int opacity = CelesteNetClientModule.Settings.InGameHUD.NameOpacity;

            if (!isOnScreen && CelesteNetClientModule.Settings.InGameHUD.OffScreenNames == CelesteNetClientSettings.OffScreenModes.Hidden)
                return;

            if (CelesteNetClientModule.Settings.InGameHUD.OffScreenNames == CelesteNetClientSettings.OffScreenModes.Opacity)
                opacity = CelesteNetClientModule.Settings.InGameHUD.OffScreenNameOpacity;

            float a = Alpha * (opacity / 4f);

            if (a <= 0f)
                return;

            CelesteNetClientFont.DrawOutline(
                Name,
                pos,
                new(0.5f, 1f),
                Vector2.One * 0.5f * scale,
                Color.White * a,
                2f,
                Color.Black * (a * a * a)
            );
        }

    }
}
