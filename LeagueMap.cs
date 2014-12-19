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
      public byte[] name = new byte[256];
      public uint unknown1;
      public uint unknown2;
      public uint unknown3;
      public List<Texture> textures;
   }

   public class Texture {
      public Color4 color;
      public byte[] name = new byte[256];
      public byte[] additional = new byte[68];
   }

   public class Mesh {
      public Float4 boundingSphere;
      public AABB aabb;
      public int materialIndex;
      public MeshData simpleMesh;
      public MeshData complexMesh;
   }

   public class AABB {
      public Float3 min;
      public Float3 max;
   }

   public class MeshData {
      public int vertexBufferIndex;
      public int vertexBufferOffset;
      public int vertexCount;
      
      public int indexBufferIndex;
      public int indexBufferOffset;
      public int indexCount;
   }

   public enum VertexType {
      SIMPLE,
      COMPLEX
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
}
