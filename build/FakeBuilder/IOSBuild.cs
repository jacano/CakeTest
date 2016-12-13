
namespace FakeBuilder
{
    public class IOSBuild : PlatformBuild
    {
        public IOSBuild()
        {
            BuildHelper.InitCommon(this);

            BuildHelper.Clear(this);

            BuildHelper.GenerateArtifactsAndMove(this);

            BuildHelper.UploadToTeamCity(this);
        }
    }
}
