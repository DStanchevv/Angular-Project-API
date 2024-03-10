using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static MovieAPI.Common.EntityValidationConstants.Comment;

namespace MovieAPI.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }
        
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        
        [MaxLength(CommentMaxLength)]
        public string Description { get; set; } = null!;

    }
}
