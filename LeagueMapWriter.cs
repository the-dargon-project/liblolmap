using System.IO;

namespace Dargon.League.Maps {
   public class LeagueMapWriter : ILeagueMapWriter {
      public void WriteToObj(LeagueMap map, Stream outputStream, VertexType type) {
         using (StreamWriter writer = new StreamWriter(outputStream)) {
            // Write out some meta data as comments
            writer.WriteLine(@"// This file was auto generated from a League of Legends .nvr file by liblolmap, which is part of The Dargon Project");
            writer.WriteLine();

            var vertexOffsets = 0;
            foreach (var mesh in map.meshes) {
               var meshData = type == VertexType.COMPLEX ? mesh.complexMesh : mesh.simpleMesh;

               var vertexBuffer = map.vertexBuffers[meshData.vertexBufferIndex];
               for (var i = meshData.vertexBufferOffset; i < meshData.vertexBufferOffset + meshData.vertexCount; ++i) {
                  if (type == VertexType.COMPLEX) {
                     writer.WriteComplexVertex((ComplexVertex)vertexBuffer[i]);
                  } else {
                     writer.WriteSimpleVertex((SimpleVertex)vertexBuffer[i]);
                  }
               }

               var indexBuffer = map.indexBuffers[meshData.indexBufferIndex];
               var vertexOffset = meshData.vertexBufferOffset;

               for (var i = meshData.indexBufferOffset; i < meshData.indexBufferOffset + meshData.indexCount; i += 3) {
                  var index1 = indexBuffer[i] - vertexOffset + vertexOffsets + 1;
                  var index2 = indexBuffer[i + 1] - vertexOffset + vertexOffsets + 1;
                  var index3 = indexBuffer[i + 2] - vertexOffset + vertexOffsets + 1;

                  writer.WriteLine("f " + index1 + "/" + index1 + "/" + index1 +
                                   " " + index2 + "/" + index2 + "/" + index2 +
                                   " " + index3 + "/" + index3 + "/" + index3);
               }

               vertexOffsets += meshData.vertexCount;
            }
         }
      }
   }
}
