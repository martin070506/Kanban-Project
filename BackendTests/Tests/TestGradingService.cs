using IntroSE.Kanban.Backend.ServiceLayer;

using System;
using System.Text.Json;

namespace BackendTests.Tests
{
    internal class TestGradingService : AbstractTestRunner
    {
        private const string TestEmail = "liav@gmail.com";
        private const string SecondTestEmail = "martin@gmail.com";
        private const string UnregisterTestEmail = "tal@gmail.com";
        private const string InvalidEmail = "invalid.com";
        private const string TestPassword = "Liav1234";
        private const string SecondTestPassword = "Martin1234";
        private const string WeakPassword = "123";
        private const string TestBoard = "liavBoard";

        private GradingService gradingService = new GradingService();

        public override void RunAllTests()
        {
            gradingService.DeleteData();

            Console.WriteLine("Running Register Tests...");

            RunTest("Register - valid", TestRegister_ValidData());
            RunTest("Register - duplicate email", TestRegister_DuplicateEmail_Fails());
            RunTest("Register - invalid email", TestRegister_InvalidEmail_Fails());
            RunTest("Register - empty email", TestRegister_EmptyEmail_Fails());
            RunTest("Register - null email", TestRegister_NullEmail_Fails());
            RunTest("Register - empty password", TestRegister_EmptyPassword_Fails());
            RunTest("Register - weak password", TestRegister_WeakPassword_Fails());
        //    RunTest("Register - null password", TestRegister_NullPassword_Fails());
            RunTest("Register - valid email with numbers and symbols", TestRegister_ValidEmailWithNumbersAndSymbols());
            RunTest("Register - valid email with subdomain", TestRegister_ValidEmailWithSubdomain());
            RunTest("Register - email with uppercase letters", TestRegister_EmailWithUppercaseLetters());
            RunTest("Register - too long email", TestRegister_TooLongEmail_Fails());
            RunTest("Register - too long password", TestRegister_TooLongPassword_Fails());
            RunTest("Register - password without uppercase", TestRegister_PasswordWithoutUppercase());
            RunTest("Register - password without lowercase", TestRegister_PasswordWithoutLowercase());
            RunTest("Register - password without digits", TestRegister_PasswordWithoutDigits());
            RunTest("Register - whitespace in email", TestRegister_WhitespaceInEmail_Fails());
            RunTest("Register - whitespace in password", TestRegister_WhitespaceInPassword_Fails());
            RunTest("Register - email missing @ symbol", TestRegister_EmailMissingAtSymbol());
            RunTest("Register - email missing domain", TestRegister_EmailMissingDomain());
            RunTest("Register - email with special characters", TestRegister_EmailWithSpecialCharacters());
            RunTest("Register - already registered email", TestRegister_AlreadyRegisteredEmail_Fails());
            RunTest("Register - password exactly min length", TestRegister_PasswordExactlyMinLength());
            RunTest("Register - password exactly max length", TestRegister_PasswordExactlyMaxLength());
            RunTest("Register - empty email and password", TestRegister_EmptyEmailAndPassword_Fails());
            RunTest("Register - same password different users", TestRegister_SamePasswordDifferentUsers());
            RunTest("Register - password with symbols", TestRegister_PasswordWithSymbols());
            RunTest("Register - email with dot at start", TestRegister_EmailWithDotStart_Fails());
            RunTest("Register - email with dot at end", TestRegister_EmailWithDotEnd_Fails());
            RunTest("Register - email with consecutive dots", TestRegister_EmailWithConsecutiveDots_Fails());
            RunTest("Register - password all special chars", TestRegister_PasswordAllSpecialChars_Fails());
            RunTest("Register - email with plus sign", TestRegister_EmailWithPlus_Fails());

            RunTest("Test Register Invalid Email No At", TestRegisterInvalidEmail_NoAt());
            RunTest("Test Register Invalid Email No Domain", TestRegisterInvalidEmail_NoDomain());
            RunTest("Test Register Empty Email", TestRegisterInvalidEmail_Empty());
            RunTest("Test Register Null Email", TestRegisterInvalidEmail_Null());
            RunTest("Test Register Empty Password", TestRegisterInvalidPassword_Empty());
            RunTest("Test Register Null Password", TestRegisterInvalidPassword_Null());
            RunTest("Test Register Password Too Short", TestRegisterInvalidPassword_TooShort());
            RunTest("Test Register Password No Uppercase", TestRegisterInvalidPassword_NoUppercase());
            RunTest("Test Register Password No Lowercase", TestRegisterInvalidPassword_NoLowercase());
            RunTest("Test Register Password No Digit", TestRegisterInvalidPassword_NoDigit());
            RunTest("Test Register Duplicate Email", TestRegisterDuplicateEmail());
            RunTest("Test Register Special Characters Email", TestRegisterSpecialCharactersEmail());
            RunTest("Test Register Password Min Length", TestRegisterPassword_MinLength());
            RunTest("Test Register Password Max Length", TestRegisterPassword_MaxLength());
            RunTest("Test Register Password Only Numbers", TestRegisterInvalidPassword_OnlyNumbers());
            RunTest("Test Register Password Only Letters", TestRegisterInvalidPassword_OnlyLetters());
            RunTest("Test Register Email With Spaces", TestRegisterInvalidEmail_WithSpaces());
            RunTest("Test Register Password With Spaces", TestRegisterInvalidPassword_WithSpaces());
            RunTest("Test Register Very Long Email", TestRegisterInvalidEmail_TooLong());
            RunTest("Test Register Valid Email And Password 1", TestRegisterValid_Email1());
            RunTest("Test Register Valid Email And Password 2", TestRegisterValid_Email2());
            RunTest("Test Register Valid Email And Password 3", TestRegisterValid_Email3());
            RunTest("Test Register Unicode Email", TestRegisterUnicodeEmail());
            RunTest("Test Register Email With Dot At End", TestRegisterInvalidEmail_EndDot());
        }


        private bool TestRegister_ValidData()
        {
            string json = gradingService.Register("valid@gmail.com", "StrongPass123");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] == null;
        }

        private bool TestRegister_DuplicateEmail_Fails()
        {
            gradingService.Register("dup@gmail.com", "Strong123");
            string json = gradingService.Register("dup@gmail.com", "Strong123");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_InvalidEmail_Fails()
        {
            string json = gradingService.Register("not-an-email", "StrongPass123");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_EmptyEmail_Fails()
        {
            string json = gradingService.Register("", "StrongPass123");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_NullEmail_Fails()
        {
            string json = gradingService.Register(null, "StrongPass123");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_EmptyPassword_Fails()
        {
            string json = gradingService.Register("emptyPass@gmail.com", "");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_WeakPassword_Fails()
        {
            string json = gradingService.Register("weakpass@gmail.com", "123");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_NullPassword_Fails()
        {
            string json = gradingService.Register("nullpass@gmail.com", null);
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_ValidEmailWithNumbersAndSymbols()
        {
            string json = gradingService.Register("user.name123@gmail.com", "StrongPass123");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] == null;
        }

        private bool TestRegister_ValidEmailWithSubdomain()
        {
            string json = gradingService.Register("user@sub.domain.com", "StrongPass123");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] == null;
        }

        private bool TestRegister_EmailWithUppercaseLetters()
        {
            string json = gradingService.Register("UserUpper@Gmail.Com", "StrongPass123");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] == null;
        }

        private bool TestRegister_TooLongEmail_Fails()
        {
            string email = new string('a', 250) + "@gmail.com";
            string json = gradingService.Register(email, "StrongPass123");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_TooLongPassword_Fails()
        {
            string password = new string('a', 500);
            string json = gradingService.Register("longpass@gmail.com", password);
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_PasswordWithoutUppercase()
        {
            string json = gradingService.Register("noupcase@gmail.com", "password123");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_PasswordWithoutLowercase()
        {
            string json = gradingService.Register("nolower@gmail.com", "PASSWORD123");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_PasswordWithoutDigits()
        {
            string json = gradingService.Register("nodigits@gmail.com", "Password");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_WhitespaceInEmail_Fails()
        {
            string json = gradingService.Register("white space@gmail.com", "StrongPass123");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_WhitespaceInPassword_Fails()
        {
            string json = gradingService.Register("whitespacepass@gmail.com", "Pass word1");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_EmailMissingAtSymbol()
        {
            string json = gradingService.Register("missingatsymbol.com", "StrongPass123");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_EmailMissingDomain()
        {
            string json = gradingService.Register("user@", "StrongPass123");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_EmailWithSpecialCharacters()
        {
            string json = gradingService.Register("user!@gmail.com", "StrongPass123");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_AlreadyRegisteredEmail_Fails()
        {
            gradingService.Register("already@gmail.com", "StrongPass123");
            string json = gradingService.Register("already@gmail.com", "AnotherPass123");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_PasswordExactlyMinLength()
        {
            string json = gradingService.Register("minlength@gmail.com", "Abc123");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] == null;
        }

        private bool TestRegister_PasswordExactlyMaxLength()
        {
            string password = new string('A', 14) + new string('1', 3) + new string('c', 3); // 20-char valid password
            string json = gradingService.Register("maxlength@gmail.com", password);
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] == null;
        }

        private bool TestRegister_EmptyEmailAndPassword_Fails()
        {
            string json = gradingService.Register("", "");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_SamePasswordDifferentUsers()
        {
            gradingService.Register("first@gmail.com", "SamePass123");
            string json = gradingService.Register("second@gmail.com", "SamePass123");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] == null;
        }

        private bool TestRegister_PasswordWithSymbols()
        {
            string json = gradingService.Register("symbolpass@gmail.com", "Passw0rd!");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] == null;
        }

        private bool TestRegister_EmailWithDotStart_Fails()
        {
            string json = gradingService.Register(".dotstart@gmail.com", "ValidPass1");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_EmailWithDotEnd_Fails()
        {
            string json = gradingService.Register("dotend.@gmail.com", "ValidPass1");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_EmailWithConsecutiveDots_Fails()
        {
            string json = gradingService.Register("user..name@gmail.com", "ValidPass1");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_PasswordAllSpecialChars_Fails()
        {
            string json = gradingService.Register("specialonly@gmail.com", "!@#$%^&*");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null;
        }

        private bool TestRegister_EmailWithPlus_Fails()
        {
            string json = gradingService.Register("user+label@gmail.com", "StrongPass123");
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return response["ErrorMessage"] != null; // Depending on your policy
        }

        private bool TestRegisterInvalidEmail_NoAt()
        {
            return IsError(gradingService.Register("invalidemail.com", TestPassword));
        }

        private bool TestRegisterInvalidEmail_NoDomain()
        {
            return IsError(gradingService.Register("user@", TestPassword));
        }

        private bool TestRegisterInvalidEmail_Empty()
        {
            return IsError(gradingService.Register("", TestPassword));
        }

        private bool TestRegisterInvalidEmail_Null()
        {
            return IsError(gradingService.Register(null, TestPassword));
        }

        private bool TestRegisterInvalidPassword_Empty()
        {
            return IsError(gradingService.Register(TestEmail + "1", ""));
        }

        private bool TestRegisterInvalidPassword_Null()
        {
            return IsError(gradingService.Register(TestEmail + "2", null));
        }

        private bool TestRegisterInvalidPassword_TooShort()
        {
            return IsError(gradingService.Register(TestEmail + "3", "A1a"));
        }

        private bool TestRegisterInvalidPassword_NoUppercase()
        {
            return IsError(gradingService.Register(TestEmail + "4", "liav1234"));
        }

        private bool TestRegisterInvalidPassword_NoLowercase()
        {
            return IsError(gradingService.Register(TestEmail + "5", "LIAV1234"));
        }

        private bool TestRegisterInvalidPassword_NoDigit()
        {
            return IsError(gradingService.Register(TestEmail + "6", "LiavLiav"));
        }

        private bool TestRegisterDuplicateEmail()
        {
            gradingService.Register(TestEmail + "7", TestPassword);
            return IsError(gradingService.Register(TestEmail + "7", TestPassword));
        }

        private bool TestRegisterSpecialCharactersEmail()
        {
            return IsSuccess(gradingService.Register("special+char.email@kanban.com", TestPassword));
        }

        private bool TestRegisterPassword_MinLength()
        {
            return IsSuccess(gradingService.Register("8" + TestEmail, "Liav12"));
        }

        private bool TestRegisterPassword_MaxLength()
        {
            return IsSuccess(gradingService.Register("9" + TestEmail, "Liav123456789012345"));
        }

        private bool TestRegisterInvalidPassword_OnlyNumbers()
        {
            return IsError(gradingService.Register(TestEmail + "10", "12345678"));
        }

        private bool TestRegisterInvalidPassword_OnlyLetters()
        {
            return IsError(gradingService.Register(TestEmail + "11", "LiavLiav"));
        }

        private bool TestRegisterInvalidEmail_WithSpaces()
        {
            return IsError(gradingService.Register("liav @gmail.com", TestPassword));
        }

        private bool TestRegisterInvalidPassword_WithSpaces()
        {
            return IsError(gradingService.Register(TestEmail + "12", "Liav 1234"));
        }

        private bool TestRegisterInvalidEmail_TooLong()
        {
            string longEmail = new string('a', 250) + "@gmail.com";
            return IsError(gradingService.Register(longEmail, TestPassword));
        }

        private bool TestRegisterValid_Email1()
        {
            return IsSuccess(gradingService.Register("email1@test.com", "Password1"));
        }

        private bool TestRegisterValid_Email2()
        {
            return IsSuccess(gradingService.Register("email2@test.org", "Pass1234"));
        }

        private bool TestRegisterValid_Email3()
        {
            return IsSuccess(gradingService.Register("email3@test.net", "L123456a"));
        }

        private bool TestRegisterUnicodeEmail()
        {
            return IsSuccess(gradingService.Register("liavשלום@gmail.com", "Pass1234"));
        }

        private bool TestRegisterInvalidEmail_EndDot()
        {
            return IsError(gradingService.Register("liav.@gmail.com", TestPassword));
        }

        private bool IsError(string json)
        {
            return json.Contains("ErrorMessage") && !json.Contains("\"ErrorMessage\":null");
        }

        private bool IsSuccess(string json)
        {
            return json.Contains("\"ErrorMessage\":null");
        }

    }
}
