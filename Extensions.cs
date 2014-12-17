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
   }
}
