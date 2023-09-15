namespace Auth0CRUDPOC
{
    public class UpdateUserResponse
    {
        public string email { get; set; }
        public string password { get; set; }
        public bool email_verified { get; set; }
        public bool blocked { get; set; }
    }
}
