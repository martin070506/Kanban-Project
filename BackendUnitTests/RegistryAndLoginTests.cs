using NUnit.Framework;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace BackendUnitTests
{
    public class RegistryAndLoginTests
    {
        private FactoryService factoryService;
        private string testPassword;
        [SetUp]
        public void Setup()
        {
            factoryService = new FactoryService();
            testPassword="ValidPass123";
            factoryService.DataService.DeleteAll(); // Clear any existing data before each test
        }

        // ----------- REGISTER -----------

        [Test]
        public void Register_ValidUser_ShouldSucceed_AndBeLoggedIn()
        {
           
            var email = "reg1@example.com";
            factoryService.UserService.DeleteUser(email);
            var pass = "Valid123!";
            var res = factoryService.UserService.Register(email, pass);

            Assert.IsNull(res.ErrorMessage);
            Assert.AreEqual(email, res.ReturnValue.email);
            

            // Already logged in – second login should fail
            var loginAgain = factoryService.UserService.Login(email, pass);
            Assert.IsNotNull(loginAgain.ErrorMessage);

            factoryService.UserService.Logout(email);
           factoryService.UserService.DeleteUser(email); // Clean up after test
        }

        [Test]
        public void Register_SameUserTwice_ShouldFailSecondTime()
        {
            var email = "reg2@example.com";
            factoryService.UserService.DeleteUser(email);
            var pass = "DoubleReg123!";
            factoryService.UserService.Register(email, pass);

            var second = factoryService.UserService.Register(email, pass);
            Assert.IsNotNull(second.ErrorMessage);

            factoryService.UserService.Logout(email);
            factoryService.UserService.DeleteUser(email); // Clean up after test

        }

        [Test]
        public void Register_InvalidEmailFormat_ShouldFail()
        {
            var res = factoryService.UserService.Register("bademail", "Pass123!");
            Assert.IsNotNull(res.ErrorMessage);
            factoryService.UserService.DeleteUser("bademail"); // Clean up after test

        }

        [Test]
        public void Register_WeakPassword_ShouldFail()
        {
            var res = factoryService.UserService.Register("user@weak.com", "123");
            Assert.IsNotNull(res.ErrorMessage);
            factoryService.UserService.DeleteUser("user@weak.com"); // Clean up after test
        }

        // ----------- LOGIN -----------

        [Test]
        public void Login_CorrectAfterLogout_ShouldSucceed()
        {
            var email = "login1@example.com";
            factoryService.UserService.DeleteUser(email);
            var pass = "TestLogin123!";
            factoryService.UserService.Register(email, pass);
            factoryService.UserService.Logout(email);

            var login = factoryService.UserService.Login(email, pass);
            Assert.IsNull(login.ErrorMessage);
            Assert.AreEqual(email, login.ReturnValue.email);
            

            factoryService.UserService.Logout(email);
           factoryService.UserService.DeleteUser(email); // Clean up after test
        }

        [Test]
        public void Login_WhileAlreadyLoggedIn_ShouldFail()
        {
            var email = "login2@example.com";
            factoryService.UserService.DeleteUser(email);
            var pass = "AlreadyLogged!";
            factoryService.UserService.Register(email, pass);

            var loginAgain = factoryService.UserService.Login(email, pass);
            Assert.IsNotNull(loginAgain.ErrorMessage);

            factoryService.UserService.Logout(email);
            factoryService.UserService.DeleteUser(email);
        }

        [Test]
        public void Login_InvalidPassword_ShouldFail()
        {
            var email = "login3@example.com";
            factoryService.UserService.DeleteUser(email);
            var pass = "RightPass123!";
            factoryService.UserService.Register(email, pass);
            factoryService.UserService.Logout(email);

            var res = factoryService.UserService.Login(email, "WrongPass!");
            Assert.IsNotNull(res.ErrorMessage);
           factoryService.UserService.DeleteUser(email); // Clean up after test
        }

        [Test]
        public void Login_InvalidEmailFormat_ShouldFail()
        {
            var res = factoryService.UserService.Login("invalidemail", "Password123!");
            Assert.IsNotNull(res.ErrorMessage);

        }

        [Test]
        public void Login_UnregisteredEmail_ShouldFail()
        {
            var res = factoryService.UserService.Login("never@seen.com", "SomePass123!");
            Assert.IsNotNull(res.ErrorMessage);
        }

        // ----------- LOGOUT -----------

        [Test]
        public void Logout_WhenLoggedIn_ShouldSucceed()
        {
            var email = "logout1@example.com";
            var pass = "LogoutGood123!";

            factoryService.UserService.Register(email, pass);

            var res = factoryService.UserService.Logout(email);
            Assert.IsNull(res.ErrorMessage);
            Assert.IsTrue(res.ReturnValue);
            factoryService.UserService.DeleteUser(email);
            
        }

        [Test]
        public void Logout_Twice_ShouldFailSecondTime()
        {
            var email = "logout2@example.com";
            var pass = "DoubleOut!";
            factoryService.UserService.Register(email, pass);
            factoryService.UserService.Logout(email);

            var second = factoryService.UserService.Logout(email);
            Assert.IsNotNull(second.ErrorMessage);
            factoryService.UserService.DeleteUser(email);
           
        }

        [Test]
        public void Logout_WithoutLogin_ShouldFail()
        {
            var email = "logout3@example.com";
            var pass = "OutNoLogin!";
            factoryService.UserService.Register(email, pass);
            factoryService.UserService.Logout(email); // first logout
            var res = factoryService.UserService.Logout(email); // second logout

            Assert.IsNotNull(res.ErrorMessage);
            factoryService.UserService.DeleteUser(email);
            
        }

        [Test]
        public void Logout_InvalidEmail_ShouldFail()
        {
            var res = factoryService.UserService.Logout("bademail");
            Assert.IsNotNull(res.ErrorMessage);
        }

        [Test]
        public void Register_EmailWithoutAt_ShouldFail()
        {
            var response = factoryService.UserService.Register("invalidemail.com", testPassword);
            Assert.IsNotNull(response.ErrorMessage);
            factoryService.UserService.DeleteUser("invalidemail.com"); // Clean up after test
        }

        [Test]
        public void Register_EmailWithoutDomain_ShouldFail()
        {
            var response = factoryService.UserService.Register("user@", testPassword);
            Assert.IsNotNull(response.ErrorMessage);
            factoryService.UserService.DeleteUser("user@");
        }

        [Test]
        public void Register_EmailWithMultipleAts_ShouldFail()
        {
            var response = factoryService.UserService.Register("user@@example.com", testPassword);
            Assert.IsNotNull(response.ErrorMessage);
            factoryService.UserService.DeleteUser("user@@example.com"); // Clean up after test
        }

        [Test]
        public void Register_EmptyPassword_ShouldFail()
        {
            var response = factoryService.UserService.Register("user1@example.com", "");
            Assert.IsNotNull(response.ErrorMessage);
            factoryService.UserService.DeleteUser("user1@example.com");
        }

        [Test]
        public void Register_PasswordWithOnlySpaces_ShouldFail()
        {
            var response = factoryService.UserService.Register("user2@example.com", "     ");
            Assert.IsNotNull(response.ErrorMessage);
            factoryService.UserService.DeleteUser("user2@example.com");
        }

        [Test]
        public void Register_TooLongPassword_ShouldFail()
        {
            string longPassword = new string('A', 1000); // extremely long password
            var response = factoryService.UserService.Register("user3@example.com", longPassword);
            Assert.IsNotNull(response.ErrorMessage);
            factoryService.UserService.DeleteUser("user3@example.com");
        }

        [Test]
        public void Register_EmailWithLeadingWhitespace_ShouldSucceed()
        {
            var response = factoryService.UserService.Register("   user@example.com", testPassword);
            Assert.IsNull(response.ErrorMessage);
            factoryService.UserService.DeleteUser("   user@example.com");
        }

        [Test]
        public void Register_EmailWithTrailingWhitespace_ShouldSucceed()
        {
            var response = factoryService.UserService.Register("user@example.com   ", testPassword);
            Assert.IsNull(response.ErrorMessage);
            factoryService.UserService.DeleteUser("user@example.com   ");
        }

        [Test]
        public void Register_EmailWithLeadingAndTrailingWhitespace_ShouldSucceed()
        {
            var response = factoryService.UserService.Register("   user@example.com   ", testPassword);
            Assert.IsNull(response.ErrorMessage);
            factoryService.UserService.DeleteUser("   user@example.com   ");
        }

        [Test]
        public void Register_EmailCaseInsensitive_ShouldSucceed()
        {
            factoryService.UserService.Register("case@test.com", testPassword);
            var response = factoryService.UserService.Register("CASE@test.com", testPassword);
            Assert.IsNotNull(response.ErrorMessage);
            factoryService.UserService.DeleteUser("case@test.com");
            factoryService.UserService.DeleteUser("CASE@test.com");
        }

    }
}
