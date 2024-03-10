using System.ComponentModel.DataAnnotations;
using static MovieAPI.Common.EntityValidationConstants.Comment;

namespace MovieAPI.ViewModels
{
    public class AddCommentDTO
    {

        [MaxLength(CommentMaxLength)]
        public string Description { get; set; } = null!;
    }
}
