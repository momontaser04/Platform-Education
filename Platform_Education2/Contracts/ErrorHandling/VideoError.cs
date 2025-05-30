using PlatformEduPro.Contracts.Errors;

namespace PlatformEduPro.Contracts.ErrorHandling
{
    public class VideoError
    {
        public static readonly Error VideoNOtFound =
           new("Video.VideoDIDNOtFound", "NoVideo Was found With The Given Id", StatusCodes.Status404NotFound);

        public static readonly Error VideoAdding =
          new("Video.VideoAddingError", "InvalidVideoFeatures", StatusCodes.Status401Unauthorized);
    }
}
