using Terraria.ModLoader;
using System;
using Terraria;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Collections.Generic;
using Terraria.Graphics.Effects;

namespace Realms.Effects
{
    public class BasicNormalEffect : Effect, IEffectMatrices//Using this interface lets the Model.Draw() method set the matrices
    {
        protected EffectParameter _world;
        protected EffectParameter _view;
        protected EffectParameter _projection;

        /// <summary>
        /// float4(1, 1, 1, 1)
        /// </summary>
        public EffectParameter AmbientColor;
        /// <summary>
        /// 0.1
        /// </summary>
        public EffectParameter AmbientIntensity;

        /// <summary>
        /// No default
        /// </summary>
        //public EffectParameter WorldInverseTranspose;

        /// <summary>
        /// float3(1, 0, 0)
        /// </summary>
        public EffectParameter LightDirection;
        /// <summary>
        /// float4(1, 1, 1, 1)
        /// </summary>
        public EffectParameter DiffuseColor;
        /// <summary>
        /// 1.0
        /// </summary>
        public EffectParameter DiffuseIntensity;

        /// <summary>
        /// 200
        /// </summary>
        //public EffectParameter Shininess;
        /// <summary>
        /// float4(1, 1, 1, 1)
        /// </summary>
        public EffectParameter SpecularColor;
        /// <summary>
        /// 1
        /// </summary>
        //public EffectParameter SpecularIntensity;
        /// <summary>
        /// float3(1, 0, 0)
        /// </summary>
        public EffectParameter EyePosition;

        /// <summary>
        /// No default
        /// </summary>
        public EffectParameter TextureMap;

        /// <summary>
        /// 1
        /// </summary>
        //public EffectParameter BumpConstant;
        /// <summary>
        /// No default
        /// </summary>
        public EffectParameter NormalMap;
        public Matrix World {
            get => _world.GetValueMatrix();
            set => _world.SetValue(value); }

        public Matrix View {
            get => _view.GetValueMatrix();
            set => _view.SetValue(value); }

        public Matrix Projection { 
            get => _projection.GetValueMatrix(); 
            set => _projection.SetValue(value); }

        public BasicNormalEffect() : base(Filters.Scene["NormalEffect"].GetShader().Shader)//this is the shader to wrap around
        {
            _world = Parameters["World"];
            _view = Parameters["View"];
            _projection = Parameters["Projection"];

            AmbientColor = Parameters["AmbientColor"];
            AmbientIntensity = Parameters["AmbientIntensity"];

            //WorldInverseTranspose = Parameters["WorldInverseTranspose"];

            LightDirection = Parameters["LightDirection"];
            DiffuseColor = Parameters["DiffuseColor"];
            DiffuseIntensity = Parameters["DiffuseIntensity"];

            //Shininess = Parameters["Shininess"];
            SpecularColor = Parameters["SpecularColor"];
            //SpecularIntensity = Parameters["SpecularIntensity"];
            EyePosition = Parameters["EyePosition"];

            TextureMap = Parameters["ColorMap"];

            //BumpConstant = Parameters["BumpConstant"];
            NormalMap = Parameters["NormalMap"];
        }
    }
}
