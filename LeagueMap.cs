using System.Collections.Generic;

namespace Dargon.League.Maps {
   public class LeagueMap {
      public ushort majorVersion;
      public ushort minorVersion;

      public List<Material> materials;
      public List<byte[]> vertexBuffers;
      public List<List<ushort> > indexBuffers;
      public List<Mesh> meshes;
      public List<AABB> AABBs;
   }

   public class Material {
      public char[] name = new char[256];
      public uint unknown1;
      public uint unknown2;
      public uint unknown3;
      public List<Texture> textures;
   }

   public class Texture {
      public Color color;
      public char[] name = new char[256];
      public byte[] additional = new byte[68];
   }

   public class AABB {
      public Float3 min;
      public Float3 max;
   }

   public class SimpleVertex {
      public Float3 position;
   }

   public class ComplexVertex {
      public Float3 position;
      public Float3 normal;
      public Float2 uv;
      public byte[] extraData;
   }

   public enum VertexType : byte {
      SIMPLE = 0,
      COMPLEX = 1
   }

   public class Mesh {
      public Float4 boundingSphere;
      public AABB aabb;
      public uint materialIndex;
      public MeshData simpleMesh;
      public MeshData complexMesh;
   }

   public class MeshData {
      public VertexType vertexType;

      public uint vertexBufferIndex;
      public uint vertexBufferOffset;
      public uint vertexCount;
      
      public uint indexBufferIndex;
      public uint indexBufferOffset;
      public uint indexCount;
   }
}
