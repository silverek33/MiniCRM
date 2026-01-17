namespace MiniCRM.Models
{
    public class AdminUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string? Email { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public bool IsAdmin => Roles.Contains("Admin");
    }
}
