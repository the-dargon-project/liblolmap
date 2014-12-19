using System.IO;

namespace Dargon.League.Maps {
   public interface ILeagueMapWriter {
      void WriteToObj(LeagueMap map, Stream outputStream, VertexType type);
   }
}
