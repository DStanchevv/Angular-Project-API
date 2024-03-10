using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static MovieAPI.Common.EntityValidationConstants.Rating;

namespace MovieAPI.Data.Models
{
    public class Rating
    {
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        [Range(RatingMinValue, RatingMaxValue)]
        public int RatingScore { get; set; }
    }
}
