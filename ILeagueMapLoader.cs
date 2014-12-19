using Dargon.FileSystem;

namespace Dargon.League.Maps
{
   public interface ILeagueMapLoader {
      LeagueMap LoadFromNVR(IFileSystem system, IFileSystemHandle mapFolderHandle);
   }
}
