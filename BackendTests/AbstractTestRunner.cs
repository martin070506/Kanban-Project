namespace BackendTests
{
    public abstract class AbstractTestRunner
    {
        public abstract void RunAllTests();

        public void RunTest(string testName, bool result)
        {
            if (result)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{testName}: PASS");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"{testName}: FAILED");
            }

            Console.ResetColor(); // Reset to default console color
        }
    }
}
