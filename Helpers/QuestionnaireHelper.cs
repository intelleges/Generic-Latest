using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Helpers.Questionnaire
{
    public static class CommentType
    {
        public const int YN_WARNING_N = 1;
        public const int YN_WARNING_Y = 2;
        public const int YN_COMMENT_Y = 3;
        public const int YN_COMMENT_N = 4;
        public const int YN_UPLOAD_Y = 5;
        public const int YN_UPLOAD_N = 6;
        public const int YN_NO_COMMENT = 7;

        public const int YN_COMMENT_REQUIRED_Y = 1;
        public const int YN_COMMENT_REQUIRED_N = 0;

        // old system
//        commentType 4 = File  Upload
//commentType 1 = Warning
//commentType 3 = Comment
//commentType 0 = Nothing

    }
}