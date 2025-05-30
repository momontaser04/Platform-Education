using PlatformEduPro.Contracts.Errors;

namespace PlatformEduPro.Contracts.ErrorHandling
{
    public class QuestionError
    {
        public static readonly Error QuestionNOtFound =
           new("Question.QuestionIDNOtFound", "NoQuestion Was found With The Given Id", StatusCodes.Status404NotFound);

        public static readonly Error QuestionAdding =
          new("Question.QuestionAddingError", "InvalidQuestionFeatures", StatusCodes.Status401Unauthorized);
    }
}
