#region File Description
//-----------------------------------------------------------------------------
// BloomComponent.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
#endregion

namespace STACK
{
	public class BloomComponent
	{
		#region Fields

		public bool Visible = false;
		private readonly GraphicsDevice _graphicsDevice;
		private SpriteBatch _spriteBatch;
		private Effect _bloomExtractEffect;
		private Effect _bloomCombineEffect;
		private Effect _gaussianBlurEffect;
		private RenderTarget2D _sceneRenderTarget, _finalRenderTarget;
		private RenderTarget2D _renderTarget1;
		private RenderTarget2D _renderTarget2;


		// Choose what display settings the bloom should use.
		////public BloomSettings Settings
		////{
		////    get { return settings; }
		////    set { settings = value; }
		////}

		////BloomSettings settings = BloomSettings.PresetSettings[0];


		// Optionally displays one of the intermediate buffers used
		// by the bloom postprocess, so you can see exactly what is
		// being drawn into each rendertarget.
		public enum IntermediateBuffer
		{
			PreBloom,
			BlurredHorizontally,
			BlurredBothWays,
			FinalResult,
		}

		public IntermediateBuffer ShowBuffer
		{
			get => _showBuffer;
			set => _showBuffer = value;
		}

		private IntermediateBuffer _showBuffer = IntermediateBuffer.FinalResult;


		#endregion

		#region Initialization

		public ContentManager Content;
		public BloomComponent(GraphicsDevice device)
		{
			_graphicsDevice = device;

		}


		/// <summary>
		/// Load your graphics content.
		/// </summary>
		public void LoadContent(ContentManager content)
		{
			_spriteBatch = new SpriteBatch(_graphicsDevice);

			_bloomExtractEffect = content.Load<Effect>(STACK.content.shaders.BloomExtract);
			_bloomCombineEffect = content.Load<Effect>(STACK.content.shaders.BloomCombine);
			_gaussianBlurEffect = content.Load<Effect>(STACK.content.shaders.GaussianBlur);

			// Look up the resolution and format of our main backbuffer.
			var pp = _graphicsDevice.PresentationParameters;

			var width = pp.BackBufferWidth;
			var height = pp.BackBufferHeight;

			var format = pp.BackBufferFormat;

			// Create a texture for rendering the main scene, prior to applying bloom.
			_sceneRenderTarget = new RenderTarget2D(_graphicsDevice, width, height, false,
												   format, pp.DepthStencilFormat, pp.MultiSampleCount,
												   RenderTargetUsage.DiscardContents);

			_finalRenderTarget = new RenderTarget2D(_graphicsDevice, width, height, false,
												   format, pp.DepthStencilFormat, pp.MultiSampleCount,
												   RenderTargetUsage.DiscardContents);

			// Create two rendertargets for the bloom processing. These are half the
			// size of the backbuffer, in order to minimize fillrate costs. Reducing
			// the resolution in this way doesn't hurt quality, because we are going
			// to be blurring the bloom images in any case.
			//width /= 2;
			//height /= 2;

			_renderTarget1 = new RenderTarget2D(_graphicsDevice, width, height, false, format, DepthFormat.None);
			_renderTarget2 = new RenderTarget2D(_graphicsDevice, width, height, false, format, DepthFormat.None);
		}


		/// <summary>
		/// Unload your graphics content.
		/// </summary>
		public void UnloadContent()
		{
			_sceneRenderTarget.Dispose();
			_finalRenderTarget.Dispose();
			_renderTarget1.Dispose();
			_renderTarget2.Dispose();
		}


		#endregion

		#region Draw


		/// <summary>
		/// This should be called at the very start of the scene rendering. The bloom
		/// component uses it to redirect drawing into its custom rendertarget, so it
		/// can capture the scene image in preparation for applying the bloom filter.
		/// </summary>
		public void BeginDraw()
		{
			if (Visible)
			{
				_graphicsDevice.SetRenderTarget(_sceneRenderTarget);
			}
		}


		/// <summary>
		/// This is where it all happens. Grabs a scene that has already been rendered,
		/// and uses postprocess magic to add a glowing bloom effect over the top of it.
		/// </summary>
		public void Draw(BloomSettings settings)
		{
			if (!Visible)
			{
				return;
			}

			_graphicsDevice.SamplerStates[1] = SamplerState.AnisotropicClamp;

			// Pass 1: draw the scene into rendertarget 1, using a
			// shader that extracts only the brightest parts of the image.
			_bloomExtractEffect.Parameters["BloomThreshold"].SetValue(
				settings.BloomThreshold);

			DrawFullscreenQuad(_sceneRenderTarget, _renderTarget1,
							   _bloomExtractEffect,
							   IntermediateBuffer.PreBloom);

			// Pass 2: draw from rendertarget 1 into rendertarget 2,
			// using a shader to apply a horizontal gaussian blur filter.
			_gaussianBlurEffect.CurrentTechnique = _gaussianBlurEffect.Techniques["GaussianBlur"];

			SetBlurEffectParameters(1.0f / (float)_renderTarget1.Width, 0, settings);

			DrawFullscreenQuad(_renderTarget1, _renderTarget2,
							   _gaussianBlurEffect,
							   IntermediateBuffer.BlurredHorizontally);

			// Pass 3: draw from rendertarget 2 back into rendertarget 1,
			// using a shader to apply a vertical gaussian blur filter.
			SetBlurEffectParameters(0, 1.0f / (float)_renderTarget1.Height, settings);

			DrawFullscreenQuad(_renderTarget2, _renderTarget1,
							   _gaussianBlurEffect,
							   IntermediateBuffer.BlurredBothWays);

			// Pass 4: draw both rendertarget 1 and the original scene
			// image back into the main backbuffer, using a shader that
			// combines them to produce the final bloomed result.
			_graphicsDevice.SetRenderTarget(_finalRenderTarget);

			var parameters = _bloomCombineEffect.Parameters;

			parameters["BloomIntensity"].SetValue(settings.BloomIntensity);
			parameters["BaseIntensity"].SetValue(settings.BaseIntensity);
			parameters["BloomSaturation"].SetValue(settings.BloomSaturation);
			parameters["BaseSaturation"].SetValue(settings.BaseSaturation);

			// FNA
			_graphicsDevice.Textures[1] = _sceneRenderTarget;
			//parameters["BaseSampler"].SetValue(sceneRenderTarget);

			var viewport = _graphicsDevice.Viewport;

			DrawFullscreenQuad(_renderTarget1,
							   viewport.Width, viewport.Height,
							   _bloomCombineEffect,
							   IntermediateBuffer.FinalResult);

			_graphicsDevice.SetRenderTarget(null);
		}

		public void Flush()
		{
			if (!Visible)
			{
				return;
			}

			_spriteBatch.Begin(0, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone);
			_spriteBatch.Draw(_finalRenderTarget, Vector2.Zero, Color.White);
			_spriteBatch.End();
		}


		/// <summary>
		/// Helper for drawing a texture into a rendertarget, using
		/// a custom shader to apply postprocessing effects.
		/// </summary>
		private void DrawFullscreenQuad(Texture2D texture, RenderTarget2D renderTarget,
								Effect effect, IntermediateBuffer currentBuffer)
		{
			_graphicsDevice.SetRenderTarget(renderTarget);

			DrawFullscreenQuad(texture,
							   renderTarget.Width, renderTarget.Height,
							   effect, currentBuffer);
		}


		/// <summary>
		/// Helper for drawing a texture into the current rendertarget,
		/// using a custom shader to apply postprocessing effects.
		/// </summary>
		private void DrawFullscreenQuad(Texture2D texture, int width, int height,
								Effect effect, IntermediateBuffer currentBuffer)
		{
			// If the user has selected one of the show intermediate buffer options,
			// we still draw the quad to make sure the image will end up on the screen,
			// but might need to skip applying the custom pixel shader.
			if (_showBuffer < currentBuffer)
			{
				effect = null;
			}

			_spriteBatch.Begin(0, BlendState.Opaque, SamplerState.AnisotropicClamp, null, null, effect);
			_spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
			_spriteBatch.End();
		}

		private struct WeightingsCache
		{
			public float[] SampleWeights;
			public Vector2[] SampleOffsets;
			public float BlurAmount;
		}

		private WeightingsCache _weightingsCacheX;
		private WeightingsCache _weightingsCacheY;

		/// <summary>
		/// Computes sample weightings and texture coordinate offsets
		/// for one pass of a separable gaussian blur filter.
		/// </summary>
		private void SetBlurEffectParameters(float dx, float dy, BloomSettings settings)
		{
			// Look up the sample weight and offset effect parameters.
			EffectParameter weightsParameter, offsetsParameter;

			weightsParameter = _gaussianBlurEffect.Parameters["SampleWeights"];
			offsetsParameter = _gaussianBlurEffect.Parameters["SampleOffsets"];

			float[] sampleWeights;
			Vector2[] sampleOffsets;

			if ((dy == 0 && (_weightingsCacheX.Equals(default(WeightingsCache)) || settings.BlurAmount != _weightingsCacheX.BlurAmount)) ||
				(dx == 0 && (_weightingsCacheY.Equals(default(WeightingsCache)) || settings.BlurAmount != _weightingsCacheY.BlurAmount)))
			{
				// Look up how many samples our gaussian blur effect supports.
				var sampleCount = weightsParameter.Elements.Count;

				// Create temporary arrays for computing our filter settings.
				sampleWeights = new float[sampleCount];
				sampleOffsets = new Vector2[sampleCount];

				// The first sample always has a zero offset.
				sampleWeights[0] = ComputeGaussian(0, settings);
				sampleOffsets[0] = new Vector2(0);

				// Maintain a sum of all the weighting values.
				var totalWeights = sampleWeights[0];

				// Add pairs of additional sample taps, positioned
				// along a line in both directions from the center.
				for (var i = 0; i < sampleCount / 2; i++)
				{
					// Store weights for the positive and negative taps.
					var weight = ComputeGaussian(i + 1, settings);

					sampleWeights[i * 2 + 1] = weight;
					sampleWeights[i * 2 + 2] = weight;

					totalWeights += weight * 2;

					// To get the maximum amount of blurring from a limited number of
					// pixel shader samples, we take advantage of the bilinear filtering
					// hardware inside the texture fetch unit. If we position our texture
					// coordinates exactly halfway between two texels, the filtering unit
					// will average them for us, giving two samples for the price of one.
					// This allows us to step in units of two texels per sample, rather
					// than just one at a time. The 1.5 offset kicks things off by
					// positioning us nicely in between two texels.
					var sampleOffset = i * 2 + 1.5f;

					var delta = new Vector2(dx, dy) * sampleOffset;

					// Store texture coordinate offsets for the positive and negative taps.
					sampleOffsets[i * 2 + 1] = delta;
					sampleOffsets[i * 2 + 2] = -delta;
				}

				// Normalize the list of sample weightings, so they will always sum to one.
				for (var i = 0; i < sampleWeights.Length; i++)
				{
					sampleWeights[i] /= totalWeights;
				}

				if (dx == 0)
				{
					_weightingsCacheY = new WeightingsCache()
					{
						SampleOffsets = sampleOffsets,
						SampleWeights = sampleWeights,
						BlurAmount = settings.BlurAmount
					};
				}
				else
				{
					_weightingsCacheX = new WeightingsCache()
					{
						SampleOffsets = sampleOffsets,
						SampleWeights = sampleWeights,
						BlurAmount = settings.BlurAmount
					};
				}
			}
			else
			{
				if (dx == 0)
				{
					sampleWeights = _weightingsCacheY.SampleWeights;
					sampleOffsets = _weightingsCacheY.SampleOffsets;
				}
				else
				{
					sampleWeights = _weightingsCacheX.SampleWeights;
					sampleOffsets = _weightingsCacheX.SampleOffsets;
				}
			}

			// Tell the effect about our new filter settings.
			weightsParameter.SetValue(sampleWeights);
			offsetsParameter.SetValue(sampleOffsets);
		}


		/// <summary>
		/// Evaluates a single point on the gaussian falloff curve.
		/// Used for setting up the blur filter weightings.
		/// </summary>
		private float ComputeGaussian(float n, BloomSettings settings)
		{
			var theta = settings.BlurAmount;

			return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
						   Math.Exp(-(n * n) / (2 * theta * theta)));
		}


		#endregion
	}
}