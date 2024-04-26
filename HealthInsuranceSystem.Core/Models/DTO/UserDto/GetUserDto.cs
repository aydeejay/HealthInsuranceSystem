namespace HealthInsuranceSystem.Core.Models.DTO.UserDto
{
    public class GetUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string UserPolicyNumber { get; set; }
        public string NationalID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
