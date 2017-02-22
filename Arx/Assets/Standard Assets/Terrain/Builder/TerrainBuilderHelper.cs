﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;
using MathHelper.DataStructures;
using MathHelper.Extensions;
using Utils;
using GenericComponents.Builders;
using Assets.Standard_Assets.Terrain.Builder.Helper.Interfaces;
using Assets.Standard_Assets.Terrain.Builder.Helper.SegmentBuilders;

namespace Assets.Standard_Assets.Terrain.Builder
{
    public class TerrainBuilderHelper :
        ITerrainBuilderHelper,
        IFloorSegmentBuilder,
        ISlopeSegmentBuilder,
        ICeilingSegmentBuilder
    {
        private float _floorHeight;
        private float _slopeHeight;
        private float _ceilingHeight;
        private float _cornerWidth;

        protected BuilderDataContext DataContext { get; private set; }
        protected FloorSegmentBuilder FloorBuilder { get; private set; }
        protected SlopeSegmentBuilder SlopeBuilder { get; private set; }
        protected CeilingSegmentBuilder CeilingBuilder { get; private set; }
        protected FillingBuilder FillingBuilder { get; private set; }

        public Vector3[] Vertices
        {
            get { return DataContext.Vertices.Select(v => v.ToVector3()).ToArray(); }
        }

        public int[] Indices
        {
            get { return DataContext.Indices.ToArray(); }
        }

        public Vector2[] Uvs
        {
            get { return DataContext.Uvs.ToArray(); }
        }

        public Color[] Colors
        {
            get { return DataContext.Colors.ToArray(); }
        }

        protected TerrainBuilderHelper(
            float floorHeight,
            float slopeHeight,
            float ceilingHeight,
            float cornerWidth,
            float fillingLowPoint,
            float fillingUFactor,
            float fillingVFactor)
        {
            _floorHeight = floorHeight;
            _slopeHeight = slopeHeight;
            _ceilingHeight = ceilingHeight;
            _cornerWidth = cornerWidth;
            DataContext = new BuilderDataContext();
            FloorBuilder = new FloorSegmentBuilder(DataContext, _floorHeight, _cornerWidth);
            SlopeBuilder = new SlopeSegmentBuilder(DataContext, _slopeHeight, _cornerWidth);
            CeilingBuilder = new CeilingSegmentBuilder(DataContext, _ceilingHeight, _cornerWidth);
            FillingBuilder = new FillingBuilder(DataContext, fillingLowPoint, fillingUFactor, fillingVFactor);
        }

        #region ITerrainBuilderHelper

        public virtual IFloorSegmentBuilder AddFloorSegmentStart(LineSegment2D segment)
        {
            FloorBuilder.AddSegmentStartingCorner(segment.P1, segment.GetOrientationInRadians());
            FloorBuilder.AddFirstSegment(segment);
            return this;
        }

        public virtual ISlopeSegmentBuilder AddSlopeSegmentStart(LineSegment2D segment)
        {
            SlopeBuilder.AddSegmentStartingCorner(segment.P1, segment.GetOrientationInRadians());
            SlopeBuilder.AddFirstSegment(segment);
            return this;
        }

        public virtual ICeilingSegmentBuilder AddCeilingSegmentStart(LineSegment2D segment)
        {
            CeilingBuilder.AddSegmentStartingCorner(segment.P1, segment.GetOrientationInRadians());
            CeilingBuilder.AddFirstSegment(segment);
            return this;
        }

        public virtual ITerrainBuilderHelper AddFilling(IEnumerable<LineSegment2D> segments)
        {
            FillingBuilder.AddOpenFilling(segments);
            return this;
        }

        #endregion

        #region IFloorSegmentBuilder

        public virtual IFloorSegmentBuilder AddFloorSegment(LineSegment2D segment)
        {
            FloorBuilder.AddNextSegment(segment);
            return this;
        }

        public virtual ITerrainBuilderHelper AddFloorSegmentEnd(Vector2 endPoint, float rotationInRadians)
        {
            FloorBuilder.AddSegmentEndingCorner(endPoint, rotationInRadians);
            return this;
        }

        #endregion

        #region ISlopeSegmentBuilder

        public virtual ISlopeSegmentBuilder AddSlopeSegment(LineSegment2D segment)
        {
            SlopeBuilder.AddNextSegment(segment);
            return this;
        }

        public virtual ITerrainBuilderHelper AddSlopeSegmentEnd(Vector2 endPoint, float rotationInRadians)
        {
            SlopeBuilder.AddSegmentEndingCorner(endPoint, rotationInRadians);
            return this;
        }

        #endregion

        #region ICeilingSegmentBuilder

        public virtual ICeilingSegmentBuilder AddCeilingSegment(LineSegment2D segment)
        {
            CeilingBuilder.AddNextSegment(segment);
            return this;
        }

        public virtual ITerrainBuilderHelper AddCeilingSegmentEnd(Vector2 endPoint, float rotationInRadians)
        {
            CeilingBuilder.AddSegmentEndingCorner(endPoint, rotationInRadians);
            return this;
        }

        #endregion
    }
}
