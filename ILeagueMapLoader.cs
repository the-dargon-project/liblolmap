using Dargon.FileSystem;

namespace Dargon.League.Maps
{
   public interface ILeagueMapLoader {
      LeagueMap Load(IFileSystem system, IFileSystemHandle mapFolderHandle);
   }
}
