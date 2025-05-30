using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.DTO.Roles
{
    public class RoleRequestDto {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 250 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Permissions must not be null.")]
        [MinLength(1, ErrorMessage = "Permissions list must not be empty.")]
        public IList<string> Permissions { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (Permissions != null && Permissions.Distinct().Count() != Permissions.Count)
            {
                yield return new ValidationResult(
                    "You cannot add duplicated permissions for the same role",
                    new[] { nameof(Permissions) });
            }
        }

    }

       
}
