namespace helps.Service.DataObjects
{
    public class helpsLoginResult
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudentId { get; set; }
        public string Email { get; set; }
        public bool HasLoggedIn { get; set; }
        public string AuthToken { get; set; }
    }
}