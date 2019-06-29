using System;

namespace NextDoor.ApiGateway.Dto.Admin
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime LastLogin { get; set; }
        public Guid Guid { get; set; }
    }
}
