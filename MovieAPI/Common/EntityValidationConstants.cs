namespace MovieAPI.Common
{
    public static class EntityValidationConstants
    {
        public static class Comment
        {
            public const int CommentMaxLength = 200;
        }

        public static class Rating
        {
            public const int RatingMinValue = 1;
            public const int RatingMaxValue = 10;
        }
    }
}
