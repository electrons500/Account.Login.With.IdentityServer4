using IdentityModel.Client;
using System.Net.Mail;
using Webapi.Models.ApiModel;
using Webapi.Models.Data.BookStoreDBContext;
using Webapi.Models.Hashing;

namespace Webapi.Models.Data.Service
{
    public class AccountService
    {
        private BookStoreDbContext _db;
        public AccountService(BookStoreDbContext db)
        {
            _db = db;
        }

        //Create account
        public bool UserAccount(UserRegistrationApiModel model)
        {
            MailAddress mailAddress = new(model.Email);

            User user = new()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                FullName = $"{model.FirstName} {model.LastName}",
                UserName = mailAddress.User,
                NormalizedUserName = mailAddress.User.ToUpper(),
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper(),
                EmailConfirmed = false,
                PasswordHash = PasswordHashing.HashPassword(model.Password),
                PhoneNumber = model.PhoneNumber,
                PhoneNumberConfirmed = false,
                LockoutEnd = null,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                RegistrationDate = DateTime.Now.ToShortDateString()

            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return true;
        }


        //Login
        public bool UserAccountLogin(LoginApiModel model)
        {
           var user = _db.Users.Where(x => x.Email == model.Email).FirstOrDefault();   
            if (user != null)
            {
                //Authenticate user password against the hash password in the DB
                bool authenticatePassword = PasswordHashing.VerifyHashedPassword(user.PasswordHash, model.Password);
                if (authenticatePassword)
                {
                    return true;
                }

            }
            return false;
        }

        //Get Token when user is successfull authenticated
        public async Task<TokenModel> RequestTokenFromIdentityServerAsync(string email)
        {
            // discover endpoints from metadata
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (disco.IsError)
            {
                return new TokenModel();
            }

            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "client",
                ClientSecret = "secret",
                Scope = "IdentityServer"
            });

            if (tokenResponse.IsError)
            {
                return new TokenModel();
            }
            var accessToken = tokenResponse.AccessToken;
            var user = _db.Users.Where(x => x.Email == email).FirstOrDefault();
            if (accessToken != null)
            {
                return new TokenModel
                {
                    AccessToken = accessToken,
                    UserName = user.FullName

                };
            }

            return new TokenModel();
        }


    }
}
