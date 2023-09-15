namespace Auth0CRUDPOC
{
    public class UserModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool EmailVerified { get; set; }
        public bool Blocked { get; set; }
    }
}
