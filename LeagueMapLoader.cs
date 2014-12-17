using System;
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
               leagueMap.materials.Capacity = materialCount;
               for (var i = 0; i < materialCount; ++i) {
                  leagueMap.materials.Add(reader.ReadMaterial());
               }

               // Read vertex buffers
               leagueMap.vertexBuffers.Capacity = vertexBufferCount;
               for (var i = 0; i < vertexBufferCount; ++i) {
                  leagueMap.vertexBuffers.Add(reader.ReadVertexBuffer());
               }

               // Read index buffers
               leagueMap.indexBuffers.Capacity = indexBufferCount;
               for (var i = 0; i < indexBufferCount; ++i) {
                  leagueMap.indexBuffers.Add(reader.ReadIndexBuffer());
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
