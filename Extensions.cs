using System.Collections.Generic;
using System.IO;

namespace Dargon.League.Maps {
   public static class Extensions {
      public static Float2 ReadFloat2(this BinaryReader reader) {
         return new Float2(reader.ReadSingle(), reader.ReadSingle());
      }

      public static Float3 ReadFloat3(this BinaryReader reader) {
         return new Float3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
      }

      public static Float4 ReadFloat4(this BinaryReader reader) {
         return new Float4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
      }

      public static Color4 ReadColor4(this BinaryReader reader) {
         return new Color4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
      }

      public static AABB ReadAABB(this BinaryReader reader) {
         var aabb = new AABB {
            min = reader.ReadFloat3(),
            max = reader.ReadFloat3()
         };

         return aabb;
      }

      public static Material ReadMaterial(this BinaryReader reader) {
         var material = new Material {
            // The size of the name field is always 256 bytes. It is padded with nulls
            name = reader.ReadBytes(256),
            unknown1 = reader.ReadUInt32(),
            unknown2 = reader.ReadUInt32(),
            unknown3 = reader.ReadUInt32()
         };

         for (var j = 0; j < 8; ++j) {
            material.textures.Add(reader.ReadTexture());
         }

         return material;
      }

      public static Texture ReadTexture(this BinaryReader reader) {
         return new Texture {
            color = reader.ReadColor4(),
            name = reader.ReadBytes(256),
            additional = reader.ReadBytes(68)
         };
      }

      public static byte[] ReadVertexBuffer(this BinaryReader reader) {
         var dataSize = reader.ReadInt32();
         return reader.ReadBytes(dataSize);
      }

      public static List<ushort> ReadIndexBuffer(this BinaryReader reader) {
         var dataSize = reader.ReadInt32();
         var d3dType = reader.ReadUInt32();

         var indexCount = dataSize / sizeof(ushort);
         var indexBuffer = new List<ushort> {
            Capacity = indexCount
         };

         for (var i = 0; i < indexCount; ++i) {
            indexBuffer.Add(reader.ReadUInt16());
         }

         return indexBuffer;
      }

      public static Mesh ReadMesh(this BinaryReader reader) {
         var flag0 = reader.ReadUInt32();
         var zero = reader.ReadUInt32();

         return new Mesh {
            boundingSphere = reader.ReadFloat4(),
            aabb = reader.ReadAABB(),
            materialIndex = reader.ReadUInt32(),
            simpleMesh = new MeshData {
               vertexType = VertexType.SIMPLE,
               vertexBufferIndex = reader.ReadUInt32(),
               vertexBufferOffset = reader.ReadUInt32(),
               vertexCount = reader.ReadUInt32(),
               indexBufferIndex = reader.ReadUInt32(),
               indexBufferOffset = reader.ReadUInt32(),
               indexCount = reader.ReadUInt32()
            },
            complexMesh = new MeshData {
               vertexType = VertexType.COMPLEX,
               vertexBufferIndex = reader.ReadUInt32(),
               vertexBufferOffset = reader.ReadUInt32(),
               vertexCount = reader.ReadUInt32(),
               indexBufferIndex = reader.ReadUInt32(),
               indexBufferOffset = reader.ReadUInt32(),
               indexCount = reader.ReadUInt32()
            }
         };
      }
   }
}
