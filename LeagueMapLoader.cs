// This code is a port / rewrite in C# of https://github.com/eekysam/LeagueLevel
// Credit where credit is due

using System.IO;
using System.Linq;
using Dargon.FileSystem;

namespace Dargon.League.Maps {
   class LeagueMapLoader : ILeagueMapLoader {
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

               for (uint i = 0; i < materialCount; ++i) {
                  var material = new Material();

                  // The size of the name field is always 256 bytes. It is padded with nulls
                  reader.Read(material.name, 0, 256);
                  material.unknown1 = reader.ReadUInt32();
                  material.unknown2 = reader.ReadUInt32();
                  material.unknown3 = reader.ReadUInt32();

                  for (var j = 0; j < 8; ++j) {
                     var texture = new Texture {
                        color = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
                     };

                     texture.name = reader.ReadChars(256);
                     texture.additional = reader.ReadBytes(68);

                     material.textures.Add(texture);
                  }

                  leagueMap.materials.Add(material);
               }

               // Read vertex buffers
               leagueMap.vertexBuffers.Capacity = vertexBufferCount;

               for (var i = 0; i < vertexBufferCount; ++i) {
                  var dataSize = reader.ReadInt32();
                  leagueMap.vertexBuffers[i] = reader.ReadBytes(dataSize);
               }

               // Read index buffers
               leagueMap.indexBuffers.Capacity = indexBufferCount;

               for (var i = 0; i < indexBufferCount; ++i) {
                  var dataSize = reader.ReadInt32();
                  var d3dType = reader.ReadUInt32();

                  var indexCount = dataSize / 2;
                  leagueMap.indexBuffers[i].Capacity = indexCount;

                  for (var j = 0; j < indexCount; ++j) {
                     leagueMap.indexBuffers[i][j] = reader.ReadUInt16();
                  }
               }

               // Read meshes
               leagueMap.meshes.Capacity = meshCount;

               for (var i = 0; i < meshCount; ++i) {
                  var flag0 = reader.ReadUInt32();
                  var zero = reader.ReadUInt32();

                  leagueMap.meshes[i].boundingSphere.x = reader.ReadSingle();
                  leagueMap.meshes[i].boundingSphere.y = reader.ReadSingle();
                  leagueMap.meshes[i].boundingSphere.z = reader.ReadSingle();
                  leagueMap.meshes[i].boundingSphere.w = reader.ReadSingle();

                  leagueMap.meshes[i].aabb.min.x = reader.ReadSingle();
                  leagueMap.meshes[i].aabb.min.y = reader.ReadSingle();
                  leagueMap.meshes[i].aabb.min.z = reader.ReadSingle();

                  leagueMap.meshes[i].aabb.max.x = reader.ReadSingle();
                  leagueMap.meshes[i].aabb.max.y = reader.ReadSingle();
                  leagueMap.meshes[i].aabb.max.z = reader.ReadSingle();

                  leagueMap.meshes[i].materialIndex = reader.ReadUInt32();

                  leagueMap.meshes[i].simpleMesh.vertexType = VertexType.SIMPLE;
                  leagueMap.meshes[i].simpleMesh.vertexBufferIndex = reader.ReadUInt32();
                  leagueMap.meshes[i].simpleMesh.vertexBufferOffset = reader.ReadUInt32();
                  leagueMap.meshes[i].simpleMesh.vertexCount = reader.ReadUInt32();
                  leagueMap.meshes[i].simpleMesh.indexBufferIndex = reader.ReadUInt32();
                  leagueMap.meshes[i].simpleMesh.indexBufferOffset = reader.ReadUInt32();
                  leagueMap.meshes[i].simpleMesh.indexCount = reader.ReadUInt32();

                  leagueMap.meshes[i].complexMesh.vertexType = VertexType.COMPLEX;
                  leagueMap.meshes[i].complexMesh.vertexBufferIndex = reader.ReadUInt32();
                  leagueMap.meshes[i].complexMesh.vertexBufferOffset = reader.ReadUInt32();
                  leagueMap.meshes[i].complexMesh.vertexCount = reader.ReadUInt32();
                  leagueMap.meshes[i].complexMesh.indexBufferIndex = reader.ReadUInt32();
                  leagueMap.meshes[i].complexMesh.indexBufferOffset = reader.ReadUInt32();
                  leagueMap.meshes[i].complexMesh.indexCount = reader.ReadUInt32();
               }

               // Read AABB data
               leagueMap.AABBs.Capacity = aabbCount;

               for (var i = 0; i < aabbCount; ++i) {
                  leagueMap.AABBs[i].min.x = reader.ReadSingle();
                  leagueMap.AABBs[i].min.y = reader.ReadSingle();
                  leagueMap.AABBs[i].min.z = reader.ReadSingle();

                  leagueMap.AABBs[i].max.x = reader.ReadSingle();
                  leagueMap.AABBs[i].max.y = reader.ReadSingle();
                  leagueMap.AABBs[i].max.z = reader.ReadSingle();

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
