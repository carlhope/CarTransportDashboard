using CarTransportDashboard.Models.Dtos.TransportJob;
using CarTransportDashboard.Models.Dtos.Users;

public class AuthUserDto
{
    public string Id { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}