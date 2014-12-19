using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dargon.FileSystem;
using ItzWarty;
using ItzWarty.Collections;

namespace Dargon.League.Maps {
   public class LeagueMapLoader : ILeagueMapLoader {
      private static readonly byte[] fileMagic = {0x4E, 0x56, 0x52, 0x00};

      private IReadOnlyDictionary<VertexType, Func<BinaryReader, Vertex> > kVertexReadersByVertexType = ImmutableDictionary.Of<VertexType, Func<BinaryReader, Vertex> >(
         VertexType.SIMPLE, reader => reader.ReadSimpleVertex(),
         VertexType.COMPLEX, reader => reader.ReadComplexVertex()
      );

      public LeagueMap LoadFromNVR(IFileSystem system, IFileSystemHandle mapFolderHandle) {
         // Find the .nvr file
         IFileSystemHandle nvrFileHandle;
         if (system.AllocateRelativeHandleFromPath(mapFolderHandle, @"Scene/room.nvr", out nvrFileHandle) != IoResult.Success) {
            throw new FileNotFoundException();
         }

         // Read the file and put it into a stream for easy processing
         byte[] nvrFileRawData;
         if (system.ReadAllBytes(nvrFileHandle, out nvrFileRawData) != IoResult.Success) {
            throw new FileLoadException();
         }

         using (var ms = new MemoryStream(nvrFileRawData)) {
            using (var reader = new BinaryReader(ms)) {
               var leagueMap = new LeagueMap();

               // Check magic
               var magic = reader.ReadBytes(4);
               if (!magic.SequenceEqual(fileMagic)) {
                  throw new Exception("NVR file magic number is not correct");
               }

               // Read meta data
               leagueMap.majorVersion = reader.ReadUInt16();
               leagueMap.minorVersion = reader.ReadUInt16();
               var materialCount = reader.ReadInt32();
               var vertexBufferCount = reader.ReadInt32();
               var indexBufferCount = reader.ReadInt32();
               var meshCount = reader.ReadInt32();
               var aabbCount = reader.ReadInt32();

               // Read materials
               leagueMap.materials = Util.Generate(materialCount, i => reader.ReadMaterial());
            
               // Read vertex buffers raw data
               var vertexBuffers = Util.Generate(vertexBufferCount, i => reader.ReadVertexBufferRawData());
               
               // Read index buffers
               leagueMap.indexBuffers = Util.Generate(indexBufferCount, i => reader.ReadIndexBuffer());

               // Read meshes
               leagueMap.meshes = Util.Generate(meshCount, i => reader.ReadMesh());

               // Classify the vertex buffers
               var vertexBufferTypes = new VertexType[vertexBufferCount];
               foreach (var mesh in leagueMap.meshes) {
                  vertexBufferTypes[mesh.simpleMesh.vertexBufferIndex] = VertexType.SIMPLE;
                  vertexBufferTypes[mesh.complexMesh.vertexBufferIndex] = VertexType.COMPLEX;
               }

               // Parse the vertexBuffers
               leagueMap.vertexBuffers = Util.Generate(vertexBufferCount, i => ReadVertexBuffer(vertexBuffers[i], vertexBufferTypes[i]));

               // Read AABB data
               leagueMap.AABBs = new AABB[aabbCount];
               for (var i = 0; i < aabbCount; ++i) {
                  leagueMap.AABBs[i] = reader.ReadAABB();

                  var unknown1 = reader.ReadSingle();
                  var unknown2 = reader.ReadSingle();
                  var unknown3 = reader.ReadSingle();
               }

               return leagueMap;
            }
         }
      }

      private List<Vertex> ReadVertexBuffer(byte[] rawVertexBufferData, VertexType type) {
         using (var ms = new MemoryStream(rawVertexBufferData)) {
            using (var reader = new BinaryReader(ms)) {
               Func<Vertex> vertexReader;
               if (type == VertexType.COMPLEX) {
                  vertexReader = () => reader.ReadComplexVertex();
               } else {
                  vertexReader = () => reader.ReadSimpleVertex();
               }

               var vertexBuffer = new List<Vertex>();
               while (reader.BaseStream.Position < reader.BaseStream.Length) {
                  vertexBuffer.Add(vertexReader());
               }

               return vertexBuffer;
            }
         }
      }
   }
}
