﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace LabelsOnFloor
{
    public class LabelPlacementHandler
    {
        private readonly FontHandler _fontHandler;

        private readonly LabelHolder _labelHolder;

        private readonly LabelMaker _labelMaker = new LabelMaker();

        private readonly Dictionary<string, Mesh> _cachedMeshes = new Dictionary<string, Mesh>();

        private int _nextUpdateTick;

        private Map _map;

        public LabelPlacementHandler(LabelHolder labelHolder, FontHandler fontHandler)
        {
            _labelHolder = labelHolder;
            _fontHandler = fontHandler;
        }

        public bool IsReady()
        {
            return _fontHandler.IsFontLoaded();
        }

        public void RegenerateIfNeeded()
        {
            var tick = Find.TickManager.TicksGame;
            if (_map == Find.VisibleMap && tick < _nextUpdateTick)
                return;

            _nextUpdateTick = tick + 200;
            Regenerate();
        }

        private void Regenerate()
        {
            _map = Find.VisibleMap;
            _labelHolder.Clear();

            RegenerateRoomLabels();
            RegenerateZoneLabels();
        }

        private void RegenerateRoomLabels()
        {
            var foundRooms = new HashSet<Room>();
            var listerBuildings = _map.listerBuildings;
            // Room roles are defined by buildings, so only need to check rooms with buildings
            foreach (var building in listerBuildings.allBuildingsColonist)
            {
                var room = GetRoomContainingBuildingIfRelevant(building);
                if (room == null)
                    continue;

                if (foundRooms.Contains(room))
                    continue;

                foundRooms.Add(room);
                var text = _labelMaker.GetRoomLabel(room);
                var label = new Label()
                {
                    LabelMesh = GetMeshFor(text),
                    LabelPlacementData = GetLabelPlacementDataForRoom(room, text.Length)
                };

                if (label.LabelPlacementData != null)
                    _labelHolder.Add(label);
            }
        }

        // Filter for indoor rooms with a role
        private Room GetRoomContainingBuildingIfRelevant(Building building)
        {
            if (building.Faction != Faction.OfPlayer)
                return null;

            if (building.Position.Fogged(_map))
                return null;

            var room = building.Position.GetRoom(_map);
            if (room == null || room.PsychologicallyOutdoors)
                return null;

            if (room.Role == RoomRoleDefOf.None || room.Role.defName == "Room")
                return null;

            return room;
        }

        private PlacementData GetLabelPlacementDataForRoom(Room room, int labelLength)
        {
            return EdgeFinder.GetBestPlacementData(
                room.Cells.ToList(),
                c => false,
                c => !_map.thingGrid.CellContains(c, ThingDefOf.Wall),
                c => true,
                labelLength
                );
        }

        private void RegenerateZoneLabels()
        {
            foreach (var zone in _map.zoneManager.AllZones)
            {
                var text = _labelMaker.GetZoneLabel(zone);
                Main.Instance.Logger.Message($"Got zone: {text}");
                if (string.IsNullOrEmpty(text))
                    continue;

                var label = new Label()
                {
                    LabelMesh = GetMeshFor(text),
                    LabelPlacementData = GetLabelPlacementDataForZone(zone, text.Length)
                };

                if (label.LabelPlacementData != null)
                    _labelHolder.Add(label);
            }
        }

        private PlacementData GetLabelPlacementDataForZone(Zone zone, int labelLength)
        {
            return EdgeFinder.GetBestPlacementData(
                zone.Cells,
                c => c.Fogged(_map),
                c => true,
                c => true,
                labelLength
            );
        }


        private Mesh GetMeshFor(string label)
        {
            if (!_cachedMeshes.ContainsKey(label))
            {
                _cachedMeshes[label] = CreateMeshFor(label);
            }

            return _cachedMeshes[label];
        }

        private Mesh CreateMeshFor(string label)
        {
            var vertices = new List<Vector3>();
            var uvMap = new List<Vector2>();
            var triangles = new List<int>();
            var size = new Vector2
            {
                x = 1f,
                y = 2f
            };

            Main.Instance.Logger.Message($"Mesh for: {label}");


            var boundsInTexture = _fontHandler.GetBoundsInTextureFor(label);
            var startingTriangleVertex = 0;
            var startingVertexXOffset = 0f;
            var yTop = size.y - 0.4f;
            foreach (var charBoundsInTexture in boundsInTexture)
            {
                vertices.Add(new Vector3(startingVertexXOffset, 0f, -0.4f));
                vertices.Add(new Vector3(startingVertexXOffset, 0f, yTop));
                vertices.Add(new Vector3(startingVertexXOffset + size.x, 0f, yTop));
                vertices.Add(new Vector3(startingVertexXOffset + size.x, 0f, -0.4f));
                startingVertexXOffset += size.x;

                uvMap.Add(new Vector2(charBoundsInTexture.Left, 0f));
                uvMap.Add(new Vector2(charBoundsInTexture.Left, 1f));
                uvMap.Add(new Vector2(charBoundsInTexture.Right, 1f));
                uvMap.Add(new Vector2(charBoundsInTexture.Right, 0f));

                triangles.Add(startingTriangleVertex + 0);
                triangles.Add(startingTriangleVertex + 1);
                triangles.Add(startingTriangleVertex + 2);
                triangles.Add(startingTriangleVertex + 0);
                triangles.Add(startingTriangleVertex + 2);
                triangles.Add(startingTriangleVertex + 3);
                startingTriangleVertex += 4;
            }

            var mesh = new Mesh
            {
                name = "NewPlaneMesh()",
                vertices = vertices.ToArray(),
                uv = uvMap.ToArray()
            };
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

    }
}