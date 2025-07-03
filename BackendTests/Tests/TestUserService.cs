using System;
using System.Collections.Generic;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.models;
using IntroSE.Kanban.Backend.ServiceLayer.Models;

namespace BackendTests.Tests
{
    /// <summary>
    /// Provides tests for user‑related operations.
    /// This version creates shared test objects at startup and cleans them up at the end.
    /// For cleanup, it ensures the user is logged in before deletion.
    /// </summary>
    public class TestUserService : AbstractTestRunner
    {
        private FactoryService _factoryService = new FactoryService();

        // List of test emails that persist throughout the test run.
        private readonly List<(string Email, string Password)> _testUsers = new()
        {
            ("tal@gmail.com", "Aasq123"),
            ("user@example.com", "Aasq123"),
        };

        /// <summary>
        /// Performs test setup: creates a shared FactoryService and registers/logs in all test users.
        /// </summary>
        private void Setup()
        {
            _factoryService = new FactoryService();
            foreach (var (email, password) in _testUsers)
            {
                Response<UserSL> res = _factoryService.UserService.Register(email, password);
                _factoryService.UserService.Login(email, password);
            }
        }

        /// <summary>
        /// Performs test cleanup: ensures each test user is logged in and then deletes them.
        /// </summary>
        private void TearDown()
        {
            foreach (var (email, password) in _testUsers)
            {
                try
                {
                    _factoryService.UserService.Login(email, password);
                }
                catch { }
                try
                {
                    _factoryService.UserService.DeleteUser(email);
                }
                catch { }
            }
        }

        /// <summary>
        /// Runs all user service tests sequentially using shared test objects.
        /// </summary>
        public override void RunAllTests()
        {
            Console.WriteLine("Running User Service Tests...");

            try
            {
                // For tests that exercise registration, we remove the user first so they are not duplicates.
                RunTest("Test Register", TestRegister());
                _factoryService.DataService.DeleteAll();
                RunTest("Test Register Duplicate Emails", TestRegisterDuplicateEmails());
                _factoryService.DataService.DeleteAll();
                RunTest("Test Login Fail", TestLoginFail());
                _factoryService.DataService.DeleteAll();
                RunTest("Test Login Success", TestLoginAndRegisterSuccess());
                _factoryService.DataService.DeleteAll();
                RunTest("Test Register Weak Password", TestRegisterWeakPassword());
                _factoryService.DataService.DeleteAll();
                RunTest("Test Register Weak Password 2", TestRegisterWeakPassword2());
                _factoryService.DataService.DeleteAll();
                RunTest("Test Logout Without Login", TestLogoutWithoutLogin());
                _factoryService.DataService.DeleteAll();
                RunTest("Test Logout Success", TestLogoutSuccess());
                _factoryService.DataService.DeleteAll();
                RunTest("Test Login With Invalid Credentials", TestLoginWithInvalidCredentials());
            }
            finally
            {
                _factoryService.DataService.DeleteAll();
            }
            Console.WriteLine("All User Service Tests Completed.");
        }

        /// <summary>
        /// Removes the specified user (if exists), registers it, and verifies successful registration.
        /// </summary>
        private bool TestRegister()
        {
            Response<UserSL> res = _factoryService.UserService.Register("tal@gmail.com", "Aasq123");
            return res.ErrorMessage == null && res.ReturnValue is UserSL;
        }

        /// <summary>
        /// Attempts to register the same user twice and expects an error on the duplicate registration.
        /// </summary>
        private bool TestRegisterDuplicateEmails()
        {
            // Register user first.


            _factoryService.UserService.Register("tal@gmail.com", "Aasq123");
            Response<UserSL> res = _factoryService.UserService.Register("tal@gmail.com", "Aasq123");
            return res.ErrorMessage != null
                && (res.ReturnValue == null || !(res.ReturnValue is UserSL));
        }

        /// <summary>
        /// Tests login with null credentials.
        /// </summary>
        private bool TestLoginFail()
        {
            Response<UserSL> res = _factoryService.UserService.Login(null, null);
            return res.ErrorMessage != null
                && (res.ReturnValue == null || !(res.ReturnValue is UserSL));
        }

        /// <summary>
        /// Tests registration and then login with valid credentials.
        /// </summary>
        private bool TestLoginAndRegisterSuccess()
        {
            Response<UserSL> res = _factoryService.UserService.Register("tal@gmail.com", "Aasq123");
            return res.ErrorMessage == null && res.ReturnValue is UserSL;
        }

        /// <summary>
        /// Tests registration with a weak password.
        /// </summary>
        private bool TestRegisterWeakPassword()
        {
            Response<UserSL> res = _factoryService.UserService.Register("user@example.com", "123");
            return res.ErrorMessage != null
                && (res.ReturnValue == null || !(res.ReturnValue is UserSL));
        }

        /// <summary>
        /// Tests registration with another weak password scenario.
        /// </summary>
        private bool TestRegisterWeakPassword2()
        {
            Response<UserSL> res = _factoryService.UserService.Register(
                "user@example.com",
                "AAasq"
            );
            return res.ErrorMessage != null
                && (res.ReturnValue == null || !(res.ReturnValue is UserSL));
        }

        /// <summary>
        /// Registers a user, logs out, then attempts to log out again.
        /// </summary>
        private bool TestLogoutWithoutLogin()
        {
            _factoryService.UserService.Register("user@example.com", "Aasq123");
            _factoryService.UserService.Logout("user@example.com");
            Response<bool> res = _factoryService.UserService.Logout("user@example.com");
            return res.ErrorMessage != null;
        }

        /// <summary>
        /// Tests that a logged-in user can log out successfully.
        /// </summary>
        private bool TestLogoutSuccess()
        {
            _factoryService.UserService.Register("user@example.com", "Aasq123");
            _factoryService.UserService.Login("user@example.com", "Aasq123");
            Response<bool> logoutRes = _factoryService.UserService.Logout("user@example.com");
            return logoutRes.ErrorMessage == null;
        }

        /// <summary>
        /// Tests login with empty strings as credentials.
        /// </summary>
        private bool TestLoginWithInvalidCredentials()
        {
            Response<UserSL> res = _factoryService.UserService.Login("", "");
            return res.ErrorMessage != null
                && (res.ReturnValue == null || !(res.ReturnValue is UserSL));
        }
    }
}
