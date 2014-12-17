using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dargon.FileSystem;

namespace Dargon.League.Maps {
   public class LeagueMapLoader : ILeagueMapLoader {
      private static readonly byte[] fileMagic = {0x4E, 0x56, 0x52, 0x00};

      public LeagueMap Load(IFileSystem system, IFileSystemHandle mapFolderHandle) {
         // Find the .nvr file
         IFileSystemHandle nvrFileHandle;
         if (system.AllocateRelativeHandleFromPath(mapFolderHandle, @"Scene/room.nvr", out nvrFileHandle) != IoResult.Success) {
            return null;
         }

         // Read the file and put it into a stream for easy processing
         byte[] nvrFileRawData;
         if (system.ReadAllBytes(nvrFileHandle, out nvrFileRawData) != IoResult.Success) {
            return null;
         }

         using (var ms = new MemoryStream(nvrFileRawData)) {
            using (var reader = new BinaryReader(ms)) {
               var leagueMap = new LeagueMap();

               // Check magic
               var magic = reader.ReadBytes(4);
               if (!magic.SequenceEqual(fileMagic)) {
                  return null;
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
               leagueMap.materials.Capacity = materialCount;
               for (var i = 0; i < materialCount; ++i) {
                  leagueMap.materials.Add(reader.ReadMaterial());
               }

               // Read vertex buffers
               leagueMap.vertexBuffers.Capacity = vertexBufferCount;
               for (var i = 0; i < vertexBufferCount; ++i) {
                  var dataSize = reader.ReadInt32();
                  leagueMap.vertexBuffers.Add(reader.ReadBytes(dataSize));
               }

               // Read index buffers
               leagueMap.indexBuffers.Capacity = indexBufferCount;
               for (var i = 0; i < indexBufferCount; ++i) {
                  var dataSize = reader.ReadInt32();
                  var d3dType = reader.ReadUInt32();

                  var indexCount = dataSize / 2;
                  leagueMap.indexBuffers[i] = new List<ushort> {
                     Capacity = indexCount
                  };

                  for (var j = 0; j < indexCount; ++j) {
                     leagueMap.indexBuffers[i].Add(reader.ReadUInt16());
                  }
               }

               // Read meshes
               leagueMap.meshes.Capacity = meshCount;
               for (var i = 0; i < meshCount; ++i) {
                  leagueMap.meshes.Add(reader.ReadMesh());
               }

               // Read AABB data
               leagueMap.AABBs.Capacity = aabbCount;

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
   }
}
