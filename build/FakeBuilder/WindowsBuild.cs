
namespace FakeBuilder
{
    public class WindowsBuild : PlatformBuild
    {
        public WindowsBuild()
        {
            BuildHelper.InitCommon(this);

            BuildHelper.Clear(this);

            BuildHelper.GenerateArtifactsAndMove(this);

            BuildHelper.UploadToTeamCity(this);
        }
    }
}
