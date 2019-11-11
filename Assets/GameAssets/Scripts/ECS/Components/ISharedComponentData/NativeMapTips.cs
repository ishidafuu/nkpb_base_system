using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using System.Collections.Generic;
using System;

namespace NKPB
{

    public class NativeMapTips
    {
        public int m_mapSizeX;
        public int m_mapSizeY;
        public int m_mapSizeZ;
        public NativeArray<EnumShapeType> shapes;
        public NativeArray<int> events;

        public NativeMapTips(MapTips mapTips)
        {
            m_mapSizeX = mapTips.mapSizeX;
            m_mapSizeY = mapTips.mapSizeY;
            m_mapSizeZ = mapTips.mapSizeZ;

            shapes = new NativeArray<EnumShapeType>(mapTips.shapes, Allocator.Persistent);
            events = new NativeArray<int>(mapTips.events, Allocator.Persistent);
        }

        public void Dispose()
        {
            shapes.Dispose();
            events.Dispose();
        }
    }
}