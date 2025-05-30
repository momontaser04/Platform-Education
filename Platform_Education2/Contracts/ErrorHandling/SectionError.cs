using PlatformEduPro.Contracts.Errors;

namespace PlatformEduPro.Contracts.ErrorHandling
{
    public class SectionError
    {
        public static readonly Error SectionNOtFound =
           new("Section.SectionDIDNOtFound", "NoSection Was found With The Given Id", StatusCodes.Status404NotFound);

        public static readonly Error SectionAdding =
          new("Section.SectionAddingError", "InvalidSectionFeatures", StatusCodes.Status401Unauthorized);
    }
}
