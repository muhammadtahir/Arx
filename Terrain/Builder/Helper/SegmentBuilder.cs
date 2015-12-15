﻿using MathHelper.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Extensions;
using MathHelper.Extensions;

namespace Terrain.Builder.Helper
{
    public class SegmentBuilder
    {
        private readonly Vector2[] SegmentStartUvs =
            new[]{
                new Vector2(),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };

        private readonly Vector2[] SegmentStartMirroredUvs =
            new[]{
                new Vector2(1, 0),
                new Vector2(),
                new Vector2(1, 1),
                new Vector2(0, 1)
            };

        private Vector2 PreviousTopRightCorner
        {
            get
            {
                return _dataContext.Vertices[_dataContext.Vertices.Count() - 1];
            }
            set
            {
                _dataContext.Vertices[_dataContext.Vertices.Count() - 1] = value;
            }
        }

        private BuilderDataContext _dataContext;
        private int _currentSegmentIndex;
        private List<LineSegment2D> _processedSegments;

        private float _segmentHeight;
        private float _cornerWidth;
        private Color _segmentColor;
        private Color _cornerColor;

        protected SegmentBuilder(
            BuilderDataContext dataContext,
            float segmentHeight,
            float cornerWidth,
            Color segmentColor = new Color(),
            Color cornerColor = new Color())
        {
            _dataContext = dataContext;
            _processedSegments = new List<LineSegment2D>();
            _segmentHeight = segmentHeight;
            _cornerWidth = cornerWidth;
            _segmentColor = segmentColor;
            _cornerColor = cornerColor;
        }

        public void AddSegmentStart(LineSegment2D segment)
        {
            _currentSegmentIndex = 0;
            AddSegmentCornerStart(segment.P1, segment.GetOrientationInRadians());
            AddFirstSegmentStart(segment);
        }

        public void AddNextSegment(LineSegment2D segment)
        {
            _currentSegmentIndex++;
            ArrangePreviousTopRightCorner(segment.GetOrientationInRadians());
            AddNextSegmentData(segment);
        }

        public void AddSegmentCornerEnd(Vector2 endPoint, float rotationInRadians)
        {
            var halfHeight = _segmentHeight / 2;
            var vectors =
                new[] {
                    endPoint + new Vector2(0, -halfHeight),
                    endPoint + new Vector2(_cornerWidth, -halfHeight),
                    endPoint + new Vector2(0, halfHeight),
                    endPoint + new Vector2(_cornerWidth, halfHeight)
                };

            AddSegmentDataStart(true, GetRotatedVectors(endPoint, rotationInRadians, vectors));
        }

        private void AddSegmentCornerStart(Vector2 origin, float rotationInRadians)
        {
            var halfHeight = _segmentHeight / 2;
            var vectors =
                new[] {
                    origin + new Vector2(-_cornerWidth, -halfHeight),
                    origin + new Vector2(0, -halfHeight),
                    origin + new Vector2(-_cornerWidth, halfHeight),
                    origin + new Vector2(0, halfHeight)
                };

            AddSegmentDataStart(false, GetRotatedVectors(origin, rotationInRadians, vectors));
        }

        private void AddSegmentDataStart(bool mirrowedUvs, params Vector2[] vertices)
        {
            _dataContext.Vertices.AddRange(vertices);

            _dataContext.Indices.AddRange(GetSegmentDataStartIndices());

            if (mirrowedUvs)
            {
                _dataContext.Uvs.AddRange(SegmentStartMirroredUvs);
            }
            else
            {
                _dataContext.Uvs.AddRange(SegmentStartUvs);
            }

            _dataContext.Colors.AddRange(
                new[]{
                    _cornerColor,
                    _cornerColor,
                    _cornerColor,
                    _cornerColor
                });
        }

        private Vector2[] GetRotatedVectors(Vector2 originPoint, float rotationInRadians, params Vector2[] vectors)
        {
            return vectors.Select(v => v.RotateAround(originPoint, rotationInRadians)).ToArray();
        }

        private void AddFirstSegmentStart(LineSegment2D segment)
        {
            var halfHeight = new Vector2(0, _segmentHeight / 2);
            var radians = segment.GetOrientationInRadians();
            var bottomLeft = (segment.P1 - halfHeight).RotateAround(segment.P1, radians);
            var bottomRight = (segment.P2 - halfHeight).RotateAround(segment.P2, radians);
            var topLeft = (segment.P1 + halfHeight).RotateAround(segment.P1, radians);
            var topRight = (segment.P2 + halfHeight).RotateAround(segment.P2, radians);

            var v = new[] { bottomLeft, bottomRight, topLeft, topRight };

            AddSegmentDataStart(false, v);
            _processedSegments.Add(segment);
        }

        private void ArrangePreviousTopRightCorner(float rotationInRadians)
        {
            var lastProcessedSegment = _processedSegments.Last();
            var lastSegmentOrientation = lastProcessedSegment.GetOrientationInRadians();
            var radians = (rotationInRadians + lastSegmentOrientation) / 2;
            var bottomRight = lastProcessedSegment.P2;

            if ((Mathf.Abs(rotationInRadians - lastSegmentOrientation)).NormalizeRadians() > Mathf.PI)
            {
                radians -= Mathf.PI;
            }

            PreviousTopRightCorner = (bottomRight + new Vector2(0, _segmentHeight / 2)).RotateAround(bottomRight, radians);
        }

        private void AddNextSegmentData(LineSegment2D segment)
        {
            _processedSegments.Add(segment);

            var radians = segment.GetOrientationInRadians();

            _dataContext.Vertices.AddRange(
                new[]
                {
                    (segment.P2 - new Vector2(0, _segmentHeight/2)).RotateAround(segment.P2, radians),
                    (segment.P2 + new Vector2(0, _segmentHeight/2)).RotateAround(segment.P2, radians)
                });

            _dataContext.Indices.AddRange(GetNextSegmentData());

            _dataContext.Uvs.AddRange(
                new[]{
                    new Vector2(_currentSegmentIndex + 1.0f, 0),
                    new Vector2(_currentSegmentIndex + 1.0f, 1),
                });

            _dataContext.Colors.AddRange(
                new[]{
                    _segmentColor,
                    _segmentColor
                });
        }

        private int[] GetSegmentDataStartIndices()
        {
            var currentIndice = _dataContext.CurrentIndice;
            return new[] {
                    currentIndice + 1, currentIndice + 4, currentIndice + 2,
                    currentIndice + 1, currentIndice + 3, currentIndice + 4
                };
        }

        private int[] GetNextSegmentData()
        {
            var currentIndice = _dataContext.CurrentIndice;
            if (_currentSegmentIndex == 1)
            {
                return
                    new[] {
                        currentIndice - 2 , currentIndice + 2, currentIndice + 1,
                        currentIndice - 2, currentIndice, currentIndice + 2
                    };
            }
            else
            {
                return
                    new[] {
                        currentIndice - 1 , currentIndice + 2, currentIndice + 1,
                        currentIndice - 1, currentIndice, currentIndice + 2
                    };
            }
        }
    }
}